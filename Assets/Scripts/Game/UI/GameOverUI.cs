using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverUI : TelegramMessageCanvas
{
    protected override void Start()
    {
        base.Start();

        LevelLoader.Instance.LoadMainMenuAsync(false);
    }

}
