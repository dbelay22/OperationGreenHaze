using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBlocker : MonoBehaviour
{
    public void OnBoomBoxExplosion() 
    {
        Debug.Log("[ExitBlocker] OnBoomBoxExplosion)");

        Game.Instance.ReportExitClear();
    }
}
