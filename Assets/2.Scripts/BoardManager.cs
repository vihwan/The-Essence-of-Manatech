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
    public GameObject characterTilePrefab;  //Tile Prefab
    public GameObject tileBackgroundPrefab;
    public int width;
    public int height;

    public GameObject[,] characterTiles;
    public GameObject[,] backTiles;

    private int matchFoundCount = 0;


    //Property
    public bool IsShifting { get; set; }
    public int MatchFoundCount { get => matchFoundCount; set => matchFoundCount = value; }

    //초기화함수
    public void Init()
    {
        instance = GetComponent<BoardManager>(); //싱글톤
        backTiles = new GameObject[width, height];
        characterTiles = new GameObject[width, height];

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
                backTiles[x, y] = newBackTile;

                GameObject characterTile = Instantiate(characterTilePrefab,
                                new Vector3(startX + (xOffset * x),
                                startY + (yOffset * y), transform.position.z),
                                Quaternion.identity);
                characterTile.transform.SetParent(newBackTile.transform);
                characterTile.gameObject.name = "Character [" + x + ", " + y + "]";
                characterTile.GetComponent<Tile>().SetArrNumber(x, y);
                characterTiles[x, y] = characterTile;


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


    public string SetTileName(int x, int y)
    {
        return "Tile [" + x + ", " + y + "]";
    }

    public void SetTargetPos(GameObject first, GameObject second)
    {
        first.GetComponent<Tile>().targetX = second.GetComponentInParent<BackgroundTile>().positionX;
        first.GetComponent<Tile>().targetY = second.GetComponentInParent<BackgroundTile>().positionY;

        second.GetComponent<Tile>().targetX = first.GetComponentInParent<BackgroundTile>().positionX;
        second.GetComponent<Tile>().targetY = first.GetComponentInParent<BackgroundTile>().positionY;
        second.GetComponent<Tile>().isSwapping = true;
    }


    //캐릭터 타일을 파괴
    private void DestroyTile(int row, int col)
    {
        if (characterTiles[row, col].GetComponent<Tile>().isMatched)
        {
            characterTiles[row, col].gameObject.SetActive(false);
        }
    }

    //파괴할 캐릭터 타일을 검색
    //해당되는 타일들을 전부 돌려서 matchFound가 true인 타일들을 전부 찾는다.
    public void FindDestroyMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (characterTiles[i, j] != null)
                {
                    DestroyTile(i, j);
                }
            }
        }
        FindNullTiles();
    }


    //비어있는 타일 자리를 찾는 코루틴
    public void FindNullTiles()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //비어있는 자리를 찾으면
                if (characterTiles[x, y].gameObject.activeSelf == false && !IsShifting)
                {
                    //타일 내리기 
                    Debug.Log("빈 타일 찾았따 : " + x + ", " + y);
                    ShiftTilesDown(x, y);
                    break;
                }
                else
                    MatchFoundCount = 0;
            }
        }
    }


    //타일 내리기 코루틴
    private void ShiftTilesDown(int x, int yStart)
    {
        IsShifting = true;
        int nullCount = 0;
        //비어있는 타일의 카운트를 세고, 카운트만큼 Col의 값을 깎는다.
        for (int y = yStart; y < height; y++)
        {
            Tile shiftTile = characterTiles[x, y].GetComponent<Tile>();
            if (characterTiles[x, y].GetComponent<Tile>().gameObject.activeSelf == false)
            {  
                nullCount++;
                IsShifting = true;
            }
            else
            {
                for (int i = 0; i < nullCount; i++)
                {
                    shiftTile.Col -= 1;
                }
                shiftTile.targetX = backTiles[x, y - nullCount].GetComponent<BackgroundTile>().positionX;
                shiftTile.targetY = backTiles[x, y - nullCount].GetComponent<BackgroundTile>().positionY - (80 * nullCount);
                shiftTile.isSwapping = true;
            }
        }
    }

    private IEnumerator ShiftingTileCoroutine(Tile shiftTile)
    {
        Vector2 tempPosition;
        //자신과 옮길 목표 위치 사이의 절대값이 0.1 이상이면 계속 Lerp를 실행
        if (Mathf.Abs(shiftTile.targetX - transform.position.x) > .1 ||
            Mathf.Abs(shiftTile.targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(shiftTile.targetX, shiftTile.targetX);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .03f);
        }
        else
        {   //타일 위치 이동 완료
            tempPosition = new Vector2(shiftTile.targetX, shiftTile.targetX);
            transform.position = tempPosition;
            //옮겨진 타일 오브젝트를 해당 BackTile 오브젝트로 종속시키기
            shiftTile.gameObject.transform.SetParent(backTiles[shiftTile.Row, shiftTile.Col].transform);
            //저장되어있는 characterTile의 정보를 바꾸기
            characterTiles[shiftTile.Row, shiftTile.Col] = shiftTile.gameObject;
            IsShifting = false;
        }

        yield return new WaitForSeconds(.3f);
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

