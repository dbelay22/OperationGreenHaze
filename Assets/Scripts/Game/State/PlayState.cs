using System.Threading.Tasks;
using UnityEngine;
using Yxp.Helpers;
using Yxp.StateMachine;

public class PlayState : GameState
{
    public PlayState() {
    }

    public override void EnterState()
    {
        Debug.Log("[PlayState] (EnterState)...");

        UICore.LockCursor();

        Time.timeScale = 1;

        AudioController.Instance.GameplayStart();

        GameUI.Instance.ShowGameplay();        
    }

    public override void ExitState(bool isShuttingDown)
    {
    }

    public override void Update()
    {
    }
}
