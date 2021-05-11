using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    //Component
    private ComboSystem comboSystem;

    private FindMatches findMatches;

    private List<PassiveSkill> passiveSkills;

    public void Init()
    {
        instance = GetComponent<ScoreManager>();
        comboSystem = FindObjectOfType<ComboSystem>();
        findMatches = FindObjectOfType<FindMatches>();

    }

    public void PlusScore()
    {
        passiveSkills = SkillManager.instance.PasSkillDic.Values.ToList();
        GUIManager.instance.Score += ((20 * (comboSystem.ComboCounter + 1) * passiveSkills[2].EigenValue) 
                                      + (50 * ((int)findMatches.LolipopCount / 3) * passiveSkills[3].EigenValue)
                                      * passiveSkills[1].EigenValue);
    }

    //점수를 3자리마다 ,를 넣어주는 함수
    public string ScoreWithComma(float score)
    {
        string withCommaString = string.Format("{0:#,0}", score);
        return withCommaString;
    }
}