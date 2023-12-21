using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BriefingUI : TelegramMessageCanvas
{
    protected override void AfterStart()
    {
        LevelLoader.Instance.LoadNextLevelAsync();
    }

}
