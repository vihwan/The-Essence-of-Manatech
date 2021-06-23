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

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
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

    private CharacterKinds kind;

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
            if (GameManager.instance.GameState == GameState.READY)
                moveSpeed = 5f * Time.deltaTime;
            else
                moveSpeed = 17f * Time.deltaTime;

            return moveSpeed;
        }
    }

    public CharacterKinds Kind { get => kind; set => kind = value; }

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
            Kind = CharacterKinds.Frost;
            gameObject.tag = CharacterKinds.Frost.ToString();
        }
        else if (image.sprite.name == "3잭오랜턴")
        {
            Kind = CharacterKinds.Lantern;
            gameObject.tag = CharacterKinds.Lantern.ToString();
        }
        else if (image.sprite.name == "1플로레")
        {
            Kind = CharacterKinds.Fluore;
            gameObject.tag = CharacterKinds.Fluore.ToString();
        }
        else if (image.sprite.name == "4플루토")
        {
            Kind = CharacterKinds.Pluto;
            gameObject.tag = CharacterKinds.Pluto.ToString();
        }
        else if (image.sprite.name == "7망치스패너")
        {
            Kind = CharacterKinds.HammerSpanner;
            gameObject.tag = CharacterKinds.HammerSpanner.ToString();
        }
        else if (image.sprite.name == "9슈르르")
        {
            Kind = CharacterKinds.Shururu;
            gameObject.tag = CharacterKinds.Shururu.ToString();
        }
        else if (image.sprite.name == "7잭오할로윈")
        {
            Kind = CharacterKinds.Bomb;
            gameObject.tag = CharacterKinds.Bomb.ToString();
        }
        else if (image.sprite.name == "8빗자루_2")
        {
            Kind = CharacterKinds.Bloom;
            gameObject.tag = CharacterKinds.Bloom.ToString();
        }
        else if (image.sprite.name == "Lolipop")
        {
            Kind = CharacterKinds.Lolipop;
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
        if (GameManager.instance.GameState != GameState.PLAYING)
            return;

        //21.06.23 추가 - 타일의 이중이동 조작을 막기 위함
        if (BoardManager.instance.currentState == PlayerState.MOVE 
            && BoardManager.instance.IsCanControlTile == true)
        {
            GameObject currentTile = eventData.pointerCurrentRaycast.gameObject;
            previousTile = currentTile.GetComponent<Tile>();

            if (hintManager != null)
            {
                hintManager.DestroyHint();
            }

            if (UtilHelper.HasComponent<Tile>(currentTile))
            {
                //만약 클릭한 타일이 폭탄이라면 폭탄 실행
                if (currentTile.CompareTag("Bomb"))
                {
                    SoundManager.instance.PlayEffectSound("jackohalloween_03");
                    BoardManager.instance.JackBombIsMatch(Row, Col);
                    BoardManager.instance.DestroyMatches();
                    return;
                }

                if (currentTile.GetComponent<Tile>().isSealed)
                {
                    Debug.Log("<color=#E36250>봉인당한 타일</color>은 옮겨지지 않습니다.");
                    //previousTile = eventData.pointerCurrentRaycast.gameObject.GetComponent<Tile>();
                    return;
                }

                firstTouchPosition = currentTile.transform.position;
                SoundManager.instance.PlaySE("Select");
            }
        }
        else
            return;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (BoardManager.instance.currentState != PlayerState.MOVE)
            return;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GameManager.instance.GameState != GameState.PLAYING)
            return;

        if (BoardManager.instance.currentState == PlayerState.MOVE 
            && BoardManager.instance.IsCanControlTile == true)
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

                if (UtilHelper.HasComponent<Tile>(currentTile_GO))
                {
                    //만약 옮겨질 두번째 타일이 봉인 상태가 아니라면
                    if (!currentTile_GO.GetComponent<Tile>().isSealed)
                    {
/*                        //옮겨질 두번째 타일이 인접한 타일이 아니라면
                        if (!IsNearTile(currentTile_GO))
                            return;*/

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
                    else //봉인 상태라면
                    {
      

                        Debug.Log("<color=#E36250>봉인당한 타일</color>은 옮겨지지 않습니다.");
                        return;
                    }

                    //BoardManager.instance.currentState = PlayerState.WAIT; //유저 조작 대기 상태
                    secondTouchPosition = eventData.position;
                    //Debug.Log("바꿀 타일 : " + eventData.pointerCurrentRaycast.gameObject);
                    CalculateSwapAngle();
                }
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
            //BoardManager.instance.currentState = PlayerState.WAIT;
        }
        else
        {
            //BoardManager.instance.currentState = PlayerState.MOVE;
        }
    }

    //타일 위치 서로 바꾸기
    public void SwapTile()
    {
        if (swapAngle > -45 && swapAngle <= 45 && Row + 1 < BoardManager.instance.width)
        {
            //오른쪽 타일과 교체
            otherCharacterTile = BoardManager.instance.characterTilesBox[Row + 1, Col];
            if(otherCharacterTile.GetComponent<Tile>().isSealed)
            {
                print("인접한 봉인된 타일은 교체되지 않습니다.");
                return;
            }

            otherCharacterTile.GetComponent<Tile>().Row -= 1;
            previousRow = Row;
            previousCol = Col;
            Row += 1;
        }
        else if (swapAngle > 45 && swapAngle <= 135 && Col + 1 < BoardManager.instance.height)
        {
            //위쪽 타일과 교체
            otherCharacterTile = BoardManager.instance.characterTilesBox[Row, Col + 1];
            if (otherCharacterTile.GetComponent<Tile>().isSealed)
            {
                print("인접한 봉인된 타일은 교체되지 않습니다.");
                return;
            }
            otherCharacterTile.GetComponent<Tile>().Col -= 1;
            previousRow = Row;
            previousCol = Col;
            Col += 1;
        }
        else if ((swapAngle > 135 || swapAngle <= -135) && (Row - 1) >= 0)
        {
            //왼쪽 타일과 교체
            otherCharacterTile = BoardManager.instance.characterTilesBox[Row - 1, Col];
            if (otherCharacterTile.GetComponent<Tile>().isSealed)
            {
                print("인접한 봉인된 타일은 교체되지 않습니다.");
                return;
            }
            otherCharacterTile.GetComponent<Tile>().Row += 1;
            previousRow = Row;
            previousCol = Col;
            Row -= 1;
        }
        else if (swapAngle < -45 && swapAngle >= -135 && (Col - 1) >= 0)
        {
            //아래쪽 타일과 교체
            otherCharacterTile = BoardManager.instance.characterTilesBox[Row, Col - 1];
            if (otherCharacterTile.GetComponent<Tile>().isSealed)
            {
                print("인접한 봉인된 타일은 교체되지 않습니다.");
                return;
            }
            otherCharacterTile.GetComponent<Tile>().Col += 1;
            previousRow = Row;
            previousCol = Col;
            Col -= 1;
        }
        else
            return;


        //목표로 하는 타겟을 설정
        BoardManager.instance.SetTargetPos(gameObject, otherCharacterTile);
        //SoundManager.instance.PlaySE("Swap"); //바꾸기 소리 실행
        PlayerSound.PlayMoveTile();
        //21.06.23 추가 
        BoardManager.instance.IsCanControlTile = false;
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
                PlayerSound.PlayFailTileMatch();
            }
            else
            {
                BoardManager.instance.DestroyMatches();
                BoardManager.instance.currentState = PlayerState.WAIT;
            }
            otherCharacterTile = null;
        }
        BoardManager.instance.IsCanControlTile = true;
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

            //2021.06.23 가끔씩 간헐적으로 PlayerState가 WAIT에서 멈추는 버그로 인해 임시로 추가한 코드
            BoardManager.instance.currentState = PlayerState.MOVE;
        }
    }

    public void SetArrNumber(int x, int y)
    {
        Row = x;
        Col = y;
    }
}