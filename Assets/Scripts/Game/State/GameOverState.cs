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
        
        AudioController.Instance.StopBackgroundMusic();

        HUD.Instance.ShowGameOver();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void ExitState()
    {
       
    }

    public override void Update()
    {
        
    }

}