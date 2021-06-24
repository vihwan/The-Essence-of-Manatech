using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.UI;

[System.Obsolete("사용되지 않는 클래스DA")]
public class SkillLevelSystem : MonoBehaviour
{

    List<ActiveSkill> activeSkills;
    List<PassiveSkill> passiveSkills;

    private SkillConversionData skillConversion;
    //TestSkillLevelText testSkillLevelText;

    public void Init()
    {
        activeSkills = SkillManager.instance.ActSkillDic.Values.ToList();
        passiveSkills = SkillManager.instance.PasSkillDic.Values.ToList();
        skillConversion = new SkillConversionData();
       // testSkillLevelText = FindObjectOfType<TestSkillLevelText>();
        CreateCustomBtn();
    }


    public void CreateCustomBtn()
    {
        GameObject skillupbtns = Instantiate(Resources.Load<GameObject>("SkillUpButton"), transform.position, Quaternion.identity);
        skillupbtns.transform.SetParent(transform);

        Button[] buttons = GetComponentsInChildren<Button>(skillupbtns);
        if (buttons != null)
        {
            /*
             # Closure Problem
               람다식은 실제 실행되기 전에는 참조형태로 가지고있는데,
               for문을 돌리면서 같은 변수인 i를 계속 줬기 때문에 마지막 값으로 통일된 것.
               이를 closure problem이라고 부른다.
             */
            for (int i = 0; i < buttons.Length; i++)
            {
                int temp = i;
                buttons[temp].onClick.AddListener(() => ActSkillLevelUp(temp));
            }
        }
    }

    public void ActSkillLevelUp(int index)
    {
        if (activeSkills[index].Level >= 3)
        {
           // SkillManager.instance.appearText("<color=#B31405>더이상 스킬을 올릴 수 없습니다.</color>");
            return;
        }

       // SkillManager.instance.appearText("<color=#B31405>스킬 레벨업</color>");
        activeSkills[index].Level++;
        skillConversion.ConvertActiveSkill();
        //testSkillLevelText.ConvertText();
    }

    public void PassSkillLevelUp(int index)
    {
        if (passiveSkills[index].Level >= 3)
        {
            //SkillManager.instance.appearText("<color=#B31405>더이상 스킬을 올릴 수 없습니다.</color>");
            return;
        }
       // SkillManager.instance.appearText("<color=#B31405>스킬 레벨업</color>");
        passiveSkills[index].Level++;
        skillConversion.ConvertPassiveSkill();
        //testSkillLevelText.ConvertText();
    }
}
