using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private SkillGauge skillGauge;
    private SkillUse skillUse;
    private FindMatches findMatches;

    private void Start()
    {
        skillGauge = GetComponent<SkillGauge>();
        skillUse = GetComponent<SkillUse>();
        if (skillUse != null)
        {
            skillUse.Init();
        }
        findMatches = FindObjectOfType<FindMatches>();
        skillGauge.CurrentSkillMana = 0f; //시작시 마나는 0으로 설정
        skillGauge.TotalSkillMana = 200f; //총 마나 양을 200으로 설정
    }

    private void Update()
    {
        //매 프레임마다 게이지 상태를 갱신
        SkillGaugeStatus();
    }

    private void SkillGaugeStatus()
    {
        skillGauge.SkillGaugeText.text = Mathf.Round(skillGauge.CurrentSkillMana).ToString() + " / 200";
        skillGauge.SkiiGaugeImage.fillAmount = skillGauge.CurrentSkillMana / skillGauge.TotalSkillMana;
    }

    //타일을 파괴할 때 마다 스킬 게이지가 증가
    public void GainSkillGauge()
    {
        if (skillGauge.CurrentSkillMana >= skillGauge.TotalSkillMana)
        {
            skillGauge.CurrentSkillMana = skillGauge.TotalSkillMana;
            return;
        }

        skillGauge.CurrentSkillMana += (1f * findMatches.currentMatches.Count);
    }

    public bool UseSkillGauge(int useMana)
    {
        if (skillGauge.CurrentSkillMana < useMana)
        {
            Debug.Log("<color=#00C7FF>마나</color> 부족 ");
            return false;
        }

        skillGauge.CurrentSkillMana -= useMana;
        return true;
    }
}