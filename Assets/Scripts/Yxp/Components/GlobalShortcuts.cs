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
                Game.Instance.ChangeStateToPaused();
            }

            // [M]ute sound
            if (UnityEngine.Input.GetKeyDown(KeyCode.M))
            {
                AudioController.Instance.ToggleAudioOnOff();
            }
        }        
    }
}