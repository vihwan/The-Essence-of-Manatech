using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GUIManager : MonoBehaviour
{
    //싱글톤
    public static GUIManager instance;

    public TMP_Text scoreTxt;
    public TMP_Text limitTimeTxt;

    private Button pauseBtn;

    private float score = 0;
    private float limitTime;

    private int hour;
    private int min;
    private int sec;

    public bool isPauseTime = false;

    private AlertText alertText;
    private MonsterStatusController monsterStatusController;

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
/*            if (GameManager.instance.isGameOver)
            {
                limitTime = 0;
            }*/
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
        LimitTime = 600;
      //  limitTimeTxt.text = LimitTime.ToString();

        alertText = FindObjectOfType<AlertText>();
        if (alertText != null)
            alertText.Init();

        monsterStatusController = FindObjectOfType<MonsterStatusController>();
        if (monsterStatusController != null)
            monsterStatusController.Init();

    }

    public void OnInitPauseButton()
    {
        pauseBtn = transform.Find("LimitTimeRoot/LimitTimeImage/Button").GetComponent<Button>();
        if(pauseBtn != null)
        {
            pauseBtn.onClick.AddListener(OpenPauseMenu);
        }
    }

    private void OpenPauseMenu()
    {
        ExternalFuncManager.Instance.OpenPauseMenu();
    }

    private void Update()
    {
        if (GameManager.instance.GameState == GameState.PLAYING)
        {
            if (!isPauseTime)
            {
                LimitTime -= Time.deltaTime;
            }
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
}