using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;

    public GameObject gameOverPanel;
    public Text yourScoreTxt;
    public Text highScoreTxt;

    public TMP_Text scoreTxt;
    public Text comboCounterTxt;
    public TMP_Text limitTimeTxt;

    private float score = 0;
    private float limitTime;

    private int hour;
    private int min;
    private int sec;

    public bool isPauseTime = false;

    private AlertText alertText;

    //프로퍼티
    public float Score
    {
        get => score;
        set
        {
            score = value;
            scoreTxt.text = ScoreManager.instance.ScoreWithComma(score);
        }
    }

    public float LimitTime
    {
        get => limitTime;
        set
        {
            limitTime = value;
            if (GameManager.instance.isGameOver)
            {
                limitTime = 0;
            }
            hour = (int)limitTime / 3600;
            min = (int)limitTime / 60 % 60;
            sec = (int)limitTime % 60;

            limitTimeTxt.text = string.Format("{0:D2}" + " {1:D2}" + " {2:D2}", hour, min, sec);

        }
    }


    //초기화함수
    public void Init()
    {
        instance = GetComponent<GUIManager>();
        limitTime = 600;
        limitTimeTxt.text = limitTime.ToString();

        alertText = FindObjectOfType<AlertText>();
        if (alertText != null)
            alertText.Init();
    }

    private void Update()
    {
        if (!isPauseTime)
        {
            LimitTime -= Time.deltaTime;

        }
    }


    // 3번 스킬 : 잭프로스트 빙수 - 제한시간을 일정시간동안 멈추게하기
    public void OnPauseTime(float skillTime)
    {
        isPauseTime = true;

        Debug.Log("멈춰!");
        //딜레이를 주기위한 선언
        Invoke(nameof(Wait), skillTime);
    }

    //멈춰!!
    private void Wait()
    {
        Debug.Log("멈춰 끝");
        isPauseTime = false;
    }



    // 게임오버가 되면 게임 오버 패널을 액티브
    public void GameOverPanel()
    {
        StopAllCoroutines();

        gameOverPanel.SetActive(true);

        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", (int)score);
            highScoreTxt.text = "New Best: " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        else
        {
            highScoreTxt.text = "Best: " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        yourScoreTxt.text = score.ToString();
    }

/*
    private void TimeParsing(float limitTime)
    {
        int hour;
        int min;
        int sec;

        string timeStr;
        timeStr
    }*/



    //게임 오버 전에 대기 시간을 주는 코루틴
    //BoardState가 MOVE가 될때 까지 기다림
    public IEnumerator WaitForShifting()
    {
        yield return new WaitUntil(() => BoardManager.instance.IsMoveState());
        yield return new WaitForSeconds(.25f);
        GameOverPanel(); //GUI GameOver Panel
    }
}
