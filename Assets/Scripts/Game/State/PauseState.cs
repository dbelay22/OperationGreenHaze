using UnityEngine;
using Yxp.StateMachine;

public class PauseState : GameState
{
    public override void EnterState()
    {
        Debug.Log("[PauseState] (EnterState)...");

        Time.timeScale = 0;

        GameUI.Instance.ShowPause();

        AudioController.Instance.GameplayPause();

        UICore.UnlockCursor();
    }      

    public override void ExitState(bool isShuttingDown)
    {
        Debug.Log("[PauseState] (ExitState)...");

        Time.timeScale = 1;

        if (isShuttingDown)
        {
            AudioController.Instance.GameplayExitFromPauseScreen();
        }
        else
        {
            AudioController.Instance.GameplayResume();
        }
    }

    public override void Update()
    {
    }
}
