using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private Image cooldownEffect;
    [SerializeField] private Image cooldownSkillImage;

    [SerializeField] internal bool isCooldown = false;
    private float cooldownTime;
    private float cooldownTimer;

    //public ActiveSkill skillInfo;

    public void Init()
    {
        cooldownSkillImage = UtilHelper.Find<Image>(transform, "cool_Image");
        if (cooldownSkillImage != null)
            cooldownSkillImage.gameObject.SetActive(false);

        cooldownEffect = UtilHelper.Find<Image>(transform, "cooldownEffect");
        if (cooldownEffect != null)
            cooldownEffect.fillAmount = 0.0f;

        cooldownText = UtilHelper.Find<TMP_Text>(transform, "Text (TMP)");
        if (cooldownText != null)
            cooldownText.gameObject.SetActive(false);
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

        if (cooldownTimer <= 0.0f)
        {
            isCooldown = false;
            cooldownSkillImage.gameObject.SetActive(false);
            cooldownText.gameObject.SetActive(false);
            cooldownEffect.fillAmount = 0.0f;
        }
        else
        {
            cooldownText.text = Mathf.RoundToInt(cooldownTimer).ToString();
            cooldownEffect.fillAmount = cooldownTimer / cooldownTime;
        }
    }

    public bool UseSpell(float skillCoolDownTime)
    {
        if (isCooldown)
        {
            return false;
        }
        else
        {
            isCooldown = true;
            cooldownSkillImage.gameObject.SetActive(true);
            cooldownText.gameObject.SetActive(true);
            cooldownTimer = skillCoolDownTime;
            cooldownTime = skillCoolDownTime;
            return true;
        }
    }
}