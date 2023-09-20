using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinUI : TelegramMessageCanvas
{
    public override void OnFadeOutComplete()
    {
        LevelLoader.Instance.LoadMainMenu();
    }
}
