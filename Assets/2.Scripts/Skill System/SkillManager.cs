﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;


//게임 씬에서 스킬의 사용을 관리하는 매니저 입니다.
//게임 씬에서 스킬 버튼에 맞게 데이터를 할당해주고 스킬을 사용할 수 있습니다.
public class SkillManager : MonoBehaviour
{
    public static SkillManager instance;

    public delegate void CantSkill(string text);
    public CantSkill appearText;

    //Variable

    [SerializeField] private List<SkillButton> skillBtns = new List<SkillButton>();
    [SerializeField] public Dictionary<string, ActiveSkill> ActSkillDic = new Dictionary<string, ActiveSkill>();
    [SerializeField] public Dictionary<string, PassiveSkill> PasSkillDic = new Dictionary<string, PassiveSkill>();

    //외부 Component
    private PlayerStatusController playerStatusController;
    private HintManager hintManager;
    private SkillData skillData;
    private SkillEffectManager skillEffectManager;

    public void Init()
    {
        instance = GetComponent<SkillManager>();

        skillData = FindObjectOfType<SkillData>();
        if (skillData != null)
        {
            //스킬 매니저가 사용할 스킬들을 SkillData에서 가져옵니다.
            this.ActSkillDic = skillData.ActSkillDic;
            this.PasSkillDic = skillData.PasSkillDic;
        }


        playerStatusController = FindObjectOfType<PlayerStatusController>();
        if (playerStatusController != null)
        {
            playerStatusController.Init();
        }

        SkillButton[] btns = GetComponentsInChildren<SkillButton>(true);
        skillBtns.AddRange(btns);
        for (int i = 0; i < skillBtns.Count; i++)
        {
            skillBtns[i].Init();
            //skillBtns[i].skillInfo = ActSkillDic.Values.ToList()[i]; //디버그용
        }

        skillEffectManager = FindObjectOfType<SkillEffectManager>();
        if(skillEffectManager != null)
        {
            skillEffectManager.Init();
        }


        hintManager = FindObjectOfType<HintManager>();
    }

    private void Update()
    {
        if (GameManager.instance.GameState == GameState.PLAYING)
        {
            if (BoardManager.instance.currentState == PlayerState.MOVE 
                && MonsterAI.instance.Action != MonsterState.USESKILL)
            {
                //몬스터가 스킬을 사용하려는 상태일때는 스킬을 사용할 수 없습니다.

                UseSkill();
            }
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
        /* 쿨타임이 아닐때
         *      - 마나가 있으면 -> 스킬 사용
         *      -        없으면  -> 마나가 없습니다.
         * 
         * 쿨타임일때
         *      - 쿨타임입니다.
         * **/

        if (skillBtns[btnNum].CanUseSpell())
        {
            if (playerStatusController.CanUseMp(ActSkillDic[text].Mana))
            {
                //스킬 버튼의 쿨타임 
                skillBtns[btnNum].CooldownSkillImage.gameObject.SetActive(true);
                skillBtns[btnNum].CooldownText.gameObject.SetActive(true);
                skillBtns[btnNum].CooldownTimer = ActSkillDic[text].CoolTime;
                skillBtns[btnNum].CooldownTime = ActSkillDic[text].CoolTime;

                playerStatusController.CurrMp -= ActSkillDic[text].Mana;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    //1번 스킬 : 체인 플로레
    private void Skill_Chain_Fluore()
    {
        // 힌트 스킬
        if (hintManager.currentHintEffect == null)
            skillEffectManager.ActiveSkillEffect(SkillEffectType.Chain);
        else
        {
            appearText("이미 사용중입니다.");
            return;
        }
        //스킬 보이스 출력
        //스킬 이펙트 출력
    }

    //2번 스킬 : 변이 파리채
    private void Skill_Flapper()
    {
        skillEffectManager.ActiveSkillEffect(SkillEffectType.Flapper);
    }

    //3번 스킬 : 잭프로스트 빙수
    private void Skill_Jack_Frost_ShavedIce()
    {
        GUIManager.instance.OnPauseTime(ActSkillDic["잭프로스트 빙수"].EigenValue);

        //스킬 보이스 출력
        //스킬 이펙트 출력
    }

    //4번 스킬 : 잭 오 할로윈
    private void Skill_Jack_O_Halloween()
    {

        BoardManager.instance.CreateJackBomb();
        //스킬 보이스 출력
        //스킬 이펙트 출력
    }
}