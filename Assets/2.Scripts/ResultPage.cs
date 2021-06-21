using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ResultPage : MonoBehaviour
{

    private float leftLimitTime = 0f;

    private TMP_Text yourScoreTxt;
    private TMP_Text highScoreTxt;
    private TMP_Text yourClearTimeTxt;
    private TMP_Text bestClearTimeTxt;

    private Button exitBtn;
    private Image rankImage;


    private Animator animator;

    public void Init()
    {
        yourScoreTxt = transform.Find("YourScore").GetComponent<TMP_Text>();
        if (yourScoreTxt == null)
            Debug.LogWarning(yourScoreTxt.name + "이 참조되지 않았습니다.");

        highScoreTxt = transform.Find("HighScore").GetComponent<TMP_Text>();
        if (highScoreTxt == null)
            Debug.LogWarning(highScoreTxt.name + "이 참조되지 않았습니다.");

        yourClearTimeTxt = transform.Find("YourClearTime").GetComponent<TMP_Text>();
        if (yourClearTimeTxt == null)
            Debug.LogWarning(yourClearTimeTxt.name + "이 참조되지 않았습니다.");

        bestClearTimeTxt = transform.Find("BestClearTime").GetComponent<TMP_Text>();
        if (bestClearTimeTxt == null)
            Debug.LogWarning(bestClearTimeTxt.name + "이 참조되지 않았습니다.");

        animator = GetComponent<Animator>();
        if (animator == null)
            Debug.LogWarning(animator.name + "이 참조되지 않았습니다.");

        exitBtn = GetComponentInChildren<Button>(true);
        if (exitBtn != null)
            exitBtn.onClick.AddListener(GoToMainScene);

        rankImage = transform.Find("RankImage").GetComponent<Image>();
        if(rankImage == null)
            Debug.LogWarning(rankImage.name + "이 참조되지 않았습니다.");
    }

    // 게임오버가 되면 게임 오버 패널을 액티브
    public void GameOverPanel()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(true);

        DisplayScore();
        DisplayTime();

        SoundManager.instance.StopAllSE();
        SoundManager.instance.StopBGM();
        if (BoardManager.instance.currentState == PlayerState.WIN)
        {
            SoundManager.instance.PlaySE("DungeonResult");
        }
        else
        {
            rankImage.sprite = Resources.Load<Sprite>("coffin");
            SoundManager.instance.PlayBGM("coffin_rene");
        }

        BoardManager.instance = null; //보드 매니저 비활성화
        BoardManagerMonster.instance = null;
        SkillManager.instance = null;
    }

    private void GoToMainScene()
    {
        print("버튼 클릭, 마을로 이동");
        GameManager.instance.LoadScene("MenuScene");
    }

    private void DisplayTime()
    {
        leftLimitTime = GUIManager.instance.LimitTime;

        if (leftLimitTime > PlayerPrefs.GetFloat("HighLimitTime"))
        {
            PlayerPrefs.SetFloat("HighLimitTime", leftLimitTime);
            bestClearTimeTxt.text = "New 베스트타임 : " + ParseTime(PlayerPrefs.GetFloat("HighLimitTime"));
        }
        else
            bestClearTimeTxt.text = "베스트타임 : " + ParseTime(PlayerPrefs.GetFloat("HighLimitTime"));

        yourClearTimeTxt.text = "클리어타임 : " + ParseTime(leftLimitTime);
    }

    private void DisplayScore()
    {
        if (GUIManager.instance.Score > PlayerPrefs.GetFloat("HighScore"))
        {
            PlayerPrefs.SetFloat("HighScore", GUIManager.instance.Score);
            highScoreTxt.text = "New 베스트점수: " + ScoreManager.instance.ScoreWithComma(PlayerPrefs.GetFloat("HighScore"));
        }
        else
        {
            highScoreTxt.text = "베스트점수: " + ScoreManager.instance.ScoreWithComma(PlayerPrefs.GetFloat("HighScore"));
        }
        yourScoreTxt.text = "클리어점수 : " + ScoreManager.instance.ScoreWithComma(GUIManager.instance.Score);
    }




    private string ParseTime(float limitTime)
    {
        float limit = 600f - limitTime;

        int hour = (int)limit / 3600;
        int min = (int)limit / 60 % 60;
        int sec = (int)limit % 60;

        string limitTimeText = string.Empty;

        if (hour != 0)
            limitTimeText = string.Format("{0:D2}시간" + " {1:D2}분" + " {2:D2}초", hour, min, sec);
        else
            limitTimeText = string.Format("{0:D2}분" + " {1:D2}초", min, sec);

        return limitTimeText;
    }
}
