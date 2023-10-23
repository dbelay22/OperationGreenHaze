using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugComponentBase : MonoBehaviour
{
    void Awake()
    {
        if (DevDebug.Instance.IsDebugBuild)
        {
            DebugAwake();
        }
    }

    internal virtual void DebugAwake() { }

    void Start()
    {
        if (DevDebug.Instance.IsDebugBuild)
        {
            DebugStart();
        }
    }

    internal virtual void DebugStart() { }

    void Update()
    {
        if (DevDebug.Instance.IsDebugBuild)
        {
            DebugUpdate();
        }
    }

    internal virtual void DebugUpdate() { }    

}