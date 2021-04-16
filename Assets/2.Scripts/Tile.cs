using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour,IPointerDownHandler, IPointerUpHandler,IDragHandler
{
    private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
    private static Tile previousSelected = null;


    //상태변수
    private bool isSelected = false;
    private bool isDragging = false;
    private bool matchFound = false;
    //  public static bool isContinuousMatch = false;


    const float DRAG_SPEED = 0.2f;
    const float DRAG_RANGE = 0.8f;
    const float DRAG_RANGE_DIV = 1.6f;
    const float DROP_SPEED = 0.08f;
    const float DROP_TIME = 0.01f;

    private float verticalSpeed;
    private float horizontalSpeed;
    private Vector3 originPos;


    private Vector3[] adjacentDirections = new Vector3[] { Vector3.left, Vector3.right, Vector3.up, Vector3.down };

    //필요한 컴포넌트
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        originPos = transform.position;
    }

    private void Update()
    {
        if (GameManager.instance.isGameOver)
        {
            image = null;
            return;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Select();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Deselect();
    }

    public void OnDrag(PointerEventData eventData)
    {

        transform.position = eventData.position;


        #region 이동거리 제한
        Vector2 dir = eventData.position - new Vector2(originPos.x,originPos.y);
        if(dir.magnitude > gameObject.GetComponent<Image>().rectTransform.rect.width)
        {
            dir.Normalize();
            transform.position = new Vector2(originPos.x,originPos.y) + dir * gameObject.GetComponent<Image>().rectTransform.rect.width;
        }
        #endregion
    }



    //타일을 선택할 경우
    //1. 타일의 색상이 바뀐다.
    //2. 타일 선택 소리가 나온다.
    private void Select()
    {
        isSelected = true;
        image.color = selectedColor;
        previousSelected = gameObject.GetComponent<Tile>();
        SoundManager.instance.PlaySE("Select");
    }

    private void Deselect()
    {
        isSelected = false;
        image.color = Color.white;
        previousSelected = null;
    }


    private void Matching()
    {
        previousSelected.ClearAllMatches();
        previousSelected.Deselect();
        ClearAllMatches();
        if (BoardManager.instance.MatchFoundCount == 0)
            GUIManager.instance.ComboCounter = 0;
    }


    //타일 위치 서로 바꾸기
    public void SwapSprite(SpriteRenderer render2)
    {
        if (image.sprite == render2.sprite)
            return;

        //위치바꾸기 알고리즘
        Sprite tempSprite = render2.sprite;
        render2.sprite = image.sprite;
        image.sprite = tempSprite;
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
            while (hitInfo.collider != null && hitInfo.collider.GetComponent<SpriteRenderer>().sprite == image.sprite)
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
        if (image.sprite == null)
            return;

        ClearMatch(new Vector3[2] { Vector3.left, Vector3.right });
        ClearMatch(new Vector3[2] { Vector3.up, Vector3.down });

        if (matchFound)
        {
            image.sprite = null;
            matchFound = false;       
            //???
            //ShiftDelay가 너무 짧아서 코루틴 실행이 다 안된건가?
            //아니면 또다른 매칭이 이루어져서 코루틴이 멈춰버린 것인가? ★
            //StopCoroutine(BoardManager.instance.FindNullTiles());

            //비어있는 타일을 검색
            BoardManager.instance.FindNullTiles(); 
            SoundManager.instance.PlaySE("Clear");

            GUIManager.instance.ComboCounter++;
            BoardManager.instance.MatchFoundCount++;

        }
    }


}
