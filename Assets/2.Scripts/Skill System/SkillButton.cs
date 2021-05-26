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

    public float CooldownTime { get => cooldownTime; set => cooldownTime = value; }
    public float CooldownTimer { get => cooldownTimer; set => cooldownTimer = value; }
    public TMP_Text CooldownText { get => cooldownText; set => cooldownText = value; }
    public Image CooldownSkillImage { get => cooldownSkillImage; set => cooldownSkillImage = value; }

    //[SerializeField] public ActiveSkill skillInfo; //디버그용

    public void Init()
    {
        CooldownSkillImage = UtilHelper.Find<Image>(transform, "cool_Image");
        if (CooldownSkillImage != null)
            CooldownSkillImage.gameObject.SetActive(false);

        cooldownEffect = UtilHelper.Find<Image>(transform, "cooldownEffect");
        if (cooldownEffect != null)
            cooldownEffect.fillAmount = 0.0f;

        CooldownText = UtilHelper.Find<TMP_Text>(transform, "Text (TMP)");
        if (CooldownText != null)
            CooldownText.gameObject.SetActive(false);
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
        CooldownTimer -= Time.deltaTime;

        if (CooldownTimer <= 0.0f)
        {
            isCooldown = false;
            CooldownSkillImage.gameObject.SetActive(false);
            CooldownText.gameObject.SetActive(false);
            cooldownEffect.fillAmount = 0.0f;
        }
        else
        {
            CooldownText.text = Mathf.RoundToInt(CooldownTimer).ToString();
            cooldownEffect.fillAmount = CooldownTimer / CooldownTime;
        }
    }

    public bool CanUseSpell()
    {
        if (isCooldown)
        {
            SkillManager.instance.appearText("<color=#B75500>스킬 쿨타임</color> 입니다.");
            return false;
        }
        else
        {
            isCooldown = true;
            return true;
        }
    }
}