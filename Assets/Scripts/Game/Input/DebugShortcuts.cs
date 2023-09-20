using UnityEngine;

public class DebugShortcuts : MonoBehaviour
{
    void Update()
    {
        ListenDebugShortcuts();
    }

    void ListenDebugShortcuts()
    {
        // [G] GOD Mode
        if (UnityEngine.Input.GetKeyDown(KeyCode.G))
        {
            Game.Instance.ToggleGodMode();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Game.Instance.ReportAllEnemiesKilled();
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Game.Instance.ReportAllMissionPickupsCollected();
        }

    }
}
