using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameTextsData;

public class Localization : MonoBehaviour
{
    [System.Serializable]
    public class GameTextsByLanguage
    {
        public SystemLanguage _lang;
        public GameTextsData _gameTexts;
    }

    [SerializeField] GameTextsByLanguage[] _gameTextsByLanguage;

    SystemLanguage _currentSysLang;

    GameTextsData _currentGameTexts;

    #region Instance

    private static Localization _instance;

    public static Localization Instance { get { return _instance; } }

    #endregion

    void Awake()
    {
        _instance = this;

        LoadTexts();
    }

    void LoadTexts()
    {
        _currentSysLang = Application.systemLanguage;
        Debug.Log($"[Localization] LoadTextsBySystemLanguage) sysLang:{_currentSysLang}");

        _currentGameTexts = GetGameTexts(_currentSysLang);

        if (_currentGameTexts == null)
        {
            Debug.LogError($"[Localization] LoadTextsBySystemLanguage) Error getting game texts for language: {_currentSysLang}");
            return;
        }
    }

    GameTextsData GetGameTexts(SystemLanguage sysLang)
    {
        foreach (GameTextsByLanguage gameTexts in _gameTextsByLanguage) 
        {
            if (gameTexts._lang.Equals(sysLang))
            {
                return gameTexts._gameTexts;
            }
        }

        return null;
    }

    public string GetTextByKey(string localizedTextKey)
    {
        GameTextEntry textEntry = _currentGameTexts._gameTextEntries.Find(t => t._key.Equals(localizedTextKey));

        if (textEntry == null)
        {
            Debug.LogError($"[Localization] GetTextByKey) Error: text entry not found for key: {localizedTextKey}");
            return localizedTextKey;
        }

        Debug.Log($"[Localization] GetTextByKey) key: {localizedTextKey}, text: {textEntry._value}");

        return textEntry._value;
    }
}
