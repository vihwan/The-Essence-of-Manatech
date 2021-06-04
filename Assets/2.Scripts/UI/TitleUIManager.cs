using System;
using UnityEngine;

public class TitleUIManager : MonoBehaviour
{
    private bool isLoading = false;


    private void Update()
    {
        if (isLoading == true)
            return;

        if (Input.GetMouseButtonDown(0))
        {
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
