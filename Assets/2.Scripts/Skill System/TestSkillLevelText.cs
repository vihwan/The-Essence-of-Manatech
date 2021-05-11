using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TestSkillLevelText : MonoBehaviour
{

    private TMP_Text[] tMP_Texts;

    public List<ActiveSkill> activeSkills;

    // Start is called before the first frame update
    public void Init()
    {
        tMP_Texts = GetComponentsInChildren<TMP_Text>();

        activeSkills = SkillManager.instance.ActSkillDic.Values.ToList();

        if (tMP_Texts != null)
        {
            for (int i = 0; i < tMP_Texts.Length; i++)
            {
                tMP_Texts[i].text = activeSkills[i].Level.ToString();
            }
        }
    }

    public void ConvertText()
    {
        for (int i = 0; i < tMP_Texts.Length; i++)
        {
            tMP_Texts[i].text = activeSkills[i].Level.ToString();
        }
    }
}
