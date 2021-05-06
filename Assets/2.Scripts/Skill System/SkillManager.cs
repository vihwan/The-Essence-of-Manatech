using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    //Component

    private SkillGauge skillGauge;
    private HintManager hintManager;
    [SerializeField] private SkillCooldown[] skillCooldownList;

    public List<Skill> skillAllList = new List<Skill>();

    public void Init()
    {
        instance = GetComponent<SkillManager>();

        skillGauge = GetComponent<SkillGauge>();
        if (skillGauge != null)
        {
            skillGauge.Init();
        }
        hintManager = FindObjectOfType<HintManager>();

        skillCooldownList = FindObjectsOfType<SkillCooldown>();
        if (skillCooldownList != null)
        {
            foreach (var item in skillCooldownList)
            {
                item.Init();
            }
        }
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
                if (skillGauge.UseSkillGauge(skillAllList[0].necessaryMana))
                    Skill_Chain_Fluore();
            }
            else
            {
                AlertText.instance.ActiveText("이미 사용중 입니다.");
                return;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("두번째 스킬 사용");
            AlertText.instance.ActiveText("미구현 스킬 입니다.");
            return;

            if (skillGauge.UseSkillGauge(30))
            {
                AlertText.instance.ActiveText("미구현 스킬 입니다.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("세번째 스킬 사용");
            AlertText.instance.ActiveText("미구현 스킬 입니다.");
            return;

            if (skillGauge.UseSkillGauge(50))
            {
                AlertText.instance.ActiveText("미구현 스킬 입니다.");
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //쿨타임도 고려
            Debug.Log("네번째 스킬 사용");
            if (skillGauge.UseSkillGauge(skillAllList[3].necessaryMana))
                Skill_Jack_O_Halloween();
        }
    }

    private void Skill_Chain_Fluore()
    {
        //1번 스킬 : 체인 플로레
        // 힌트 스킬
        if (skillCooldownList[3].UseSpell(skillAllList[0].cooldownTime))
        {
            hintManager.MarkHint();
        }
        else
        {
            AlertText.instance.ActiveText("<color=#B75500>스킬 쿨타임</color> 입니다.");
            return;
        }

        //스킬 보이스 출력
        //스킬 이펙트 출력
    }

    private void Skill_Flapper()
    {
        //2번 스킬
        if (skillCooldownList[2].UseSpell(skillAllList[1].cooldownTime))
        {
            //스킬 함수
            
        }
        else
        {
            AlertText.instance.ActiveText("<color=#B75500>스킬 쿨타임</color> 입니다.");
            return;
        }
    }

    private void Skill_Jack_Frost_ShavedIce()
    {
        //3번 스킬
        if (skillCooldownList[1].UseSpell(skillAllList[2].cooldownTime))
        {
            //스킬 함수
        }
        else
        {
            AlertText.instance.ActiveText("<color=#B75500>스킬 쿨타임</color> 입니다.");
            return;
        }
    }

    private void Skill_Jack_O_Halloween()
    {
        //4번 스킬
        if (skillCooldownList[0].UseSpell(skillAllList[3].cooldownTime))
        {
            BoardManager.instance.CreateJackBomb();
            
        }
        else
        {
            AlertText.instance.ActiveText("<color=#B75500>스킬 쿨타임</color> 입니다.");
            return;
        }

        //스킬 보이스 출력
        //스킬 이펙트 출력
    }
}