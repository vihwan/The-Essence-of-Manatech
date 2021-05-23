using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterNotify : MonoBehaviour
{

    private Image notifyImage;
    private Text notifyText;
    private Animator animator;

    public Image NotifyImage { get => notifyImage; set => notifyImage = value; }


    // Start is called before the first frame update
    public void init()
    {
        NotifyImage = GetComponentInChildren<Image>();
        notifyText = GetComponentInChildren<Text>();
        animator = GetComponent<Animator>();
    }

    public void PlayAnim()
    {
        animator.SetTrigger("active");
    }

    public void SetText(string _text)
    {
        notifyText.text = _text;
    }
}
