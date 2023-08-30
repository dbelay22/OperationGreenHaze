using UnityEngine;
using Yxp.StateMachine;

public class WinState : GameState
{
    public override void EnterState()
    {
        Debug.Log("*** WIN ***");

        AudioController.Instance.PlayWinMusic();

        GameUI.Instance.ShowWin();

        Director.Instance.DumpStats();

        Director.Instance.GameIsOver();

        UnlockCursor();        
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