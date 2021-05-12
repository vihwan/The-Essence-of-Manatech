using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private int skillPoint;
    private int highScore;

    public int SkillPoint { get => skillPoint; }
    public int HighScore { get => highScore; }

    public void SetSkillPoint(int skillPoint)
    {
        this.skillPoint = skillPoint;
    }

    public void SetHighScore(int highScore)
    {
        this.highScore = highScore;
    }
}
