﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 외부 Json파일로부터 스킬 정보들을 받아와 저장하는 함수입니다.
// 게임 오브젝트가 스킬 데이터를 가져오기 위해서는 이 컴포넌트를 참조해야합니다.
// 다른 씬에서도 이 스킬 데이터를 참조해야함으로, 씬이 변경되어도 파괴되지 않도록 해야합니다.
public class SkillData : MonoBehaviour
{
    public static SkillData instance;

    [SerializeField] public Dictionary<string, ActiveSkill> ActSkillDic = new Dictionary<string, ActiveSkill>();
    [SerializeField] public Dictionary<string, PassiveSkill> PasSkillDic = new Dictionary<string, PassiveSkill>();

    //외부 컴포넌트
    private SaveAndLoad saveAndLoad;
    private SkillConversionData skillConversion;

    private void Start()
    {
        instance = this;
        saveAndLoad = FindObjectOfType<SaveAndLoad>();
        if (saveAndLoad != null)
        {
            saveAndLoad.LoadData(ActSkillDic);
            saveAndLoad.LoadData(PasSkillDic);
        }

        skillConversion = new SkillConversionData();

        DontDestroyOnLoad(this.gameObject);
    }

    public void ActSkillLevelUp(string skillName)
    {
        if (ActSkillDic[skillName].Level >= 3)
        {
            // 스킬 레벨업 불가능
            print("더이상 레벨을 올릴 수 없습니다.");
            return;
        }

        //액티브 스킬 레벨업
        ActSkillDic[skillName].Level++;
        skillConversion.ConvertActiveSkill();
    }

    public void PassSkillLevelUp(string skillName)
    {
        if (PasSkillDic[skillName].Level >= 3)
        {
            // 스킬 레벨업 불가능
            print("더이상 레벨을 올릴 수 없습니다.");
            return;
        }
        //패시브 스킬 레벨업
        PasSkillDic[skillName].Level++;
        skillConversion.ConvertActiveSkill();
    }
}
