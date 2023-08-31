using System.Threading.Tasks;
using UnityEngine;
using Yxp.Helpers;
using Yxp.StateMachine;

public class PauseState : GameState
{
    public PauseState() {
    }

    public override void EnterState()
    {
        Debug.Log("*** PAUSE ***");

        Time.timeScale = 0;
        
        UnlockCursor();

        GameUI.Instance.ShowPause();

        AudioController.Instance.StopBackgroundMusic();
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public override void ExitState()
    {
        Debug.Log("[PauseState] (ExitState) time scale back to 1");

        Time.timeScale = 1;
    }

    public override void Update()
    {
    }
}
