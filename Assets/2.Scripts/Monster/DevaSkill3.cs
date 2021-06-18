using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DevaSkill3 : MonoBehaviour
{

    [SerializeField] private float limitTime;
    [SerializeField] private float remainTime;

    public bool isRemainTimeUpdate = false;
    private bool isActive = false;
    internal bool isBerserk = false;

    private GameObject rootUI;
    private TMP_Text remainTimeText;
    private TMP_Text tmp_Text_Hp;
    private Image limitTimeImage;
    private MonsterStatusController monster;
    private GameObject shieldGauge;

    private Animator chaosFusionAni;

    public bool IsActive { get => isActive; set => isActive = value; }

    // Start is called before the first frame update
    public void Init()
    {
        rootUI = transform.Find("RootUI").gameObject;
        if (rootUI != null)
            rootUI.SetActive(false);

        remainTimeText = GetComponentInChildren<TMP_Text>(true);

        limitTimeImage = transform.Find("RootUI/SkillLimitTimeSlide/BaseUI/Gauge").GetComponent<Image>();
        if (limitTimeImage != null)
            limitTimeImage.fillAmount = 1f;

        monster = FindObjectOfType<MonsterStatusController>();
        tmp_Text_Hp = monster.transform.Find("Slider/MonsterHpSlide/BaseUI/Gauge/Text (TMP)").GetComponent<TMP_Text>();
        shieldGauge = monster.transform.Find("Slider/MonsterShieldSlide").gameObject;

        chaosFusionAni = transform.Find("ChaosFusion").GetComponent<Animator>();
    }
    private void GaugeUpdate()
    {
        limitTimeImage.fillAmount = remainTime / limitTime;
        remainTimeText.text = Mathf.RoundToInt(remainTime) + "s";
    }

    // Update is called once per frame
    public void Execute()
    {
        limitTime = 30f;
        remainTime = limitTime;
        GaugeUpdate();
        rootUI.SetActive(true);

        IsActive = true;

        // 사운드 출력
        // 혼돈의 힘은 무한하다. (3스킬)
        MonsterAI.instance.SoundandNotify.SetVoiceAndNotify(DevastarState.Skill_Three);

        if (shieldGauge != null)
        {
            shieldGauge.SetActive(true);
            tmp_Text_Hp.enabled = false;
            monster.CurrShield = 200f;
        }

        //이펙트 실행
        chaosFusionAni.SetTrigger("Init");
        

        IsActive = false;
        isRemainTimeUpdate = true;
    }

    private void Update()
    {
        if (GameManager.instance.GameState != GameState.PLAYING)
            return;

        if (MonsterAI.instance.IsHolding == true)
            return;

        if (isRemainTimeUpdate)
        {
            remainTime -= Time.deltaTime;

            GaugeUpdate();

            //실드를 전부 부순다면, 패턴 파훼 성공
            if (monster.CurrShield <= 0f)
            {             
                Debug.Log("<color=#0456F1>패턴 파훼</color> 성공!!");
                chaosFusionAni.SetTrigger("Fail");

                //그로기 타임
                MonsterAI.instance.Action = MonsterState.GROGGY;
                MonsterAI.instance.StandardGroggyTime = 15f;
                MonsterAI.instance.RemainGroggyTime = MonsterAI.instance.StandardGroggyTime;
                rootUI.SetActive(false);
                shieldGauge.SetActive(false);
                tmp_Text_Hp.enabled = true;
                isRemainTimeUpdate = false;
                MonsterAI.instance.isUsingSkill = false;
            }

            if (remainTime <= 0)
            {
                remainTime = 0f;
                limitTimeImage.fillAmount = 0f;

                if (MonsterAI.instance.Action == MonsterState.CASTING)
                {
                    StartCoroutine(SkillBerserk());
                    isRemainTimeUpdate = false;
                    MonsterAI.instance.isUsingSkill = false;
                }
            }
        }
    }

    private IEnumerator SkillBerserk()
    {
        isBerserk = true;

        //혼돈 속에 처박혀라!!
        MonsterAI.instance.Action = MonsterState.BERSERK;
        MonsterAI.instance.SoundandNotify.SetVoiceAndNotify(DevastarState.Skill_Three_Berserk);

        rootUI.SetActive(false);
        shieldGauge.SetActive(false);
        tmp_Text_Hp.enabled = true;
        chaosFusionAni.SetTrigger("Berserk");
        yield return null;
    }

    //Animation Event
    private void PlaySoundChaosFusion()
    {
        MonsterAI.instance.SoundandNotify.SetVoiceAndNotify(DevastarState.Skill_Three_ChaosFusion);   
    }

    //Animation Event
    private void BerserkEnd()
    {
        PlayerStatusController playerStatusController = FindObjectOfType<PlayerStatusController>();
        playerStatusController.DecreaseHP(500);

        MonsterAI.instance.Action = MonsterState.MOVE;
        isBerserk = false;
    }
}
