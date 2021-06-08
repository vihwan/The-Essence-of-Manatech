using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum CharacterKinds
{
    Frost,
    Pluto,
    Lantern,
    Fluore,
    HammerSpanner,
    Shururu,
    Bloom,
    Bomb,
    Lolipop
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
    private int previousRow; //타일 이동시 이전 Row 임시저장
    private int previousCol; //타일 이동시 이전 Col 임시저장
    private float swapAngle; //마우스 조작 방향

    private Tile previousTile;
    private GameObject currentTile_GO;
    private GameObject otherCharacterTile;

    //상태변수

    public bool isMatched = false;
    public bool isShifting = false;
    public bool canShifting = false;
    public bool isSealed = false; // 데바스타르 인간 스킬을 당한 상태인가?
    public bool isActiveNen = false; // 데바스타르 스킬2 넨 활성화 상태인가?

    //Vector
    private Vector2 firstTouchPosition;
    private Vector2 secondTouchPosition;
    private Vector2 tempPosition;

    //Component
    private Image image;

    private FindMatches findMatches;
    private ComboSystem comboSystem;
    private HintManager hintManager;
    public bool isCheckMoveUpdate = false;
    private float moveSpeed;


    //Property
    public int Row { get => row; set => row = value; }

    public int Col { get => col; set => col = value; }
    public float MoveSpeed
    {
        get
        {
            if (GameManager.instance.GameState == GameState.BEGIN)
                moveSpeed = .05f;
            else
                moveSpeed = .3f;

            return moveSpeed;
        }
    }

    #endregion Field Variable

    private void Start()
    {
        image = GetComponent<Image>();
        findMatches = FindObjectOfType<FindMatches>();
        comboSystem = FindObjectOfType<ComboSystem>();
        hintManager = FindObjectOfType<HintManager>();
        previousTile = GetComponent<Tile>();

        SetCharacterTileTag();
        isMatched = false;
        isShifting = false;
        canShifting = true;
    }

    //캐릭터 타일의 태그를 설정해주는 함수
    private void SetCharacterTileTag()
    {
        if (image.sprite.name == "2잭프로스트")
        {
            gameObject.tag = CharacterKinds.Frost.ToString();
        }
        else if (image.sprite.name == "3잭오랜턴")
        {
            gameObject.tag = CharacterKinds.Lantern.ToString();
        }
        else if (image.sprite.name == "1플로레")
        {
            gameObject.tag = CharacterKinds.Fluore.ToString();
        }
        else if (image.sprite.name == "4플루토")
        {
            gameObject.tag = CharacterKinds.Pluto.ToString();
        }
        else if (image.sprite.name == "7망치스패너")
        {
            gameObject.tag = CharacterKinds.HammerSpanner.ToString();
        }
        else if (image.sprite.name == "9슈르르")
        {
            gameObject.tag = CharacterKinds.Shururu.ToString();
        }
        else if (image.sprite.name == "7잭오할로윈")
        {
            gameObject.tag = CharacterKinds.Bomb.ToString();
        }
        else if (image.sprite.name == "8빗자루_2")
        {
            gameObject.tag = CharacterKinds.Bloom.ToString();
        }
        else if (image.sprite.name == "Lolipop")
        {
            gameObject.tag = CharacterKinds.Lolipop.ToString();
        }
        else
            return;
    }

    private void Update()
    {
        if (isMatched)
        {
            image.color = new Color(.5f, .5f, .5f, 1.0f);
            BoardManager.instance.currentState = PlayerState.WAIT;
        }

        if (canShifting == true)
            MoveTileAnimation();
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        print(eventData.position);

        if(GameManager.instance.GameState != GameState.PLAYING)
            return;

        if (BoardManager.instance.currentState == PlayerState.MOVE)
        {
            if (hintManager != null)
            {
                hintManager.DestroyHint();
            }

            //만약 클릭한 타일이 폭탄이라면 폭탄 실행
            if (eventData.pointerCurrentRaycast.gameObject.CompareTag("Bomb"))
            {
                BoardManager.instance.JackBombIsMatch(Row, Col);
                BoardManager.instance.DestroyMatches();
                return;
            }

            if (eventData.pointerCurrentRaycast.gameObject.GetComponent<Tile>().isSealed)
            {
                Debug.Log("<color=#E36250>봉인당한 타일</color>은 옮겨지지 않습니다.");
                previousTile = eventData.pointerCurrentRaycast.gameObject.GetComponent<Tile>();
                return;
            }

            firstTouchPosition = eventData.pointerCurrentRaycast.gameObject.transform.position;
            SoundManager.instance.PlaySE("Select");
            //Debug.Log("선택한 타일 : " + eventData.pointerCurrentRaycast.gameObject);
        }
        else
            return;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameManager.instance.GameState != GameState.PLAYING)
            return;

        if (BoardManager.instance.currentState == PlayerState.MOVE)
        {
            currentTile_GO = eventData.pointerCurrentRaycast.gameObject;

            if (currentTile_GO != null)
            {
                //만약 바꾸고 싶은 타일이 폭탄이라면
                if (currentTile_GO.CompareTag("Bomb"))
                {
                    //옮기지 못한다!!!
                    Debug.Log("<color=#C86200>폭탄은 옮겨지지 않습니다.</color>");
                    return;
                }

                if (!currentTile_GO.GetComponent<Tile>().isSealed)
                {
                    if (previousTile != null)
                    {
                        if (previousTile.isSealed)
                        {
                            Debug.Log("<color=#E36250>봉인당한 타일</color>은 옮겨지지 않습니다.");
                            previousTile = null;
                            return;
                        }
                    }
                }
                else
                {
                    Debug.Log("<color=#E36250>봉인당한 타일</color>은 옮겨지지 않습니다.");
                    return;
                }

                BoardManager.instance.currentState = PlayerState.WAIT; //유저 조작 대기 상태
                secondTouchPosition = eventData.position;
                //Debug.Log("바꿀 타일 : " + eventData.pointerCurrentRaycast.gameObject);
                CalculateSwapAngle();
            }
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
            BoardManager.instance.currentState = PlayerState.WAIT;
        }
        else
        {
            BoardManager.instance.currentState = PlayerState.MOVE;
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
            previousRow = Row;
            previousCol = Col;
            Row += 1;
        }
        else if (swapAngle > 45 && swapAngle <= 135 && Col + 1 < BoardManager.instance.height)
        {
            //위쪽 타일과 교체
            otherCharacterTile = BoardManager.instance.characterTilesBox[Row, Col + 1];
            otherCharacterTile.GetComponent<Tile>().Col -= 1;
            previousRow = Row;
            previousCol = Col;
            Col += 1;
        }
        else if ((swapAngle > 135 || swapAngle <= -135) && (Row - 1) >= 0)
        {
            //왼쪽 타일과 교체
            otherCharacterTile = BoardManager.instance.characterTilesBox[Row - 1, Col];
            otherCharacterTile.GetComponent<Tile>().Row += 1;
            previousRow = Row;
            previousCol = Col;
            Row -= 1;
        }
        else if (swapAngle < -45 && swapAngle >= -135 && (Col - 1) >= 0)
        {
            //아래쪽 타일과 교체
            otherCharacterTile = BoardManager.instance.characterTilesBox[Row, Col - 1];
            otherCharacterTile.GetComponent<Tile>().Col += 1;
            previousRow = Row;
            previousCol = Col;
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
        isCheckMoveUpdate = true;
        yield return new WaitForSeconds(.7f);
        yield return new WaitUntil(() => !findMatches.IsMatchFinding()); //WaitWhile
        if (otherCharacterTile != null)
        {
            //이동한 두 타일 둘다 매칭 상태가 아니라면 다시 원래 자리로 돌린다.
            if (!isMatched && !otherCharacterTile.GetComponent<Tile>().isMatched)
            {
                otherCharacterTile.GetComponent<Tile>().Row = Row;
                otherCharacterTile.GetComponent<Tile>().Col = Col;
                Row = previousRow;
                Col = previousCol;
                BoardManager.instance.SetTargetPos(gameObject, otherCharacterTile);
                comboSystem.PlayComboFailAnimation();
                yield return new WaitForSeconds(.5f);
                comboSystem.ComboCounter = 0;
            }
            else
            {
                BoardManager.instance.DestroyMatches();
                BoardManager.instance.currentState = PlayerState.WAIT;
            }
            otherCharacterTile = null;
        }
        isCheckMoveUpdate = false;
    }

    //타일 이동 애니메이션
    //목표로 하는 지점이 바뀔 때 마다 애니메이션이 실행된다.
    private void MoveTileAnimation()
    {
        //자신과 옮길 목표 위치 사이의 절대값이 0.1 이상이면 계속 Lerp를 실행
        if (Mathf.Abs(targetX - transform.position.x) > .1 ||
            Mathf.Abs(targetY - transform.position.y) > .1)
        {
            isShifting = true;
            tempPosition = new Vector2(targetX, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, MoveSpeed);
            if (BoardManager.instance.characterTilesBox[Row, Col] != this.gameObject)
            {
                BoardManager.instance.characterTilesBox[Row, Col] = this.gameObject;
            }
        }
        else
        {   //타일 위치 이동 완료
            transform.position = new Vector2(targetX, targetY);
            //저장되어있는 characterTile의 정보를 바꾸기
            BoardManager.instance.characterTilesBox[Row, Col] = gameObject;
            gameObject.name = "S Character [" + Row + ", " + Col + "]";
            isShifting = false;
            canShifting = false;
            findMatches.FindAllMatches();
        }
    }

    public void SetArrNumber(int x, int y)
    {
        Row = x;
        Col = y;
    }
}