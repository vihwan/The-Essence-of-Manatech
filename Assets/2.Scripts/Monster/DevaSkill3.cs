using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DevaSkill3 : MonoBehaviour
{

    [SerializeField] private float limitTime;
    [SerializeField] private float remainTime;

    public bool isRemainTimeUpdate = false;
    public bool isUsingSkill = false;
    internal bool isBerserk = false;

    private GameObject rootUI;
    private TMP_Text tmp_Text;
    private TMP_Text tmp_Text_Hp;
    private Image limitTimeImage;
    private MonsterNotify notify;
    private MonsterStatusController monster;
    private GameObject shieldGauge;

    // Start is called before the first frame update
    public void Init()
    {
        rootUI = transform.Find("RootUI").gameObject;
        if (rootUI != null)
            rootUI.SetActive(false);

        tmp_Text = GetComponentInChildren<TMP_Text>(true);

        limitTimeImage = transform.Find("RootUI/SkillLimitTimeSlide/BaseUI/Gauge").GetComponent<Image>();
        if (limitTimeImage != null)
            limitTimeImage.fillAmount = 1f;

        notify = FindObjectOfType<MonsterNotify>();
        monster = FindObjectOfType<MonsterStatusController>();
        tmp_Text_Hp = monster.transform.Find("Slider/MonsterHpSlide/BaseUI/Gauge/Text (TMP)").GetComponent<TMP_Text>();
        shieldGauge = monster.transform.Find("Slider/MonsterShieldSlide").gameObject;
    }
    private void GaugeUpdate()
    {
        limitTimeImage.fillAmount = remainTime / limitTime;
        tmp_Text.text = remainTime + "s";
    }

    // Update is called once per frame
    public void Execute()
    {
        limitTime = 30f;
        remainTime = limitTime;
        GaugeUpdate();
        rootUI.SetActive(true);

        isUsingSkill = true;

        // 사운드 출력
        // 혼돈의 힘은 무한하다.
        SoundManager.instance.PlayCV("Devil_Skill2");

        if(shieldGauge != null)
        {
            shieldGauge.SetActive(true);
            tmp_Text_Hp.enabled = false;
            monster.CurrShield = 200f;
        }

        isUsingSkill = false;
        isRemainTimeUpdate = true;
    }

    private void Update()
    {
        if (isRemainTimeUpdate)
        {
            remainTime -= Time.deltaTime;

            GaugeUpdate();

            if(monster.CurrShield <= 0f)
            {

                //패턴 파훼 성공
                Debug.Log("<color=#0456F1>패턴 파훼</color> 성공!!");
                //그로기 타임
                MonsterAI.instance.Action = MonsterState.GROGGY;
                MonsterAI.instance.GroggyTime = 15f;
                SoundManager.instance.PlayCV("Devil_Skill_Groggy");
                notify.SetText("크윽.. 이럴수가!!");
                notify.PlayAnim();
                rootUI.SetActive(false);
                shieldGauge.SetActive(false);
                tmp_Text_Hp.enabled = true;
                isRemainTimeUpdate = false;
            }


            if (remainTime <= 0)
            {
                remainTime = 0f;
                limitTimeImage.fillAmount = 0f;

                if (MonsterAI.instance.Action == MonsterState.CASTING)
                {
                    SkillBerserk();
                }
            }
        }
    }

    private void SkillBerserk()
    {
        isBerserk = true;
        notify.SetText("혼돈 속에 쳐박혀라!!!");
        notify.PlayAnim();
        SoundManager.instance.PlayCV("Devil_Skill2_Berserk");

        PlayerStatusController playerStatusController = FindObjectOfType<PlayerStatusController>();
        playerStatusController.DecreaseHP(500);

        rootUI.SetActive(false);
        shieldGauge.SetActive(false);
        tmp_Text_Hp.enabled = true;

        isBerserk = false;
        isRemainTimeUpdate = false;
        MonsterAI.instance.Action = MonsterState.MOVE;
    }
}
