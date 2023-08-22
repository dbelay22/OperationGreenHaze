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

        HUD.Instance.ShowPause();

        AudioController.Instance.StopBackgroundMusic();
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public override void ExitState()
    {        
    }

    public override void Update()
    {
    }
}
