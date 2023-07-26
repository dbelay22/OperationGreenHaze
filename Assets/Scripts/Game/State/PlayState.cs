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

        //Time.timeScale = 1;
        LockCursor();

        HUD.Instance.ShowGameplay();

        AudioController.Instance.PlayBackgroundMusic();
    }

    void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public override void ExitState()
    {        
    }

    public override void Update()
    {
    }
}
