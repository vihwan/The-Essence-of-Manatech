using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

internal class MoveTo
{
    public GameObject pickUpTile;
    public Vector2 moveDir;
}

public class BoardManagerMonster : MonoBehaviour
{
    //Singleton
    public static BoardManagerMonster instance;

    public int width;
    public int height;

    public List<Sprite> monsterTilesList = new List<Sprite>(); //에디터에서 사용할 타일들을 저장하는 리스트
    public GameObject[,] monsterTilesBox; //AI의 타일 보관하는 배열
    public GameObject monsterTilePrefab;  //Tile Prefab
    public MonsterState currentState;

    private float elaspedTime = 0f;
    private float timeStandard = 4f;

    private FindMatchesMonster findMatchesMonster;
    private CreateBackTilesMonster createBoardMonster;
    private MonsterStatusController monsterStatusController;
    private DevaSkill1 devaSkill1;
    private Vector2 moveDirF;

    public void Init()
    {
        instance = GetComponent<BoardManagerMonster>();
        findMatchesMonster = FindObjectOfType<FindMatchesMonster>();
        createBoardMonster = FindObjectOfType<CreateBackTilesMonster>();
        monsterStatusController = FindObjectOfType<MonsterStatusController>();
        devaSkill1 = FindObjectOfType<DevaSkill1>();


        monsterTilesBox = new GameObject[width, height];
        Vector2 offset = monsterTilePrefab.GetComponent<RectTransform>().rect.size;
        CreateTiles(offset.x, offset.y); //타일 프리팹의 사이즈를 매개변수로 보드 생성
    }

    private void Update()
    {
        CanMovePlayerState();
        if (currentState == MonsterState.MOVE)
        {
            elaspedTime += Time.deltaTime;
            if (elaspedTime >= timeStandard)
            {
                MoveTile();
                elaspedTime = 0f;
            }
        }
    }

    //타일을 생성하는 함수
    private void CreateTiles(float xOffset, float yOffset)
    {
        //BoardManager 위치에 따라 시작점이 달라짐
        //왼쪽 하단을 기준으로.
        float startX = transform.position.x;
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[height];
        Sprite previousBelow = null;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject monsterTile = Instantiate(monsterTilePrefab,
                                new Vector2(startX + (xOffset * x),
                                startY + (yOffset * y * 2)),
                                Quaternion.identity);
                monsterTile.transform.SetParent(transform);
                monsterTile.gameObject.name = "Monster [" + x + ", " + y + "]";
                monsterTile.GetComponent<TileMonster>().SetArrNumber(x, y);
                monsterTile.GetComponent<TileMonster>().targetX = startX + (xOffset * x);
                monsterTile.GetComponent<TileMonster>().targetY = startY + (yOffset * y);
                monsterTilesBox[x, y] = monsterTile;

                CreateTileSprite(y, monsterTile, previousLeft, ref previousBelow);
            }
        }
    }

    //타일의 스프라이트를 입혀주는 함수. 스프라이트에 따라 타일의 속성(태그)가 바뀐다. (Tile Class)
    private void CreateTileSprite(int col, GameObject gameObject, Sprite[] previousLeft, ref Sprite previousBelow)
    {
        List<Sprite> possibleCharacters = new List<Sprite>(); //가능한캐릭터들의 리스트를 생성
        possibleCharacters.AddRange(monsterTilesList); //모든 캐릭터들을 리스트에 때려넣음
        possibleCharacters.Remove(previousLeft[col]); //이전의 왼쪽에 해당되는 열 리스트들을 전부 삭제
        possibleCharacters.Remove(previousBelow);   //이전의 아래에 해당되는 캐릭터를 삭제

        Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)]; //저장된 캐릭터들을 랜덤으로 받아서
        gameObject.GetComponent<Image>().sprite = newSprite; //생성된 타일에 대입한다.
        previousLeft[col] = newSprite;
        previousBelow = newSprite;
    }

    //가상으로 옮긴 타일을 교환하는 함수
    private void SwitchPieces(int row, int col, Vector2 direction)
    {
        GameObject holder = monsterTilesBox[row + (int)direction.x, col + (int)direction.y];
        monsterTilesBox[row + (int)direction.x, col + (int)direction.y] = monsterTilesBox[row, col];
        monsterTilesBox[row, col] = holder;
    }

    //가상으로 옮긴 타일과 나머지 타일들의 매칭 여부를 확인하는 함수
    private bool CheckForMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (monsterTilesBox[x, y] != null)
                {
                    if (x < width - 2)
                    {
                        if (monsterTilesBox[x + 1, y] != null && monsterTilesBox[x + 2, y] != null)
                        {
                            if (monsterTilesBox[x, y].CompareTag(monsterTilesBox[x + 1, y].tag) &&
                                monsterTilesBox[x, y].CompareTag(monsterTilesBox[x + 2, y].tag))
                            {
                                return true;
                            }
                        }
                    }
                    if (y < height - 2)
                    {
                        if (monsterTilesBox[x, y + 1] != null && monsterTilesBox[x, y + 2] != null)
                        {
                            if (monsterTilesBox[x, y].CompareTag(monsterTilesBox[x, y + 1].tag) &&
                                monsterTilesBox[x, y].CompareTag(monsterTilesBox[x, y + 2].tag))
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    private bool SwitchingAndCheck(int row, int col, Vector2 direction)
    {
        SwitchPieces(row, col, direction);
        if (CheckForMatches())
        {
            SwitchPieces(row, col, direction);
            return true;
        }
        SwitchPieces(row, col, direction);
        return false;
    }

    //몬스터가 옮길 수 있는 타일을 찾기 위해 동작하는 함수
    private List<MoveTo> FindAllMatches()
    {
        List<MoveTo> possibleMoves = new List<MoveTo>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (monsterTilesBox[x, y] != null)
                {
                    if (x < width - 1)
                    {
                        if (SwitchingAndCheck(x, y, Vector2.right))
                        {
                            possibleMoves.Add(new MoveTo { pickUpTile = monsterTilesBox[x, y], moveDir = Vector2.right });
                        }
                    }
                    if (y < height - 1)
                    {
                        if (SwitchingAndCheck(x, y, Vector2.up))
                        {
                            possibleMoves.Add(new MoveTo { pickUpTile = monsterTilesBox[x, y], moveDir = Vector2.up });
                        }
                    }
                }
            }
        }

        return possibleMoves;
    }

    private GameObject PickUpRandom()
    {
        List<MoveTo> possibleMovesList = new List<MoveTo>();

        possibleMovesList = FindAllMatches();
        if (possibleMovesList.Count > 0)
        {
            int tileToUse = Random.Range(0, possibleMovesList.Count);
            GameObject moveTile = possibleMovesList[tileToUse].pickUpTile;
            moveDirF = possibleMovesList[tileToUse].moveDir;
            return moveTile;
        }
        else
        {
            Debug.Log("옮길 수 있는 타일이 없습니다!!");
        }
        return null;
    }

    public void MoveTile()
    {
        GameObject tile = PickUpRandom();

        if (tile != null)
        {
            //타일을 원하는 방향으로 이동
            tile.GetComponent<TileMonster>().SwapTile(moveDirF);
        }
    }

    public void SetTargetPos(GameObject first, GameObject second)
    {
        first.GetComponent<TileMonster>().targetX = second.GetComponent<TileMonster>().transform.position.x;
        first.GetComponent<TileMonster>().targetY = second.GetComponent<TileMonster>().transform.position.y;
        second.GetComponent<TileMonster>().targetX = first.GetComponent<TileMonster>().transform.position.x;
        second.GetComponent<TileMonster>().targetY = first.GetComponent<TileMonster>().transform.position.y;

        first.GetComponent<TileMonster>().canShifting = true;
        second.GetComponent<TileMonster>().canShifting = true;
    }

    //TODO
    public void DestroyMatches()
    {
        currentState = MonsterState.WAIT;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (monsterTilesBox[x, y] != null)
                {
                    DestroyMatchesAt(x, y);
                }
            }
        }

        StartCoroutine(DecreaseColCoroutine());
    }

    private void DestroyMatchesAt(int row, int col)
    {
        if (monsterTilesBox[row, col].GetComponent<TileMonster>().isMatched)
        {
            monsterStatusController.IncreaseMp(1f * findMatchesMonster.currentMatches.Count); //타일 파괴시 스킬 게이지 획득
            findMatchesMonster.currentMatches.Remove(monsterTilesBox[row, col]);

            #region 파괴 이펙트

            //TODO : ObjectPool Test
            GameObject flashMon = ObjectPool.GetObjectPoolEffect<FlashEffectMonster>(transform,"FlashEffect_Devastar");
            flashMon.transform.position = monsterTilesBox[row, col].GetComponent<TileMonster>().transform.position;
            flashMon.GetComponent<FlashEffectMonster>().RemoveEffect();

            #endregion 파괴 이펙트

            Destroy(monsterTilesBox[row, col].gameObject);
            monsterTilesBox[row, col] = null;
        }
    }

    private IEnumerator DecreaseColCoroutine()
    {
        int nullCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (monsterTilesBox[x, y] == null)
                {
                    nullCount++;
                    //  Debug.Log("카운트 세는중");
                }
                else if (nullCount > 0)
                {
                    monsterTilesBox[x, y].GetComponent<TileMonster>().Col -= nullCount;
                    monsterTilesBox[x, y].GetComponent<TileMonster>().targetY -= (80 * nullCount);
                    monsterTilesBox[x, y].GetComponent<TileMonster>().canShifting = true;
                    monsterTilesBox[x, y] = null;
                    //  Debug.Log("정보 변경");
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCoroutine());
    }

    private IEnumerator FillBoardCoroutine()
    {
        currentState = MonsterState.WAIT;

        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatchesMonster.currentMatches.Clear();
        yield return new WaitForSeconds(.5f);
    }

    private void RefillBoard()
    {
        Sprite[] previousLeft = new Sprite[height];
        Sprite previousBelow = null;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (monsterTilesBox[x, y] == null)
                {
                    float newPositionX = createBoardMonster.backTilesBox[x, y].GetComponent<BackgroundTile>().positionX;
                    float newPositionY = createBoardMonster.backTilesBox[x, y].GetComponent<BackgroundTile>().positionY;
                    Vector2 newPosition = new Vector2(newPositionX, newPositionY + monsterTilePrefab.GetComponent<RectTransform>().rect.size.y);
                    GameObject newTile = Instantiate(monsterTilePrefab, newPosition, Quaternion.identity);
                    newTile.transform.SetParent(transform);
                    newTile.GetComponent<TileMonster>().SetArrNumber(x, y);
                    newTile.GetComponent<TileMonster>().targetX = newPositionX;
                    newTile.GetComponent<TileMonster>().targetY = newPositionY;
                    monsterTilesBox[x, y] = newTile; //배열에 새 타일을 추가

                    //일반 타일 생성
                    CreateTileSprite(y, newTile, previousLeft, ref previousBelow);
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (monsterTilesBox[x, y] != null)
                {
                    if (monsterTilesBox[x, y].GetComponent<TileMonster>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //타일들의 상태가 하나라도 Shifting이면 PlayerState는 WAIT인 함수
    private void CanMovePlayerState()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (monsterTilesBox[x, y] != null)
                {
                    TileMonster movingTile = monsterTilesBox[x, y].GetComponent<TileMonster>();
                    if (movingTile.isShifting || movingTile.isMatched || movingTile.canShifting)
                    {
                        currentState = MonsterState.WAIT;
                        return;
                    }
                }
            }
        }

        if (devaSkill1.isRemainTimeUpdate == true)
        {
            currentState = MonsterState.USESKILL;
            return;
        }

        if (currentState == MonsterState.GROGGY || currentState == MonsterState.TRANSFORM)
            return;

        currentState = MonsterState.MOVE;
    }

    public bool IsMoveState()
    {
        if (currentState == MonsterState.WAIT)
        {
            return false;
        }
        return true;
    }
}