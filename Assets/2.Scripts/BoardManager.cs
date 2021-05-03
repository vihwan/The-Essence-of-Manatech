using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/*
>> Enum 타입의 State 변수를 만들어서 게임의 동작 상태를 제어하는 방식으로 해보자

    <BoardState>
	1) WAIT
	대기 상태. 보드가 동작이 이루어져 있어 기다려야하는 상태

	2) MOVE
    유저가 조작을 할 수 있는 상태

*/

public enum PlayerState
{
    WAIT,
    MOVE
}

internal class RC
{
    public int row;
    public int col;
}

public class BoardManager : MonoBehaviour
{
    //Singleton
    public static BoardManager instance;

    [Header("Board Variable")]
    public int width;

    public int height;

    public List<Sprite> characters = new List<Sprite>(); //외부에서 사용할 캐릭터들을 저장하는 리스트
    public GameObject[,] characterTilesBox; //캐릭터 타일 보관하는 배열
    public GameObject characterTilePrefab;  //Tile Prefab

    public PlayerState currentState = PlayerState.MOVE;

    private float nextTime = 0f;
    private float TimeLeft = 3f;
    public Text stateText;

    //필요한 컴포넌트

    private FindMatches findMatches;
    private CreateBackTiles createBoard;
    private ComboSystem comboSystem;
    private SkillManager skillManager;

    private List<RC> randomSelectList = new List<RC>();

    //Property

    //초기화함수
    public void Init()
    {
        instance = GetComponent<BoardManager>(); //싱글톤

        findMatches = FindObjectOfType<FindMatches>();
        createBoard = FindObjectOfType<CreateBackTiles>();
        comboSystem = FindObjectOfType<ComboSystem>();
        skillManager = FindObjectOfType<SkillManager>();

        characterTilesBox = new GameObject[width, height];

        Vector2 offset = characterTilePrefab.GetComponent<RectTransform>().rect.size;
        CreateTiles(offset.x, offset.y); //타일 프리팹의 사이즈를 매개변수로 보드 생성
        SoundManager.instance.PlayBGM("데바스타르");
        SoundManager.instance.audioSourceBGM.volume = 0.1f;
    }

    private void Update()
    {
        PrintBoardState();
    }

    private void PrintBoardState()
    {
        stateText.text = "PlayerState : " + currentState;
    }

    //게임 보드 생성
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
                GameObject characterTile = Instantiate(characterTilePrefab,
                                new Vector2(startX + (xOffset * x),
                                startY + (yOffset * y * 2)),
                                Quaternion.identity);
                characterTile.transform.SetParent(transform);
                characterTile.gameObject.name = "Character [" + x + ", " + y + "]";
                characterTile.GetComponent<Tile>().SetArrNumber(x, y);
                characterTile.GetComponent<Tile>().targetX = startX + (xOffset * x);
                characterTile.GetComponent<Tile>().targetY = startY + (yOffset * y);
                characterTilesBox[x, y] = characterTile;

                CreateTileSprite(y, characterTile, previousLeft, ref previousBelow);
            }
        }
    }

    public void SetTargetPos(GameObject first, GameObject second)
    {
        first.GetComponent<Tile>().targetX = second.GetComponent<Tile>().transform.position.x;
        first.GetComponent<Tile>().targetY = second.GetComponent<Tile>().transform.position.y;
        second.GetComponent<Tile>().targetX = first.GetComponent<Tile>().transform.position.x;
        second.GetComponent<Tile>().targetY = first.GetComponent<Tile>().transform.position.y;
    }

    private void DestroyMatchesAt(int row, int col)
    {
        if (characterTilesBox[row, col].GetComponent<Tile>().isMatched)
        {
            skillManager.GainSkillGauge(); //타일 파괴시 스킬 게이지 획득

            #region 파괴 이펙트

            findMatches.currentMatches.Remove(characterTilesBox[row, col]);
            GameObject flashEffect = Instantiate(Resources.Load<GameObject>("FlashEffect")
                                                , characterTilesBox[row, col].GetComponent<Tile>().transform.position
                                                , Quaternion.identity);
            flashEffect.transform.SetParent(transform);
            Destroy(flashEffect, .5f);

            #endregion 파괴 이펙트

            Destroy(characterTilesBox[row, col].gameObject);
            characterTilesBox[row, col] = null;

            ScoreManager.instance.PlusScore();
            // Debug.Log("파괴 완료");
        }
    }

    public void DestroyMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (characterTilesBox[x, y] != null)
                {
                    DestroyMatchesAt(x, y);
                }
            }
        }
        comboSystem.ComboCounter++; //TODO
        StartCoroutine(DecreaseColCoroutine());
    }

    private IEnumerator DecreaseColCoroutine()
    {
        int nullCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (characterTilesBox[x, y] == null)
                {
                    nullCount++;
                    //  Debug.Log("카운트 세는중");
                }
                else if (nullCount > 0)
                {
                    characterTilesBox[x, y].GetComponent<Tile>().Col -= nullCount;
                    characterTilesBox[x, y].GetComponent<Tile>().targetY -= (80 * nullCount);
                    characterTilesBox[x, y] = null;
                    //  Debug.Log("정보 변경");
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCoroutine());
    }

    //타일 채우기
    private void RefillBoard()
    {
        Sprite[] previousLeft = new Sprite[height];
        Sprite previousBelow = null;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (characterTilesBox[x, y] == null)
                {
                    float newPositionX = createBoard.backTilesBox[x, y].GetComponent<BackgroundTile>().positionX;
                    float newPositionY = createBoard.backTilesBox[x, y].GetComponent<BackgroundTile>().positionY;
                    Vector2 newPosition = new Vector2(newPositionX, newPositionY + characterTilePrefab.GetComponent<RectTransform>().rect.size.y);
                    GameObject newTile = Instantiate(characterTilePrefab, newPosition, Quaternion.identity);
                    newTile.transform.SetParent(transform);
                    newTile.GetComponent<Tile>().SetArrNumber(x, y);
                    newTile.GetComponent<Tile>().targetX = newPositionX;
                    newTile.GetComponent<Tile>().targetY = newPositionY;
                    characterTilesBox[x, y] = newTile; //배열에 새 타일을 추가

                    // 일정 확률로 비터스 캔디바 타일이 생성되도록 추가
                    float randomNum = Random.Range(0.0f, 100.1f);

                    if (randomNum > 3)
                    {
                        //일반 타일 생성
                        CreateTileSprite(y, newTile, previousLeft, ref previousBelow);
                    }
                    else
                    {
                        //캔디바타일 생성
                        newTile.gameObject.name = "Candy [" + x + ", " + y + "]";
                        newTile.GetComponent<Image>().sprite = Resources.Load<Sprite>("Lolipop");
                    }
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
                if (characterTilesBox[x, y] != null)
                {
                    if (characterTilesBox[x, y].GetComponent<Tile>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    //타일 생성하여 보드 채우기 코루틴
    private IEnumerator FillBoardCoroutine()
    {
        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        yield return new WaitForSeconds(.5f);
    }

    public bool IsMoveState()
    {
        if (currentState == PlayerState.WAIT)
        {
            return false;
        }
        return true;
    }

    public void CreateJackBomb()
    {
        randomSelectList.Clear();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile jack = characterTilesBox[x, y].GetComponent<Tile>();
                if (jack.CompareTag("Lantern"))
                {
                    randomSelectList.Add(new RC() { row = jack.Row, col = jack.Col });
                }
            }
        }

        for (int i = 0; i < 3; i++)
        {
            int rIndex = Random.Range(0, randomSelectList.Count);
            int row = randomSelectList[rIndex].row;
            int col = randomSelectList[rIndex].col;
            randomSelectList.RemoveAt(rIndex);

            //기존의 잭오랜턴 타일을 삭제
            Tile jack = characterTilesBox[row, col].GetComponent<Tile>();
            Destroy(jack.gameObject);
            characterTilesBox[row, col] = null;

            //랜덤하게 뽑은 잭오랜턴을 폭탄으로 교체
            float newPositionX = createBoard.backTilesBox[row, col].GetComponent<BackgroundTile>().positionX;
            float newPositionY = createBoard.backTilesBox[row, col].GetComponent<BackgroundTile>().positionY;
            Vector2 newPosition = new Vector2(newPositionX, newPositionY);

            GameObject newJackBomb = Instantiate(characterTilePrefab, newPosition, Quaternion.identity);
            newJackBomb.GetComponent<Tile>().SetArrNumber(row, col);
            newJackBomb.GetComponent<Tile>().targetX = newPositionX;
            newJackBomb.GetComponent<Tile>().targetY = newPositionY;
            newJackBomb.transform.SetParent(transform);
            newJackBomb.gameObject.name = "Bomb [" + row + ", " + col + "]";
            newJackBomb.GetComponent<Image>().sprite = Resources.Load<Sprite>("Bomb");
            characterTilesBox[row, col] = newJackBomb;
        }
    }

    public void IsMatchedJackBomb(int row, int col)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (row + i < 0 || row + i > width - 1 || col + j < 0 || col + j > height - 1)
                    continue;

                characterTilesBox[row + i, col + j].GetComponent<Tile>().isMatched = true;

                //Stack Overflow Exception
                /*if (characterTilesBox[row + i, col + j].GetComponent<Tile>().CompareTag("bomb"))
                {
                    JackBombIsMatched(row + i, col + j);
                }*/
            }
        }
    }

    private void CreateTileSprite(int col, GameObject gameObject, Sprite[] previousLeft, ref Sprite previousBelow)
    {
        List<Sprite> possibleCharacters = new List<Sprite>(); //가능한캐릭터들의 리스트를 생성
        possibleCharacters.AddRange(characters); //모든 캐릭터들을 리스트에 때려넣음
        possibleCharacters.Remove(previousLeft[col]); //이전의 왼쪽에 해당되는 열 리스트들을 전부 삭제
        possibleCharacters.Remove(previousBelow);   //이전의 아래에 해당되는 캐릭터를 삭제

        Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)]; //저장된 캐릭터들을 랜덤으로 받아서
        gameObject.GetComponent<Image>().sprite = newSprite; //생성된 타일에 대입한다.
        previousLeft[col] = newSprite;
        previousBelow = newSprite;
    }

    internal bool SwitchAndCheck(int x, int y)
    {
        if (characterTilesBox[x, y] != null)
        {
            //1
            if (characterTilesBox[x - 1, y - 1].tag == characterTilesBox[x, y].tag
                && characterTilesBox[x - 1, y - 2].tag == characterTilesBox[x, y].tag)
                return true;

            //2
            else if (characterTilesBox[x - 1, y - 1].tag == characterTilesBox[x, y].tag
                && characterTilesBox[x - 1, y + 1].tag == characterTilesBox[x, y].tag)
                return true;

            //3
            else if (characterTilesBox[x - 1, y + 1].tag == characterTilesBox[x, y].tag
                && characterTilesBox[x - 1, y + 2].tag == characterTilesBox[x, y].tag)
                return true;

            //4
            else if (characterTilesBox[x - 2, y - 1].tag == characterTilesBox[x, y].tag
                && characterTilesBox[x - 1, y - 1].tag == characterTilesBox[x, y].tag)
                return true;

            //5
            else if (characterTilesBox[x - 1, y - 1].tag == characterTilesBox[x, y].tag
                && characterTilesBox[x + 1, y - 1].tag == characterTilesBox[x, y].tag)
                return true;

            //6
            else if (characterTilesBox[x + 1, y - 1].tag == characterTilesBox[x, y].tag
                && characterTilesBox[x + 2, y - 1].tag == characterTilesBox[x, y].tag)
                return true;

            //7
            else if (characterTilesBox[x + 1, y - 1].tag == characterTilesBox[x, y].tag
                && characterTilesBox[x + 1, y - 2].tag == characterTilesBox[x, y].tag)
                return true;

            //8
            else if (characterTilesBox[x + 1, y + 1].tag == characterTilesBox[x, y].tag
                && characterTilesBox[x + 1, y - 1].tag == characterTilesBox[x, y].tag)
                return true;

            //9
            else if (characterTilesBox[x + 1, y + 2].tag == characterTilesBox[x, y].tag
                && characterTilesBox[x + 1, y + 1].tag == characterTilesBox[x, y].tag)
                return true;

            //10
            else if (characterTilesBox[x - 2, y + 1].tag == characterTilesBox[x, y].tag
                && characterTilesBox[x - 1, y + 1].tag == characterTilesBox[x, y].tag)
                return true;

            //11
            else if (characterTilesBox[x - 1, y + 1].tag == characterTilesBox[x, y].tag
                && characterTilesBox[x + 1, y + 1].tag == characterTilesBox[x, y].tag)
                return true;

            //12
            else if (characterTilesBox[x + 1, y + 1].tag == characterTilesBox[x, y].tag
                && characterTilesBox[x + 1, y + 2].tag == characterTilesBox[x, y].tag)
                return true;

            //1
            /*                    if (characterTilesBox[x - 1, y - 1] != null && characterTilesBox[x - 1, y - 2] != null)
                                {
                                    if (characterTilesBox[x - 1, y - 1].tag == characterTilesBox[x, y].tag
                                        && characterTilesBox[x - 1, y - 2].tag == characterTilesBox[x, y].tag)
                                        return true;
                                }
                                //2
                                else if (characterTilesBox[x - 1, y - 1] != null && characterTilesBox[x - 1, y + 1] != null)
                                {
                                    if (characterTilesBox[x - 1, y - 1].tag == characterTilesBox[x, y].tag
                                        && characterTilesBox[x - 1, y + 1].tag == characterTilesBox[x, y].tag)
                                        return true;
                                }
                                //3
                                if (characterTilesBox[x - 1, y + 1] != null && characterTilesBox[x - 1, y + 2] != null)
                                {
                                    if (characterTilesBox[x - 1, y + 1].tag == characterTilesBox[x, y].tag
                                        && characterTilesBox[x - 1, y + 2].tag == characterTilesBox[x, y].tag)
                                        return true;
                                }
                                //4
                                if (characterTilesBox[x - 2, y - 1] != null && characterTilesBox[x - 1, y - 1] != null)
                                {
                                    if (characterTilesBox[x - 2, y - 1].tag == characterTilesBox[x, y].tag
                                        && characterTilesBox[x - 1, y - 1].tag == characterTilesBox[x, y].tag)
                                        return true;
                                }
                                //5
                                if (characterTilesBox[x - 1, y - 1] != null && characterTilesBox[x + 1, y - 1] != null)
                                {
                                    if (characterTilesBox[x - 1, y - 1].tag == characterTilesBox[x, y].tag
                                        && characterTilesBox[x + 1, y - 1].tag == characterTilesBox[x, y].tag)
                                        return true;
                                }
                                //6
                                if (characterTilesBox[x + 1, y - 1] != null && characterTilesBox[x + 2, y - 1] != null)
                                {
                                    if (characterTilesBox[x + 1, y - 1].tag == characterTilesBox[x, y].tag
                                        && characterTilesBox[x + 2, y - 1].tag == characterTilesBox[x, y].tag)
                                        return true;
                                }
                                //7
                                if (characterTilesBox[x + 1, y - 1] != null && characterTilesBox[x + 1, y - 2] != null)
                                {
                                    if (characterTilesBox[x + 1, y - 1].tag == characterTilesBox[x, y].tag
                                        && characterTilesBox[x + 1, y - 2].tag == characterTilesBox[x, y].tag)
                                        return true;
                                }
                                //8
                                if (characterTilesBox[x + 1, y + 1] != null && characterTilesBox[x + 1, y - 1] != null)
                                {
                                    if (characterTilesBox[x + 1, y + 1].tag == characterTilesBox[x, y].tag
                                        && characterTilesBox[x + 1, y - 1].tag == characterTilesBox[x, y].tag)
                                        return true;
                                }
                                //9
                                if (characterTilesBox[x + 1, y + 2] != null && characterTilesBox[x + 1, y + 1] != null)
                                {
                                    if (characterTilesBox[x + 1, y + 2].tag == characterTilesBox[x, y].tag
                                        && characterTilesBox[x + 1, y + 1].tag == characterTilesBox[x, y].tag)
                                        return true;
                                }
                                //10
                                if (characterTilesBox[x - 2, y + 1] != null && characterTilesBox[x - 1, y + 1] != null)
                                {
                                    if (characterTilesBox[x - 2, y + 1].tag == characterTilesBox[x, y].tag
                                        && characterTilesBox[x - 1, y + 1].tag == characterTilesBox[x, y].tag)
                                        return true;
                                }
                                //11
                                if (characterTilesBox[x - 1, y + 1] != null && characterTilesBox[x + 1, y + 1] != null)
                                {
                                    if (characterTilesBox[x - 1, y + 1].tag == characterTilesBox[x, y].tag
                                        && characterTilesBox[x + 1, y + 1].tag == characterTilesBox[x, y].tag)
                                        return true;
                                }
                                //12
                                if (characterTilesBox[x + 1, y + 1] != null && characterTilesBox[x + 1, y + 2] != null)
                                {
                                    if (characterTilesBox[x + 1, y + 1].tag == characterTilesBox[x, y].tag
                                        && characterTilesBox[x + 1, y + 2].tag == characterTilesBox[x, y].tag)
                                        return true;
                                }*/
        }

        return false;
    }

    private void CanMovePlayerState()
    {
    }
}