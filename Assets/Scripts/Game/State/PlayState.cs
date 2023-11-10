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
        Time.timeScale = 1;
        
        LockCursor();

        GameUI.Instance.ShowGameplay();

        //AudioController.Instance.GameplayStart();
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public override void ExitState()
    {
        UnlockCursor();        
    }

    public override void Update()
    {
    }
}
