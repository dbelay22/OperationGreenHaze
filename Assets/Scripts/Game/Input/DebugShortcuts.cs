using UnityEngine;

public class DebugShortcuts : MonoBehaviour
{
    void Update()
    {
        ListenDebugShortcuts();
    }

    void ListenDebugShortcuts()
    {
        // [T] Dump Director Stats
        if (UnityEngine.Input.GetKeyDown(KeyCode.T))
        {
            DirectorAI.Instance.DumpStats();
        }
    }
}
