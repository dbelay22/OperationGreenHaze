using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FMOD.Studio;

public class BriefingUI : TelegramMessageCanvas
{
    EventInstance _helicopterBriefingSFX;

    protected override void AfterStart()
    {
        LevelLoader.Instance.LoadNextLevelAsync();

        Invoke(nameof(PlayHelicopterBriefing), 1.2f);
    }

    void PlayHelicopterBriefing()
    {
        AudioController.Instance.PlayInstanceOrCreate(_helicopterBriefingSFX, FMODEvents.Instance.Helicopter_Briefing, out _helicopterBriefingSFX, true);
    }

    override protected void OnFadeOutComplete()
    {
        base.OnFadeOutComplete();

        CancelInvoke();

        AudioController.Instance.StopFadeEvent(_helicopterBriefingSFX);
    }

}
