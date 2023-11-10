using UnityEngine;
using Yxp.Helpers;

namespace Yxp.Input
{
    public class GlobalShortcuts : MonoBehaviour
    {
        void Update()
        {
            // [Esc]ape
            if (UnityEngine.Input.GetKeyDown(KeyCode.Escape))
            {
                if (Game.Instance.IsGamePaused())
                {
                    Game.Instance.ResumeGame();
                }
                else
                {
                    Game.Instance.ChangeStateToPaused();
                }                
            }
        }        
    }
}