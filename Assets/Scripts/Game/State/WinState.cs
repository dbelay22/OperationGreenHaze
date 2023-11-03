using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yxp.StateMachine;

public class WinState : GameState
{
    public override void EnterState()
    {
#if UNITY_EDITOR
        Debug.Log("*** WIN ***");
#endif

        Director.Instance.DumpStats();

        GameUI.Instance.ShowWin();

        UnlockCursor();       
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