using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultPage : MonoBehaviour
{

    private Text yourScoreTxt;
    private Text highScoreTxt;

    private Text yourClearTimeTxt;
    private Text bestClearTimeTxt;

    public void Init()
    {
        yourScoreTxt = transform.Find("YourScore").GetComponent<Text>();
        if (yourScoreTxt == null)
            Debug.LogWarning(yourScoreTxt.name + "이 참조되지 않았습니다.");

        highScoreTxt = transform.Find("HighScore").GetComponent<Text>();
        if (highScoreTxt == null)
            Debug.LogWarning(highScoreTxt.name + "이 참조되지 않았습니다.");

        yourClearTimeTxt = transform.Find("YourClearTime").GetComponent<Text>();
        if (yourClearTimeTxt == null)
            Debug.LogWarning(yourClearTimeTxt.name + "이 참조되지 않았습니다.");

        bestClearTimeTxt = transform.Find("BestClearTime").GetComponent<Text>();
        if (bestClearTimeTxt == null)
            Debug.LogWarning(bestClearTimeTxt.name + "이 참조되지 않았습니다.");
    }

    // 게임오버가 되면 게임 오버 패널을 액티브
    public void GameOverPanel()
    {
        StopAllCoroutines();
        this.gameObject.SetActive(true);

        if (GUIManager.instance.Score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", Mathf.RoundToInt(GUIManager.instance.Score));
            highScoreTxt.text = "New 베스트점수: " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        else
        {
            highScoreTxt.text = "베스트점수 : " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        yourScoreTxt.text = "클리어점수 : " + (Mathf.RoundToInt(GUIManager.instance.Score)).ToString();


        yourClearTimeTxt.text = "클리어타임 : " + 
            Mathf.RoundToInt(600 - GUIManager.instance.LimitTime).ToString() + "초";
        bestClearTimeTxt.text = "베스트타임 : ??";

        SoundManager.instance.StopAllSE();
        SoundManager.instance.StopBGM();
        SoundManager.instance.PlaySE("DungeonResult");
        BoardManager.instance = null; //보드 매니저 비활성화
        BoardManagerMonster.instance = null;
    }
}
