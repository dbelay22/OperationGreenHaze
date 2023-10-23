using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quality : DebugComponentBase
{
    int _currentQualityLevel;

    internal override void DebugAwake()
    {
        _currentQualityLevel = QualitySettings.GetQualityLevel();

        Debug.Log($"[Quality] Current quality level: {_currentQualityLevel}");
    }

    public void IncreaseLevel()
    {
        int nextLevel = _currentQualityLevel + 1;

        SetQualityLevel(nextLevel);
    }

    public void DecreaseLevel()
    {
        int nextLevel = _currentQualityLevel - 1;

        SetQualityLevel(nextLevel);
    }

    void SetQualityLevel(int level)
    {
        level = Math.Clamp(level, 0, 2);

        Debug.Log($"[Quality] SetQualityLevel) level:{level}");

        QualitySettings.SetQualityLevel(level);
        
        GetCurrentQLevel();
    }

    int GetCurrentQLevel()
    {
        _currentQualityLevel = QualitySettings.GetQualityLevel();

        Debug.Log($"[Quality] Current quality level: {_currentQualityLevel}, settings: {QualitySettings.GetQualitySettings()}");

        return _currentQualityLevel;
    }

}
