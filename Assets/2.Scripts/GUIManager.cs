using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIManager : MonoBehaviour
{
	public static GUIManager instance;

	public GameObject gameOverPanel;
	public Text yourScoreTxt;
	public Text highScoreTxt;

	public Text scoreTxt;
	public Text moveCounterTxt;
	public Text comboCounterTxt;
	public Text limitTimeTxt;

	private int score = 0;
	private int moveCounter;
    private int comboCounter;
	private float limitTime;

    //프로퍼티
    public int Score
	{	get => score;
		set
		{
			score = value;
			scoreTxt.text = ScoreManager.instance.ScoreWithComma(score);
		}
	}
	public int MoveCounter
	{
		get => moveCounter;
		set
		{
			moveCounter = value;
			if (moveCounter <= 0)
			{
				moveCounter = 0;
				//StartCoroutine(WaitForShifting()); //GameOver
			}
			moveCounterTxt.text = moveCounter.ToString();
		}
	}

    public int ComboCounter { get => comboCounter;
        set
        {
			comboCounter = value;
            comboCounterTxt.text = "Combo " + comboCounter.ToString();
		}
	}

    public float LimitTime { get => limitTime; 
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
		moveCounter = 5;
		comboCounter = 0;
		limitTime = 3;
		moveCounterTxt.text = moveCounter.ToString();
		comboCounterTxt.text = comboCounter.ToString();
		limitTimeTxt.text = limitTime.ToString();
	}

    private void Update()
    {
		LimitTime -= Time.deltaTime;
		//SetActiveComboText();
	}

	private void SetActiveComboText()
    {
		if(ComboCounter >= 2)
        {
			comboCounterTxt.gameObject.SetActive(true);
        }
		else
			comboCounterTxt.gameObject.SetActive(false);
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
	public IEnumerator WaitForShifting()
	{
		yield return new WaitUntil(() => !BoardManager.instance.IsShifting);
		yield return new WaitForSeconds(.25f);
		GameOverPanel(); //GUI GameOver Panel
	}
}
