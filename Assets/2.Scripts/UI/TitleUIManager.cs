using System;
using UnityEngine;


public class TitleUIManager : MonoBehaviour
{
    private bool isLoading = false;

    private void Start()
    {
        SoundManager.instance.PlayBGM("Title");

    }

    private void Update()
    {
        if (isLoading == true)
            return;

        if (Input.GetMouseButtonDown(0))
        {

            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetTrigger("Start");
                SoundManager.instance.PlaySE("title_start");
            }

            print("게임 시작");
            Invoke(nameof(GameStart), 1.0f);
            isLoading = true;
        }
    }

    private void GameStart()
    {
        GameManager.instance.LoadScene("MenuScene");
    }
}
