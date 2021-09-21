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
    private TMP_Text rankLetterTxt;

    private Button pauseBtn;

    private float score = 0;
    private float limitTime;
    internal float maxLimitTime;

    private int hour;
    private int min;
    private int sec;

    public bool isPauseTime = false;

    private Animator rankVariationAnim;
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
            SetRankLetter();
        }
    }

    public float LimitTime
    {
        get => limitTime;
        set
        {
            limitTime = value;

            hour = (int)limitTime / 3600;
            min = (int)limitTime / 60 % 60;
            sec = (int)limitTime % 60;

            limitTimeTxt.text = string.Format("{0:D2}" + " {1:D2}" + " {2:D2}", hour, min, sec);
        }
    }

    public TMP_Text RankLetterTxt { get => rankLetterTxt; }

    //초기화함수
    public void Init()
    {
        instance = GetComponent<GUIManager>();
        LimitTime = 600;
      //  limitTimeTxt.text = LimitTime.ToString();

        alertText = GetComponentInChildren<AlertText>(true);
        if (alertText != null)
            alertText.Init();

        monsterStatusController = FindObjectOfType<MonsterStatusController>();
        if (monsterStatusController != null)
            monsterStatusController.Init();

        pauseBtn = transform.Find("LimitTimeRoot/LimitTimeImage/Button").GetComponent<Button>();
        if (pauseBtn != null)
        {
            pauseBtn.onClick.AddListener(OpenPauseMenu);
        }

        rankLetterTxt = transform.Find("ScorePanel/Text (TMP) Rank").GetComponent<TMP_Text>();
        if(RankLetterTxt == null)
        {
            Debug.LogWarning(RankLetterTxt.name = "이 참조되지 않았습니다");
        }

        rankVariationAnim = rankLetterTxt.GetComponentInChildren<Animator>();
        if(rankVariationAnim == null)
        {
            Debug.LogWarning(rankVariationAnim.name = "이 참조되지 않았습니다");
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
            if (pauseBtn != null)
                pauseBtn.interactable = true;

            if (!isPauseTime)
            {
                LimitTime -= Time.deltaTime;
            }
        }
        else
        {
            if (pauseBtn != null)
                pauseBtn.interactable = false;
        }

    }

    //플레이어의 획득 점수에 따라 랭크 표시를 다르게 세팅해주는 함수입니다.
    private void SetRankLetter()
    {
        if (Score >= 150000f)
        {
            RankLetterTxt.text = "SSS";
        }
        else if (Score >= 100000f)
        {
            RankLetterTxt.text = "SS";
        }
        else if (Score >= 70000f)
        {
            RankLetterTxt.text = "S";
        }
        else if (Score >= 55000f)
        {
            RankLetterTxt.text = "A";
        }
        else if (Score >= 40000f)
        {
            RankLetterTxt.text = "B";
        }
        else if (Score >= 25000f)
        {
            RankLetterTxt.text = "C";
        }
        else if (Score >= 15000f)
        {
            RankLetterTxt.text = "D";
        }
        else if (Score >= 5000f)
        {
            RankLetterTxt.text = "E";
        }
        else
        {
            RankLetterTxt.text = "F";
        }

        //Text의 속성이 바뀌었는지 검사하는 함수. 
        //참이라면 애니메이션 실행
        if (rankLetterTxt.havePropertiesChanged)
        {
            rankVariationAnim.SetTrigger("Active");
            return;
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