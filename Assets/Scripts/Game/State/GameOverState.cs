using UnityEngine;
using Yxp.Helpers;
using Yxp.StateMachine;

public class GameOverState : GameState
{
    public override void EnterState()
    {
#if UNITY_EDITOR        
        Debug.Log("*** GAME OVER ***");
#endif

        GameUI.Instance.ShowGameOver();
    }

    public override void ExitState()
    {
    }

    public override void Update()
    {        
    }

}