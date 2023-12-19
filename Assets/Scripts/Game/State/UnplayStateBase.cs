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

    public override void ExitState()
    {
        AudioController.Instance.GameplayStop();

        UnlockCursor();
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public override void Update()
    {
    }

}
