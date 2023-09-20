using UnityEngine;
using Yxp.Helpers;
using Yxp.StateMachine;

public class GameOverState : GameState
{
    public GameOverState()
    {
    }

    public override void EnterState()
    {
        Debug.Log("*** GAME OVER ***");
        
        AudioController.Instance.PlayGameOverMusic();

        GameUI.Instance.ShowGameOver();

        Director.Instance.DumpStats();

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