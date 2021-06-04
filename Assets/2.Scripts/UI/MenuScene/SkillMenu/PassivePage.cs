using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PassivePage : MonoBehaviour
{
    private Button skillBtn1;
    private Button skillBtn2;
    private Button skillBtn3;
    private Button skillBtn4;

    private ExplainPage explainPage;

    // Start is called before the first frame update
    public void Init()
    {
        skillBtn1 = transform.Find("SkillSlotBg/Slot01").GetComponent<Button>();
        if (skillBtn1 != null)
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

        explainPage = GetComponentInChildren<ExplainPage>(true);
        if (explainPage != null)
        {
            explainPage.Init();
        }

        this.gameObject.SetActive(false);
    }

    private void OpenExplainPage01()
    {
        explainPage.gameObject.SetActive(true);
        explainPage.SetSkillName("고대의 도서관");
        explainPage.SetExplainPage(SkillData.instance.PasSkillDic["고대의 도서관"]);
    }

    private void OpenExplainPage02()
    {
        explainPage.gameObject.SetActive(true);
        explainPage.SetSkillName("쇼타임");
        explainPage.SetExplainPage(SkillData.instance.PasSkillDic["쇼타임"]);
    }

    private void OpenExplainPage03()
    {
        explainPage.gameObject.SetActive(true);
        explainPage.SetSkillName("현자의 돌");
        explainPage.SetExplainPage(SkillData.instance.PasSkillDic["현자의 돌"]);
    }

    private void OpenExplainPage04()
    {
        explainPage.gameObject.SetActive(true);
        explainPage.SetSkillName("붉은 사탕");
        explainPage.SetExplainPage(SkillData.instance.PasSkillDic["붉은 사탕"]);
    }
}
