using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    private SkillManager skillManager;
    private HintManager hintManager;

    public void Init()
    {
        skillManager = GetComponent<SkillManager>();
        hintManager = FindObjectOfType<HintManager>();
    }

    private void Update()
    {
        if (BoardManager.instance.currentState == PlayerState.MOVE)
        {
            UseSkill();
        }
    }

    private void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("첫번째 스킬 사용");
            if (skillManager.UseSkillGauge(0))
                Skill_Chain_Fluore();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("두번째 스킬 사용");
            if (skillManager.UseSkillGauge(30))
            {
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("세번째 스킬 사용");
            if (skillManager.UseSkillGauge(50))
            {
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //쿨타임도 고려
            Debug.Log("네번째 스킬 사용");
            if (skillManager.UseSkillGauge(10))
                Skill_Jack_O_Halloween();
        }
    }

    private void Skill_Chain_Fluore()
    {
        //1번 스킬 : 체인 플로레
        // 힌트 스킬
        if (hintManager.currentHintEffect == null)
        {
            hintManager.MarkHint();
        }
        else
        {
            Debug.Log("이미 사용중");
            return;
        }
        //스킬 보이스 출력
        //스킬 이펙트 출력
    }

    private void Skill_Flapper()
    {
        //2번 스킬
    }

    private void Skill_Jack_Frost_ShavedIce()
    {
        //3번 스킬
    }

    private void Skill_Jack_O_Halloween()
    {
        //4번 스킬
        BoardManager.instance.CreateJackBomb();
        //스킬 보이스 출력
        //스킬 이펙트 출력
    }
}