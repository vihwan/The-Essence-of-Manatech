using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class ScoreManager : MonoBehaviour
{
    //singleton
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
                                      + (50 * (findMatches.LolipopCount / 3) * (passiveSkills[3].EigenValue / 3)))
                                      * (1 + passiveSkills[1].EigenValue);

        //점수 계산 공식
        // ((기본점수(20) * (콤보수 + 1) * 현자의 돌 배율) + (롤리팝점수 * (롤리팝갯수 /3) * (붉은사탕 확률 / 3))) * (1+ 쇼타임 점수 추가 배율)
    }

    //점수를 3자리마다 ,를 넣어주는 함수
    public string ScoreWithComma(float score)
    {
        string withCommaString = string.Format("{0:#,0}", score);
        return withCommaString;
    }
}