using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 스킬 설명창을 담당하는 컴포넌트입니다.
// 외부에서 스킬 데이터를 가져와, 선택한 스킬에 맞게 정보를 변경시켜줍니다.
public class ExplainPage : MonoBehaviour
{
    //Text 컴포넌트
    private Text skillLevelText;
    private Text coolDownText;
    private Text mpText;
    private Text explainText;

    //Button 컴포넌트

    private Button levelupBtn;
    public string skillName = string.Empty;
    //필요한 컴포넌트

    internal void Init()
    {
        skillLevelText = transform.Find("SkillPropBg/skill_level_text").GetComponent<Text>();
        if(skillLevelText == null)
        {
            Debug.Log("스킬레벨 텍스트 참조못함");
        }

        coolDownText = transform.Find("SkillPropBg/cool_text").GetComponent<Text>();
        if (coolDownText == null)
        {
            Debug.Log("쿨타임 텍스트 참조못함");
        }

        mpText = transform.Find("SkillPropBg/mp_text").GetComponent<Text>();
        if (mpText == null)
        {
            Debug.Log("마나소모량 텍스트 참조못함");
        }

        explainText = transform.Find("ExplainBg/explainText").GetComponent<Text>();
        if (explainText == null)
        {
            Debug.Log("스킬설명 텍스트 참조못함");
        }

        levelupBtn = GetComponentInChildren<Button>(true);
        if(levelupBtn != null)
        {
            levelupBtn.onClick.AddListener(() => LevelUp(skillName));
        }

        this.gameObject.SetActive(false);
    }

    public void SetSkillName(string skillName)
    {
        this.skillName = skillName;
    }

    public void SetExplainPage(ActiveSkill activeSkill)
    {
        skillLevelText.text = "스킬 레벨 : " + activeSkill.Level.ToString();
        coolDownText.text = "쿨타임 : " + activeSkill.CoolTime.ToString();
        mpText.text = "MP 소모량 : " + activeSkill.Mana.ToString();
        explainText.text = activeSkill.Description;
    }

    public void SetExplainPage(PassiveSkill passiveSkill)
    {
        skillLevelText.text = "스킬 레벨 : " + passiveSkill.Level.ToString();
        explainText.text = passiveSkill.Description;
    }

    private void LevelUp(string skillName)
    {
        //TODO :
        if (levelupBtn.name == "ActLevelUpButton")
        {
            print("스킬 레벨 업 : " + skillName);
            SkillData.instance.ActSkillLevelUp(skillName);

            //레벨업 된 스킬 정보로 새로 갱신해줘야 한다.
            SetExplainPage(SkillData.instance.ActSkillDic[skillName]);

        }
        else if (levelupBtn.name == "PassLevelUpButton")
        {
            print("스킬 레벨 업 : " + skillName);
            SkillData.instance.PassSkillLevelUp(skillName);

            //레벨업 된 스킬 정보로 새로 갱신해줘야 한다.
            SetExplainPage(SkillData.instance.PasSkillDic[skillName]);
        }
    }

}
