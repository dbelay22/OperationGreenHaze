using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(AudioSource))]
public class OptionsMenu : MonoBehaviour
{
    [SerializeField] AudioClip _sfxSample;

    [Header("Slider range [-80db, 0db]")]
    [SerializeField] Slider _musicVolumeSlider;
    [SerializeField] Slider _sfxVolumeSlider;

    AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        _musicVolumeSlider.value = PlayerSettings.Instance.LoadMusicSetting();
        _sfxVolumeSlider.value = PlayerSettings.Instance.LoadSFXSetting();
    }

    #region Slider_Events

    public void OnMusicVolumeChange()
    {
        float newVolume = _musicVolumeSlider.value;

        // mixer
        PlayerSettings.Instance.SetAudioMixerMusicVolume(newVolume);
        
        // persist
        PlayerSettings.Instance.SaveMusicSetting(newVolume);
    }

    public void OnSFXVolumeChange()
    {
        float newVolume = _sfxVolumeSlider.value;

        // mixer
        bool volumeChanged = PlayerSettings.Instance.SetAudioMixerSFXVolume(newVolume);

        if (volumeChanged == false)
        {
            return;
        }

        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(_sfxSample);
        }

        // persist
        PlayerSettings.Instance.SaveSFXSetting(newVolume);
    }

    #endregion

}
