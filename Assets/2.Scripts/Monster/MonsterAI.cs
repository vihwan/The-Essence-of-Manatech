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
    GROGGY,
    BERSERK,
    DEAD
}

public class MonsterAI : MonoBehaviour
{

    [SerializeField] private float groggyTime = 10f;

    private bool isUpdate = false;
    private bool isPhase1 = false;
    private bool isPhase2 = false;

    private MonsterStatusController monsterStatusController;
    private MonsterNotify notify;
    private DevaSkill1 devaSkill1;



    // Start is called before the first frame update
    public void Init()
    {
        monsterStatusController = FindObjectOfType<MonsterStatusController>();

        devaSkill1 = FindObjectOfType<DevaSkill1>();
        if (devaSkill1 != null)
            devaSkill1.Init();

        notify = FindObjectOfType<MonsterNotify>();
        if (notify != null)
        {
            notify.init();
            notify.gameObject.SetActive(false);
        }

        isPhase1 = true;
        groggyTime = 10f;
    }

    // Update is called once per frame
    private void Update()
    {
        //페이즈1에서 사망할 경우, 페이즈2로 돌입
        if (isPhase1)
        {
            if(monsterStatusController.CurrHp == 0f)
            {
                //벼 ㅇ 신!
                TransToDevil();
                isPhase1 = false;
            }
        }

        //마나가 꽉 차면 스킬 사용
        if (monsterStatusController.images_Gauge[MonsterStatusController.MP].fillAmount == 1f)
        {
            if (!isUpdate)
                StartCoroutine(WaitForStateMove());
        }

        //그로기 상태
        if(BoardManagerMonster.instance.currentState == MonsterState.GROGGY)
        {
            groggyTime -= Time.deltaTime;
            if(groggyTime <= 0)
            {
                groggyTime = 10f;
                BoardManagerMonster.instance.currentState = MonsterState.MOVE;
            }
        }
    }

    //페이즈 2가 되어 악마로 변신하는 메소드
    private void TransToDevil()
    {
        //유저와 몬스터의 조작을 멈추고 대기 시간을 가진다.
        BoardManager.instance.currentState = PlayerState.WAIT;
        BoardManagerMonster.instance.currentState = MonsterState.WAIT;



        isPhase2 = true;
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
            && BoardManagerMonster.instance.currentState == MonsterState.MOVE)
        {
            devaSkill1.Execute();
            BoardManagerMonster.instance.currentState = MonsterState.USESKILL;
            notify.gameObject.SetActive(true);
            monsterStatusController.DecreaseMp((int)monsterStatusController.MaxMp);
            notify.SetText("서로를 옭아매는 어리석은 인간들이여");
            notify.PlayAnim();
        }
    }

    private void UseSkill_2()
    {
        //그분을 대신하여 / 그분의 의지대로
    }
}