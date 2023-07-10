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
        
        SceneHelper.ReloadCurrentScene();
    }

    public override void ExitState()
    {
       
    }

    public override void Update()
    {
        
    }

}