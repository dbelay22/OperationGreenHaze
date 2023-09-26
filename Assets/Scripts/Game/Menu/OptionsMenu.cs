using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using System;

[RequireComponent(typeof(AudioSource))]
public class OptionsMenu : MonoBehaviour
{
    const float DEFAULT_MUSIC_VOLUME = 0;
    const float DEFAULT_SFX_VOLUME = 0f;

    [SerializeField] AudioMixer _audioMixer;
    [SerializeField] AudioClip _sfxSample;
 
    [Header("Slider range [-80db, 0db]")]
    [SerializeField] Slider _musicVolumeSlider;
    [SerializeField] Slider _sfxVolumeSlider;

    AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        _musicVolumeSlider.value = LoadMusicSetting();

        _sfxVolumeSlider.value = LoadSFXSetting();
    }

    #region Slider_Events

    public void OnMusicVolumeChange()
    {
        float newVolume = _musicVolumeSlider.value;
        
        // mixer
        SetAudioMixerMusicVolume(newVolume);

        // persist
        SaveMusicSetting(newVolume);
    }

    public void OnSFXVolumeChange()
    {
        float newVolume = _sfxVolumeSlider.value;

        // mixer
        bool volumeChanged = SetAudioMixerSFXVolume(newVolume);

        if (volumeChanged == false)
        {
            return;
        }

        // play sample
        if (!_audioSource.isPlaying)
        {
            _audioSource.PlayOneShot(_sfxSample);
        }

        // persist
        SaveSFXSetting(newVolume);
    }

    #endregion

    void SetAudioMixerMusicVolume(float volume)
    {
        // Music on Audio Mixer
        _audioMixer.SetFloat("MusicVolume", volume);
    }

    bool SetAudioMixerSFXVolume(float volume)
    {
        _audioMixer.GetFloat("SFXVolume", out float currentVolume);

        if (currentVolume == volume)
        {
            return false;
        }

        // SFX on Audio Mixer
        _audioMixer.SetFloat("SFXVolume", volume);

        return true;
    }

    #region Persistance

    void SaveMusicSetting(float volume)
    {
    }

    void SaveSFXSetting(float volume)
    {    
    }

    float LoadMusicSetting()
    {
        return DEFAULT_MUSIC_VOLUME;
    }

    float LoadSFXSetting()
    {
        return DEFAULT_SFX_VOLUME;
    }

    #endregion


}
