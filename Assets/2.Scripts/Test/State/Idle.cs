using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : MonsterStateTest
{
    public Idle(MonsterAI monsterAI) : base(monsterAI)
    {

    }

    public override IEnumerator Start()
    {
        while(true)
        {
            _monsterAI.ElaspedTime += Time.deltaTime;
            if (_monsterAI.ElaspedTime >= _monsterAI.TimeStandard)
            {
                if (_monsterAI.MonsterStatusController.images_Gauge[MonsterStatusController.MP].fillAmount == 1f)
                {
                    _monsterAI.SetState(new UseSkill(_monsterAI));
                    _monsterAI.ElaspedTime = 0f;
                    yield break;
                }
                else
                {
                    _monsterAI.SetState(new MoveTile(_monsterAI));
                    _monsterAI.ElaspedTime = 0f;
                    yield break;
                }
            }
        } 
    }
}
