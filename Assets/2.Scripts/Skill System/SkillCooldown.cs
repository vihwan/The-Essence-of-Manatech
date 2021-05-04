using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillCooldown : MonoBehaviour
{
    [SerializeField] private Image cooldownEffect;
    [SerializeField] private Text cooldownText;

    private bool isCooldown = false;
    private float cooldownTime = 10.0f;
    private float cooldownTimer = 0.0f;

    private void Start()
    {
        cooldownText.gameObject.SetActive(false);
        cooldownEffect.fillAmount = 0.0f;
    }

    private void Update()
    {
        if (isCooldown)
        {
            ApplyCooldown();
        }
    }

    private void ApplyCooldown()
    {
        cooldownTimer -= Time.deltaTime;

        if (cooldownTimer < 0.0f)
        {
            isCooldown = false;
            cooldownText.gameObject.SetActive(false);
            cooldownEffect.fillAmount = 0.0f;
        }
        else
        {
            cooldownText.text = Mathf.RoundToInt(cooldownTimer).ToString();
            cooldownEffect.fillAmount = cooldownTimer / cooldownTime;
        }
    }

    public bool UseSpell()
    {
        if (isCooldown)
        {
            return false;
        }
        else
        {
            isCooldown = true;
            cooldownText.gameObject.SetActive(true);
            cooldownTimer = cooldownTime;
            return true;
        }
    }
}