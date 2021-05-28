using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject resumeBtn;
    //private Button settingBtn;
    [SerializeField] private GameObject exitBtn;

    public void Init()
    {
/*        pauseUI = transform.Find("UIRoot").gameObject;
        if (pauseUI != null)
        {
            pauseUI.SetActive(false);
        }*/

        /*        resumeBtn = transform.Find("Background/ButtonRoot/ResumeButton").gameObject;
                if(resumeBtn != null)
                {
                    resumeBtn.GetComponent<Button>().onClick.AddListener(CloseMenu);
                }

                exitBtn = transform.Find("Background/ButtonRoot/ExitButton").gameObject;
                if(exitBtn != null)
                {
                    exitBtn.GetComponent<Button>().onClick.AddListener(ClickExit);
                }*/
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
        //LoadingSceneManager.SetLoadScene("GameTitle");
    }

}
