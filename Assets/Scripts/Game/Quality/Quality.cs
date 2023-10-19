using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quality : MonoBehaviour
{
    int _currentQualityLevel;

    void Awake()
    {
        _currentQualityLevel = QualitySettings.GetQualityLevel();

        Debug.Log($"[Quality] Current quality level: {_currentQualityLevel}, settings: {QualitySettings.GetQualitySettings()}");
    }

    public void SetQualityLevel(int level)
    {
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

    internal void IncreaseLevel()
    {
        int nextLevel = _currentQualityLevel + 1;
        
        if (nextLevel > 2)
        {
            nextLevel = 0;
        }

        SetQualityLevel(nextLevel);
    }

    internal void DecreaseLevel()
    {
        int nextLevel = _currentQualityLevel - 1;

        if (nextLevel < 0)
        {
            nextLevel = 0;
        }

        SetQualityLevel(nextLevel);
    }
}
