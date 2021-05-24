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


    [SerializeField] private float elaspedTime = 0f;
    [SerializeField] private float timeStandard = 4f;

    internal MonsterState currentState;

    private MonsterStateTest monsterStateTest;
    private MonsterStatusController monsterStatusController;
    private MonsterNotify notify;
    private DevaSkill1 devaSkill1;

    public float ElaspedTime { get => elaspedTime; set => elaspedTime = value; }
    public float TimeStandard { get => timeStandard; set => timeStandard = value; }
    public MonsterStatusController MonsterStatusController { get => monsterStatusController; set => monsterStatusController = value; }
    public MonsterNotify Notify { get => notify; set => notify = value; }
    public DevaSkill1 DevaSkill1 { get => devaSkill1; set => devaSkill1 = value; }


    // Start is called before the first frame update
    public void Init()
    {
        instance = GetComponent<MonsterAI>();
        MonsterStatusController = FindObjectOfType<MonsterStatusController>();

        DevaSkill1 = FindObjectOfType<DevaSkill1>();
        if (DevaSkill1 != null)
            DevaSkill1.Init();

        Notify = FindObjectOfType<MonsterNotify>();
        if (Notify != null)
        {
            Notify.init();
            Notify.gameObject.SetActive(false);
        }

        isPhase1 = true;
        groggyTime = 10f;

        currentState = MonsterState.MOVE;
    }

    public void SetState(MonsterStateTest monsterState)
    {
        monsterStateTest = monsterState;
        StartCoroutine(monsterState.Start());
    }

    // Update is called once per frame
    private void Update()
    {
        //페이즈1에서 사망할 경우, 페이즈2로 돌입
        if (isPhase1)
        {
            if (currentState == MonsterState.MOVE)
            {
                ElaspedTime += Time.deltaTime;
                if (ElaspedTime >= TimeStandard)
                {
                    //타일 옮기기 
                    BoardManagerMonster.instance.MoveTile();
                    ElaspedTime = 0f;
                }
            }

            //마나가 꽉 차면 스킬 사용
            if (MonsterStatusController.images_Gauge[MonsterStatusController.MP].fillAmount == 1f)
            {
                if (!isUpdate)
                    StartCoroutine(WaitForStateMove());

                ElaspedTime = 0f;
            }


            if (MonsterStatusController.CurrHp == 0f)
            {
                //유저와 몬스터의 조작을 멈추고 대기 시간을 가진다.
                currentState = MonsterState.TRANSFORM;
                isTransform = true;
                isPhase1 = false;
            }
        }

        if (isTransform)
        {
            //벼 ㅇ 신!
            StartCoroutine(TransToDevil());
            isTransform = false;
            isPhase2 = true;
        }


        if (isPhase2)
        {
            if (currentState == MonsterState.MOVE)
            {
                ElaspedTime += Time.deltaTime;
                if (ElaspedTime >= TimeStandard)
                {
                    //타일 옮기기 
                    BoardManagerMonster.instance.MoveTile();
                    ElaspedTime = 0f;
                }
            }

            //마나가 꽉 차면 스킬 사용
            if (MonsterStatusController.images_Gauge[MonsterStatusController.MP].fillAmount == 1f)
            {
                if (!isUpdate)
                    StartCoroutine(WaitForStateMove());

                ElaspedTime = 0f;
            }

            if (MonsterStatusController.CurrHp == 0f)
            {
                //데바스타르 사망
                //게임 오버
                currentState = MonsterState.DEAD;
                isPhase2 = false;
            }
        }


        //그로기 상태
        if (currentState == MonsterState.GROGGY)
        {
            groggyTime -= Time.deltaTime;
            if (groggyTime <= 0)
            {
                groggyTime = 10f;
                currentState = MonsterState.MOVE;
            }
        }
    }

    //페이즈 2가 되어 악마로 변신하는 메소드
    private IEnumerator TransToDevil()
    {
        Notify.SetText("크윽.. 방해하는 자에게 고통을!!");
        SoundManager.instance.PlayCV("Human_Death1");
        Notify.PlayAnim();

        yield return new WaitForSeconds(3f);

        Notify.NotifyImage.sprite = Resources.Load<Sprite>("notify2");
        Notify.SetText("진정한 혼돈의 힘을 보여주마!!!");
        SoundManager.instance.PlayCV("HumanToDevil");
        Notify.PlayAnim();

        yield return null;
    }


    //플레이어와 몬스터가 타일을 옮기는 중이라면, 스킬을 사용할 수 없습니다.
    public IEnumerator WaitForStateMove()
    {
        isUpdate = true;
        yield return new WaitUntil(() => BoardManager.instance.IsMoveState() && BoardManagerMonster.instance.IsMoveState());
        yield return new WaitForSeconds(.25f);

        if (isPhase1)
            UseSkill_1();
        else if (isPhase2)
            UseSkill_2();

        isUpdate = false;
    }



    private void UseSkill_1()
    {
        if (BoardManager.instance.currentState == PlayerState.MOVE
            && currentState == MonsterState.MOVE)
        {
            DevaSkill1.Execute();
            currentState = MonsterState.USESKILL;
            Notify.gameObject.SetActive(true);
            MonsterStatusController.DecreaseMp((int)MonsterStatusController.MaxMp);
            Notify.SetText("서로를 옭아매는 어리석은 인간들이여");
            Notify.PlayAnim();
        }
    }

    private void UseSkill_2()
    {
        //그분을 대신하여 / 그분의 의지대로
    }
}