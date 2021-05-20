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

        groggyTime = 10f;
    }

    // Update is called once per frame
    private void Update()
    {
        if (monsterStatusController.images_Gauge[MonsterStatusController.MP].fillAmount == 1f)
        {
            if (!isUpdate)
                StartCoroutine(WaitForShifting());
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

    //플레이어 상태가 타일을 옮기는 중이라면, 스킬을 사용할 수 없습니다.
    public IEnumerator WaitForShifting()
    {
        isUpdate = true;
        yield return new WaitUntil(() => BoardManager.instance.IsMoveState() && BoardManagerMonster.instance.IsMoveState());
        yield return new WaitForSeconds(.25f);
        UseSkill();
        isUpdate = false;
    }

    private void UseSkill()
    {
        if (BoardManager.instance.currentState == PlayerState.MOVE
            && BoardManagerMonster.instance.currentState == MonsterState.MOVE)
        {
            devaSkill1.Execute();
            BoardManagerMonster.instance.currentState = MonsterState.USESKILL;
            notify.gameObject.SetActive(true);
            monsterStatusController.DecreaseMp((int)monsterStatusController.MaxMp);
            notify.PlayAnim();
        }
    }

    public void Groggy()
    {
        BoardManagerMonster.instance.currentState = MonsterState.GROGGY;
    }
}