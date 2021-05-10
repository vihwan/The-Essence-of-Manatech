using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlertText : MonoBehaviour
{
    private TMP_Text textContents;
    private Animator animator;
    private SkillManager skillManager;


    public void Init()
    {
        textContents = GetComponent<TMP_Text>();
        animator = GetComponent<Animator>();
        skillManager = FindObjectOfType<SkillManager>();
        skillManager.appearText += ActiveText;
    }

    public void ActiveText(string _text)
    {
        textContents.text = _text;
        animator.SetTrigger("appear");
    }
}