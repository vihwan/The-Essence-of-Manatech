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
    private Text propertyText;

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

    public string GetSkillName()
    {
        return skillName;
    }

    public void SetSkillName(string skillName)
    {
        this.skillName = skillName;
    }
    
    //액티브 스킬 세팅 
    public void SetExplainPage(ActiveSkill activeSkill)
    {
        skillLevelText.text = "스킬 레벨 : <color=#00BAFF>" + activeSkill.Level.ToString() + "</color>";
        coolDownText.text = "쿨타임 : <color=#00BAFF>" + activeSkill.CoolTime.ToString() + "</color>";
        mpText.text = "MP 소모량 : <color=#00BAFF>" + activeSkill.Mana.ToString() + "</color>";
        explainText.text = activeSkill.Description;
    }

    //패시브 스킬 세팅 - 메소드 오버로딩
    public void SetExplainPage(PassiveSkill passiveSkill)
    {
        skillLevelText.text = "스킬 레벨 : <color=#00BAFF>" + passiveSkill.Level.ToString() + "</color>";
        explainText.text = passiveSkill.Description;
        switch (passiveSkill.Name)
        {
            case "고대의 도서관":
                coolDownText.text = "추가 시간 : <color=#00BAFF>" + passiveSkill.EigenValue + "</color> 초";
                break;

            case "쇼타임":
                coolDownText.text = "플레이어 데미지 : <color=#00BAFF>" + passiveSkill.EigenValue + "</color>";
                break;

            case "현자의 돌":
                coolDownText.text = "점수 배율 : <color=#00BAFF>" + passiveSkill.EigenValue + "</color> 배";
                break;

            case "붉은 사탕":
                coolDownText.text = "캔디바 타일 등장 확률 : <color=#00BAFF>" + passiveSkill.EigenValue + "</color> %";
                break;
        }
    }
    private void LevelUp(string skillName)
    {
        if (levelupBtn.name == "ActLevelUpButton")
        {
            print("스킬 레벨 업 : " + skillName);
            if (SkillData.instance.ActSkillLevelUp(skillName))
            {
                UISound.ClickLevelUpButton();
                CreateLevelUpParticle();
            }
            //레벨업 된 스킬 정보로 새로 갱신해줘야 한다.
            SetExplainPage(SkillData.instance.ActSkillDic[skillName]);
        }
        else if (levelupBtn.name == "PassLevelUpButton")
        {
            print("스킬 레벨 업 : " + skillName);
            if (SkillData.instance.PassSkillLevelUp(skillName))
            {
                UISound.ClickLevelUpButton();
                CreateLevelUpParticle();
            }

            //레벨업 된 스킬 정보로 새로 갱신해줘야 한다.
            SetExplainPage(SkillData.instance.PasSkillDic[skillName]);
        }
    }
    private void CreateLevelUpParticle()
    {
        //파티클 생성
        ParticleSystem levelupEffect = Instantiate(Resources.Load<ParticleSystem>("CFXR Firework (Custom)"),
                                                    transform.Find("SkillPropBg").transform.position,
                                                    Quaternion.identity);
        levelupEffect.transform.SetParent(transform);
    }
}
