using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterStateMachine : MonoBehaviour
{
    protected MonsterStateTest monsterState;

    public void SetState(MonsterStateTest monsterState)
    {
        this.monsterState = monsterState;
        monsterState.Start();
    }
}
