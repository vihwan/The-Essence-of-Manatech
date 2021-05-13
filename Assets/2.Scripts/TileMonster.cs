using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum MonsterKinds
{
    M_One,
    M_Two,
    M_Three,
    M_Four,
    M_Five,
    M_Six
}


public class TileMonster : MonoBehaviour
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

    //상태변수
    public bool isMatched = false;
    public bool isShifting = false;
    public bool canShifting = false;



    //Vector
    private Vector2 tempPosition;

    private GameObject otherCharacterTile;

    //Component
    private Image image;

    private FindMatches findMatches;

    //Property
    public int Row { get => row; set => row = value; }

    public int Col { get => col; set => col = value; }

    #endregion Field Variable

    // Start is called before the first frame update
    private void Start()
    {
        image = GetComponent<Image>();
        findMatches = FindObjectOfType<FindMatches>();

        SetCharacterTileTag();
        isMatched = false;
        isShifting = false;
        canShifting = true;
    }

    //캐릭터 타일의 태그를 설정해주는 함수
    private void SetCharacterTileTag()
    {
        if (image.sprite.name == "1")
        {
            gameObject.tag = MonsterKinds.M_One.ToString();
        }
        else if (image.sprite.name == "2")
        {
            gameObject.tag = MonsterKinds.M_Two.ToString();
        }
        else if (image.sprite.name == "3")
        {
            gameObject.tag = MonsterKinds.M_Three.ToString();
        }
        else if (image.sprite.name == "4")
        {
            gameObject.tag = MonsterKinds.M_Four.ToString();
        }
        else if (image.sprite.name == "5")
        {
            gameObject.tag = MonsterKinds.M_Five.ToString();
        }
        else if (image.sprite.name == "6")
        {
            gameObject.tag = MonsterKinds.M_Six.ToString();
        }
        else
            return;
    }


    // Update is called once per frame
    private void Update()
    {
        if (isMatched)
        {
            image.color = new Color(.5f, .5f, .5f, 1.0f);
            BoardManagerMonster.instance.currentState = MonsterState.WAIT;
        }

        if (canShifting == true)
            MoveTileAnimation();
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
            transform.position = Vector2.Lerp(transform.position, tempPosition, .2f);
            if (BoardManager.instance.characterTilesBox[Row, Col] != this.gameObject)
            {
                BoardManager.instance.characterTilesBox[Row, Col] = this.gameObject;
            }
            //findMatches.FindAllMatches();
        }
        else
        {   //타일 위치 이동 완료
            transform.position = new Vector2(targetX, targetY);
            //저장되어있는 characterTile의 정보를 바꾸기
            BoardManager.instance.characterTilesBox[Row, Col] = gameObject;
            gameObject.name = "S Monster [" + Row + ", " + Col + "]";
            isShifting = false;
            canShifting = false;
        }
    }

    //타일 위치 서로 바꾸기
    public void SwapTile(Vector3 direction)
    {
        if (direction == Vector3.right)
        {
            //오른쪽 타일과 교체
            otherCharacterTile = BoardManagerMonster.instance.monsterTilesBox[Row + 1, Col];
            otherCharacterTile.GetComponent<TileMonster>().Row -= 1;
            previousRow = Row;
            previousCol = Col;
            Row += 1;
        }
        else if (direction == Vector3.up)
        {
            //위쪽 타일과 교체
            otherCharacterTile = BoardManagerMonster.instance.monsterTilesBox[Row, Col + 1];
            otherCharacterTile.GetComponent<TileMonster>().Col -= 1;
            previousRow = Row;
            previousCol = Col;
            Col += 1;
        }
        else if (direction == Vector3.left)
        {
            //왼쪽 타일과 교체
            otherCharacterTile = BoardManagerMonster.instance.monsterTilesBox[Row - 1, Col];
            otherCharacterTile.GetComponent<TileMonster>().Row += 1;
            previousRow = Row;
            previousCol = Col;
            Row -= 1;
        }
        else if (direction == Vector3.down)
        {
            //아래쪽 타일과 교체
            otherCharacterTile = BoardManagerMonster.instance.monsterTilesBox[Row, Col - 1];
            otherCharacterTile.GetComponent<TileMonster>().Col += 1;
            previousRow = Row;
            previousCol = Col;
            Col -= 1;
        }
        else
            return;
        //목표로 하는 타겟을 설정
        BoardManagerMonster.instance.SetTargetPos(gameObject, otherCharacterTile);

        StartCoroutine(CheckMoveCoroutine());
    }

    public IEnumerator CheckMoveCoroutine()
    {
        yield return new WaitForSeconds(.7f);
        if (otherCharacterTile != null)
        {
            if (!isMatched && !otherCharacterTile.GetComponent<TileMonster>().isMatched)
            {
                otherCharacterTile.GetComponent<TileMonster>().Row = Row;
                otherCharacterTile.GetComponent<TileMonster>().Col = Col;
                Row = previousRow;
                Col = previousCol;
                BoardManagerMonster.instance.SetTargetPos(gameObject, otherCharacterTile);
                yield return new WaitForSeconds(.5f);
            }
            else
            {
                BoardManagerMonster.instance.DestroyMatches();
                BoardManagerMonster.instance.currentState = MonsterState.WAIT;
            }
            otherCharacterTile = null;
        }
    }


    public void SetArrNumber(int x, int y)
    {
        Row = x;
        Col = y;
    }
}
