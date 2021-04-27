using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/*
>> Enum 타입의 State 변수를 만들어서 게임의 동작 상태를 제어하는 방식으로 해보자

    <BoardState>

	1) WAIT
	대기 상태. 보드가 동작이 이루어져 있어 기다려야하는 상태
	
	2) MOVE
    유저가 조작을 할 수 있는 상태

    > 1과 3의 차이는 마우스 조작이 가능하냐 불가능하냐의 차이로 해야할듯 싶다. 
*/
public enum BoardState
{
    WAIT,
    MOVE
}


public class BoardManager : MonoBehaviour
{
    //Singleton
    public static BoardManager instance;

    [Header("Board Variable")]
    public int width;
    public int height;

    public List<Sprite> characters = new List<Sprite>(); //외부에서 사용할 캐릭터들을 저장하는 리스트
    public GameObject characterTilePrefab;  //Tile Prefab
    public GameObject[,] characterTilesBox; //캐릭터 타일 보관하는 배열

    private int matchFoundCount = 0;
    public BoardState currentState = BoardState.MOVE;

    private float nextTime = 0f;
    private float TimeLeft = 3f;

    //필요한 컴포넌트
    private FindMatches findMatches;
    private CreateBoard createBoard;


    //Property
    public int MatchFoundCount { get => matchFoundCount; set => matchFoundCount = value; }

    //초기화함수
    public void Init()
    {
        instance = GetComponent<BoardManager>(); //싱글톤
        findMatches = FindObjectOfType<FindMatches>();
        createBoard = FindObjectOfType<CreateBoard>();
        characterTilesBox = new GameObject[width, height];

        Vector2 offset = characterTilePrefab.GetComponent<RectTransform>().rect.size;
        CreateTiles(offset.x, offset.y); //타일 프리팹의 사이즈를 매개변수로 보드 생성
        SoundManager.instance.PlayBGM("데바스타르");
        SoundManager.instance.audioSourceBGM.volume = 0.1f;
    }


    private void Update()
    {
        if (Time.time > nextTime)
        {
            nextTime = Time.time + TimeLeft;
            //PrintBoardState();
        }

    }

    private void PrintBoardState()
    {
        Debug.Log(currentState.ToString());
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


                #region 처음 보드를 생성할 때, 바로 3개가 연결되어 나오지 않도록 방지하는 코드
                List<Sprite> possibleCharacters = new List<Sprite>(); //가능한캐릭터들의 리스트를 생성
                possibleCharacters.AddRange(characters); //모든 캐릭터들을 리스트에 때려넣음

                possibleCharacters.Remove(previousLeft[y]); //이전의 왼쪽에 해당되는 열 리스트들을 전부 삭제
                possibleCharacters.Remove(previousBelow);   //이전의 아래에 해당되는 캐릭터를 삭제
                #endregion

                Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)]; //저장된 캐릭터들을 랜덤으로 받아서
                characterTile.GetComponent<Image>().sprite = newSprite; //생성된 타일에 대입한다.
                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }

    public void SetTargetPos(GameObject first, GameObject second)
    {
        first.GetComponent<Tile>().targetX = second.GetComponent<Tile>().transform.position.x;
        first.GetComponent<Tile>().targetY = second.GetComponent<Tile>().transform.position.y;

        second.GetComponent<Tile>().targetX = first.GetComponent<Tile>().transform.position.x;
        second.GetComponent<Tile>().targetY = first.GetComponent<Tile>().transform.position.y;
        second.GetComponent<Tile>().isSwapping = true;
    }



    private void DestroyMatchesAt(int row, int col)
    {
        if (characterTilesBox[row, col].GetComponent<Tile>().isMatched)
        {
            findMatches.currentMatches.Remove(characterTilesBox[row, col]);
            Destroy(characterTilesBox[row, col].gameObject);
            characterTilesBox[row, col] = null;
            Debug.Log("파괴 완료");
        }
    }

    public void DestroyMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(characterTilesBox[x,y] != null)
                {
                    DestroyMatchesAt(x, y);
                }
            }
        }
        StartCoroutine(DecreaseColCoroutine());
    }


    private IEnumerator DecreaseColCoroutine()
    {
        int nullCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(characterTilesBox[x,y] == null)
                {
                    nullCount++;
                    Debug.Log("카운트 세는중");
                }
                else if(nullCount > 0)
                {
                    characterTilesBox[x, y].GetComponent<Tile>().Col -= nullCount;
                    characterTilesBox[x, y].GetComponent<Tile>().targetY -= (80 * nullCount);
                    characterTilesBox[x, y] = null;
                    Debug.Log("정보 변경");
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
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(characterTilesBox[x,y] == null)
                {
                    //TODO : 임시 위치
                    float newPositionX = createBoard.backTilesBox[x,y].GetComponent<BackgroundTile>().positionX;
                    float newPositionY = createBoard.backTilesBox[x,y].GetComponent<BackgroundTile>().positionY;
                    Vector2 newPosition = new Vector2(newPositionX, newPositionY + characterTilePrefab.GetComponent<RectTransform>().rect.size.y);
                    GameObject newTile = Instantiate(characterTilePrefab, newPosition, Quaternion.identity);
                    newTile.transform.SetParent(transform);
                    newTile.GetComponent<Tile>().SetArrNumber(x, y);
                    newTile.GetComponent<Tile>().targetX = newPositionX;
                    newTile.GetComponent<Tile>().targetY = newPositionY;

                    List<Sprite> possibleCharacters = new List<Sprite>(); //가능한캐릭터들의 리스트를 생성
                    possibleCharacters.AddRange(characters); //모든 캐릭터들을 리스트에 때려넣음
                    Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)]; //저장된 캐릭터들을 랜덤으로 받아서
                    newTile.GetComponent<Image>().sprite = newSprite; //생성된 타일에 대입한다.

                    characterTilesBox[x, y] = newTile; //배열에 새 타일을 추가
                    Debug.Log("새 타일 생성");
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
        yield return new WaitForSeconds(.5f);
        currentState = BoardState.MOVE;
    }
}

