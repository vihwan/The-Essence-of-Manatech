using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillGauge : MonoBehaviour
{

    private float currentSkillMana;
    private float totalSkillMana;

    [SerializeField] private TMP_Text skillGaugeText;
    [SerializeField] private Image skiiGaugeImage;

    public TMP_Text SkillGaugeText { get => skillGaugeText; set => skillGaugeText = value; }
    public Image SkiiGaugeImage { get => skiiGaugeImage; set => skiiGaugeImage = value; }
    public float CurrentSkillMana { get => currentSkillMana; set => currentSkillMana = value; }
    public float TotalSkillMana { get => totalSkillMana; set => totalSkillMana = value; }
}
