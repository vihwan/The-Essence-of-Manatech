using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActivePage : MonoBehaviour
{
    private Button skillBtn1;
    private Button skillBtn2;
    private Button skillBtn3;
    private Button skillBtn4;

    private ExplainPage explainPage;

    public ExplainPage ExplainPage { get => explainPage; set => explainPage = value; }

    // Start is called before the first frame update
    public void Init()
    {
        skillBtn1 = transform.Find("SkillSlotBg/Slot01").GetComponent<Button>();
        if(skillBtn1 != null)
        {
            skillBtn1.onClick.AddListener(OpenExplainPage01);
        }

        skillBtn2 = transform.Find("SkillSlotBg/Slot02").GetComponent<Button>();
        if (skillBtn2 != null)
        {
            skillBtn2.onClick.AddListener(OpenExplainPage02);
        }

        skillBtn3 = transform.Find("SkillSlotBg/Slot03").GetComponent<Button>();
        if (skillBtn3 != null)
        {
            skillBtn3.onClick.AddListener(OpenExplainPage03);
        }

        skillBtn4 = transform.Find("SkillSlotBg/Slot04").GetComponent<Button>();
        if (skillBtn4 != null)
        {
            skillBtn4.onClick.AddListener(OpenExplainPage04);
        }

        ExplainPage = GetComponentInChildren<ExplainPage>();
        if(ExplainPage != null)
        {
            ExplainPage.Init();
        }
    }

    private void OpenExplainPage01()
    {
        ExplainPage.gameObject.SetActive(true);
        ExplainPage.SetSkillName("체인 플로레");
        ExplainPage.SetExplainPage(SkillData.instance.ActSkillDic["체인 플로레"]);
    }

    private void OpenExplainPage02()
    {
        ExplainPage.gameObject.SetActive(true);
        ExplainPage.SetSkillName("변이 파리채");
        ExplainPage.SetExplainPage(SkillData.instance.ActSkillDic["변이 파리채"]);
    }

    private void OpenExplainPage03()
    {
        ExplainPage.gameObject.SetActive(true);
        ExplainPage.SetSkillName("잭프로스트 빙수");
        ExplainPage.SetExplainPage(SkillData.instance.ActSkillDic["잭프로스트 빙수"]);
    }

    private void OpenExplainPage04()
    {
        ExplainPage.gameObject.SetActive(true);
        ExplainPage.SetSkillName("잭 오 할로윈");
        ExplainPage.SetExplainPage(SkillData.instance.ActSkillDic["잭 오 할로윈"]);
    }
}
