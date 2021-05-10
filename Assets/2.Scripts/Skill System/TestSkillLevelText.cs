using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TestSkillLevelText : MonoBehaviour
{

    private TMP_Text[] tMP_Texts;


    // Start is called before the first frame update
    public void Init()
    {
        tMP_Texts = GetComponentsInChildren<TMP_Text>();
        if(tMP_Texts != null)
        {
            for (int i = 0; i < tMP_Texts.Length; i++)
            {
                tMP_Texts[i].text = SkillManager.instance.ASkillList[i].level.ToString();
            }
        }
    }

    private void Update()
    {
        for (int i = 0; i < tMP_Texts.Length; i++)
        {
            tMP_Texts[i].text = SkillManager.instance.ASkillList[i].level.ToString();
        }
    }
}
