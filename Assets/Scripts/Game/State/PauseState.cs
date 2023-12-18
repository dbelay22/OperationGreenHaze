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
        Debug.Log("[PauseState] (EnterState)...");

        Time.timeScale = 0;

        GameUI.Instance.ShowPause();

        AudioController.Instance.GameplayPause();
    }      

    public override void ExitState()
    {
        Debug.Log("[PauseState] (ExitState)...");

        Time.timeScale = 1;

        AudioController.Instance.GameplayResume();
    }

    public override void Update()
    {
    }
}
