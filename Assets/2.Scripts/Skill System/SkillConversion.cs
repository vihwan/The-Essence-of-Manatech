﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//스킬 레벨에 따라 효과가 달라지게 하는 컴포넌트이다.
public class SkillConversion
{
    List<ActiveSkill> activeSkills = SkillManager.instance.ActSkillDic.Values.ToList();
    List<PassiveSkill> passiveSkills = SkillManager.instance.PasSkillDic.Values.ToList();

    private float CandyScoreMag;

    public void ConvertActiveSkill()
    {
        //"체인 플로레"
        if (activeSkills[0].Level == 2)
        {
            activeSkills[0].Mana = 25;
            activeSkills[0].CoolTime = 8;
        }
        else if(activeSkills[0].Level == 3)
        {
            activeSkills[0].Mana = 20;
            activeSkills[0].CoolTime = 6;
        }

        //"변이 파리채"
        if (activeSkills[1].Level == 2)
        {
            activeSkills[1].Mana = 40;
            activeSkills[1].CoolTime = 8;
        }
        else if (activeSkills[1].Level == 3)
        {
            activeSkills[1].Mana = 20;
            activeSkills[1].CoolTime = 6;
        }

        //"체인 플로레"
        if (activeSkills[2].Level == 2)
        {
            activeSkills[2].Mana = 25;
            activeSkills[2].CoolTime = 8;
        }
        else if (activeSkills[2].Level == 3)
        {
            activeSkills[2].Mana = 20;
            activeSkills[2].CoolTime = 6;
        }

        //"잭 오 할로윈"
        if (activeSkills[3].Level == 2)
        {
            activeSkills[3].Mana = 60;
            activeSkills[3].CoolTime = 25;
            //잭오할로윈 폭탄갯수 = 4
            activeSkills[3].EigenValue = 4;

        }
        else if (activeSkills[3].Level == 3)
        {
            activeSkills[3].Mana = 40;
            activeSkills[3].CoolTime = 20;
            //잭오할로윈 폭탄갯수 = 5
            activeSkills[3].EigenValue = 5;
        }
    }

    public void ConvertPassiveSkill()
    {
        //"고대의 도서관" - 지속시간 증가
        if (passiveSkills[0].Level == 2)
        {
            passiveSkills[0].EigenValue = 20;
        }
        else if (passiveSkills[0].Level == 3)
        {
            passiveSkills[0].EigenValue = 30;
        }

        //"쇼타임" - 점수 배율
        if (passiveSkills[1].Level == 2)
        {
            passiveSkills[1].EigenValue = 1.2f;
        }
        else if (passiveSkills[1].Level == 3)
        {
            passiveSkills[1].EigenValue = 1.5f;
        }

        //"현자의 돌" - 점수 추가 배율
        if (passiveSkills[2].Level == 2)
        {
            passiveSkills[2].EigenValue = 1.2f;
        }
        else if (passiveSkills[2].Level == 3)
        {
            passiveSkills[2].EigenValue = 1.5f;
        }

        //"붉은 사탕" - 점수 배율
        if (passiveSkills[3].Level == 2)
        {
            passiveSkills[3].EigenValue = 6f;
            CandyScoreMag = 1.5f;
        }
        else if (passiveSkills[3].Level == 3)
        {
            passiveSkills[3].EigenValue = 10f;
            CandyScoreMag = 2f;
        }
    }
}
