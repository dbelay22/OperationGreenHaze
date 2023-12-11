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

    void Start()
    {
        _pressKeyText.enabled = true;

        _keyWasPressed = false;

        // play sfx
        AudioController.Instance.PlayInstanceOrCreate(_bgMusicInstance, _bgMusicRef, out _bgMusicInstance, true);

        _typewriterEffect = GetComponentInChildren<TypewriterEffect>();
    }

    void Update()
    {
        if (_keyWasPressed == false && Input.anyKeyDown)
        {
            OnAnyKeyPressed();
        }
    }

    void OnAnyKeyPressed()
    {
        _keyWasPressed = true;

        _typewriterEffect.Flush();

        StartFadeOut();
    }

    void StartFadeOut()
    {
        _pressKeyText.enabled = false;

        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float time = 0;

        while (time < _fadeInOutDuration)
        {
            _telegramText.alpha = Mathf.Lerp(1, 0, time / _fadeInOutDuration);

            yield return null;

            time += Time.deltaTime;
        }

        OnFadeOutComplete();
    }

    public virtual void OnFadeOutComplete() 
    {
        //Debug.Log($"TMC] OnFadeOutComplete) stopping music {_bgMusicRef.Path}");

        AudioController.Instance.StopFadeEvent(_bgMusicInstance);
    }

}
