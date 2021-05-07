using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    //Variale

    [SerializeField] private List<SkillButton> skillBtns = new List<SkillButton>();
    [SerializeField] private List<ActiveSkill> ASkillList = new List<ActiveSkill>();
    [SerializeField] private List<PassiveSkill> PSkillList = new List<PassiveSkill>();


    //Component

    private SkillGauge skillGauge;
    private HintManager hintManager;
    private SaveAndLoad saveAndLoad;

    public void Init()
    {
        instance = GetComponent<SkillManager>();

        saveAndLoad = FindObjectOfType<SaveAndLoad>();
        if (saveAndLoad != null)
        {
            saveAndLoad.LoadData<ActiveSkill>(ASkillList);
            saveAndLoad.LoadData<PassiveSkill>(PSkillList);
        }
 

        skillGauge = GetComponent<SkillGauge>();
        if (skillGauge != null)
        {
            skillGauge.Init();
        }

        SkillButton[] btns = GetComponentsInChildren<SkillButton>(true);
        skillBtns.AddRange(btns);
        for (int i = 0; i < skillBtns.Count; i++)
        {
            skillBtns[i].Init();
            skillBtns[i].skillInfo = ASkillList[i];
        }

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
            Debug.Log("1번 스킬 사용");
            if (hintManager.currentHintEffect == null)
            {
                if (skillGauge.UseSkillGauge(skillBtns[0].skillInfo.necessaryMana))
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
            Debug.Log("2번 스킬 사용");
            AlertText.instance.ActiveText("미구현 스킬 입니다.");
            return;

/*            if (skillGauge.UseSkillGauge(30))
            {
                AlertText.instance.ActiveText("미구현 스킬 입니다.");
            }*/
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("3번 스킬 사용");
            AlertText.instance.ActiveText("미구현 스킬 입니다.");
            return;

/*            if (skillGauge.UseSkillGauge(50))
            {
                AlertText.instance.ActiveText("미구현 스킬 입니다.");
            }*/
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //쿨타임도 고려
            Debug.Log("4번 스킬 사용");
            if (skillGauge.UseSkillGauge(skillBtns[3].skillInfo.necessaryMana))
                Skill_Jack_O_Halloween();
        }
    }

    private void Skill_Chain_Fluore()
    {
        //1번 스킬 : 체인 플로레
        // 힌트 스킬
        if (skillBtns[0].UseSpell(skillBtns[0].skillInfo.cooldownTime))
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
        if (skillBtns[2].UseSpell(ASkillList[1].cooldownTime))
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
        if (skillBtns[1].UseSpell(ASkillList[2].cooldownTime))
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
        if (skillBtns[3].UseSpell(skillBtns[3].skillInfo.cooldownTime))
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

    public void LoadToSkillInfo()
    {
        for (int i = 0; i < ASkillList.Count; i++)
        {
        }
    }
}