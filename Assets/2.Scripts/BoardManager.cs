﻿using System.Collections;
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

    public PlayerState currentState = PlayerState.WAIT;

    //private float nextTime = 0f;
    //private float TimeLeft = 3f;
    public Text stateText;

    //필요한 컴포넌트

    private FindMatches findMatches;
    private CreateBackTiles createBoard;
    private ComboSystem comboSystem;
    private PlayerStatusController skillGauge;
    private MonsterStatusController monsterStatusController;
    private DevaSkill1 devaSkill1;
    private DevaSkill2 devaSkill2;

    private List<RC> randomSelectList = new List<RC>();

    //Property

    //초기화함수
    public void Init()
    {
        instance = GetComponent<BoardManager>(); //싱글톤

        findMatches = FindObjectOfType<FindMatches>();
        createBoard = FindObjectOfType<CreateBackTiles>();
        comboSystem = FindObjectOfType<ComboSystem>();
        skillGauge = FindObjectOfType<PlayerStatusController>();
        monsterStatusController = FindObjectOfType<MonsterStatusController>();
        devaSkill1 = FindObjectOfType<DevaSkill1>();
        devaSkill2 = FindObjectOfType<DevaSkill2>();

        characterTilesBox = new GameObject[width, height];

        Vector2 offset = characterTilePrefab.GetComponent<RectTransform>().sizeDelta;
        CreateTiles(offset.x, offset.y); //타일 프리팹의 사이즈를 매개변수로 보드 생성
        SoundManager.instance.PlayBGM("데바스타르");
        //SoundManager.instance.audioSourceBGM.mute = true;
        SoundManager.instance.audioSourceBGM.volume = .3f;
    }

    private void Update()
    {
        CheckMovePlayerState();
       // PrintBoardState();
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
                                           new Vector3(startX + (xOffset * x), startY + (yOffset * y * 2)),
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

        first.GetComponent<Tile>().canShifting = true;
        second.GetComponent<Tile>().canShifting = true;
    }

    public void DestroyMatches()
    {
        currentState = PlayerState.WAIT;
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

    private void DestroyMatchesAt(int row, int col)
    {
        if (characterTilesBox[row, col].GetComponent<Tile>().isMatched)
        {
            skillGauge.IncreaseMp(1f * findMatches.currentMatches.Count); //타일 파괴시 스킬 게이지 획득
            findMatches.currentMatches.Remove(characterTilesBox[row, col]);


            CreateDestroyEffect(row, col);

            //주변에 봉인된 타일이 있는지 체크
            CheckNearSealedTile(row, col);

            if (characterTilesBox[row, col].transform.childCount > 0)
            {
                if (characterTilesBox[row, col].GetComponent<Tile>().isSealed)
                {
                    devaSkill1.go_List.Remove(characterTilesBox[row, col].GetComponentInChildren<SealedEffect>().gameObject);
                }
                else if (characterTilesBox[row, col].GetComponent<Tile>().isActiveNen)
                {
                    devaSkill2.go_List2.Remove(characterTilesBox[row, col].GetComponentInChildren<NenEffect>().gameObject);
                }
            }
            Destroy(characterTilesBox[row, col].gameObject);
            characterTilesBox[row, col] = null;

            ScoreManager.instance.GetScore();
        }
    }

    private void CreateDestroyEffect(int row, int col)
    {
        #region 파괴 이펙트
        GameObject flashEffect = ObjectPool.GetObjectPoolEffect<FlashEffect>(transform, "FlashEffect");
        flashEffect.transform.position = characterTilesBox[row, col].GetComponent<Tile>().transform.position;
        flashEffect.GetComponent<FlashEffect>().RemoveEffect();

        #endregion 파괴 이펙트

        #region 미사일 이펙트
        GameObject missile = Instantiate(Resources.Load<GameObject>("MissileEffect")
                                            , characterTilesBox[row, col].GetComponent<Tile>().transform.position
                                            , Quaternion.identity);
        missile.GetComponent<Image>().sprite = characterTilesBox[row, col].GetComponent<Image>().sprite;
        missile.transform.SetParent(transform);
        Destroy(missile, 10f);
        #endregion
    }

    private void CheckNearSealedTile(int row, int col)
    {
        //만약 파괴할 타일 상하좌우에 봉인된 타일이 있다면 그 또한 파괴
        //파괴하면서 데바스타르 Go_List 를 지워줘야한다. 

        //또한 넨가드 타일이 있다면 그 또한 파괴
        //파괴하면서 GO_List2를 지워주어야함

        //왼쪽
        if (CheckIndexOutOfRange(row - 1, col))
        {
            if (characterTilesBox[row - 1, col] != null)
            {
                if (characterTilesBox[row - 1, col].GetComponent<Tile>().isSealed)
                {
                    CreateDestroyEffect(row - 1, col);
                    devaSkill1.go_List.Remove(characterTilesBox[row - 1, col].GetComponentInChildren<SealedEffect>().gameObject);
                    Destroy(characterTilesBox[row - 1, col].gameObject);
                    characterTilesBox[row - 1, col] = null;
                }
                else if (characterTilesBox[row - 1, col].GetComponent<Tile>().isActiveNen)
                {
                    CreateDestroyEffect(row - 1, col);
                    devaSkill2.go_List2.Remove(characterTilesBox[row - 1, col].GetComponentInChildren<NenEffect>().gameObject);
                    Destroy(characterTilesBox[row - 1, col].gameObject);
                    characterTilesBox[row - 1, col] = null;
                }
            }
        }

        //오른쪽
        if (CheckIndexOutOfRange(row + 1, col))
        {
            if (characterTilesBox[row + 1, col] != null)
            {
                if (characterTilesBox[row + 1, col].GetComponent<Tile>().isSealed)
                {
                    CreateDestroyEffect(row + 1, col);
                    devaSkill1.go_List.Remove(characterTilesBox[row + 1, col].GetComponentInChildren<SealedEffect>().gameObject);
                    Destroy(characterTilesBox[row + 1, col].gameObject);
                    characterTilesBox[row + 1, col] = null;
                }
                else if (characterTilesBox[row + 1, col].GetComponent<Tile>().isActiveNen)
                {
                    CreateDestroyEffect(row + 1, col);
                    devaSkill2.go_List2.Remove(characterTilesBox[row + 1, col].GetComponentInChildren<NenEffect>().gameObject);
                    Destroy(characterTilesBox[row + 1, col].gameObject);
                    characterTilesBox[row + 1, col] = null;
                }
            }
        }

        //아래쪽
        if (CheckIndexOutOfRange(row, col - 1))
        {
            if (characterTilesBox[row, col - 1] != null)
            {
                if (characterTilesBox[row, col - 1].GetComponent<Tile>().isSealed)
                {
                    CreateDestroyEffect(row, col - 1);
                    devaSkill1.go_List.Remove(characterTilesBox[row, col - 1].GetComponentInChildren<SealedEffect>().gameObject);
                    Destroy(characterTilesBox[row, col - 1].gameObject);
                    characterTilesBox[row, col - 1] = null;
                }
                else if (characterTilesBox[row, col - 1].GetComponent<Tile>().isActiveNen)
                {
                    CreateDestroyEffect(row, col - 1);
                    devaSkill2.go_List2.Remove(characterTilesBox[row, col - 1].GetComponentInChildren<NenEffect>().gameObject);
                    Destroy(characterTilesBox[row, col - 1].gameObject);
                    characterTilesBox[row, col - 1] = null;
                }
            }
        }

        //위쪽
        if (CheckIndexOutOfRange(row, col + 1))
        {
            if (characterTilesBox[row, col + 1] != null)
            {
                if (characterTilesBox[row, col + 1].GetComponent<Tile>().isSealed)
                {
                    CreateDestroyEffect(row, col + 1);
                    devaSkill1.go_List.Remove(characterTilesBox[row, col + 1].GetComponentInChildren<SealedEffect>().gameObject);
                    Destroy(characterTilesBox[row, col + 1].gameObject);
                    characterTilesBox[row, col + 1] = null;
                }
                else if (characterTilesBox[row, col + 1].GetComponent<Tile>().isActiveNen)
                {
                    CreateDestroyEffect(row, col + 1);
                    devaSkill2.go_List2.Remove(characterTilesBox[row, col + 1].GetComponentInChildren<NenEffect>().gameObject);
                    Destroy(characterTilesBox[row, col + 1].gameObject);
                    characterTilesBox[row, col + 1] = null;
                }
            }
        }
    }

    private bool CheckIndexOutOfRange(int row, int col)
    {
        //IndexOutOfRange 예외 처리
        if (row - 1 < 0 || row + 1 > width - 1 || col - 1 < 0 || col + 1 > height - 1)
            return false;

        return true;
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
                    characterTilesBox[x, y].GetComponent<Tile>().canShifting = true;
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

                    if (randomNum > SkillManager.instance.PasSkillDic["붉은 사탕"].EigenValue)
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
        currentState = PlayerState.WAIT;

        RefillBoard();
        yield return new WaitForSeconds(.5f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.5f);
            DestroyMatches();
        }
        findMatches.currentMatches.Clear();
        yield return new WaitForSeconds(.5f);

        if (IsDeadlocked())
        {
            Debug.Log("<color=#FF6534> DeadLock 발생 </color> 타일들을 섞습니다.");
            Invoke(nameof(ShuffleBoard), 1f);
            SkillManager.instance.appearText("Deadlock 발생 타일을 섞습니다.");
        }
    }

    private void WaitFindCoroutine()
    {
        if (!findMatches.isUpdate)
        {
            if (MatchesOnBoard())
                DestroyMatches();
        }
    }

    //타일의 스프라이트를 입혀주는 함수. 스프라이트에 따라 타일의 속성(태그)가 바뀐다. (Tile Class)
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

    //가상으로 옮긴 타일을 교환하는 함수
    private void SwitchPieces(int row, int col, Vector2 direction)
    {
        GameObject holder = characterTilesBox[row + (int)direction.x, col + (int)direction.y];
        characterTilesBox[row + (int)direction.x, col + (int)direction.y] = characterTilesBox[row, col];
        characterTilesBox[row, col] = holder;
    }

    //가상으로 옮긴 타일과 나머지 타일들의 매칭 여부를 확인하는 함수
    private bool CheckForMatches()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (characterTilesBox[x, y] != null)
                {

                    //봉인된 타일은 데드락 조건에 포함시키지 않는다.
                    if (characterTilesBox[x, y].transform.childCount > 0)
                    {
                        continue;
                    }


                    if (x < width - 2)
                    {
                        if (characterTilesBox[x + 1, y] != null && characterTilesBox[x + 2, y] != null)
                        {
                            if (characterTilesBox[x, y].CompareTag(characterTilesBox[x + 1, y].tag) &&
                                characterTilesBox[x, y].CompareTag(characterTilesBox[x + 2, y].tag))
                            {
                                return true;
                            }
                        }
                    }
                    if (y < height - 2)
                    {
                        if (characterTilesBox[x, y + 1] != null && characterTilesBox[x, y + 2] != null)
                        {
                            if (characterTilesBox[x, y].CompareTag(characterTilesBox[x, y + 1].tag) &&
                                characterTilesBox[x, y].CompareTag(characterTilesBox[x, y + 2].tag))
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

    //방향값으로 타일을 가상으로 옮겨보고 매칭여부를 확인하는 함수
    internal bool SwitchingAndCheck(int row, int col, Vector2 direction)
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

    //데드락이 걸렸는 지 확인하는 함수
    internal bool IsDeadlocked()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (x < width - 1)
                {
                    if (SwitchingAndCheck(x, y, Vector2.right))
                    {
                        return false;
                    }
                }
                if (y < height - 1)
                {
                    if (SwitchingAndCheck(x, y, Vector2.up))
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }

    //데드락이 걸렸을 때 타일들을 섞어주는 함수
    private void ShuffleBoard()
    {
        //사운드 재생 : 반중력 기동장치

        List<GameObject> newBoard = new List<GameObject>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (characterTilesBox[x, y] != null)
                {
                    newBoard.Add(characterTilesBox[x, y]);
                }
            }
        }

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float newPositionX = createBoard.backTilesBox[x, y].GetComponent<BackgroundTile>().positionX;
                float newPositionY = createBoard.backTilesBox[x, y].GetComponent<BackgroundTile>().positionY;

                int randomNum = Random.Range(0, newBoard.Count);
                Tile tile = newBoard[randomNum].GetComponent<Tile>();
                tile.transform.SetParent(transform);
                tile.GetComponent<Tile>().SetArrNumber(x, y);
                tile.GetComponent<Tile>().targetX = newPositionX;
                tile.GetComponent<Tile>().targetY = newPositionY;
                tile.GetComponent<Tile>().canShifting = true;

                characterTilesBox[x, y] = newBoard[randomNum];
                newBoard.Remove(newBoard[randomNum]);
            }
        }

        //타일을 섞어줘도 데드락이면 한번 더 실행
        if (IsDeadlocked())
        {
            Invoke(nameof(ShuffleBoard), 1f);
        }
        //아니면 매칭된 타일 파괴 한번 하기
        else
            DestroyMatches();
    }

    // 2번 변이파리채 함수
    public void ChangePlutoTile()
    {
        randomSelectList.Clear();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile pluto = characterTilesBox[x, y].GetComponent<Tile>();
                if (pluto.CompareTag("Pluto"))
                {
                    randomSelectList.Add(new RC() { row = pluto.Row, col = pluto.Col });
                }
            }
        }

        //randomSelectList.Count < 변환 가능 갯수(3) 라면,
        //pluto 변환가능 갯수 = randomSelectList.Count
        int ableChangeTileCount = SkillManager.instance.ActSkillDic["변이 파리채"].EigenValue;
        if (randomSelectList.Count <= ableChangeTileCount)
        {
            ableChangeTileCount = randomSelectList.Count;
        }

        List<Sprite> possibleCharacters = new List<Sprite>(); //가능한캐릭터들의 리스트를 생성
        possibleCharacters.AddRange(characters); //모든 캐릭터들을 리스트에 때려넣음

        for (int i = 0; i < ableChangeTileCount; i++)
        {
            int rIndex = Random.Range(0, randomSelectList.Count);
            int row = randomSelectList[rIndex].row;
            int col = randomSelectList[rIndex].col;
            randomSelectList.RemoveAt(rIndex);

            //기존의 플루토 타일을 삭제
            Tile pluto = characterTilesBox[row, col].GetComponent<Tile>();
            if (pluto.transform.childCount > 0)
            {
                if (characterTilesBox[row, col].GetComponent<Tile>().isSealed)
                {
                    devaSkill1.go_List.Remove(characterTilesBox[row, col].GetComponentInChildren<SealedEffect>().gameObject);
                }
                else if (characterTilesBox[row, col].GetComponent<Tile>().isActiveNen)
                {
                    devaSkill2.go_List2.Remove(characterTilesBox[row, col].GetComponentInChildren<NenEffect>().gameObject);
                }
            }

            Destroy(pluto.gameObject);
            characterTilesBox[row, col] = null;

            //랜덤하게 뽑은 플루토 타일을 다른 타일로 교체
            float newPositionX = createBoard.backTilesBox[row, col].GetComponent<BackgroundTile>().positionX;
            float newPositionY = createBoard.backTilesBox[row, col].GetComponent<BackgroundTile>().positionY;
            Vector2 newPosition = new Vector2(newPositionX, newPositionY);

            GameObject newChangeTile = Instantiate(characterTilePrefab, newPosition, Quaternion.identity);
            newChangeTile.GetComponent<Tile>().SetArrNumber(row, col);
            newChangeTile.GetComponent<Tile>().targetX = newPositionX;
            newChangeTile.GetComponent<Tile>().targetY = newPositionY;
            newChangeTile.transform.SetParent(transform);
            newChangeTile.gameObject.name = "ChangeTile [" + row + ", " + col + "]";
            characterTilesBox[row, col] = newChangeTile;

            #region 타일 변환 확인 이펙트 (디버그)

            GameObject flashEffect = ObjectPool.GetObjectPoolEffect<FlashEffect>(transform, "FlashEffect");
            flashEffect.transform.position = characterTilesBox[row, col].GetComponent<Tile>().transform.position;
            flashEffect.GetComponent<FlashEffect>().RemoveEffect();

            #endregion 타일 변환 확인 이펙트 (디버그)

            Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)]; //저장된 캐릭터들을 랜덤으로 받아서
            newChangeTile.gameObject.GetComponent<Image>().sprite = newSprite; //생성된 타일에 대입한다.
        }

        //타일이 전부 바뀌면 매칭 검사를 한번 한다.
        findMatches.FindAllMatches();
        Invoke(nameof(WaitFindCoroutine), 1f);
    }

    // 4번 스킬 함수 - 잭오할로윈 타일을 생성
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

        //randomSelectList.Count < JackBomb 생성 가능 갯수 이라면,
        //jackBomb 생성가능 갯수 = randomSelectList.Count
        int ableCreateBombCount = SkillManager.instance.ActSkillDic["잭 오 할로윈"].EigenValue;
        if (randomSelectList.Count <= ableCreateBombCount)
        {
            ableCreateBombCount = randomSelectList.Count;
        }

        for (int i = 0; i < ableCreateBombCount; i++)
        {
            int rIndex = Random.Range(0, randomSelectList.Count);
            int row = randomSelectList[rIndex].row;
            int col = randomSelectList[rIndex].col;
            randomSelectList.RemoveAt(rIndex);

            //기존의 잭오랜턴 타일을 삭제
            Tile jack = characterTilesBox[row, col].GetComponent<Tile>();
            if (jack.transform.childCount > 0)
            {
                if (characterTilesBox[row, col].GetComponent<Tile>().isSealed)
                {
                    devaSkill1.go_List.Remove(characterTilesBox[row, col].GetComponentInChildren<SealedEffect>().gameObject);
                }
                else if (characterTilesBox[row, col].GetComponent<Tile>().isActiveNen)
                {
                    devaSkill2.go_List2.Remove(characterTilesBox[row, col].GetComponentInChildren<NenEffect>().gameObject);
                }
            }


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
            newJackBomb.GetComponent<Image>().sprite = Resources.Load<Sprite>("7잭오할로윈");
            characterTilesBox[row, col] = newJackBomb;
        }
    }

    //4번 스킬로 폭탄 주변의 타일을 전부 IsMatched = true로 바꾸는 함수
    public void JackBombIsMatch(int row, int col)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (row + i < 0 || row + i > width - 1 || col + j < 0 || col + j > height - 1)
                    continue;

                if (characterTilesBox[row + i, col + j].GetComponent<Tile>().isMatched)
                    continue;
                else
                    characterTilesBox[row + i, col + j].GetComponent<Tile>().isMatched = true;

                //폭탄 폭발범위 내에 다른 폭탄이 있다면 연쇄폭발
                if (characterTilesBox[row + i, col + j].GetComponent<Tile>().CompareTag("Bomb"))
                {
                    JackBombIsMatch(row + i, col + j);
                    continue;
                }
            }
        }
    }

    //타일들의 상태가 하나라도 Shifting이면 PlayerState는 WAIT인 함수
    private void CheckMovePlayerState()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (characterTilesBox[x, y] != null)
                {
                    Tile movingTile = characterTilesBox[x, y].GetComponent<Tile>();
                    if (movingTile.isShifting || movingTile.canShifting || movingTile.isMatched || hasEmptyTile())
                    {
                        currentState = PlayerState.WAIT;
                        return;
                    }
                }
            }
        }

        if (devaSkill1.isUsingSkill || devaSkill1.isBerserk || MonsterAI.instance.Action == MonsterState.TRANSFORM)
        {
            currentState = PlayerState.WAIT;
            return;
        }

        currentState = PlayerState.MOVE;
    }

    public bool IsMoveState()
    {
        if (currentState == PlayerState.WAIT)
        {
            return false;
        }
        return true;
    }

    private bool hasEmptyTile()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (characterTilesBox[x, y] == null)
                {
                    return true;
                }
            }
        }
        return false;
    }

}