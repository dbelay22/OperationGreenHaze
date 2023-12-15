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

        GameUI.Instance.ShowWin();
    }

    public override void ExitState()
    {

    }

    public override void Update()
    {

    }

}