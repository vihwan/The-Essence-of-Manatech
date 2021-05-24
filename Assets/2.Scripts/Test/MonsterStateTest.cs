using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterStateTest
{
    protected readonly MonsterAI _monsterAI;

    public MonsterStateTest(MonsterAI monsterAI)
    {
        _monsterAI = monsterAI;
    }

    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual void Idle()
    {

    }

    public virtual void MoveTile()
    {

    }

    public virtual void UseSkill()
    {

    }
    public virtual void TransformToDevil()
    {

    }
    public virtual void Groggy()
    {

    }
    public virtual void Berserk()
    {

    }
    public virtual void Dead()
    {

    }

}
