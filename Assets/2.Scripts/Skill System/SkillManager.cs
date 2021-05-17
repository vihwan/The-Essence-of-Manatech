using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;

public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public delegate void CantSkill(string text);
    public CantSkill appearText;


    //Variable

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
            //skillBtns[i].skillInfo = ActSkillDic.Values.ToList()[i]; //디버그용
        }

        hintManager = FindObjectOfType<HintManager>();

        testSkillLevelText = FindObjectOfType<TestSkillLevelText>();
        if (testSkillLevelText != null)
        {
            testSkillLevelText.Init();
        }

        skillLevelSystem = FindObjectOfType<SkillLevelSystem>();
        if (skillLevelSystem != null)
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

            if (CheckSkillUse("체인 플로레", 0))
                Skill_Chain_Fluore();

        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("2번 스킬 사용");

            if (CheckSkillUse("변이 파리채", 1))
                Skill_Flapper();

        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            Debug.Log("3번 스킬 사용");

            if (CheckSkillUse("잭프로스트 빙수", 2))
                Skill_Jack_Frost_ShavedIce();

        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            Debug.Log("4번 스킬 사용");

            if (CheckSkillUse("잭 오 할로윈", 3))
                Skill_Jack_O_Halloween();
        }
    }

    //스킬 사용 여부를 확인. 마나와 쿨타임을 비교한다.
    private bool CheckSkillUse(string text, int btnNum)
    {
        if (skillGauge.UseMp(ActSkillDic[text].Mana) && skillBtns[btnNum].UseSpell(ActSkillDic[text].CoolTime))
        {
            return true;
        }
        return false;
    }

    private void Skill_Chain_Fluore()
    {
        //1번 스킬 : 체인 플로레
        // 힌트 스킬

        if (hintManager.currentHintEffect == null)
            hintManager.MarkHint();
        else
        {
            appearText("이미 사용중입니다.");
            return;
        }
        //스킬 보이스 출력
        //스킬 이펙트 출력
    }

    private void Skill_Flapper()
    {
        //2번 스킬 : 변이 파리채
        BoardManager.instance.ChangeTile();
    }

    private void Skill_Jack_Frost_ShavedIce()
    {
        //3번 스킬 : 잭프로스트 빙수
        //스킬 함수
        GUIManager.instance.OnPauseTime(ActSkillDic["잭프로스트 빙수"].EigenValue);

        //스킬 보이스 출력
        //스킬 이펙트 출력
    }

    private void Skill_Jack_O_Halloween()
    {
        //4번 스킬 : 잭 오 할로윈

        BoardManager.instance.CreateJackBomb();
        //스킬 보이스 출력
        //스킬 이펙트 출력
    }
}