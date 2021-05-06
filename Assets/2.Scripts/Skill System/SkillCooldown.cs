using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillCooldown : MonoBehaviour
{
    [SerializeField] private Transform t;

    [SerializeField] private TMP_Text cooldownText;
    [SerializeField] private Image cooldownEffect;
    [SerializeField] private Image cooldownSkillImage;

    [SerializeField] internal bool isCooldown = false;
    private float cooldownTime;
    private float cooldownTimer = 0.0f;

    public void Init()
    {
        t = transform.Find("cool_Image");
        if (t != null)
        {
            cooldownSkillImage = t.GetComponent<Image>();
            if (cooldownSkillImage != null)
            {
                cooldownSkillImage.gameObject.SetActive(false);
            }
        }

        t = transform.Find("cooldownEffect");
        if (t != null)
        {
            cooldownEffect = t.GetComponent<Image>();
            if (cooldownEffect != null)
            {
                cooldownEffect.fillAmount = 0.0f;
            }
        }

        cooldownText = GetComponentInChildren<TMP_Text>();
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