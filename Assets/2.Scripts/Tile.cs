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


    public float targetX; //이동시 목표 지점 x좌표
    public float targetY; //이동시 목표 지점 y좌표
    private int previousRow;
    private int previousCol;

    private float swapAngle;

    //상태변수
    public bool isMatched = false;
    public bool isSwapping = false;
    private bool isDropping = false;

    private Vector2 firstTouchPosition;
    private Vector2 secondTouchPosition;
    private Vector2 tempPosition;
    private GameObject otherCharacterTile;

    //필요한 컴포넌트
    private Image image;
    private FindMatches findMatches;



    public int Row { get => row; set => row = value; }
    public int Col { get => col; set => col = value; }
    #endregion



    private void Start()
    {
        image = GetComponent<Image>();
        /*        targetX = (int)transform.position.x;
                targetY = (int)transform.position.y;*/
        previousRow = row;
        previousCol = col;
        findMatches = FindObjectOfType<FindMatches>();
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
        //FindMatches();
        if (isMatched)
        {
            image.color = new Color(.5f, .5f, .5f, 1.0f);
        }

        MoveTileAnimation();
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (BoardManager.instance.currentState == BoardState.MOVE)
        {
            firstTouchPosition = eventData.pointerCurrentRaycast.gameObject.transform.position;
            SoundManager.instance.PlaySE("Select");
            Debug.Log("선택한 타일 : " + eventData.pointerCurrentRaycast.gameObject);
        }

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (BoardManager.instance.currentState == BoardState.MOVE)
        {
            secondTouchPosition = eventData.position;
            Debug.Log("바꿀 타일 : " + eventData.pointerCurrentRaycast.gameObject);
            CalculateSwapAngle();
        }
    }

    private void CalculateSwapAngle()
    {
        //드래그한 거리가 이미지 파일 가로 크기의 0.6이상이라면
        if ((secondTouchPosition - firstTouchPosition).magnitude > this.gameObject.GetComponent<RectTransform>().rect.size.x * 0.6)
        {
            //라디안값이기 때문에 180 / Mathf.PI 곱해주기
            swapAngle = Mathf.Atan2(secondTouchPosition.y - firstTouchPosition.y, secondTouchPosition.x - firstTouchPosition.x) * 180 / Mathf.PI;
            //타일 바꾸기
            SwapTile();
            BoardManager.instance.currentState = BoardState.WAIT;
        }
        else
        {
            BoardManager.instance.currentState = BoardState.MOVE;
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
/*            previousRow = row;
            previousCol = col;*/
            Row += 1;
        }
        else if (swapAngle > 45 && swapAngle <= 135 && Col + 1 < BoardManager.instance.height)
        {
            //위쪽 타일과 교체
            otherCharacterTile = BoardManager.instance.characterTilesBox[Row, Col + 1];
            otherCharacterTile.GetComponent<Tile>().Col -= 1;
/*            previousRow = row;
            previousCol = col;*/
            Col += 1;
        }
        else if ((swapAngle > 135 || swapAngle <= -135) && (Row - 1) >= 0)
        {
            //왼쪽 타일과 교체
            otherCharacterTile = BoardManager.instance.characterTilesBox[Row - 1, Col];
            otherCharacterTile.GetComponent<Tile>().Row += 1;
/*            previousRow = row;
            previousCol = col;*/
            Row -= 1;
        }
        else if (swapAngle < -45 && swapAngle >= -135 && (Col - 1) >= 0)
        {
            //아래쪽 타일과 교체
            otherCharacterTile = BoardManager.instance.characterTilesBox[Row, Col - 1];
            otherCharacterTile.GetComponent<Tile>().Col += 1;
/*            previousRow = row;
            previousCol = col;*/
            Col -= 1;
        }
        else
            return;
        //목표로 하는 타겟을 설정
        BoardManager.instance.SetTargetPos(gameObject, otherCharacterTile);
        SoundManager.instance.PlaySE("Swap"); //바꾸기 소리 실행

        StartCoroutine(CheckMoveCoroutine());
    }

    public IEnumerator CheckMoveCoroutine()
    {
        yield return new WaitForSeconds(.5f);
        if (otherCharacterTile != null)
        {
            if (!isMatched && !otherCharacterTile.GetComponent<Tile>().isMatched)
            {
                otherCharacterTile.GetComponent<Tile>().Row = Row;
                otherCharacterTile.GetComponent<Tile>().Col = Col;
                Row = previousRow;
                Col = previousCol;
                BoardManager.instance.SetTargetPos(gameObject, otherCharacterTile);
                yield return new WaitForSeconds(.5f);
                BoardManager.instance.currentState = BoardState.MOVE;
            }
            else
            {
                BoardManager.instance.DestroyMatches();

            }
            otherCharacterTile = null;
        }
    }



    //타일 이동 애니메이션
    private void MoveTileAnimation()
    {
        //자신과 옮길 목표 위치 사이의 절대값이 0.1 이상이면 계속 Lerp를 실행
        if (Mathf.Abs(targetX - transform.position.x) > .1 ||
            Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(targetX, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .1f);
            if (BoardManager.instance.characterTilesBox[row, col] != this.gameObject)
            {
                BoardManager.instance.characterTilesBox[row, col] = this.gameObject;
            }
            findMatches.FindAllMatches();
        }
        else
        {   //타일 위치 이동 완료
            transform.position = new Vector2(targetX, targetY);
            //저장되어있는 characterTile의 정보를 바꾸기
            BoardManager.instance.characterTilesBox[Row, Col] = gameObject;
            gameObject.name = "S Character [" + Row + ", " + Col + "]";
        }
    }

    void FindMatches()
    {

        if (Row > 0 && Row < BoardManager.instance.width - 1)
        {
            GameObject leftTile = BoardManager.instance.characterTilesBox[Row - 1, Col];
            GameObject rightTile = BoardManager.instance.characterTilesBox[Row + 1, Col];

            if (leftTile == null || rightTile == null)
                return;

            if (leftTile.tag == gameObject.tag && rightTile.tag == gameObject.tag)
            {
                leftTile.GetComponent<Tile>().isMatched = true;
                rightTile.GetComponent<Tile>().isMatched = true;
                isMatched = true;
            }
        }

        if (Col > 0 && Col < BoardManager.instance.height - 1)
        {
            GameObject downTile = BoardManager.instance.characterTilesBox[Row, Col - 1];
            GameObject upTile = BoardManager.instance.characterTilesBox[Row, Col + 1];

            if (downTile == null || upTile == null)
                return;

            if (downTile.tag == gameObject.tag && upTile.tag == gameObject.tag)
            {
                downTile.GetComponent<Tile>().isMatched = true;
                upTile.GetComponent<Tile>().isMatched = true;
                isMatched = true;
            }
        }
    }

    public void SetArrNumber(int x, int y)
    {
        Row = x;
        Col = y;
    }
}
