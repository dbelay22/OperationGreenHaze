using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : TelegramMessageCanvas
{
    public override void OnFadeOutComplete()
    {
        base.OnFadeOutComplete();

        LevelLoader.Instance.LoadMainMenu();
    }
}
