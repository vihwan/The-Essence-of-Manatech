using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MoveTile : MonsterStateTest
{
    public MoveTile(MonsterAI monsterAI) : base(monsterAI)
    {
    }

    public override IEnumerator Start()
    {
        BoardManagerMonster.instance.MoveTile();
        _monsterAI.SetState(new Idle(_monsterAI));
        yield break;
    }
}