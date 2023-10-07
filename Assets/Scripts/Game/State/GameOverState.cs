﻿using UnityEngine;
using Yxp.Helpers;
using Yxp.StateMachine;

public class GameOverState : GameState
{
    public GameOverState()
    {
    }

    public override void EnterState()
    {
        Debug.Log("*** GAME OVER ***");

        Director.Instance.DumpStats();

        //AudioController.Instance.PlayGameOverMusic();
        
        GameUI.Instance.ShowGameOver();

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