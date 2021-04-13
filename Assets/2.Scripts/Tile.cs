using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
    private static Tile previousSelected = null;
    private int matchFoundCount = 0;

    //상태변수
    private bool isSelected = false;
    private bool matchFound = false;
    //  public static bool isContinuousMatch = false;

    private Vector3[] adjacentDirections = new Vector3[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down };

    //필요한 컴포넌트
    private SpriteRenderer render;

    private void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (GameManager.instance.isGameOver)
        {
            render = null;
            return;
        }
    }


    //타일을 선택할 경우
    //1. 타일의 색상이 바뀐다.
    //2. 타일 선택 소리가 나온다.
    private void Select()
    {
        isSelected = true;
        render.color = selectedColor;
        previousSelected = gameObject.GetComponent<Tile>();
        SoundManager.instance.PlaySE("Select");
    }

    private void Deselect()
    {
        isSelected = false;
        render.color = Color.white;
        previousSelected = null;
    }

    void OnMouseDown()
    {
        if (!GameManager.instance.isGameOver)
        {
            //스프라이트가 없거나, 이동중인 상태, 혹은 인스턴스가 없는 상태라면 실행 불가능
            if (render.sprite == null || BoardManager.instance.IsShifting)
                return;

            //선택된 상태에서 다시 누르면 비선택 실행
            if (isSelected)
                Deselect();
            else //선택된 상태가 아니라면
            {
                //이전 스프라이트가 선택된게 없다면
                if (previousSelected == null)
                    Select(); //새로 선택
                else //이전 스프라이트가 선택된것이 있다면
                {
                    // 인접한타일이 이전타일을 포함하고 있다면 매칭
                    if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
                    {
                        SwapSprite(previousSelected.render);
                        previousSelected.ClearAllMatches();
                        previousSelected.Deselect();
                        ClearAllMatches();
                        if (matchFoundCount == 0)
                            GUIManager.instance.ComboCounter = 0;
                    }
                    else //그렇지 않으면
                    {
                        previousSelected.GetComponent<Tile>().Deselect();
                        Select();
                    }
                }
            }
        }
    }


    //타일 위치 서로 바꾸기
    public void SwapSprite(SpriteRenderer render2)
    {
        if (render.sprite == render2.sprite)
            return;

        //위치바꾸기 알고리즘
        Sprite tempSprite = render2.sprite;
        render2.sprite = render.sprite;
        render.sprite = tempSprite;
        SoundManager.instance.PlaySE("Swap"); //바꾸기 소리 실행
    }


    //특정 타일과 근접한 모든타일을 얻어서 저장한다.
    private List<GameObject> GetAllAdjacentTiles()
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        //전역변수로 설정한 방향으로 전부 빛을 쏴서 타일을 찾은 뒤에
        //선택했던 타일과 인접하는지를 찾는 과정이다.
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentTiles;
    }

    //특정 타일에서 Ray를 쏴서 부딪히는 컬라이더 오브젝트를 찾는다.
    private GameObject GetAdjacent(Vector3 castDir)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, castDir, out hitInfo))
        {
            if (hitInfo.collider != null)
            {
                return hitInfo.collider.gameObject;
            }
        }
        return null;
    }


    //매칭이 되었는지를 검사하는 함수
    private List<GameObject> FindMatch(Vector3 castDir)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit hitInfo;
        bool isMatch = Physics.Raycast(transform.position, castDir, out hitInfo);
        if (isMatch)
        {
            while (hitInfo.collider != null && hitInfo.collider.GetComponent<SpriteRenderer>().sprite == render.sprite)
            {
                matchingTiles.Add(hitInfo.collider.gameObject);
                Physics.Raycast(hitInfo.collider.transform.position, castDir, out hitInfo);
            }
        }
        return matchingTiles;
    }

    //매칭타일을 클리어
    private void ClearMatch(Vector3[] paths)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindMatch(paths[i]));
        }

        if (matchingTiles.Count >= 2)
        {
            for (int i = 0; i < matchingTiles.Count; i++)
            {
                matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
            }
            matchFound = true;
        }
    }

    //모든 매칭들을 클리어
    public void ClearAllMatches()
    {
        if (render.sprite == null)
            return;

        ClearMatch(new Vector3[2] { Vector3.left, Vector3.right });
        ClearMatch(new Vector3[2] { Vector3.up, Vector3.down });
        if (matchFound)
        {
            render.sprite = null;
            matchFound = false;
            matchFoundCount++;

            StopCoroutine(BoardManager.instance.FindNullTiles());
            StartCoroutine(BoardManager.instance.FindNullTiles());
            SoundManager.instance.PlaySE("Clear");
            GUIManager.instance.MoveCounter--;
            GUIManager.instance.ComboCounter++;
        }
    }
}
