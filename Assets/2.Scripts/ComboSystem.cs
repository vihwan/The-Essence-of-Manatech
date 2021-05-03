using UnityEngine;
using UnityEngine.UI;

public enum AnimationState
{
    IDLE,
    UP,
    FAIL
}

public class ComboSystem : MonoBehaviour
{
    private int comboCounter;

    private Text comboCounterText;
    private Animator animator;
    private AnimationState animationState;

    public int ComboCounter
    {
        get => comboCounter;
        set
        {
            comboCounter = value;
            comboCounterText.text = "Combo " + comboCounter.ToString();
            SetActiveComboText();
        }
    }

    public void Init()
    {
        comboCounterText = GetComponentInChildren<Text>();
        animator = GetComponentInChildren<Animator>();

        comboCounter = 0;
        comboCounterText.text = comboCounter.ToString();
        comboCounterText.enabled = false;
    }

    private void SetActiveComboText()
    {
        if (ComboCounter >= 1)
        {
            if (!comboCounterText.enabled)
                comboCounterText.enabled = true;
            animator.SetTrigger("combo");
        }
        else
        {
            comboCounterText.enabled = false;
        }
    }

    internal void PlayComboFailAnimation()
    {
        animator.SetTrigger("combofail");
    }
}