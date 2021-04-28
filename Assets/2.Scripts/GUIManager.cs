﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;

    public GameObject gameOverPanel;
    public Text yourScoreTxt;
    public Text highScoreTxt;

    public Text scoreTxt;
    public Text comboCounterTxt;
    public Text limitTimeTxt;

    private int score = 0;
    private float limitTime;


    //프로퍼티
    public int Score
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
            limitTimeTxt.text = Mathf.Round(limitTime).ToString();
        }
    }


    //초기화함수
    public void Init()
    {
        instance = GetComponent<GUIManager>();
        limitTime = 600;
        limitTimeTxt.text = limitTime.ToString();
    }

    private void Update()
    {
        LimitTime -= Time.deltaTime;
    }



    // 게임오버가 되면 게임 오버 패널을 액티브
    public void GameOverPanel()
    {
        StopAllCoroutines();

        gameOverPanel.SetActive(true);

        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScoreTxt.text = "New Best: " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        else
        {
            highScoreTxt.text = "Best: " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        yourScoreTxt.text = score.ToString();
    }


    //게임 오버 전에 대기 시간을 주는 코루틴
    //BoardState가 MOVE가 될때 까지 기다림
    public IEnumerator WaitForShifting()
    {
        yield return new WaitUntil(() => BoardManager.instance.IsMoveState());
        yield return new WaitForSeconds(.25f);
        GameOverPanel(); //GUI GameOver Panel
    }
}
