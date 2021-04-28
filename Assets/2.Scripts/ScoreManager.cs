using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;


    private ComboSystem comboSystem;

    public void Init()
    {
        instance = GetComponent<ScoreManager>();
        comboSystem = FindObjectOfType<ComboSystem>();
    }

    public void PlusScore()
    {
        //타일 한 개당 기본점수 : 50
        GUIManager.instance.Score += 50 * (comboSystem.ComboCounter + 1);
        //현재 Combo 1이라면
        //50 + 100 + 100 = 250 점이 오른다.
    }

    //점수를 3자리마다 ,를 넣어주는 함수
    public string ScoreWithComma(int score)
    {
        string withCommaString = string.Format("{0:#,0}", score);
        return withCommaString;
    }
}
