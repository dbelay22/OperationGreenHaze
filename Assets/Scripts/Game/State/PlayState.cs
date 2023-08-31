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
        Debug.Log("*** PLAY ***");

        Time.timeScale = 1;
        
        LockCursor();

        GameUI.Instance.ShowGameplay();

        AudioController.Instance.PlayBackgroundMusic();
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
        Debug.Log("[PlayState] (ExitState) Unlocking cursor & stopping music");

        UnlockCursor();

        AudioController.Instance.StopBackgroundMusic();
    }

    public override void Update()
    {
    }
}
