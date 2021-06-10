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

    public ExplainPage ExplainPage { get => explainPage; set => explainPage = value; }

    // Start is called before the first frame update
    public void Init()
    {
        skillBtn1 = transform.Find("SkillSlotBg/Slot01").GetComponent<Button>();
        if (skillBtn1 != null)
        {
            skillBtn1.onClick.AddListener(() => OpenExplainPage("고대의 도서관"));
        }

        skillBtn2 = transform.Find("SkillSlotBg/Slot02").GetComponent<Button>();
        if (skillBtn2 != null)
        {
            skillBtn2.onClick.AddListener(() => OpenExplainPage("쇼타임"));
        }

        skillBtn3 = transform.Find("SkillSlotBg/Slot03").GetComponent<Button>();
        if (skillBtn3 != null)
        {
            skillBtn3.onClick.AddListener(() => OpenExplainPage("현자의 돌"));
        }

        skillBtn4 = transform.Find("SkillSlotBg/Slot04").GetComponent<Button>();
        if (skillBtn4 != null)
        {
            skillBtn4.onClick.AddListener(() => OpenExplainPage("붉은 사탕"));
        }

        ExplainPage = GetComponentInChildren<ExplainPage>(true);
        if (ExplainPage != null)
        {
            ExplainPage.Init();
        }

        this.gameObject.SetActive(false);
    }

    private void OpenExplainPage(string skillName)
    {
        ExplainPage.gameObject.SetActive(true);
        ExplainPage.SetSkillName(skillName);
        ExplainPage.SetExplainPage(SkillData.instance.PasSkillDic[skillName]);
        CreateGoldBorder(skillName);
        UISound.ClickButton();
    }
    /*
        private void OpenExplainPage02()
        {
            ExplainPage.gameObject.SetActive(true);
            ExplainPage.SetSkillName("쇼타임");
            ExplainPage.SetExplainPage(SkillData.instance.PasSkillDic["쇼타임"]);
        }

        private void OpenExplainPage03()
        {
            ExplainPage.gameObject.SetActive(true);
            ExplainPage.SetSkillName("현자의 돌");
            ExplainPage.SetExplainPage(SkillData.instance.PasSkillDic["현자의 돌"]);
        }

        private void OpenExplainPage04()
        {
            ExplainPage.gameObject.SetActive(true);
            ExplainPage.SetSkillName("붉은 사탕");
            ExplainPage.SetExplainPage(SkillData.instance.PasSkillDic["붉은 사탕"]);
        }*/


    private void CreateGoldBorder(string skillname)
    {
        AllButtonSpriteNormal();

        if (skillname == "고대의 도서관")
        {
            skillBtn1.GetComponent<Image>().sprite = Resources.Load<Sprite>("goldframe");
        }
        else if (skillname == "쇼타임")
        {
            skillBtn2.GetComponent<Image>().sprite = Resources.Load<Sprite>("goldframe");
        }
        else if (skillname == "현자의 돌")
        {
            skillBtn3.GetComponent<Image>().sprite = Resources.Load<Sprite>("goldframe");
        }
        else if (skillname == "붉은 사탕")
        {
            skillBtn4.GetComponent<Image>().sprite = Resources.Load<Sprite>("goldframe");
        }
    }

    internal void AllButtonSpriteNormal()
    {
        skillBtn1.GetComponent<Image>().sprite = Resources.Load<Sprite>("stage_frame_dim");
        skillBtn2.GetComponent<Image>().sprite = Resources.Load<Sprite>("stage_frame_dim");
        skillBtn3.GetComponent<Image>().sprite = Resources.Load<Sprite>("stage_frame_dim");
        skillBtn4.GetComponent<Image>().sprite = Resources.Load<Sprite>("stage_frame_dim");
    }
}
