using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yxp.StateMachine;

public class UnplayStateBase : GameState
{
    public override void EnterState()
    {
        Director.Instance.DumpStats();        
    }

    public override void ExitState(bool isShuttingDown)
    {
        AudioController.Instance.GameplayStop();

        UICore.UnlockCursor();
    }

    public override void Update()
    {
    }

}
