﻿using UnityEngine;
using Yxp.Helpers;

namespace Yxp.Input
{
    public class GlobalShortcuts : MonoBehaviour
    {
        void Update()
        {
            GeneralShortcuts();

            SceneShortcuts();

            StateShortcuts();
        }

        void GeneralShortcuts()
        {
            // [Q]uit
            if (UnityEngine.Input.GetKeyDown(KeyCode.Q) || UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                Debug.Log($"Quitting from platform {Application.platform}");
                Application.Quit();
            }
        }

        void SceneShortcuts()
        {
            // [R]eload Current Scene
            if (UnityEngine.Input.GetKeyDown(KeyCode.R) || UnityEngine.Input.GetButtonDown("Fire3"))
            {
                SceneHelper.ReloadCurrentScene();
            }
        }

        void StateShortcuts()
        {
            // [O] Force Game Over
            if (UnityEngine.Input.GetKeyDown(KeyCode.O))
            {
                Debug.Log($"Force Game over");
                Game.Instance.ForceGameOver();
            }
        }
        
    }
}