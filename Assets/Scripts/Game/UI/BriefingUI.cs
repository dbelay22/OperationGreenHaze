using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FMOD.Studio;

public class BriefingUI : TelegramMessageCanvas
{
    protected override void Start()
    {
        base.Start();

        LevelLoader.Instance.LoadNextLevelAsync();
    }

}
