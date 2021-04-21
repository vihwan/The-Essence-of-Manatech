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
    private bool matchFound = false;
    private bool isSwapping = false;

    private Vector2 firstTouchPosition;
    private Vector2 secondTouchPosition;
    private GameObject secondTile;

    //필요한 컴포넌트
    private Image image;


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
            BoardManager.instance.tiles[row, col] = this.gameObject;
            gameObject.name = BoardManager.instance.SetTileName(row, col);
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
        {
            transform.position = secondTile.GetComponent<Tile>().originPos;
            secondTile.transform.position = originPos;
            ReInitOriginPos(); //각 타일의 OriginPos를 초기화           
            Matching();
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

    private void Matching()
    {

        gameObject.GetComponent<Tile>().ClearAllMatches();
        if (BoardManager.instance.MatchFoundCount == 0)
            GUIManager.instance.ComboCounter = 0;

    }


    //타일 위치 서로 바꾸기
    public void SwapTile()
    {
        if (swapAngle > -45 && swapAngle <= 45 && row + 1 < BoardManager.instance.xSize)
        {
            //오른쪽 타일과 교체
            secondTile = BoardManager.instance.GetTile()[row + 1, col];
            secondTile.GetComponent<Tile>().row -= 1;
            row += 1;
        }
        else if (swapAngle > 45 && swapAngle <= 135 && col + 1 < BoardManager.instance.ySize)
        {
            //위쪽 타일과 교체
            secondTile = BoardManager.instance.GetTile()[row, col + 1];
            secondTile.GetComponent<Tile>().col -= 1;
            col += 1;
        }
        else if ((swapAngle > 135 || swapAngle <= -135) && (row - 1) >= 0)
        {
            //왼쪽 타일과 교체
            secondTile = BoardManager.instance.GetTile()[row - 1, col];
            secondTile.GetComponent<Tile>().row += 1;
            row -= 1;
        }
        else if (swapAngle < -45 && swapAngle >= -135 && (col - 1) >= 0)
        {
            //아래쪽 타일과 교체
            secondTile = BoardManager.instance.GetTile()[row, col - 1];
            secondTile.GetComponent<Tile>().col += 1;
            col -= 1;
        }
        else
            return;

        isSwapping = true;
        SoundManager.instance.PlaySE("Swap"); //바꾸기 소리 실행
    }



    //매칭이 되었는지를 검사하는 함수
    private List<GameObject> FindMatch(Vector3 castDir)
    {
        List<GameObject> matchingTiles = new List<GameObject>();
        RaycastHit hitInfo;
        bool isMatch = Physics.Raycast(transform.position, castDir, out hitInfo);
        if (isMatch)
        {
            while (hitInfo.collider != null && hitInfo.collider.GetComponent<Image>().sprite == image.sprite)
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
               Destroy(matchingTiles[i].GetComponent<Image>().gameObject);
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
            BoardManager.instance.canFindNullTiles = true;

            //비어있는 타일을 검색
           // BoardManager.instance.FindNullTiles();
            SoundManager.instance.PlaySE("Clear");

            GUIManager.instance.ComboCounter++;
            BoardManager.instance.MatchFoundCount++;
            matchFound = false;
            Destroy(gameObject);
        }
    }

    public void SetArrNumber(int x, int y)
    {
        row = x;
        col = y;
    }
}
