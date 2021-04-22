using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    #region Field Variable

    //고유 정보
    [SerializeField] private int row;
    [SerializeField] private int col;
    [SerializeField] private Vector3 originPos;
    private float swapAngle;

    //상태변수
    public bool isMatched = false;
    private bool isSwapping = false;

    private Vector2 firstTouchPosition;
    private Vector2 secondTouchPosition;
    private GameObject secondTile;

    //필요한 컴포넌트
    private Image image;

    public int Row { get => row; set => row = value; }
    public int Col { get => col; set => col = value; }


    #endregion

    private void Start()
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


        if (isSwapping)
            SwapAnimation();
        else
        {
            BoardManager.instance.tiles[Row, Col] = this.gameObject;
            gameObject.name = BoardManager.instance.SetTileName(Row, Col);
        }
    }

    private void SwapAnimation()
    {
        //옮기려는 타일과 옮길 장소의 원래 포지션의 값이 0.1 이상이면 계속 Lerp를 실행
        if (Mathf.Abs(secondTile.GetComponent<Tile>().originPos.x - transform.position.x) > .1 ||
            Mathf.Abs(secondTile.GetComponent<Tile>().originPos.y - transform.position.y) > .1)
        {
            transform.position = Vector2.Lerp(transform.position, secondTile.GetComponent<Tile>().originPos, .3f);
            secondTile.transform.position = Vector2.Lerp(secondTile.transform.position, originPos, .3f);
        }
        else
        {   //타일 위치 이동 완료
            //타일 위치의 변화가 일어날때마다 타일의 매치를 검사합니다.
            transform.position = secondTile.GetComponent<Tile>().originPos;
            secondTile.transform.position = originPos;
            ReInitOriginPos(); //각 타일의 OriginPos를 초기화           
            CheckMatching();
            isSwapping = false;
        }
    }

    private void ReInitOriginPos()
    {
        originPos = transform.position;
        secondTile.GetComponent<Tile>().originPos = secondTile.transform.position;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        firstTouchPosition = eventData.pointerCurrentRaycast.gameObject.transform.position;
        SoundManager.instance.PlaySE("Select");
        Debug.Log("선택한 타일 : " + eventData.pointerCurrentRaycast.gameObject);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        secondTouchPosition = eventData.position;
        CalculateSwapAngle();
        //Debug.Log(swapAngle);

        //드래그한 거리가 이미지 파일 가로 크기의 0.6이상이라면
        if ((secondTouchPosition - firstTouchPosition).magnitude > this.gameObject.GetComponent<RectTransform>().rect.size.x * 0.6)
        {
            //타일 바꾸기
            SwapTile();
        }

    }

    private void CalculateSwapAngle()
    {
        //라디안값이기 때문에 180 / Mathf.PI 곱해주기
        swapAngle = Mathf.Atan2(secondTouchPosition.y - firstTouchPosition.y, secondTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
    }

    private void CheckMatching()
    {
        FindAllMatchingTiles();
        secondTile.GetComponent<Tile>().FindAllMatchingTiles();
        if (BoardManager.instance.MatchFoundCount == 0)
            GUIManager.instance.ComboCounter = 0;
    }


    //타일 위치 서로 바꾸기
    public void SwapTile()
    {
        if (swapAngle > -45 && swapAngle <= 45 && Row + 1 < BoardManager.instance.width)
        {
            //오른쪽 타일과 교체
            secondTile = BoardManager.instance.GetTile()[Row + 1, Col];
            secondTile.GetComponent<Tile>().Row -= 1;
            Row += 1;
        }
        else if (swapAngle > 45 && swapAngle <= 135 && Col + 1 < BoardManager.instance.height)
        {
            //위쪽 타일과 교체
            secondTile = BoardManager.instance.GetTile()[Row, Col + 1];
            secondTile.GetComponent<Tile>().Col -= 1;
            Col += 1;
        }
        else if ((swapAngle > 135 || swapAngle <= -135) && (Row - 1) >= 0)
        {
            //왼쪽 타일과 교체
            secondTile = BoardManager.instance.GetTile()[Row - 1, Col];
            secondTile.GetComponent<Tile>().Row += 1;
            Row -= 1;
        }
        else if (swapAngle < -45 && swapAngle >= -135 && (Col - 1) >= 0)
        {
            //아래쪽 타일과 교체
            secondTile = BoardManager.instance.GetTile()[Row, Col - 1];
            secondTile.GetComponent<Tile>().Col += 1;
            Col -= 1;
        }
        else
            return;

        isSwapping = true;
        SoundManager.instance.PlaySE("Swap"); //바꾸기 소리 실행
    }



    //매칭조건을 만족하는 모든 타일을 검색하는 함수
    public void FindAllMatchingTiles()
    {
        if (image.sprite == null)
            return;

        CheckCountMatchTiles(new Vector3[2] { Vector3.left, Vector3.right });
        CheckCountMatchTiles(new Vector3[2] { Vector3.up, Vector3.down });

        if (isMatched) //매칭된 타일을 찾으면
        {           
            BoardManager.instance.FindDestroyMatches(); // 매칭된 타일을 파괴

            SoundManager.instance.PlaySE("Clear"); //매칭 성공 효과음
            GUIManager.instance.ComboCounter++;     //콤보 카운터가 올라감
            BoardManager.instance.MatchFoundCount++; //매치카운트 증가

            if (gameObject != null)
                Destroy(gameObject);
        }
    }


    //자신과 인접한 타일을 검사하는 함수
    private List<GameObject> FindAdjacentTiles(Vector3 castDir)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit hitInfo;
        bool isMatch = Physics.Raycast(transform.position, castDir, out hitInfo);
        if (isMatch)
        {
            //TODO : 나중엔 태그로 비교하기
            while (hitInfo.collider != null && hitInfo.collider.GetComponent<Image>().sprite == image.sprite)
            {
                matchingTiles.Add(hitInfo.collider.gameObject);
                Physics.Raycast(hitInfo.collider.transform.position, castDir, out hitInfo);
            }
        }
        return matchingTiles;
    }

    //인접한 타일이 매칭이 성립되는 타일인지 체크하는 함수
    private void CheckCountMatchTiles(Vector3[] paths)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        for (int i = 0; i < paths.Length; i++)
        {
            matchingTiles.AddRange(FindAdjacentTiles(paths[i]));
        }

        if (matchingTiles.Count >= 2)
        {       
            for (int i = 0; i < matchingTiles.Count; i++)
            {
                //자신을 제외한 나머지 매칭이 된 타일의 색상을 변경 (임시)
                matchingTiles[i].GetComponent<Image>().color = new Color(.5f, .5f, .5f, 1f);
                matchingTiles[i].GetComponent<Tile>().isMatched = true;
            }

            image.color = new Color(.5f, .5f, .5f, 1f); //본인의 타일 색상도 변경
            isMatched = true; //매칭되는 타일을 찾음
        }
    }

    

    public void SetArrNumber(int x, int y)
    {
        Row = x;
        Col = y;
    }

}
