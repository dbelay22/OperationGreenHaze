using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BriefingUI : TelegramMessageCanvas
{
    public override void OnFadeOutComplete()
    {
        LevelLoader.Instance.LoadNextLevel();
    }

}
