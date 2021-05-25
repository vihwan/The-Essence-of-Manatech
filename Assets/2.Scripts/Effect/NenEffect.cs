using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NenEffect : MonoBehaviour
{
    private Animator animator;
    private Image image;

    public void Start()
    {
        animator = GetComponent<Animator>();
        image = GetComponent<Image>();
    }
}
