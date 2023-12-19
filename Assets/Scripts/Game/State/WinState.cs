using UnityEngine;

public class WinState : UnplayStateBase
{
    public override void EnterState()
    {
        base.EnterState();

#if UNITY_EDITOR
        Debug.Log("*** WIN ***");
#endif

        GameUI.Instance.ShowWin();
    }

}