using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum TileMoveState
{
    WAIT,
    CANMOVE
}


public class BoardManager : MonoBehaviour
{
    //Singleton
    public static BoardManager instance;

    public List<Sprite> characters = new List<Sprite>(); //캐릭터들을 저장하는 리스트
    public GameObject characterTilePrefab;  //Tile Prefab
    public GameObject tileBackgroundPrefab;
    public int width;
    public int height;

    private bool isShifting = false;  //보드에 움직임이 있는가?
    private bool canDropping = false;  //캐릭터를 내릴 수 있는 상태인가?
    private bool canRefillTile = false; // 빈 타일에 캐릭터를 채울 수 있는 상태인가?
    public GameObject[,] characterTilesBox;
    public GameObject[,] backTilesBox;

    private int matchFoundCount = 0;

    public TileMoveState currentState = TileMoveState.CANMOVE;


    //Property
    public int MatchFoundCount { get => matchFoundCount; set => matchFoundCount = value; }
    public bool IsShifting { get => isShifting; set => isShifting = value; }
    public bool CanDropping { get => canDropping; set => canDropping = value; }

    public bool CanRefillTile{ get => canRefillTile; set => canRefillTile = value;}

    //초기화함수
    public void Init()
    {
        instance = GetComponent<BoardManager>(); //싱글톤
        backTilesBox = new GameObject[width, height];
        characterTilesBox = new GameObject[width, height];

        Vector2 offset = tileBackgroundPrefab.GetComponent<RectTransform>().rect.size;
        CreateBoard(offset.x, offset.y); //타일 프리팹의 사이즈를 매개변수로 보드 생성
        SoundManager.instance.PlayBGM("데바스타르");
        SoundManager.instance.audioSourceBGM.volume = 0f;
    }



    //게임 보드 생성
    private void CreateBoard(float xOffset, float yOffset)
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
                GameObject newBackTile = Instantiate(tileBackgroundPrefab,
                                new Vector3(startX + (xOffset * x),
                                startY + (yOffset * y), transform.position.z),
                                Quaternion.identity);
                newBackTile.transform.SetParent(transform);
                newBackTile.gameObject.name = "Tile Background [" + x + ", " + y + "]";
                newBackTile.GetComponent<BackgroundTile>().Init(
                    newBackTile.transform.position.x, newBackTile.transform.position.y);
                backTilesBox[x, y] = newBackTile;

                GameObject characterTile = Instantiate(characterTilePrefab,
                                new Vector3(startX + (xOffset * x),
                                startY + (yOffset * y), transform.position.z),
                                Quaternion.identity);
                characterTile.transform.SetParent(newBackTile.transform);
                characterTile.gameObject.name = "Character [" + x + ", " + y + "]";
                characterTile.GetComponent<Tile>().SetArrNumber(x, y);
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
        first.GetComponent<Tile>().targetX = second.GetComponentInParent<BackgroundTile>().positionX;
        first.GetComponent<Tile>().targetY = second.GetComponentInParent<BackgroundTile>().positionY;

        second.GetComponent<Tile>().targetX = first.GetComponentInParent<BackgroundTile>().positionX;
        second.GetComponent<Tile>().targetY = first.GetComponentInParent<BackgroundTile>().positionY;
        second.GetComponent<Tile>().isSwapping = true;
    }



    //파괴할 캐릭터 타일을 검색
    //해당되는 타일들을 전부 돌려서 matchFound가 true인 타일들을 전부 찾는다.
    //매칭이되면 무조건 파괴시킨다.
    public void FindDestroyMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (characterTilesBox[x, y] != null)
                {
                    if (characterTilesBox[x, y].GetComponent<Tile>().isMatched)
                    {
                       // Debug.Log("타일 파괴 : " + characterTilesBox[x, y]);
                        Destroy(characterTilesBox[x, y].GetComponent<Tile>().gameObject);
                        characterTilesBox[x, y] = null;

                    }
                }
            }
        }
        StartCoroutine(FindNullTiles());
        IsShifting = false;
    }


    //비어있는 타일 자리를 찾는 코루틴
    public IEnumerator FindNullTiles()
    {
        int nullCount = 0;
        int nonNullX = 0;
        int nonNullY = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (characterTilesBox[x, y] == null)
                {
                    nullCount++;
                }
                if (characterTilesBox[x, y] == null && characterTilesBox[x, y + 1] != null)
                {
                    nonNullX = x;
                    nonNullY = y + 1;
                    CanDropping = true;
                    break;
                }

            }
            if(CanDropping)
                ShiftingTile(nonNullX,nonNullY,nullCount);
            nullCount = 0;
        }
        yield return new WaitForSeconds(.5f);
    }


    private void ShiftingTile(int nonNullX, int nonNullY, int nullCount)
    {
        IsShifting = true;
        for (int y = nonNullY; y < height; y++)
        { //아래로 내리기 위해서 해당하는 타일의 속성을 바꿔준다.
            Tile shiftTile = characterTilesBox[nonNullX, y].GetComponent<Tile>();
            if(shiftTile == null)
                return;

            shiftTile.Col -= (1 * nullCount);
            shiftTile.targetX = backTilesBox[nonNullX, y - nullCount].GetComponent<BackgroundTile>().positionX;
            shiftTile.targetY = backTilesBox[nonNullX, y - nullCount].GetComponent<BackgroundTile>().positionY;
            shiftTile.isSwapping = true;
            Debug.Log("옮길 타일 목표 설정" + shiftTile);   
        }
        Debug.Log(nullCount + "칸 내림");
        CanDropping = false;
        canRefillTile = true;
    }


    public void RefillCharacterTiles()
    {
        Sprite[] previousLeft = new Sprite[height];
        Sprite previousBelow = null;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (characterTilesBox[i, j] == null)
                {
                    float xPosition = backTilesBox[i, j].GetComponent<BackgroundTile>().positionX;
                    float yPosition = backTilesBox[i, j].GetComponent<BackgroundTile>().positionY;
                    Vector2 tempPosition = new Vector2(xPosition, yPosition);
                    GameObject refillTile = Instantiate(characterTilePrefab, tempPosition, Quaternion.identity);

                    refillTile.transform.SetParent(backTilesBox[i, j].transform);
                    refillTile.gameObject.name = "Refilled Character [" + i + ", " + j + "]";
                    refillTile.GetComponent<Tile>().SetArrNumber(i, j);
                    characterTilesBox[i, j] = refillTile;
                    #region 처음 보드를 생성할 때, 바로 3개가 연결되어 나오지 않도록 방지하는 코드
                    List<Sprite> possibleCharacters = new List<Sprite>(); //가능한캐릭터들의 리스트를 생성
                    possibleCharacters.AddRange(characters); //모든 캐릭터들을 리스트에 때려넣음

                    possibleCharacters.Remove(previousLeft[j]); //이전의 왼쪽에 해당되는 열 리스트들을 전부 삭제
                    possibleCharacters.Remove(previousBelow);   //이전의 아래에 해당되는 캐릭터를 삭제
                    #endregion

                    Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)]; //저장된 캐릭터들을 랜덤으로 받아서
                    refillTile.GetComponent<Image>().sprite = newSprite; //생성된 타일에 대입한다.
                    previousLeft[j] = newSprite;
                    previousBelow = newSprite;

                    Debug.Log("리필 완료");                   
                }
            }
        }
    }


    // private bool MatchesOnBoard()
    // {
    //     for (int i = 0; i < width; i++)
    //     {
    //         for (int j = 0; j < height; j++)
    //         {
    //             if (characterTilesBox[i, j] != null)
    //             {
    //                 if (characterTilesBox[i, j].GetComponent<Tile>().isMatched)
    //                 {
    //                     return true;
    //                 }
    //             }
    //         }
    //     }
    //     return false;
    // }


    // private IEnumerator FillTileCoroutine()
    // {
    //     yield return new WaitForSeconds(1f);
    //     RefillCharacterTiles();

    //     yield return new WaitForSeconds(.5f);
    //     while (MatchesOnBoard())
    //     {
    //         yield return new WaitForSeconds(.5f);
    //         FindDestroyMatches();
    //     }
    // }
}

