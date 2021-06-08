using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UISound
{
    public static void ClickNPCSE()
    {
        SoundManager.instance.PlaySE("OpenList");
    }

    public static void SelectMenuButton()
    {
        SoundManager.instance.PlaySE("ClickButton");
    }

    public static void ClickLevelUpButton()
    {
        SoundManager.instance.PlaySE("SkillLevelUp");
    }
}
