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

    protected bool _keyWasPressed = false;

    TypewriterEffect _typewriterEffect;

    void Awake()
    {
        UICore.LockCursor();
    }

    protected virtual void Start()
    {
        _pressKeyText.gameObject.SetActive(false);

        _keyWasPressed = false;

        // play sfx
        AudioController.Instance.PlayInstanceOrCreate(_bgMusicInstance, _bgMusicRef, out _bgMusicInstance, true);

        _typewriterEffect = GetComponentInChildren<TypewriterEffect>();
    }

    protected virtual void Update()
    {
        if (_pressKeyText.gameObject.activeSelf && _keyWasPressed == false && Input.anyKeyDown)
        {
            StartCoroutine(OnAnyKeyPressed());
        }

        if (LevelLoader.Instance.IsNextLevelReady())
        {
            if (!_pressKeyText.gameObject.activeSelf)
            {
                // Show "PRESS ANY KEY"
                _pressKeyText.gameObject.SetActive(true);
            }            
        }
    }

    protected IEnumerator OnAnyKeyPressed()
    {
        //Debug.Log("OnAnyKeyPressed");

        _keyWasPressed = true;

        _pressKeyText.enabled = false;
        _pressKeyText.StopAllCoroutines();
        _pressKeyText.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.1f);

        _typewriterEffect.Flush();

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
        //Debug.Log($"TMC] OnFadeOutComplete) Ready to start next level / stopping music");        

        LevelLoader.Instance.ReadyToStartNextLevel();

        AudioController.Instance.StopFadeEvent(_bgMusicInstance);
    }

}
