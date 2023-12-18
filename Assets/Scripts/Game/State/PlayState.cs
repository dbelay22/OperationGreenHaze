using System.Threading.Tasks;
using UnityEngine;
using Yxp.Helpers;
using Yxp.StateMachine;

public class PlayState : GameState
{
    public PlayState() {
    }

    public override void EnterState()
    {
        Debug.Log("[PlayState] (EnterState)...");

        Time.timeScale = 1;
        
        LockCursor();

        GameUI.Instance.ShowGameplay();

        AudioController.Instance.GameplayStart();
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public override void ExitState()
    {
        Debug.Log("[PlayState] (ExitState)...");

        AudioController.Instance.GameplayStop();

        Director.Instance.DumpStats();

        UnlockCursor();
    }

    public override void Update()
    {
    }
}
