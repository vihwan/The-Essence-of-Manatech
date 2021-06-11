using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UISound
{
    public static void ClickNPCSE()
    {
        SoundManager.instance.PlaySE("commandshow");
    }
    public static void CommandSelect()
    {
        SoundManager.instance.PlaySE("commandselect");
    }
    public static void ClickButton()
    {
        SoundManager.instance.PlaySE("click2");
    }
    public static void ClickLevelUpButton()
    {
        SoundManager.instance.PlaySE("minikingdom_ui_08");
    }
}
