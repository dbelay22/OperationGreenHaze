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
    }
}
