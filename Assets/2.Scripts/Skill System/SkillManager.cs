using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public delegate void CantSkill(string text);
    public CantSkill appearText;


    //Variale

    [SerializeField] private List<SkillButton> skillBtns = new List<SkillButton>();
    [SerializeField] public Dictionary<string, ActiveSkill> ActSkillDic = new Dictionary<string, ActiveSkill>();
    [SerializeField] public Dictionary<string, PassiveSkill> PasSkillDic = new Dictionary<string, PassiveSkill>();


    //Component

    private SkillGauge skillGauge;
    private HintManager hintManager;
    private SaveAndLoad saveAndLoad;
    private TestSkillLevelText testSkillLevelText;
    private SkillLevelSystem skillLevelSystem;

    public void Init()
    {
        instance = GetComponent<SkillManager>();

        saveAndLoad = FindObjectOfType<SaveAndLoad>();
        if (saveAndLoad != null)
        {
            saveAndLoad.LoadData(ActSkillDic);
            saveAndLoad.LoadData(PasSkillDic);
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
           // skillBtns[i].skillInfo = ASkillList[i];
        }

        hintManager = FindObjectOfType<HintManager>();

        testSkillLevelText = FindObjectOfType<TestSkillLevelText>();
        if(testSkillLevelText != null)
        {
            testSkillLevelText.Init();
        }

        skillLevelSystem = FindObjectOfType<SkillLevelSystem>();
        if(skillLevelSystem != null)
        {
            skillLevelSystem.Init();
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
            Debug.Log("1번 스킬 사용");
            if (hintManager.currentHintEffect == null)
            {
                if (skillGauge.UseSkillGauge(ActSkillDic["체인 플로레"].Mana))
                    Skill_Chain_Fluore();
            }
            else
            {
                appearText("이미 사용중 입니다.");
                return;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("2번 스킬 사용");
            appearText("미구현 스킬 입니다.");
            return;

/*            if (skillGauge.UseSkillGauge(30))
            {
                AlertText.instance.ActiveText("미구현 스킬 입니다.");
            }*/
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("3번 스킬 사용");
            appearText("미구현 스킬 입니다.");
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
            if (skillGauge.UseSkillGauge(ActSkillDic["잭 오 할로윈"].Mana))
                Skill_Jack_O_Halloween();
        }
    }

    private void Skill_Chain_Fluore()
    {
        //1번 스킬 : 체인 플로레
        // 힌트 스킬
        if (skillBtns[0].UseSpell(ActSkillDic["체인 플로레"].CoolTime))
        {
            hintManager.MarkHint();
        }
        else
        {
            appearText("<color=#B75500>스킬 쿨타임</color> 입니다.");
            return;
        }

        //스킬 보이스 출력
        //스킬 이펙트 출력
    }

    private void Skill_Flapper()
    {
        //2번 스킬
        if (skillBtns[2].UseSpell(ActSkillDic["변이 파리채"].CoolTime))
        {
            //스킬 함수
        }
        else
        {
            appearText("<color=#B75500>스킬 쿨타임</color> 입니다.");
            return;
        }
    }

    private void Skill_Jack_Frost_ShavedIce()
    {
        //3번 스킬
        if (skillBtns[1].UseSpell(ActSkillDic["잭 프로스트 빙수"].CoolTime))
        {
            //스킬 함수
        }
        else
        {
            appearText("<color=#B75500>스킬 쿨타임</color> 입니다.");
            return;
        }
    }

    private void Skill_Jack_O_Halloween()
    {
        //4번 스킬
        if (skillBtns[3].UseSpell(ActSkillDic["잭 오 할로윈"].CoolTime))
        {
            BoardManager.instance.CreateJackBomb();
        }
        else
        {
            appearText("<color=#B75500>스킬 쿨타임</color> 입니다.");
            return;
        }

        //스킬 보이스 출력
        //스킬 이펙트 출력
    }
}