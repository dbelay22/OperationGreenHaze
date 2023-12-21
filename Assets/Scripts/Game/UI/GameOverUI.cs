using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : TelegramMessageCanvas
{
    protected override void AfterStart()
    {
        LevelLoader.Instance.LoadMainMenuAsync(false);
    }

}
