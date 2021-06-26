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
    private TMP_Text newScoreLetterTxt;
    private TMP_Text newTimeLetterTxt;


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
        if (rankImage == null)
            Debug.LogWarning(rankImage.name + "이 참조되지 않았습니다.");

        newScoreLetterTxt = transform.Find("YourScore/NewLetter (TMP)").GetComponent<TMP_Text>();
        if (newScoreLetterTxt != null)
        {
            newScoreLetterTxt.gameObject.SetActive(false);
        }

        newTimeLetterTxt = transform.Find("YourClearTime/NewLetter (TMP)").GetComponent<TMP_Text>();
        if (newTimeLetterTxt != null)
        {
            newTimeLetterTxt.gameObject.SetActive(false);
        }
    }
            

    // 게임오버가 되면 게임 오버 패널을 액티브
    public void GameOverPanel()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(true);

        DisplayScore();
        DisplayTime();
        SetRankImage();

        SoundManager.instance.StopAllSE();
        SoundManager.instance.StopBGM();

        if (GameManager.instance.PlayerState == PlayerState.WIN)
        {
            SoundManager.instance.PlaySE("DungeonResult");
            Invoke(nameof(PlayWinBGM), 7.5f);
        }
        else if(GameManager.instance.PlayerState == PlayerState.LOSE)
        {
            int rand = UnityEngine.Random.Range(0, 100);
            if(rand > 5)
            {
                SoundManager.instance.PlaySE("funnyfailure");
            }
            else
            {
                //갑자기 분위기 관짝춤
                rankImage.sprite = Resources.Load<Sprite>("coffin");
                SoundManager.instance.PlayBGM("coffin_rene");
            }      
        }

        //보드 매니저 비활성화
        BoardManager.instance = null;
        BoardManagerMonster.instance = null;
        SkillManager.instance = null;
    }

    private void SetRankImage()
    {
        string rank = GUIManager.instance.RankLetterTxt.text;
        rankImage.sprite = Resources.Load<Sprite>("RankLetter/" + rank);
    }

    private void PlayWinBGM()
    {
        SoundManager.instance.PlayBGM("silver_crown_old");
    }

    private void GoToMainScene()
    {
        print("버튼 클릭, 마을로 이동");
        GameManager.instance.LoadScene("MenuScene");
    }

    private void DisplayTime()
    {
        leftLimitTime = GUIManager.instance.LimitTime;

        //저장되어있는 기록보다 좋다면 기록 갱신
        if (leftLimitTime > PlayerPrefs.GetFloat("HighLimitTime"))
        {   
            //만약 플레이어가 패배상태라면 기록을 갱신하지 않습니다.
            if (BoardManager.instance.currentState == PlayerState.LOSE)
                return;

            PlayerPrefs.SetFloat("HighLimitTime", leftLimitTime);
            newTimeLetterTxt.gameObject.SetActive(true);
        }

        bestClearTimeTxt.text = "베스트타임 : " + ParseTime(PlayerPrefs.GetFloat("HighLimitTime"));
        yourClearTimeTxt.text = "클리어타임 : " + ParseTime(leftLimitTime);
    }

    private void DisplayScore()
    {
        //저장되어있는 기록보다 좋다면 기록 갱신
        if (GUIManager.instance.Score > PlayerPrefs.GetFloat("HighScore"))
        {
            //만약 플레이어가 패배상태라면 기록을 갱신하지 않습니다.
            if (BoardManager.instance.currentState == PlayerState.LOSE)
                return;

            PlayerPrefs.SetFloat("HighScore", GUIManager.instance.Score);
            newScoreLetterTxt.gameObject.SetActive(true);
        }

        highScoreTxt.text = "베스트점수 : " + ScoreManager.instance.ScoreWithComma(PlayerPrefs.GetFloat("HighScore")) + "점";
        yourScoreTxt.text = "클리어점수 : " + ScoreManager.instance.ScoreWithComma(GUIManager.instance.Score) + "점";
    }




    private string ParseTime(float limitTime)
    {
        float limit = GUIManager.instance.maxLimitTime - limitTime;

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
