using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterStatusController : MonoBehaviour
{
    // 나중에 몬스터 정보가 담긴 컴포넌트 불러오기
    private MonsterStatus monsterStatus = new MonsterStatus(3000, 30, 200);

    [SerializeField]
    private float maxHp;
    private float currHp;

    [SerializeField]
    private float maxMp;
    private float currMp;

    [SerializeField]
    private float maxShield;
    private float currShield;


    [SerializeField] private TMP_Text[] texts;
    [SerializeField] public Image[] images_Gauge;
    [SerializeField] private Image red;

    private GameObject shieldObject;

    public const int HP = 0, MP = 1, SH = 2;
    private bool isUpdate = false;

    [SerializeField] private float redSpeed = 1f;

    //Tween 클래스의 메모리를 할당
    //Tween 클래스는 시점과 종점을 받아 업데이트를 할때 사용하는 함수입니다.
    private Tween<float> tween = new Tween<float>();
    //Red이미지를 Tween 하기 위해 선언한 변수
    private Tween<float> redTween = new Tween<float>();
    //색상을 Tween 하기 위한 변수
    private Tween<Color> colorTween = new Tween<Color>();

    public float MaxHp { get => maxHp; }
    public float CurrHp { get => currHp; set => currHp = value; }
    public float MaxMp { get => maxMp;}
    public float CurrMp { get => currMp; set => currMp = value; }
    public float MaxShield { get => maxShield;}
    public float CurrShield { get => currShield; set => currShield = value; }

    public void Init()
    {
        maxHp = monsterStatus.Hp;  
        currHp = monsterStatus.Hp;
        maxMp = monsterStatus.Mp;
        currMp = 0f;
        maxShield = monsterStatus.Shield;
        currShield = monsterStatus.Shield;

        shieldObject = transform.Find("Slider/MonsterShieldSlide").gameObject;
        if(shieldObject != null)
        {
            shieldObject.SetActive(false);
        }

       // red = UtilHelper.Find<Image>(transform, "Red");
       //동작할 함수를 연결합니다.
        tween.SetTween(MathHelper.Spring);
        //red의 동작할 함수를 연결합니다.
        redTween.SetTween(MathHelper.EaseOutQuad);
        colorTween.SetTween(Color.Lerp);
    }

    private void Update()
    {
        TextUpdate();
        GaugeUpdate();

        if (Input.GetKeyDown(KeyCode.Space))
            TakeDamage(10);

        if(isUpdate)
        {
            if (!tween.IsEnd) { 
                //tween 클래스로부터 업데이트 된 값을 받는다.
                images_Gauge[HP].fillAmount = tween.Update();
                
            }

            if (!redTween.IsEnd)
            {
                red.fillAmount = redTween.Update();
            }


            if(!colorTween.IsEnd)
            {
                red.color = colorTween.Update();
            }
        }    
    }

    private void TextUpdate()
    {
        texts[HP].text = Mathf.Round(CurrHp).ToString() + " / " + MaxHp;
        texts[MP].text = Mathf.Round(CurrMp).ToString() + " / " + MaxMp;
        texts[SH].text = Mathf.Round(CurrShield).ToString() + " / " + MaxShield;
    }

    private void GaugeUpdate()
    {      
        images_Gauge[HP].fillAmount = CurrHp / MaxHp;
        images_Gauge[MP].fillAmount = CurrMp / MaxMp;
        images_Gauge[SH].fillAmount = CurrShield / MaxShield;
    }

    public void DecreaseHP(float _count)
    {
        float damage = 0f;

        if (MonsterAI.instance.Action == MonsterState.GROGGY)
            damage = _count * 10;
        else
            damage = _count;

        //스킬 시전중에는 데미지를 받지 않습니다.
        if (MonsterAI.instance.IsMonsterActiveSkill())
            damage = 0f;

        if (shieldObject.activeSelf)
        {
            if(currShield > 0)
                currShield -= damage;
            else
            {
                shieldObject.SetActive(false);
            }
        }
        else
        {
            currHp -= damage;

            if (CurrHp <= 0)
            {
                currHp = 0f;
            }
        }
    }

    public void IncreaseMp(float _count)
    {
        if (CurrMp + _count < MaxMp)
            currMp += _count;
        else
            currMp = MaxMp;
    }

    public void DecreaseMp(int _count)
    {
        currMp -= _count;   
    }


    public void TakeDamage(float damage)
    {

        isUpdate = true;

        //foreground 업데이트
        float startVal = images_Gauge[HP].fillAmount;
        currHp -= damage;
        images_Gauge[HP].fillAmount = CurrHp / MaxHp;
        float endVal = images_Gauge[HP].fillAmount;

        tween.Execute(startVal, endVal, 0.1f);

        //red 업데이트
        red.fillAmount = startVal;
        //red값이 업데이트 될수 있도록 처리
        redTween.Execute(startVal, endVal, redSpeed);

        colorTween.Execute(Color.red, new Color(1, 1, 0, 1), 0.2f);
    }
}
