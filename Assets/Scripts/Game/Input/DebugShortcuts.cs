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
            Debug.Log($"Force Game over");
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
            Debug.Log($"Quitting from platform {Application.platform}");
            Application.Quit();
        }

        // [R]eload Current Scene
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneHelper.ReloadCurrentScene();
        }
    }
        
}
