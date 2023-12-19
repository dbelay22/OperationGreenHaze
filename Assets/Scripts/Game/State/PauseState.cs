using System.Threading.Tasks;
using UnityEngine;
using Yxp.Helpers;
using Yxp.StateMachine;

public class PauseState : GameState
{
    public override void EnterState()
    {
        Debug.Log("[PauseState] (EnterState)...");

        Time.timeScale = 0;

        GameUI.Instance.ShowPause();

        AudioController.Instance.GameplayPause();

        UnlockCursor();
    }      

    public override void ExitState(bool isShuttingDown)
    {
        Debug.Log("[PauseState] (ExitState)...");

        Time.timeScale = 1;

        if (isShuttingDown)
        {
            AudioController.Instance.GameplayStop();
        }
        else
        {
            AudioController.Instance.GameplayResume();
        }
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public override void Update()
    {
    }
}
