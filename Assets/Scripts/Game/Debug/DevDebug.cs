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

    float _deltaTimeUnscaled = 0f;
    float _unscaledFrameTime = 0f;

    float _maxFrameTime = 0f;

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
        if (!_isDebugBuild)
        {
            return;
        }

        Unscaled_Update();
    }

    void Unscaled_Update()
    {
        float unscaled = Time.unscaledDeltaTime;

        _deltaTimeUnscaled += (unscaled - _deltaTimeUnscaled) * 0.1f;

        _unscaledFrameTime = _deltaTimeUnscaled * 1000f;

        if (_unscaledFrameTime > _maxFrameTime)
        {
            _maxFrameTime = _unscaledFrameTime;
        }

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
        if (!_isDebugBuild)
        {
            return;
        }

        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / 100;
        style.normal.textColor = Color.black;
        
        float fps = 1000f / _unscaledFrameTime;
        
        string text = string.Format("{0:0.0} ms ({1:0.} fps) . max {2:0.0}", _unscaledFrameTime, fps, _maxFrameTime);
        GUI.Label(rect, text, style);

        if (Input.GetKeyDown(KeyCode.Y))
        {
            ResetMaxFrameTime();
        }
    }

    void ResetMaxFrameTime()
    {
        _maxFrameTime = 0;
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
