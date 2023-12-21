using FMOD.Studio;
using UnityEngine;

public class AfterBriefingUI : TelegramMessageCanvas
{
    readonly int SFX_POSITION_CUT = 18 * 1000;

    EventInstance _helicopterBriefingSFX;

    protected override void Start()
    {
        base.Start();

        LevelLoader.Instance.LoadNextLevelAsync();

        AudioController.Instance.PlayInstanceOrCreate(_helicopterBriefingSFX, FMODEvents.Instance.Helicopter_Briefing, out _helicopterBriefingSFX, true);
    }

    protected override void Update()
    {
        base.Update();

        _helicopterBriefingSFX.getTimelinePosition(out int position);

        if (!_keyWasPressed && position >= SFX_POSITION_CUT)
        {
            StartCoroutine(OnAnyKeyPressed());
        }
    }

    override protected void OnFadeOutComplete()
    {
        base.OnFadeOutComplete();

        AudioController.Instance.StopFadeEvent(_helicopterBriefingSFX);
    }

}
