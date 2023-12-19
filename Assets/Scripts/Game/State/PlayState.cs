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

        LockCursor();

        Time.timeScale = 1;

        AudioController.Instance.GameplayStart();

        GameUI.Instance.ShowGameplay();        
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void ExitState(bool isShuttingDown)
    {
    }

    public override void Update()
    {
    }
}
