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

    float _deltaTimeUnscaled = 0.0f;
    float _unscaledFrameTime = 0.0f;

    //float _deltaTimeScaled = 0.0f;

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

    void Update()
    {
        Unscaled_Update();
    }

    void Unscaled_Update()
    {
        float unscaled = Time.unscaledDeltaTime;

        _deltaTimeUnscaled += (unscaled - _deltaTimeUnscaled) * 0.1f;

        _unscaledFrameTime = _deltaTimeUnscaled * 1000f;

        //Debug.Log($"[DevDebug] FRAME TIME: _unscaledFrameTime: {_unscaledFrameTime}ms, _deltaTimeUnscaled: {_deltaTimeUnscaled}");
    }

    /*
    void ScaledVsUnscaled_Update() 
    {
        float unscaled = Time.unscaledDeltaTime;
        float scaled = Time.deltaTime;

        Debug.Log($"[DevDebug] START: _deltaTimeScaled: {_deltaTimeScaled}, _deltaTimeUnscaled: {_deltaTimeUnscaled}");

        _deltaTimeScaled += (scaled - _deltaTimeScaled) * 0.1f;
        _deltaTimeUnscaled += (unscaled - _deltaTimeUnscaled) * 0.1f;

        Debug.Log($"[DevDebug] END: _deltaTimeScaled: {_deltaTimeScaled}, _deltaTimeUnscaled: {_deltaTimeUnscaled}");
    }
    */

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.black;
        
        float msec = _unscaledFrameTime;
        float fps = 1000f / _unscaledFrameTime;
        
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        GUI.Label(rect, text, style);
    }

    void LogAwake()
    {
        Debug.Log($"[DevDebug] Screen current res: {Screen.currentResolution}");
        Debug.Log($"[DevDebug] Screen brightness: {Screen.brightness}");
        Debug.Log($"[DevDebug] Screen dpi: {Screen.dpi}");
        Debug.Log($"[DevDebug] Screen mainWindowDisplayInfo refresh rate: {Screen.mainWindowDisplayInfo.refreshRate.value}");
        Debug.Log($"[DevDebug] Screen mainWindowDisplayInfo (w,h): ({Screen.mainWindowDisplayInfo.width},{Screen.mainWindowDisplayInfo.height})");
        Debug.Log($"[DevDebug] -------");
        Debug.Log($"[DevDebug] Time maximumDeltaTime: {Time.maximumDeltaTime}");
    }

    
}
