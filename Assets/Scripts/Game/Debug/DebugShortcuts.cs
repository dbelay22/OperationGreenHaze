using UnityEngine;
using Yxp.Helpers;

public class DebugShortcuts : MonoBehaviour
{
    void Update()   
    {
        if (Game.Instance.IsDevBuild == false)
        {
            return;
        }
        ProcessDebugShortcuts();
    }

    void ProcessDebugShortcuts()
    {
        /*
        // + Volume
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            AudioListener.volume += 0.1f;
            
            Debug.Log($"[DebugShortcuts] AudioListerner volume now: {AudioListener.volume}");
        }

        // - Volume
        if (Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            AudioListener.volume -= 0.1f;

            Debug.Log($"[DebugShortcuts] AudioListerner volume now: {AudioListener.volume}");
        }
        */

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

        // [O] Force Game Over
        if (Input.GetKeyDown(KeyCode.O))
        {
            Game.Instance.ChangeStateToGameOver();
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

    }

}
