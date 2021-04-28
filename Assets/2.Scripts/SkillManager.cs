using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    private SkillGauge skillGauge;


    private void Start()
    {
        skillGauge = GetComponent<SkillGauge>();
        skillGauge.CurrentSkillMana = 100f;
        skillGauge.TotalSkillMana = skillGauge.CurrentSkillMana;
    }


    private void Update()
    {
        SkillGaugeStatus();
    }

    private void SkillGaugeStatus()
    {
        //타일을 파괴할 때 마다 스킬 게이지가 증가
        //스킬을 사용할 때 마다 스킬 게이지가 감소

/*        <Example>
        if (~~)
        {
            skillGauge.CurrentSkillMana += 10f;
            skillGauge.SkillGaugeText.text = Mathf.Round(skillGauge.CurrentSkillMana).ToString() + " / 100";
            skillGauge.SkiiGaugeImage.fillAmount = skillGauge.CurrentSkillMana / skillGauge.TotalSkillMana;
        }*/
    }
}
