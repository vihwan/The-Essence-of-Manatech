using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;    
    public List<Sprite> characters = new List<Sprite>(); //캐릭터들을 저장하는 리스트
    public GameObject tilePrefab;  //Tile Prefab
    public int xSize, ySize;     

    private GameObject[,] tiles;
    public bool IsShifting { get; set; }    

    void Start()
    {
        instance = GetComponent<BoardManager>(); //싱글톤

        Vector2 offset = tilePrefab.GetComponent<SpriteRenderer>().bounds.size;
        CreateBoard(offset.x, offset.y); //타일 프리팹의 사이즈를 매개변수로 보드 생성
    }


    //게임 보드 생성
    private void CreateBoard(float xOffset, float yOffset)
    {
        tiles = new GameObject[xSize, ySize];

        //BoardManager 위치에 따라 시작점이 달라짐
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
                                                startY + (yOffset * y), 0), 
                                                tilePrefab.transform.rotation);
                tiles[x, y] = newTile;
                newTile.transform.parent = transform;

                #region 처음 보드를 생성할 때, 바로 3개가 연결되어 나오지 않도록 방지하는 코드
                List<Sprite> possibleCharacters = new List<Sprite>(); //가능한캐릭터들의 리스트를 생성
                possibleCharacters.AddRange(characters); //모든 캐릭터들을 리스트에 때려넣음

                possibleCharacters.Remove(previousLeft[y]); //이전의 왼쪽에 해당되는 열 리스트들을 전부 삭제
                possibleCharacters.Remove(previousBelow);   //이전의 아래에 해당되는 캐릭터를 삭제
                #endregion

                Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)]; //저장된 캐릭터들을 랜덤으로 받아서
                newTile.GetComponent<SpriteRenderer>().sprite = newSprite; //생성된 타일에 대입한다.
                previousLeft[y] = newSprite;
                previousBelow = newSprite;
            }
        }
    }


    //비어있는 타일 자리를 찾는 코루틴
    public IEnumerator FindNullTiles()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                //비어있는 자리를 찾으면
                if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null)
                {
                    //타일 내리기 코루틴 실행
                    yield return StartCoroutine(ShiftTilesDown(x, y));
                    break;
                }
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
    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f) //딜레이시간 
    {
        IsShifting = true;
        List<SpriteRenderer> renders = new List<SpriteRenderer>();
        int nullCount = 0;

        for (int y = yStart; y < ySize; y++)
        {  
            SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
            if (render.sprite == null)
            { 
                nullCount++;
            }
            renders.Add(render);
        }

        for (int i = 0; i < nullCount; i++)
        {
            GUIManager.instance.Score += 50;
            yield return new WaitForSeconds(shiftDelay);
            for (int k = 0; k < renders.Count - 1; k++)
            { 
                renders[k].sprite = renders[k + 1].sprite;
                renders[k + 1].sprite = GetNewSprite(x, ySize - 1);
            }
        }
        IsShifting = false;
    }


    //빈 자리가 생겨 타일이 내려갈 때, 새로운 타일을 생성하는 함수
    private Sprite GetNewSprite(int x, int y)
    {
        List<Sprite> possibleCharacters = new List<Sprite>();
        possibleCharacters.AddRange(characters);

        if (x > 0)
        {
            possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (x < xSize - 1)
        {
            possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
        }
        if (y > 0)
        {
            possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
        }

        return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
    }

}
