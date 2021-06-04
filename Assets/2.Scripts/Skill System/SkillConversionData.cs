using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

//스킬 레벨에 따라 효과가 달라지게 하는 컴포넌트이다.
public class SkillConversionData
{

    public void ConvertActiveSkill()
    {
        //"체인 플로레"
        if (SkillData.instance.ActSkillDic["체인 플로레"].Level == 2)
        {
            SkillData.instance.ActSkillDic["체인 플로레"].Mana = 25;
            SkillData.instance.ActSkillDic["체인 플로레"].CoolTime = 8;
        }
        else if(SkillData.instance.ActSkillDic["체인 플로레"].Level == 3)
        {
            SkillData.instance.ActSkillDic["체인 플로레"].Mana = 20;
            SkillData.instance.ActSkillDic["체인 플로레"].CoolTime = 6;
        }

        //"변이 파리채"
        if (SkillData.instance.ActSkillDic["변이 파리채"].Level == 2)
        {
            SkillData.instance.ActSkillDic["변이 파리채"].Mana = 40;
            SkillData.instance.ActSkillDic["변이 파리채"].CoolTime = 8;
            SkillData.instance.ActSkillDic["변이 파리채"].EigenValue = 7;
        }
        else if (SkillData.instance.ActSkillDic["변이 파리채"].Level == 3)
        {
            SkillData.instance.ActSkillDic["변이 파리채"].Mana = 20;
            SkillData.instance.ActSkillDic["변이 파리채"].CoolTime = 6;
            SkillData.instance.ActSkillDic["변이 파리채"].EigenValue = 10;
        }

        //"잭프로스트 빙수"
        if (SkillData.instance.ActSkillDic["잭프로스트 빙수"].Level == 2)
        {
            SkillData.instance.ActSkillDic["잭프로스트 빙수"].Mana = 70;
            SkillData.instance.ActSkillDic["잭프로스트 빙수"].CoolTime = 35;
            SkillData.instance.ActSkillDic["잭프로스트 빙수"].EigenValue = 7;
        }
        else if (SkillData.instance.ActSkillDic["잭프로스트 빙수"].Level == 3)
        {
            SkillData.instance.ActSkillDic["잭프로스트 빙수"].Mana = 50;
            SkillData.instance.ActSkillDic["잭프로스트 빙수"].CoolTime = 30;
            SkillData.instance.ActSkillDic["잭프로스트 빙수"].EigenValue = 10;
        }

        //"잭 오 할로윈"
        if (SkillData.instance.ActSkillDic["잭 오 할로윈"].Level == 2)
        {
            SkillData.instance.ActSkillDic["잭 오 할로윈"].Mana = 60;
            SkillData.instance.ActSkillDic["잭 오 할로윈"].CoolTime = 25;
            //잭오할로윈 폭탄갯수 = 4
            SkillData.instance.ActSkillDic["잭 오 할로윈"].EigenValue = 4;

        }
        else if (SkillData.instance.ActSkillDic["잭 오 할로윈"].Level == 3)
        {
            SkillData.instance.ActSkillDic["잭 오 할로윈"].Mana = 40;
            SkillData.instance.ActSkillDic["잭 오 할로윈"].CoolTime = 20;
            //잭오할로윈 폭탄갯수 = 5
            SkillData.instance.ActSkillDic["잭 오 할로윈"].EigenValue = 5;
        }
    }

    public void ConvertPassiveSkill()
    {
        //"고대의 도서관" - 지속시간 증가
        if (SkillData.instance.PasSkillDic["고대의 도서관"].Level == 2)
        {
            SkillData.instance.PasSkillDic["고대의 도서관"].EigenValue = 20;
        }
        else if (SkillData.instance.PasSkillDic["고대의 도서관"].Level == 3)
        {
            SkillData.instance.PasSkillDic["고대의 도서관"].EigenValue = 30;
        }

        //"쇼타임" - 점수 추가 배율
        if (SkillData.instance.PasSkillDic["쇼타임"].Level == 2)
        {
            SkillData.instance.PasSkillDic["쇼타임"].EigenValue = 0.5f;
        }
        else if (SkillData.instance.PasSkillDic["쇼타임"].Level == 3)
        {
            SkillData.instance.PasSkillDic["쇼타임"].EigenValue = 1f;
        }

        //"현자의 돌" - 점수 추가 배율
        if (SkillData.instance.PasSkillDic["현자의 돌"].Level == 2)
        {
            SkillData.instance.PasSkillDic["현자의 돌"].EigenValue = 1.2f;
        }
        else if (SkillData.instance.PasSkillDic["현자의 돌"].Level == 3)
        {
            SkillData.instance.PasSkillDic["현자의 돌"].EigenValue = 1.5f;
        }

        //"붉은 사탕" - 점수 배율
        if (SkillData.instance.PasSkillDic["붉은 사탕"].Level == 2)
        {
            SkillData.instance.PasSkillDic["붉은 사탕"].EigenValue = 6f;
        }
        else if (SkillData.instance.PasSkillDic["붉은 사탕"].Level == 3)
        {
            SkillData.instance.PasSkillDic["붉은 사탕"].EigenValue = 9f;
        }
    }
}
