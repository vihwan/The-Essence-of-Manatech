using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusController : MonoBehaviour
{
    /*  1. 스킬 게이지의 상태를 관리
        2. 사용자의 스킬 사용 기능
     */

    //이후 사용자 데이터 컴포넌트를 받아와서 사용

    #region Variable
private PlayerStatus playerStatus = new PlayerStatus(500, 200);

    [SerializeField] private float currHp;
    [SerializeField] private float maxHp;
    [SerializeField] private float currMp;
    [SerializeField] private float maxMp;
    [SerializeField] private TMP_Text[] texts;
    [SerializeField] private Image[] images_Gauge;
    [SerializeField] private bool isInvincible = false; //무적 상태인가 아닌가

    public const int HP = 0, MP = 1;

    //Component
    private FindMatches findMatches;

    //Property

    public float CurrMp { get => currMp; set => currMp = value; }
    public float MaxMp { get => maxMp; set => maxMp = value; }
    public float CurrHp { get => currHp; set => currHp = value; }
    public float MaxHp { get => maxHp; set => maxHp = value; }
    public bool IsInvincible { get => isInvincible; set => isInvincible = value; }

    #endregion Variable

    public void Init()
    {
        findMatches = FindObjectOfType<FindMatches>();

        MaxHp = playerStatus.Hp; // 시작세 체력을 500
        CurrHp = playerStatus.Hp;
        CurrMp = 0f; //시작시 마나는 0으로 설정
        MaxMp = playerStatus.Mp; //총 마나 양을 200으로 설정
    }

    private void Update()
    {
        //매 프레임마다 게이지 상태를 갱신
        GaugeUpdate();
    }

    private void GaugeUpdate()
    {
        texts[HP].text = Mathf.Round(CurrHp).ToString() + " / " + MaxHp;
        texts[MP].text = Mathf.Round(CurrMp).ToString() + " / " + MaxMp;

        images_Gauge[HP].fillAmount = CurrHp / MaxHp;
        images_Gauge[MP].fillAmount = CurrMp / MaxMp;
    }

    
    public void IncreaseHP(float _count)
    {
        currHp += _count;
        if(currHp >= MaxHp)
        {
            currHp = MaxHp;
        }
    }

    public void DecreaseHP(float _count)
    {
        currHp -= _count;

        if (CurrHp <= 0)
        {
            currHp = 0f;
            Debug.Log("캐릭터의 hp가 0이 되었습니다");
            //게임 오버
        }
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

    public bool CanUseMp(int necessaryMana)
    {
        if (CurrMp < necessaryMana)
        {
            SkillManager.instance.appearText("<color=#00C7FF>마나</color>가 부족합니다.\n" + necessaryMana +"만큼 마나가 필요합니다.");
            return false;
        }
        else
            return true;
    }
}