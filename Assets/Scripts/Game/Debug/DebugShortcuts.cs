using UnityEngine;
using Yxp.Helpers;

public class DebugShortcuts : DebugComponentBase
{
    internal override void DebugUpdate()
    {
        ProcessDebugShortcuts();
    }

    void ProcessDebugShortcuts()
    {
        // + Volume
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            AudioController.Instance.SetMusicBusVolume(0.5f);            
        }

        // - Volume
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            AudioController.Instance.SetMusicBusVolume(0f);
        }

        // [G] GOD Mode
        if (Input.GetKeyDown(KeyCode.G))
        {
            Game.Instance.ToggleGodMode();
        }

        // [K]ill enemies objective completed
        if (Input.GetKeyDown(KeyCode.K))
        {
            Game.Instance.ReportAllEnemiesKilled();
        }

        // [P]ickup mission items collected
        if (Input.GetKeyDown(KeyCode.P))
        {
            Game.Instance.ReportAllMissionPickupsCollected();
        }

        // [Q]uit
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Application.Quit();
        }

        // [R]eload Current Scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneHelper.ReloadCurrentScene();
        }

        // [X]Clear exit (final mission objective)
        if (Input.GetKeyDown(KeyCode.X))
        {
            Game.Instance.ReportExitClear();
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            // TURN PP FX ON/OFF
            Game.Instance.TogglePPFx();
        }

        /////////////////////////////////////
        // QUALITY
        //         
        // [3] Lower quality
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DevDebug.Instance.Quality.DecreaseLevel();
        }

        // [4] Better quality
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            DevDebug.Instance.Quality.IncreaseLevel();
        }

    }

}
