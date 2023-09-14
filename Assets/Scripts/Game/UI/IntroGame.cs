using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class IntroGame : MonoBehaviour
{
    [Header("Telegram message")]
    [SerializeField] float _fadeInTextDuration = 3f;
    [SerializeField] TMP_Text _textToFadeIn;
    [SerializeField] TMP_Text _pressKeyText;

    [Header("Arrival")]
    [SerializeField] AudioClip _helicopterSFX;

    AudioSource _audioSource;

    bool _keyWasPressed = false;

    void Start()
    {
        _pressKeyText.enabled = true;
        
        _keyWasPressed = false;

        _audioSource = GetComponent<AudioSource>();

        StartCoroutine(Step1_ShowTelegramMessage());
    }

    void Update()
    {
        if (Input.anyKeyDown && _keyWasPressed == false)
        {
            OnAnyKeyPressed();
        }
    }

    void OnAnyKeyPressed()
    {
        _keyWasPressed = true;

        StopAllCoroutines();

        Step2_HelicopterArrival();
    }

    IEnumerator Step1_ShowTelegramMessage()
    {
        float time = 0;

        while (time < _fadeInTextDuration)
        {
            _textToFadeIn.alpha = Mathf.Lerp(0, 1, time / _fadeInTextDuration);

            yield return null;

            time += Time.deltaTime;
        }
    }        

    void Step2_HelicopterArrival()
    {
        _pressKeyText.enabled = false;

        _audioSource.PlayOneShot(_helicopterSFX);

        StartCoroutine(Step3_FadeOut());
    }

    IEnumerator Step3_FadeOut()
    {
        float time = 0;       

        while (time < _fadeInTextDuration)
        {
            _textToFadeIn.alpha = Mathf.Lerp(1, 0, time / _fadeInTextDuration);

            yield return null;

            time += Time.deltaTime;
        }

        LevelLoader.Instance.LoadNextLevel();
    }
}
