using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AlertText : MonoBehaviour
{
    public static AlertText instance;

    private TMP_Text textContents;
    private Animator animator;

    public void Init()
    {
        instance = GetComponent<AlertText>();
        textContents = GetComponent<TMP_Text>();
        animator = GetComponent<Animator>();
    }

    public void ActiveText(string _text)
    {
        textContents.text = _text;
        animator.SetTrigger("appear");
    }
}