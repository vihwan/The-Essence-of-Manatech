using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


internal class Deva
{
    public int row;
    public int col;
}


public class DevaSkill1 : MonoBehaviour
{
    /*
     * 데바스타르 (인간) 스킬
     * 
     * 플레이어 타일 중 랜덤하게 5개를 골라 움직이지 못하게 하는 마법진을 설치합니다.
     * 제한 시간 내에 마법진을 모두 파괴하면, 데바스타르는 그로기 상태가 됩니다.
     * 모두 파괴하지 못하면, 광폭화 스킬이 발동되어 플레이어는 큰 데미지를 입습니다.
     * 
     * **/

    private List<Deva> deva1s = new List<Deva>();
    public List<GameObject> go_List;


    [SerializeField] private float limitTime;
    [SerializeField] private float remainTime;
    public bool isRemainTimeUpdate = false;
    public bool isUsingSkill = false;
    internal bool isBerserk = false;


    internal GameObject rootUI;
    private TMP_Text remainTimeText;
    private Image limitTimeImage;


    public void Init()
    {
        rootUI = transform.Find("RootUI").gameObject;
        if (rootUI != null)
            rootUI.SetActive(false);

        remainTimeText = GetComponentInChildren<TMP_Text>(true);

        limitTimeImage = transform.Find("RootUI/SkillLimitTimeSlide/BaseUI/Gauge").GetComponent<Image>();
        if (limitTimeImage != null)
            limitTimeImage.fillAmount = 1f;
    }

    public void Execute()
    {
        limitTime = 60f;
        remainTime = limitTime;
        GaugeUpdate();
        rootUI.SetActive(true);

        //목소리 출력
        //"서로를 옭아매는 어리석은 인간이여"
        MonsterAI.instance.SoundandNotify.SetVoiceAndNotify(DevastarState.Skill_One);

        deva1s.Clear();
        for (int x = 0; x < BoardManager.instance.width; x++)
        {
            for (int y = 0; y < BoardManager.instance.height; y++)
            {
                if (x == 0 || x == BoardManager.instance.width - 1 || y == 0 || y == BoardManager.instance.height - 1)
                    continue;

                if (BoardManager.instance.characterTilesBox[x, y] != null)
                {
                    Tile tile = BoardManager.instance.characterTilesBox[x, y].GetComponent<Tile>();

                    if (tile.CompareTag("Lolipop") || tile.CompareTag("Bomb"))
                        continue;

                    deva1s.Add(new Deva() { row = tile.Row, col = tile.Col });
                }
            }
        }
        StartCoroutine(MakeMagicCircle());
    }

    private void Update()
    {
        if (isRemainTimeUpdate)
        {
            remainTime -= Time.deltaTime;

            GaugeUpdate();

            if (remainTime <= 0)
            {
                remainTime = 0f;
                limitTimeImage.fillAmount = 0f;

                if (BoardManager.instance.currentState == PlayerState.MOVE
                        && MonsterAI.instance.Action == MonsterState.CASTING)
                {
                    if (!isBerserk)
                    {
                        StartCoroutine(SkillBerserk());
                        isRemainTimeUpdate = false;
                        MonsterAI.instance.isUsingSkill = false;
                    }
                }
            }
            else
            {
                if (go_List.Count == 0)
                {
                    //패턴 파훼 성공
                    Debug.Log("<color=#0456F1>패턴 파훼</color> 성공!!");
                    //그로기 타임
                    MonsterAI.instance.Action = MonsterState.GROGGY;        
                    rootUI.SetActive(false);
                    isRemainTimeUpdate = false;
                    isUsingSkill = false;
                    MonsterAI.instance.isUsingSkill = false;
                }
            }

        }
    }

    private void GaugeUpdate()
    {
        limitTimeImage.fillAmount = remainTime / limitTime;
        remainTimeText.text = Mathf.RoundToInt(remainTime) + "s";
    }

    private IEnumerator SkillBerserk()
    {
        isBerserk = true;
        // 플레이어가 제한시간 내에 패턴을 파훼하지 못했을 시 발동한다.
        // 400의 데미지를 줌
        // 보이스 출력 : 파멸하라
        // 알림 이미지 출력
        MonsterAI.instance.Action = MonsterState.BERSERK;

        for (int i = 0; i < go_List.Count; i++)
        {
            //마법진 폭발 이펙트 실행
            Destroy(go_List[i].gameObject);
        }

        //캐릭터 타일 봉인 해제
        for (int x = 0; x < BoardManager.instance.width; x++)
        {
            for (int y = 0; y < BoardManager.instance.height; y++)
            {
                if (BoardManager.instance.characterTilesBox[x, y].GetComponent<Tile>().isSealed)
                {
                    BoardManager.instance.characterTilesBox[x, y].GetComponent<Tile>().isSealed = false;
                }
                else
                    continue;
            }
        }

        yield return new WaitForSeconds(1f);

        //파티클 이펙트
        GameObject bombObject = Instantiate(Resources.Load<GameObject>("DevaSkill_1_Berserk_Particle")
                                            , GameObject.Find("StageManager/BackgroundCanvas/BoardRoot/BoardImagePlayer").transform.position
                                            , Quaternion.identity
                                            , BoardManager.instance.transform);

        Destroy(bombObject, 3f);

        PlayerStatusController playerStatusController = FindObjectOfType<PlayerStatusController>();
        playerStatusController.DecreaseHP(400);
        go_List.Clear();
        rootUI.SetActive(false);
        MonsterAI.instance.Action = MonsterState.MOVE;
        isBerserk = false;
    }

    private IEnumerator MakeMagicCircle()
    {
        isUsingSkill = true;
        for (int i = 0; i < 5; i++)
        {
            int rIndex = Random.Range(0, deva1s.Count);

            int x = deva1s[rIndex].row;
            int y = deva1s[rIndex].col;
            deva1s.RemoveAt(rIndex);

            Tile tile = BoardManager.instance.characterTilesBox[x, y].GetComponent<Tile>();
            tile.isSealed = true;

            //파티클 이펙트 생성
            GameObject particle = Instantiate(Resources.Load<GameObject>("DevaSkill_1_Particle")
                                                    , tile.transform.position
                                                    , Quaternion.identity
                                                    , tile.transform);

            Destroy(particle, 3f);

            yield return new WaitForSeconds(.5f);

            SealedEffect seal = Instantiate(Resources.Load<SealedEffect>("circleC")
                                    , tile.transform.position
                                    , Quaternion.identity);
            seal.transform.SetParent(tile.transform);
            go_List.Add(seal.gameObject);

            yield return null;
        }
        isUsingSkill = false;
        isRemainTimeUpdate = true;
    }
}
