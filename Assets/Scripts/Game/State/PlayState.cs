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
        
        HUD.Instance.Reset();

        AudioController.Instance.PlayBackgroundMusic();
    }
   
    public override void ExitState()
    {        
    }

    public override void Update()
    {
    }
}
