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
    
    public List<Sprite> characters = new List<Sprite>(); //캐릭터들을 저장하는 리스트
    public GameObject tilePrefab;  //Tile Prefab
    public int xSize, ySize;     

    private GameObject[,] tiles;

    private int matchFoundCount  = 0 ;
    public float coroutineTime = 0f; 

    //Property
    public bool IsShifting { get; set; }
    public int MatchFoundCount { get => matchFoundCount; set => matchFoundCount = value; }

    //초기화함수
    public void Init()
    {
        instance = GetComponent<BoardManager>(); //싱글톤

        Vector2 offset = tilePrefab.GetComponent<RectTransform>().rect.size;
        CreateBoard(offset.x, offset.y); //타일 프리팹의 사이즈를 매개변수로 보드 생성
        SoundManager.instance.PlayBGM("데바스타르");
        SoundManager.instance.audioSourceBGM.volume = 0.7f;
    }

    //게임 보드 생성
    private void CreateBoard(float xOffset, float yOffset)
    {
        tiles = new GameObject[xSize, ySize];

        //BoardManager 위치에 따라 시작점이 달라짐
        //왼쪽 하단을 기준으로.
        float startX = transform.position.x;     
        float startY = transform.position.y;

        Sprite[] previousLeft = new Sprite[ySize];
        Sprite previousBelow = null;

        for (int x = 0; x < xSize; x++)
        {     
            for (int y = 0; y < ySize; y++)
            {
                GameObject newTile = Instantiate(tilePrefab, 
                                                new Vector3(startX + (xOffset * x), 
                                                startY + (yOffset * y), transform.position.z), 
                                                tilePrefab.transform.rotation);
                tiles[x, y] = newTile;
                tiles[x, y].gameObject.name = SetTileName(x, y);
                tiles[x, y].gameObject.GetComponentInChildren<Tile>().SetArrNumber(x, y);
                newTile.transform.SetParent(transform);
                

                #region 처음 보드를 생성할 때, 바로 3개가 연결되어 나오지 않도록 방지하는 코드
                List<Sprite> possibleCharacters = new List<Sprite>(); //가능한캐릭터들의 리스트를 생성
                possibleCharacters.AddRange(characters); //모든 캐릭터들을 리스트에 때려넣음

                possibleCharacters.Remove(previousLeft[y]); //이전의 왼쪽에 해당되는 열 리스트들을 전부 삭제
                possibleCharacters.Remove(previousBelow);   //이전의 아래에 해당되는 캐릭터를 삭제
                #endregion

                Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)]; //저장된 캐릭터들을 랜덤으로 받아서
                newTile.GetComponent<Image>().sprite = newSprite; //생성된 타일에 대입한다.
                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }

    public GameObject GetTile(int x, int y)
    {
        return tiles[x,y];
    }

    public string SetTileName(int x, int y)
    {
        return "Tile [" + x + ", " + y + "]";
    }



    //비어있는 타일 자리를 찾는 코루틴
    public void FindNullTiles()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                //비어있는 자리를 찾으면
                if (tiles[x, y].GetComponent<Image>().sprite == null)
                {
                    //타일 내리기 코루틴 실행 및 대기
                    ShiftTilesDown(x, y);
                    break;
                }
                else
                   MatchFoundCount = 0;
                
            }
        }

        //타일을 내렸을 때 매칭이 성립되면 매칭 함수를 계속 실행
        //콤보시스템
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                tiles[x, y].GetComponent<Tile>().ClearAllMatches();
            }
        }
    }

    //타일 내리기 코루틴
    private void ShiftTilesDown(int x, int yStart, float shiftDelay = .03f) //딜레이시간 
    {       
        IsShifting = true;
        List<Image> rendersList = new List<Image>();
        int nullCount = 0;


        for (int y = yStart; y < ySize; y++)
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
                rendersList[k + 1].sprite = GetNewSprite(x, ySize - 1);
            }
        }
        IsShifting = false;
    }

    private void MovingTile()

    {
    }


    //빈 자리가 생겨 타일이 내려갈 때, 새로운 스프라이트를 생성하는 함수
    //새로 생성한 스프라이트가 좌,우,아래에 존재하는 스프라이트와 같지 않도록 생성
    private Sprite GetNewSprite(int x, int y) // x는 tile의 x좌표, y는 ysize -1
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(characters);

        if (x > 0)
        {
            possibleCharacters.Remove(tiles[x - 1, y].GetComponent<Image>().sprite);
        }
        if (x < xSize - 1)
        {
            possibleCharacters.Remove(tiles[x + 1, y].GetComponent<Image>().sprite);
        }
        if (y > 0)
        {
            possibleCharacters.Remove(tiles[x, y - 1].GetComponent<Image>().sprite);
        }

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }

   


    //타일 내리기 실행이 끝마친 이후에 다시 검사를 해서 비어있는 타일이 있는지를 검사
    //비어있는 자리를 찾으면 타일 내리기 실행
}
