using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class BriefingUI : MonoBehaviour
{
    [Header("Telegram message")]
    [SerializeField] float _fadeInOutDuration = 3f;
    [SerializeField] TMP_Text _textToFadeIn;
    [SerializeField] TMP_Text _pressKeyText;
    [SerializeField] AudioClip _bgMusic;

    AudioSource _audioSource;

    bool _keyWasPressed = false;

    TypewriterEffect _typewriterEffect;

    void Start()
    {
        _pressKeyText.enabled = true;
        
        _keyWasPressed = false;

        // play helicopter sfx
        _audioSource = GetComponent<AudioSource>();
        _audioSource.PlayOneShot(_bgMusic);

        _typewriterEffect = GetComponentInChildren<TypewriterEffect>();

        // show telegram
        StartCoroutine(ShowTelegramMessage());
    }

    IEnumerator ShowTelegramMessage()
    {
        float time = 0;

        while (time < _fadeInOutDuration)
        {
            _textToFadeIn.alpha = Mathf.Lerp(0, 1, time / _fadeInOutDuration);

            yield return null;

            time += Time.deltaTime;
        }
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

        StopAllCoroutines();

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
            _textToFadeIn.alpha = Mathf.Lerp(1, 0, time / _fadeInOutDuration);

            yield return null;

            time += Time.deltaTime;
        }

        OnFadeOutComplete();
    }

    void OnFadeOutComplete()
    {
        LevelLoader.Instance.LoadNextLevel();
    }
}
