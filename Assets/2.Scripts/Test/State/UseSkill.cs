using System.Collections;
using System.Collections.Generic;
using UnityEngine;


internal class UseSkill : MonsterStateTest
{
    public UseSkill(MonsterAI monsterAI) : base(monsterAI)
    {
    }

    public override IEnumerator Start()
    {
        _monsterAI.Notify.gameObject.SetActive(true);
        _monsterAI.MonsterStatusController.DecreaseMp((int)_monsterAI.MonsterStatusController.MaxMp);
        _monsterAI.Notify.SetText("서로를 옭아매는 어리석은 인간들이여");
        _monsterAI.Notify.PlayAnim();
        yield break;
    }
}