using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerData
{
    private static int skillPoint = 16;
    private static int highScore;

    public static int SkillPoint { get => skillPoint; set => skillPoint = value; }
    public static int HighScore { get => highScore; set => skillPoint = value; }

    public static void IncreaseSkillPoint()
    {
        skillPoint++;
    }

    public static void DecreaseSkillPoint()
    {
        skillPoint--;
    }
}
