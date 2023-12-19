using UnityEngine;

public class GameOverState : UnplayStateBase
{
    public override void EnterState()
    {
        base.EnterState();

#if UNITY_EDITOR        
        Debug.Log("*** GAME OVER ***");
#endif

        GameUI.Instance.ShowGameOver();
    }

}