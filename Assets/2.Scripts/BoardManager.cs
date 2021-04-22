using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BoardManager : MonoBehaviour
{
    //Singleton
    public static BoardManager instance;   
    
   
    
    public GameObject tileBackgroundPrefab;
    public int width;
    public int height;     

    public GameObject[,] tiles;
    private BackgroundTile[,] backTiles;

    private int matchFoundCount  = 0 ;
    public float coroutineTime = 0f;

    //Property
    public bool IsShifting { get; set; }
    public int MatchFoundCount { get => matchFoundCount; set => matchFoundCount = value; }

    //초기화함수
    public void Init()
    {
        instance = GetComponent<BoardManager>(); //싱글톤
        backTiles = new BackgroundTile[width, height];

        Vector2 offset = tileBackgroundPrefab.GetComponent<RectTransform>().rect.size;
        CreateBoard(offset.x, offset.y); //타일 프리팹의 사이즈를 매개변수로 보드 생성
        SoundManager.instance.PlayBGM("데바스타르");
        SoundManager.instance.audioSourceBGM.volume = 0.2f;
    }

/*
    private void Update()
    {
        //매칭되는 타일이 있는가?
        //비어있는 타일이 있는가?
    }*/


    //게임 보드 생성
    private void CreateBoard(float xOffset, float yOffset)
    {

        //BoardManager 위치에 따라 시작점이 달라짐
        //왼쪽 하단을 기준으로.
        float startX = transform.position.x;     
        float startY = transform.position.y;

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
            }
        }
    }


    public GameObject[,] GetTile()
    {
        return tiles;
    }

    public string SetTileName(int x, int y)
    {
        return "Tile [" + x + ", " + y + "]";
    }



    //
    private void DestroyTile(int row, int col)
    {
        if(tiles[row,col].GetComponent<Tile>().isMatched)
        {
            Destroy(tiles[row, col].gameObject);
        }
    }

    //파괴할 타일을 검색
    //해당되는 타일들을 전부 돌려서 matchFound가 true인 타일들을 전부 찾는다.
    public void FindDestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(tiles[i,j] != null)
                {
                    DestroyTile(i, j);
                }
            }
        }
    }


    IEnumerator DecreaseCol()
    {



        yield return new WaitForSeconds(.4f);
    }



    //비어있는 타일 자리를 찾는 코루틴
    public void FindNullTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //비어있는 자리를 찾으면
                if (tiles[x, y] == null)
                {
                    Debug.Log("비어있는 타일 : " + x + ", " + y);

                    //타일 내리기 코루틴 실행 및 대기
                  // ShiftTilesDown(x, y);
                    break;
                }
                else
                   MatchFoundCount = 0;
                
            }
        }

        /*//타일을 내렸을 때 매칭이 성립되면 매칭 함수를 계속 실행
        //콤보시스템
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                tiles[x, y].GetComponent<Tile>().FindAllMatchingTiles();
            }
        }*/
    }

    //타일 내리기 코루틴
/*    private void ShiftTilesDown(int x, int yStart, float shiftDelay = .03f) //딜레이시간 
    {       
        IsShifting = true;
        List<Image> rendersList = new List<Image>();
        int nullCount = 0;

        //GameObject refillTile = Instantiate(tilePrefab,)

        for (int y = yStart; y < height; y++)
        {
            //매칭된 타일의 y좌표가 동일한(같은 열에 있는) 타일들을 전부 대입하는 과정.
            Image render = tiles[x, y].GetComponent<Image>();
            if (render.sprite == null)
            {  //타일의 스프라이트가 비어있다면 nullcount++
                nullCount++;
            }
            rendersList.Add(render); //renders 리스트에 추가
        }

        //nullcount만큼 루프
        for (int i = 0; i < nullCount; i++)
        {          
            ScoreManager.instance.PlusScore(); //임시 배치
            Invoke(nameof(MovingTile),shiftDelay); //대기시간

            //renders 리스트의 count-1만큼 루프
            for (int k = 0; k < rendersList.Count - 1; k++)
            { 
                //
                rendersList[k].sprite = rendersList[k + 1].sprite;
                rendersList[k + 1].sprite = GetNewSprite(x, height - 1);
            }
        }
        IsShifting = false;
    }*/

    public void CreateNewTile()
    {
        StartCoroutine(DecreaseRowCol());
    }

    private IEnumerator DecreaseRowCol()
    {
        int nullCount = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if(tiles[x,y] == null)
                {
                    nullCount++;
                }
                else if(nullCount > 0)
                {
                    tiles[x, y].GetComponent<Tile>().Row -= nullCount;
                }
                nullCount = 0;
            }
        }
        yield return new WaitForSeconds(.4f);
    }



    private void MovingTile()

    {
    }


/*    //빈 자리가 생겨 타일이 내려갈 때, 새로운 스프라이트를 생성하는 함수
    //새로 생성한 스프라이트가 좌,우,아래에 존재하는 스프라이트와 같지 않도록 생성
    private Sprite GetNewSprite(int x, int y) // x는 tile의 x좌표, y는 ysize -1
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(characters);

        if (x > 0)
        {
            possibleCharacters.Remove(tiles[x - 1, y].GetComponent<Image>().sprite);
        }
        if (x < width - 1)
        {
            possibleCharacters.Remove(tiles[x + 1, y].GetComponent<Image>().sprite);
        }
        if (y > 0)
        {
            possibleCharacters.Remove(tiles[x, y - 1].GetComponent<Image>().sprite);
        }

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }
*/
   


    //타일 내리기 실행이 끝마친 이후에 다시 검사를 해서 비어있는 타일이 있는지를 검사
    //비어있는 자리를 찾으면 타일 내리기 실행
}
