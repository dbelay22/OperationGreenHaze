using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DebugShortcuts))]
[RequireComponent(typeof(Quality))]
public class DevDebug : MonoBehaviour
{
    static Quality _quality;
    public Quality Quality { get { return _quality; } }

    [SerializeField] bool _isDebugBuild;

    public bool IsDebugBuild { get { return _isDebugBuild; } }

    #region Instance

    private static DevDebug _instance;

    public static DevDebug Instance { get { return _instance; } }
    
    #endregion
    
    void Awake()
    {
        _instance = this;

        _isDebugBuild = Debug.isDebugBuild;

        if (!_isDebugBuild)
        {
            return;
        }

        _quality = GetComponent<Quality>();

        LogAwake();
    }    

    void Start()
    {
        if (!_isDebugBuild)
        {
            return;
        }

        _quality = GetComponent<Quality>();
    }

    void LogAwake()
    {
        Debug.Log($"[DevDebug] Screen current res: {Screen.currentResolution}");
        Debug.Log($"[DevDebug] Screen brightness: {Screen.brightness}");
        Debug.Log($"[DevDebug] Screen dpi: {Screen.dpi}");
        Debug.Log($"[DevDebug] Screen mainWindowDisplayInfo refresh rate: {Screen.mainWindowDisplayInfo.refreshRate.value}");
        Debug.Log($"[DevDebug] Screen mainWindowDisplayInfo (w,h): ({Screen.mainWindowDisplayInfo.width},{Screen.mainWindowDisplayInfo.height})");
    }
}
