using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Begin : MonsterStateTest
{
    public Begin(MonsterAI monsterAI) : base(monsterAI)
    {
    }

    public override IEnumerator Start()
    {
        Debug.Log("Monster AI Begin");

        yield return new WaitForSeconds(2f);

        //_monsterAI.SetState(new Idle(_monsterAI));

        yield break;
    }
}