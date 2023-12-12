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

        Director.Instance.DumpStats();

        GameUI.Instance.ShowGameOver();

        UnlockCursor();

        AudioController.Instance.GameplayStop();
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