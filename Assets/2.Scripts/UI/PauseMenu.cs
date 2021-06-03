using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject resumeBtn;
    [SerializeField] private GameObject settingBtn;
    [SerializeField] private GameObject exitBtn;

    public void Init()
    {
        resumeBtn = transform.Find("UIRoot/ButtonRoot/ResumeButton").gameObject;
        if (resumeBtn != null)
        {
            resumeBtn.GetComponent<Button>().onClick.AddListener(CloseMenu);
        }

        settingBtn = transform.Find("UIRoot/ButtonRoot/SettingButton").gameObject;
        if (settingBtn != null)
        {
            settingBtn.GetComponent<Button>().onClick.AddListener(OpenSettingMenu);
        }

        exitBtn = transform.Find("UIRoot/ButtonRoot/ExitButton").gameObject;
        if (exitBtn != null)
        {
            exitBtn.GetComponent<Button>().onClick.AddListener(ClickExit);
        }
        pauseUI = transform.Find("UIRoot").gameObject;
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }
    }

    internal void CallMenu()
    {
        GameManager.instance.GameState = GameState.PAUSE;
        pauseUI.SetActive(true);
        Time.timeScale = 0f; //0배속
    }

    private void OpenSettingMenu()
    {
        //설정창을 여는 함수
        //추후에 추가해서 연결할 계획.
        SettingMenu.instance.gameObject.SetActive(true);
    }


    public void CloseMenu() //Resume
    {
        GameManager.instance.GameState = GameState.PLAYING;
        pauseUI.SetActive(false);
        Time.timeScale = 1f; //1배속
    }

    public void ClickExit() //Quit
    {
        CloseMenu();
        GameManager.instance.LoadScene("MenuScene");
    }

}
