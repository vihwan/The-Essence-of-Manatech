using UnityEngine;

public class SkillEffect : MonoBehaviour
{
    private SkillManager skillManager;

    public void Init()
    {
        skillManager = GetComponent<SkillManager>();
    }

    private void Update()
    {
        if (BoardManager.instance.currentState == BoardState.MOVE)
        {
            UseSkill();
        }
    }

    private void UseSkill()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("첫번째 스킬 사용");
            if (skillManager.UseSkillGauge(20))
            {
            }
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
        //1번 스킬
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