using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UISound
{
    public static void ClickNPCSE()
    {
        SoundManager.instance.PlaySE("CommandShow");
    }
    public static void CommandSelect()
    {
        SoundManager.instance.PlaySE("CommandSelect");
    }
    public static void ClickButton()
    {
        SoundManager.instance.PlaySE("ClickButton");
    }
    public static void ClickLevelUpButton()
    {
        SoundManager.instance.PlaySE("SkillLevelUp");
    }
}
