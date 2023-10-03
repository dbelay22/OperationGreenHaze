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

    [Header("Game Texts by Language")]
    [SerializeField] List<GameTextsByLanguage> _gameTextsByLanguage;

    [Header("Force Language")]
    [SerializeField] bool _shouldForceGameTexts;
    [SerializeField] GameTextsData _forcedGameTexts;

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

        if (_shouldForceGameTexts == true && _forcedGameTexts != null)
        {
            _currentGameTexts = _forcedGameTexts;

            if (_currentGameTexts == null)
            {
                Debug.LogError($"[Localization] LoadTextsBySystemLanguage) Error getting forced game texts");
            }
        }
        else
        {
            _currentGameTexts = GetGameTextsByLanguage(_currentSysLang);
            
            if (_currentGameTexts == null)
            {
                Debug.LogError($"[Localization] LoadTextsBySystemLanguage) Error getting game texts for language: {_currentSysLang}");
            }
        }

    }

    GameTextsData GetGameTextsByLanguage(SystemLanguage sysLang)
    {
        GameTextsByLanguage gameTexts = _gameTextsByLanguage.Find(t => t._lang.Equals(sysLang));

        if (gameTexts == null)
        {
            Debug.LogError($"[Localization] GetGameTexts) Error: text entry not found for lang: {sysLang}");
            return null;
        }

        return gameTexts._gameTexts;        
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
