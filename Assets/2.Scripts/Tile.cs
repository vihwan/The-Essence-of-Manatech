using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public enum CharacterKinds
{
    Frost,
    Pluto,
    Lantern,
    Fluore,
    test1,
    test2,
    test3
}
public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    #region Field Variable

    //고유 정보
    [Header("Tile Variables")]
    [SerializeField] private int row;
    [SerializeField] private int col;
    public float tilePositionX; //현재 캐릭터 타일의 좌표 X
    public float tilePositionY; //현재 캐릭터 타일의 좌표 Y
    public float targetX; //이동시 목표 지점 x좌표
    public float targetY; //이동시 목표 지점 y좌표

    private float swapAngle;

    //상태변수
    public bool isMatched = false;
    public bool isSwapping = false;

    private Vector2 firstTouchPosition;
    private Vector2 secondTouchPosition;
    private Vector2 tempPosition;
    private GameObject otherCharacterTile;

    //필요한 컴포넌트
    private Image image;

    public int Row { get => row; set => row = value; }
    public int Col { get => col; set => col = value; }
    #endregion



    private void Start()
    {
        image = GetComponent<Image>();
        tilePositionX = GetComponentInParent<BackgroundTile>().positionX;
        tilePositionY = GetComponentInParent<BackgroundTile>().positionY;

        SetCharacterTileTag();
    }

    //캐릭터 타일의 태그를 설정해주는 함수
    private void SetCharacterTileTag()
    {
        if (image.sprite.name == "JackFrost")
        {
            gameObject.tag = CharacterKinds.Frost.ToString();
        }
        else if (image.sprite.name == "JackOLantern")
        {
            gameObject.tag = CharacterKinds.Lantern.ToString();
        }
        else if (image.sprite.name == "FluoreSang")
        {
            gameObject.tag = CharacterKinds.Fluore.ToString();
        }
        else if (image.sprite.name == "PlutoNyang")
        {
            gameObject.tag = CharacterKinds.Pluto.ToString();
        }
        else if (image.sprite.name == "characters_0001")
        {
            gameObject.tag = CharacterKinds.test1.ToString();
        }
        else if (image.sprite.name == "characters_0003")
        {
            gameObject.tag = CharacterKinds.test2.ToString();
        }
        else if (image.sprite.name == "characters_0005")
        {
            gameObject.tag = CharacterKinds.test3.ToString();
        }
        else
            return;
    }

    private void Update()
    {
        if (isSwapping)
            SwapAnimation();
        else
        {
            if (!BoardManager.instance.CanDropping && !BoardManager.instance.IsShifting)
            {
                CheckMatching();
                if (isMatched)
                    BoardManager.instance.FindDestroyMatches();
            }
        }
    }

    //타일 이동 애니메이션
    private void SwapAnimation()
    {
        //자신과 옮길 목표 위치 사이의 절대값이 0.1 이상이면 계속 Lerp를 실행
        if (Mathf.Abs(targetX - transform.position.x) > .1 ||
            Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(targetX, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .1f);
        }
        else
        {   //타일 위치 이동 완료
            transform.position = new Vector2(targetX, targetY);

            //옮겨진 타일 오브젝트를 해당 BackTile 오브젝트로 종속시키기
            gameObject.transform.SetParent(BoardManager.instance.backTilesBox[Row, Col].transform);
            //저장되어있는 characterTile의 정보를 바꾸기
            BoardManager.instance.characterTilesBox[Row, Col] = gameObject;
            gameObject.name = "S Character [" + Row + ", " + Col + "]";
            isSwapping = false;
            BoardManager.instance.IsShifting = false;
            //TODO : 나중에 이 코드 위치 수정
            BoardManager.instance.currentState = TileMoveState.CANMOVE;
        }
    }


    //타일의 매칭을 검사하는 함수
    //서로의 태그를 비교하여 일치하면 isMatched가 true가 된다.
    //TODO : 2개만 모여도 isMatched가 true가 되는 현상이 있음. 수정요망
    private void CheckMatching()
    {
        if (Row > 0 && Row < BoardManager.instance.width - 1)
        {
            GameObject leftTile = BoardManager.instance.characterTilesBox[Row - 1, Col];
            GameObject rightTile = BoardManager.instance.characterTilesBox[Row + 1, Col];

            if (leftTile == null || rightTile == null)
                return;

            if (leftTile.tag == this.gameObject.tag && rightTile.tag == this.gameObject.tag)
            {
                leftTile.GetComponent<Tile>().isMatched = true;
                rightTile.GetComponent<Tile>().isMatched = true;
                isMatched = true;
                Debug.Log("매칭 찾음");
            }
        }
        if (Col > 0 && Col < BoardManager.instance.height - 1)
        {
            GameObject DownTile = BoardManager.instance.characterTilesBox[Row, Col - 1];
            GameObject UpTile = BoardManager.instance.characterTilesBox[Row, Col + 1];

            if (UpTile == null || DownTile == null)
                return;

            if (DownTile.tag == this.gameObject.tag && UpTile.tag == this.gameObject.tag)
            {
                DownTile.GetComponent<Tile>().isMatched = true;
                UpTile.GetComponent<Tile>().isMatched = true;
                isMatched = true;
                Debug.Log("매칭 찾음");
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (BoardManager.instance.currentState == TileMoveState.CANMOVE)
        {
            firstTouchPosition = eventData.pointerCurrentRaycast.gameObject.transform.position;
            SoundManager.instance.PlaySE("Select");
            Debug.Log("선택한 타일 : " + eventData.pointerCurrentRaycast.gameObject);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        secondTouchPosition = eventData.position;
        Debug.Log("바꿀 타일 : " + eventData.pointerCurrentRaycast.gameObject);
        CalculateSwapAngle();
    }

    private void CalculateSwapAngle()
    {
        //라디안값이기 때문에 180 / Mathf.PI 곱해주기
        swapAngle = Mathf.Atan2(secondTouchPosition.y - firstTouchPosition.y, secondTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;

        //드래그한 거리가 이미지 파일 가로 크기의 0.6이상이라면
        if ((secondTouchPosition - firstTouchPosition).magnitude > this.gameObject.GetComponent<RectTransform>().rect.size.x * 0.6)
        {
            //타일 바꾸기
            SwapTile();
            BoardManager.instance.currentState = TileMoveState.WAIT;
        }
        else
        {
            BoardManager.instance.currentState = TileMoveState.CANMOVE;
        }
    }

    //타일 위치 서로 바꾸기
    public void SwapTile()
    {
        if (swapAngle > -45 && swapAngle <= 45 && Row + 1 < BoardManager.instance.width)
        {
            //오른쪽 타일과 교체
            otherCharacterTile = BoardManager.instance.characterTilesBox[Row + 1, Col];
            otherCharacterTile.GetComponent<Tile>().Row -= 1;
            Row += 1;
        }
        else if (swapAngle > 45 && swapAngle <= 135 && Col + 1 < BoardManager.instance.height)
        {
            //위쪽 타일과 교체
            otherCharacterTile = BoardManager.instance.characterTilesBox[Row, Col + 1];
            otherCharacterTile.GetComponent<Tile>().Col -= 1;
            Col += 1;
        }
        else if ((swapAngle > 135 || swapAngle <= -135) && (Row - 1) >= 0)
        {
            //왼쪽 타일과 교체
            otherCharacterTile = BoardManager.instance.characterTilesBox[Row - 1, Col];
            otherCharacterTile.GetComponent<Tile>().Row += 1;
            Row -= 1;
        }
        else if (swapAngle < -45 && swapAngle >= -135 && (Col - 1) >= 0)
        {
            //아래쪽 타일과 교체
            otherCharacterTile = BoardManager.instance.characterTilesBox[Row, Col - 1];
            otherCharacterTile.GetComponent<Tile>().Col += 1;
            Col -= 1;
        }
        else
            return;

        isSwapping = true;

        //목표로 하는 타겟을 설정
        BoardManager.instance.SetTargetPos(gameObject, otherCharacterTile);
        SoundManager.instance.PlaySE("Swap"); //바꾸기 소리 실행
    }


    public void SetArrNumber(int x, int y)
    {
        Row = x;
        Col = y;
    }

    // private IEnumerator DropTile(){

    //     while (Col > 0 && BoardManager.instance.characterTilesBox[Row, Col - 1] == null)
    //     {
    //         Col--;
    //         targetX = BoardManager.instance.backTilesBox[Row, Col].GetComponent<BackgroundTile>().positionX;
    //         targetY = BoardManager.instance.backTilesBox[Row, Col].GetComponent<BackgroundTile>().positionY;
    //         isSwapping = true;
    //     }

    //     yield return new WaitForSeconds(0.4f);
    // }
}
