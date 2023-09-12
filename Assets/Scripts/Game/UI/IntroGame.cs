using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class IntroGame : MonoBehaviour
{
    [Header("Telegram message")]
    [SerializeField] float _fadeTextDuration = 3f;
    [SerializeField] TMP_Text _text;
    [SerializeField] TMP_Text _pressKeyText;

    [Header("Arrival")]
    [SerializeField] AudioClip _helicopterSFX;

    AudioSource _audioSource;

    void Start()
    {
        _pressKeyText.enabled = false;

        _audioSource = GetComponent<AudioSource>();

        StartCoroutine(Step1_ShowTelegramMessage());
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            Debug.Log("[IntroGame] (Update) Any key pressed!");
            Step2_HelicopterArrival();
        }
    }

    IEnumerator Step1_ShowTelegramMessage()
    {
        Debug.Log("[IntroGame] (Step1_ShowTelegramMessage)...");

        float time = 0;

        while (time < _fadeTextDuration)
        {
            _text.alpha = Mathf.Lerp(0, 1, time / _fadeTextDuration);

            yield return null;

            time += Time.deltaTime;
        }

        _pressKeyText.enabled = true;
    }        

    void Step2_HelicopterArrival()
    {
        Debug.Log("[IntroGame] (Step2_HelicopterArrival)...");
        
        _pressKeyText.enabled = false;

        _audioSource.PlayOneShot(_helicopterSFX);

        StartCoroutine(Step3_FadeOut());
    }

    IEnumerator Step3_FadeOut()
    {
        Debug.Log("[IntroGame] (Step3_FadeOut)...");
        float time = 0;

        while (time < _fadeTextDuration)
        {
            _text.alpha = Mathf.Lerp(1, 0, time / _fadeTextDuration);

            yield return null;

            time += Time.deltaTime;
        }

        Debug.Log("[IntroGame] (LoadNextLevel)...");

        // let sfx be heard
        yield return new WaitForSeconds(5f);

        LevelLoader.Instance.LoadNextLevel();
    }
}
