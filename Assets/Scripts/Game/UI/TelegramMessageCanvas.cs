using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FMODUnity;
using FMOD.Studio;

public class TelegramMessageCanvas : MonoBehaviour
{
    [Header("Telegram message")]
    [SerializeField] float _fadeInOutDuration = 3f;
    [SerializeField] TMP_Text _telegramText;
    [SerializeField] TMP_Text _pressKeyText;
    
    [SerializeField] EventReference _bgMusicRef;

    EventInstance _bgMusicInstance;

    bool _keyWasPressed = false;

    TypewriterEffect _typewriterEffect;

    void Awake()
    {
        UICore.LockCursor();
    }

    void Start()
    {
        _pressKeyText.enabled = false;

        _keyWasPressed = false;

        // play sfx
        AudioController.Instance.PlayInstanceOrCreate(_bgMusicInstance, _bgMusicRef, out _bgMusicInstance, true);

        _typewriterEffect = GetComponentInChildren<TypewriterEffect>();

        AfterStart();
    }

    protected virtual void AfterStart()
    { }

    void Update()
    {
        if (_pressKeyText.enabled = true && _keyWasPressed == false && Input.anyKeyDown)
        {
            OnAnyKeyPressed();
        }

        if (LevelLoader.Instance.IsNextLevelReady())
        {
            if (!_pressKeyText.enabled)
            {
                // Show "PRESS ANY KEY"
                _pressKeyText.enabled = true;
            }            
        }
    }

    void OnAnyKeyPressed()
    {
        _keyWasPressed = true;

        _typewriterEffect.Flush();

        _pressKeyText.enabled = false;

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float time = 0;

        while (time < _fadeInOutDuration)
        {
            _telegramText.alpha = Mathf.Lerp(1, 0, time / _fadeInOutDuration);

            yield return new WaitForEndOfFrame();

            time += Time.deltaTime;
        }

        OnFadeOutComplete();
    }

    protected virtual void OnFadeOutComplete() 
    {
        Debug.Log($"TMC] OnFadeOutComplete) Ready to start next level / stopping music");        

        LevelLoader.Instance.ReadyToStartNextLevel();

        AudioController.Instance.StopFadeEvent(_bgMusicInstance);
    }

}
