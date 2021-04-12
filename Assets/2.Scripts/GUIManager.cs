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

	private int score = 0;
	private int moveCounter;

	//프로퍼티
	public int Score
	{	get => score;
		set
		{
			score = value;
			scoreTxt.text = score.ToString();
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
				StartCoroutine(WaitForShifting());
			}
			moveCounterTxt.text = moveCounter.ToString();
		}
	}


	void Awake()
	{
		instance = GetComponent<GUIManager>();
		moveCounter = 5;
		moveCounterTxt.text = moveCounter.ToString();
	}

	// 게임오버가 되면 게임 오버 패널을 액티브
	public void GameOver()
	{
		GameManager.instance.gameOver = true;

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
	private IEnumerator WaitForShifting()
	{
		yield return new WaitUntil(() => !BoardManager.instance.IsShifting);
		yield return new WaitForSeconds(.25f);
		GameOver();
	}

}
