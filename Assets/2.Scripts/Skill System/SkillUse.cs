using UnityEngine;

public class SkillUse : MonoBehaviour
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
            if (hintManager.currentHintEffect == null)
            {
                if (skillManager.UseSkillGauge(1))
                    Skill_Chain_Fluore();
            }
            else
            {
                Debug.Log("이미 사용중");
                return;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("두번째 스킬 사용");
            if (skillManager.UseSkillGauge(30))
            {
                Debug.Log("미구현 스킬입니다.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("세번째 스킬 사용");
            if (skillManager.UseSkillGauge(50))
            {
                Debug.Log("미구현 스킬입니다.");
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

        hintManager.MarkHint();

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