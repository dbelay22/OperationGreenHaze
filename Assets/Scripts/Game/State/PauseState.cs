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
        Time.timeScale = 0;

        GameUI.Instance.ShowPause();

        //AudioController.Instance.GameplayPause();
    }      

    public override void ExitState()
    {
        Debug.Log("[PauseState] (ExitState) time scale back to 1");

        Time.timeScale = 1;
    }

    public override void Update()
    {
    }
}
