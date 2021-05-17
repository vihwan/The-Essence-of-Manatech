using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillGauge : MonoBehaviour
{
    /*  1. 스킬 게이지의 상태를 관리
        2. 사용자의 스킬 사용 기능
     */

    //이후 사용자 데이터 컴포넌트를 받아와서 사용

    #region Variable

    private float currMp;
    private float maxMp;
    [SerializeField] private TMP_Text skillGaugeText;
    [SerializeField] private Image skiiGaugeImage;

    //Component
    private FindMatches findMatches;

    //Property

    public TMP_Text SkillGaugeText { get => skillGaugeText; set => skillGaugeText = value; }
    public Image SkiiGaugeImage { get => skiiGaugeImage; set => skiiGaugeImage = value; }
    public float CurrMp { get => currMp; set => currMp = value; }
    public float MaxMp { get => maxMp; set => maxMp = value; }

    #endregion Variable

    public void Init()
    {
        findMatches = FindObjectOfType<FindMatches>();

        CurrMp = 0f; //시작시 마나는 0으로 설정
        MaxMp = 200f; //총 마나 양을 200으로 설정
    }

    private void Update()
    {
        //매 프레임마다 게이지 상태를 갱신
        GaugeUpdate();
    }

    private void GaugeUpdate()
    {
        SkillGaugeText.text = Mathf.Round(CurrMp).ToString() + " / 200";
        SkiiGaugeImage.fillAmount = CurrMp / MaxMp;
    }

    //타일을 파괴할 때 마다 스킬 게이지가 증가
    public void IncreaseMp(float _count)
    {
        if (CurrMp >= MaxMp)
        {
            CurrMp = MaxMp;
            return;
        }
        CurrMp += _count;
    }

    public bool UseMp(int useMana)
    {
        if (CurrMp < useMana)
        {
            SkillManager.instance.appearText("<color=#00C7FF>마나</color>가 부족합니다.\n" + useMana +"만큼 마나가 필요합니다.");
            return false;
        }

        CurrMp -= useMana;
        return true;
    }
}