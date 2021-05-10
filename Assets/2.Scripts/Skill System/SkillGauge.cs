using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillGauge : MonoBehaviour
{
    /*  1. 스킬 게이지의 상태를 관리
        2. 사용자의 스킬 사용 기능
     */

    #region Variable

    private float currentSkillMana;
    private float totalSkillMana;
    [SerializeField] private TMP_Text skillGaugeText;
    [SerializeField] private Image skiiGaugeImage;

    //Component
    private FindMatches findMatches;

    //Property

    public TMP_Text SkillGaugeText { get => skillGaugeText; set => skillGaugeText = value; }
    public Image SkiiGaugeImage { get => skiiGaugeImage; set => skiiGaugeImage = value; }
    public float CurrentSkillMana { get => currentSkillMana; set => currentSkillMana = value; }
    public float TotalSkillMana { get => totalSkillMana; set => totalSkillMana = value; }

    #endregion Variable

    public void Init()
    {
        findMatches = FindObjectOfType<FindMatches>();

        CurrentSkillMana = 0f; //시작시 마나는 0으로 설정
        TotalSkillMana = 200f; //총 마나 양을 200으로 설정
    }

    private void Update()
    {
        //매 프레임마다 게이지 상태를 갱신
        SkillGaugeStatus();
    }

    private void SkillGaugeStatus()
    {
        SkillGaugeText.text = Mathf.Round(CurrentSkillMana).ToString() + " / 200";
        SkiiGaugeImage.fillAmount = CurrentSkillMana / TotalSkillMana;
    }

    //타일을 파괴할 때 마다 스킬 게이지가 증가
    public void GainSkillGauge()
    {
        if (CurrentSkillMana >= TotalSkillMana)
        {
            CurrentSkillMana = TotalSkillMana;
            return;
        }
        CurrentSkillMana += (1f * findMatches.currentMatches.Count);
    }

    public bool UseSkillGauge(int useMana)
    {
        if (CurrentSkillMana < useMana)
        {
            SkillManager.instance.appearText("<color=#00C7FF>마나</color>가 부족합니다.\n" + useMana +"만큼 마나가 필요합니다.");
            return false;
        }

        CurrentSkillMana -= useMana;
        return true;
    }
}