using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


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

    public static MonsterAI instance;

    [SerializeField] private float groggyTime = 10f;

    private bool isUpdate = false;
    private bool isPhase1 = false;
    private bool isPhase2 = false;
    private bool isTransform = false;
    private bool isSkillTurn2 = true;


    [SerializeField] private float elaspedTime = 0f;
    [SerializeField] private float timeStandard = 4f;

    private MonsterState monsterAction;

    //private MonsterStateTest monsterStateTest;
    private MonsterStatusController monsterStatusController;
    private MonsterNotify notify;
    private DevaSkill1 devaSkill1;
    private DevaSkill2 devaSkill2;
    private DevaSkill3 devaSkill3;

    private ParticleSystem fireParticle;

    public float ElaspedTime { get => elaspedTime; set => elaspedTime = value; }
    public float TimeStandard { get => timeStandard; set => timeStandard = value; }
    public MonsterStatusController MonsterStatusController { get => monsterStatusController; set => monsterStatusController = value; }
    public MonsterNotify Notify { get => notify; set => notify = value; }
    public float GroggyTime { get => groggyTime; set => groggyTime = value; }

    public MonsterState Action
    {
        get
        {
            return monsterAction;
        }

        set
        {
            monsterAction = value;

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
                    break;

                case MonsterState.GROGGY:
                    break;

                case MonsterState.BERSERK:
                    break;

                case MonsterState.DEAD:
                    SoundManager.instance.PlayCV("Devil_Skill_Dead");
                    GameManager.instance.isGameOver = true;
                    GameManager.instance.GameOver();
                    break;
            }
        }
    }

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

        notify = GameObject.Find("StageManager/GUIManagerCanvas/MonsterUI/Notify").GetComponent<MonsterNotify>();
        if (notify != null)
        {
            notify.init();
            notify.gameObject.SetActive(false);
        }

        isPhase1 = true;
        devaSkill1.enabled = true;
        devaSkill2.enabled = false;
        GroggyTime = 10f;

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
        GroggyTime -= Time.deltaTime;
        if (GroggyTime <= 0)
        {
            GroggyTime = 10f;
            Action = MonsterState.MOVE;
        }
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
                    isTransform = true;
                    isPhase1 = false;
                }
            }


            if (isPhase2)
            {
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

        //데바스타르 스킬 발동중에 변신한다면 스킬을 취소해야한다.
        devaSkill1.go_List.Clear();

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < BoardManager.instance.width; i++)
        {
            for (int j = 0; j < BoardManager.instance.height; j++)
            {
                if (BoardManager.instance.characterTilesBox[i, j].transform.childCount > 0)
                {
                    BoardManager.instance.characterTilesBox[i, j].GetComponent<Tile>().isSealed = false;
                    Destroy(BoardManager.instance.characterTilesBox[i, j].GetComponentInChildren<SealedEffect>().gameObject);
                }
            }
        }
        devaSkill1.rootUI.SetActive(false);

        Notify.SetText("크윽.. 방해하는 자에게 고통을!!");
        SoundManager.instance.PlayCV("Human_Death1");
        Notify.PlayAnim();

        yield return new WaitForSeconds(3f);

        Notify.NotifyImage.sprite = Resources.Load<Sprite>("notify2");
        Notify.SetText("진정한 혼돈의 힘을 보여주마!!!");
        SoundManager.instance.PlayCV("HumanToDevil");
        SoundManager.instance.PlayBGMWithCrossFade("데바스타르2");
        Notify.PlayAnim();

        isTransform = false;
        isPhase2 = true;


        MonsterStatusController.CurrHp = MonsterStatusController.MaxHp;
        MonsterStatusController.CurrMp = 0;
        devaSkill1.enabled = false;
        devaSkill2.enabled = true;
        fireParticle.Play();

        Action = MonsterState.MOVE;
        Debug.Log("2페이즈 돌입");
        isUpdate = false;
    }


    //플레이어와 몬스터가 타일을 옮기는 중이라면, 스킬을 사용할 수 없습니다.
    public IEnumerator WaitForStateMove()
    {
        isUpdate = true;
        yield return new WaitUntil(() => BoardManager.instance.IsMoveState() && BoardManagerMonster.instance.IsMoveState());
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
        isUpdate = false;
    }



    private void UseSkill_1()
    {
        if (BoardManager.instance.currentState == PlayerState.MOVE)
        {
            Action = MonsterState.CASTING;
            Debug.Log("<color=#1287F6>데바</color> 1스킬 사용");
            Notify.gameObject.SetActive(true);
            MonsterStatusController.DecreaseMp((int)MonsterStatusController.MaxMp);
            Notify.SetText("서로를 옭아매는 어리석은 인간들이여");
            Notify.PlayAnim();
            devaSkill1.Execute();
        }
    }

    private void UseSkill_2()
    {
        //그분을 대신하여 / 그분의 의지대로
        if (BoardManager.instance.currentState == PlayerState.MOVE)
        {
            Action = MonsterState.CASTING;
            Debug.Log("<color=#1287F6>데바</color> 2스킬 사용");
            Notify.gameObject.SetActive(true);
            MonsterStatusController.DecreaseMp((int)MonsterStatusController.MaxMp);
            Notify.SetText("그 분을 대신하여");
            Notify.PlayAnim();
            devaSkill2.Execute();
        }
    }

    private void UseSkill_3()
    {
        //묵시록의 빛이여 / 혼돈의 힘은 무한하다.

        if (BoardManager.instance.currentState == PlayerState.MOVE)
        {
            Action = MonsterState.CASTING;
            Debug.Log("<color=#1287F6>데바</color> 3스킬 사용");
            Notify.gameObject.SetActive(true);
            MonsterStatusController.DecreaseMp((int)MonsterStatusController.MaxMp);
            Notify.SetText("혼돈의 힘은 무한하다.");
            Notify.PlayAnim();
            devaSkill3.Execute();
        }
    }
}