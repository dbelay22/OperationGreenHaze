using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yxp.StateMachine;

public class UnplayStateBase : GameState
{
    public override void EnterState()
    {
        //Debug.Log("UnplayStateBase] EnterState)...");

        Director.Instance.DumpStats();        
    }

    public override void ExitState(bool isShuttingDown)
    {
        //Debug.Log("UnplayStateBase] ExitState)...");

        AudioController.Instance.GameplayStop();
    }

    public override void Update()
    {
    }

}
