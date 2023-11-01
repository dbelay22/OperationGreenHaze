using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitBlocker : MonoBehaviour
{
    bool _reportedExitClear;

    void Awake()
    {
        _reportedExitClear = false;
    }

    public void OnBoomBoxExplosion()
    {
        if (_reportedExitClear)
        {
            return;
        }

        Game.Instance.ReportExitClear();

        _reportedExitClear = true;
    }
}
