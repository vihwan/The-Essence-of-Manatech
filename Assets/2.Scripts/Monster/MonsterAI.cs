using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum MonsterState
{
    WAIT,
    MOVE,
    USESKILL,
    CASTING,
    TRANSFORM,
    GROGGY,
    BERSERK,
    DEAD
}

public class MonsterAI : MonoBehaviour
{
    //Singleton
    public static MonsterAI instance;

    #region Variable

    [SerializeField] private float remainGroggyTime = 10f;
    private float standardGroggyTime = 10f;

    private bool isUpdate = false;
    private bool isPhase1 = false;
    private bool isPhase2 = false;
    private bool isTransform = false;
    private bool isSkillTurn2 = true;
    public bool isUsingSkill = false;

    [SerializeField] private float elaspedTime = 0f;
    [SerializeField] private float timeStandard = 4f;

    private MonsterState monsterAction;
    private ParticleSystem fireParticle;
    private Image bg;
    private bool isChangeBgColor = false;
    private float timeElapsed = 0f;

    private GameObject groggyRootUI;
    private Image groggyGauge;
    private TMP_Text remainTimeText;

    //Component
    private SetDevastarSoundandNotify soundandNotify;
    private MonsterStatusController monsterStatusController;
    private DevaSkill1 devaSkill1;
    private DevaSkill2 devaSkill2;
    private DevaSkill3 devaSkill3;
    #endregion

    #region Property
    public float ElaspedTime { get => elaspedTime; set => elaspedTime = value; }
    public float TimeStandard { get => timeStandard; set => timeStandard = value; }
    public MonsterStatusController MonsterStatusController { get => monsterStatusController; set => monsterStatusController = value; }
    public float RemainGroggyTime { get => remainGroggyTime; set => remainGroggyTime = value; }
    public float StandardGroggyTime { get => standardGroggyTime; set => standardGroggyTime = value; }
    public SetDevastarSoundandNotify SoundandNotify { get => soundandNotify; set => soundandNotify = value; }

    public MonsterState Action
    {
        get
        {
            return monsterAction;
        }

        set
        {
            monsterAction = value;

            //특수한 변수 대입이 필요한 경우를 대비해 스위치문을 따로 작성
            switch (monsterAction)
            {
                case MonsterState.WAIT:
                    break;

                case MonsterState.MOVE:
                    break;

                case MonsterState.USESKILL:
                    break;

                case MonsterState.CASTING:
                    break;

                case MonsterState.TRANSFORM:
                    isTransform = true;
                    isPhase1 = false;
                    break;

                case MonsterState.GROGGY:
                    {
                        if (isPhase1)
                            SoundandNotify.SetVoiceAndNotify(DevastarState.GroggyHuman);
                        else if (isPhase2)
                            SoundandNotify.SetVoiceAndNotify(DevastarState.GroggyDevil);
                    }
                    break;

                case MonsterState.BERSERK:
                    {
                        //버그가 너무 많아서 페이즈 2는 따로 해당 함수에서 소리와 알림이 출력되도록 설정
                        if (isPhase1)
                            SoundandNotify.SetVoiceAndNotify(DevastarState.Skill_One_Berserk);
                    }
                    break;

                case MonsterState.DEAD:
                    SoundManager.instance.PlayMonV("devastar_devil_die_01");
                    GameManager.instance.isGameOver = true;
                    GameManager.instance.GameWin();
                    break;
            }
        }
    }


    #endregion

    // Start is called before the first frame update
    public void Init()
    {
        instance = GetComponent<MonsterAI>();
        MonsterStatusController = FindObjectOfType<MonsterStatusController>();

        devaSkill1 = FindObjectOfType<DevaSkill1>();
        if (devaSkill1 != null)
            devaSkill1.Init();

        devaSkill2 = FindObjectOfType<DevaSkill2>();
        if (devaSkill2 != null)
            devaSkill2.Init();

        devaSkill3 = FindObjectOfType<DevaSkill3>();
        if (devaSkill3 != null)
            devaSkill3.Init();

        fireParticle = transform.Find("Canvas/Fire").GetComponent<ParticleSystem>();


        SoundandNotify = GetComponent<SetDevastarSoundandNotify>();
        if (SoundandNotify != null)
        {
            SoundandNotify.Initailze();
        }

        bg = GameObject.Find("StageManager/BackgroundCanvas/BackgroundImage").GetComponent<Image>();


        groggyRootUI = transform.Find("Canvas/RootUI").gameObject;
        if(groggyRootUI != null)
        {
            groggyGauge = groggyRootUI.transform.Find("GroggySlide/BaseUI/Gauge").GetComponent<Image>();
            remainTimeText = GetComponentInChildren<TMP_Text>(true);
            groggyRootUI.SetActive(false);
        }


        isPhase1 = true;
        devaSkill1.enabled = true;
        devaSkill2.enabled = false;
        RemainGroggyTime = 10f;

        Action = MonsterState.MOVE;
    }


    private void WAIT()
    {
        if (BoardManagerMonster.instance.CanMovePlayerState())
        {
            Action = MonsterState.MOVE;
        }
    }


    private void MOVE()
    {
        ElaspedTime += Time.deltaTime;
        if (ElaspedTime >= TimeStandard)
        {
            //마나가 꽉 차면 스킬 사용
            if (MonsterStatusController.images_Gauge[MonsterStatusController.MP].fillAmount == 1f
                && !BoardManager.instance.IsDeadlocked())
            {
                Action = MonsterState.USESKILL;
                ElaspedTime = 0f;
            }
            else
            {
                //타일 옮기기 
                BoardManagerMonster.instance.MoveTile();
                Action = MonsterState.WAIT;
                ElaspedTime = 0f;
            }
        }
    }

    private void SKILLUSE()
    {
        if (!isUpdate)
            StartCoroutine(WaitForStateMove());
    }

    private void TRANSFORM()
    {
        if (isTransform)
        {
            //만약 그로기 게이지UI가 켜져있으면 꺼준다.
            if (groggyRootUI.activeSelf == true)
                groggyRootUI.SetActive(false);

            //벼 ㅇ 신!
            if (!isUpdate)
            {
                StartCoroutine(TransToDevil());
                ElaspedTime = 0f;
            }
        }
    }

    private void GROGGY()
    {
        if (isPhase1 && monsterStatusController.CurrHp == 0)
            Action = MonsterState.TRANSFORM;

        RemainGroggyTime -= Time.deltaTime;
        UpdateGroggyGauge();
        if (RemainGroggyTime <= 0)
        {
            groggyRootUI.SetActive(false);
            RemainGroggyTime = 10f;
            Action = MonsterState.MOVE;
        }
    }

    private void UpdateGroggyGauge()
    {
        groggyRootUI.SetActive(true);
        groggyGauge.fillAmount = RemainGroggyTime / StandardGroggyTime;
        remainTimeText.text = Mathf.RoundToInt(remainGroggyTime) + "s";
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameManager.instance.GameState == GameState.PLAYING)
        {
            switch (monsterAction)
            {
                case MonsterState.WAIT:
                    WAIT();
                    break;

                case MonsterState.MOVE:
                    MOVE();
                    break;

                case MonsterState.USESKILL:
                    SKILLUSE();
                    break;

                case MonsterState.TRANSFORM:
                    TRANSFORM();
                    break;

                case MonsterState.GROGGY:
                    GROGGY();
                    break;
            }

            //페이즈1에서 사망할 경우, 페이즈2로 돌입
            if (isPhase1)
            {
                if (MonsterStatusController.CurrHp == 0f)
                {
                    //유저와 몬스터의 조작을 멈추고 대기 시간을 가진다.
                    Action = MonsterState.TRANSFORM;
                }
            }


            if (isPhase2)
            {
                if (!isChangeBgColor)
                {
                    if(timeElapsed < 5f)
                    {
                        bg.color = Color.Lerp(Color.white, new Color(1.0f, 0.8f, 0.8f, 1.0f), timeElapsed / 3f);
                        timeElapsed += Time.deltaTime;
                    }
                    else
                    {
                        bg.color = new Color(1.0f, 0.8f, 0.8f, 1.0f);
                        isChangeBgColor = true;
                    }
                }


                if (MonsterStatusController.CurrHp == 0f)
                {
                    //데바스타르 사망
                    //게임 오버
                    Action = MonsterState.DEAD;
                    isPhase2 = false;
                }
            }
        }
    }



    //페이즈 2가 되어 악마로 변신하는 메소드
    private IEnumerator TransToDevil()
    {
        isUpdate = true;

        //만약 데바스타르 스킬이 실행중이라면(isRemainTime)
        if (devaSkill1.isRemainTimeUpdate == true)
        {
            CancelDevaSkill1(out bool isCancelSkill);
            yield return new WaitUntil(() => isCancelSkill == true);
        }

        //1페이즈 사망 대사
        SoundandNotify.SetVoiceAndNotify(DevastarState.HumanDead);

        yield return new WaitForSeconds(3f);

        //2페이즈 시작 대사
        SoundManager.instance.PlayBGMWithCrossFade("resting_place_for_extinction_p2");
        SoundandNotify.SetVoiceAndNotify(DevastarState.Transform);

        isTransform = false;
        isPhase2 = true;


        MonsterStatusController.CurrHp = MonsterStatusController.MaxHp;
        MonsterStatusController.CurrMp = 0;
        devaSkill1.enabled = false;
        devaSkill2.enabled = true;
        fireParticle.Play();

        Action = MonsterState.WAIT;
        Debug.Log("2페이즈 돌입");
        isUpdate = false;
    }

    private void CancelDevaSkill1(out bool state)
    {
        //데바스타르 스킬 발동중에 변신한다면 스킬을 취소해야한다.
        devaSkill1.go_List.Clear();
        for (int i = 0; i < BoardManager.instance.width; i++)
        {
            for (int j = 0; j < BoardManager.instance.height; j++)
            {
                if(UtilHelper.HasComponent<Tile>(BoardManager.instance.characterTilesBox[i, j]))
                {
                    if (BoardManager.instance.characterTilesBox[i, j].transform.childCount > 0)
                    {
                        BoardManager.instance.characterTilesBox[i, j].GetComponent<Tile>().isSealed = false;
                        Destroy(BoardManager.instance.characterTilesBox[i, j].GetComponentInChildren<SealedEffect>().gameObject);
                    }
                }
            }
        }
        devaSkill1.rootUI.SetActive(false);
        state = true;
    }


    //플레이어와 몬스터가 타일을 옮기는 중이라면, 스킬을 사용할 수 없습니다.
    public IEnumerator WaitForStateMove()
    {
        isUpdate = true;
        yield return new WaitUntil(() => BoardManager.instance.IsPlayerMoveState() && BoardManagerMonster.instance.IsMonsterMoveState());
        yield return new WaitForSeconds(.25f);

        if (isPhase1)
        {
            UseSkill_1();
        }
        else if (isPhase2)
        {
            if (isSkillTurn2)
            {
                isSkillTurn2 = false;
                UseSkill_2();
            }
            else
            {
                isSkillTurn2 = true;
                UseSkill_3();
            }
        }
        yield return new WaitUntil(() => isUsingSkill == false);
        isUpdate = false;
    }



    private void UseSkill_1()
    {
        if (BoardManager.instance.currentState == PlayerState.MOVE)
        {
            isUsingSkill = true;

            Action = MonsterState.CASTING;
            Debug.Log("<color=#1287F6>데바</color> 1스킬 사용");
            MonsterStatusController.DecreaseMp((int)MonsterStatusController.MaxMp);
            devaSkill1.Execute();
        }
    }

    private void UseSkill_2()
    {
        //그분을 대신하여 / 그분의 의지대로
        if (BoardManager.instance.currentState == PlayerState.MOVE)
        {
            isUsingSkill = true;

            Action = MonsterState.CASTING;
            Debug.Log("<color=#1287F6>데바</color> 2스킬 사용");
            MonsterStatusController.DecreaseMp((int)MonsterStatusController.MaxMp);
            devaSkill2.Execute();
        }
    }

    private void UseSkill_3()
    {
        //묵시록의 빛이여 / 혼돈의 힘은 무한하다.

        if (BoardManager.instance.currentState == PlayerState.MOVE)
        {
            isUsingSkill = true;

            Action = MonsterState.CASTING;
            Debug.Log("<color=#1287F6>데바</color> 3스킬 사용");
            MonsterStatusController.DecreaseMp((int)MonsterStatusController.MaxMp);
            devaSkill3.Execute();
        }
    }
}