using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class OptionsMenu : MonoBehaviour
{
    [Header("Slider range [-80db, 0db]")]
    [SerializeField] Slider _musicVolumeSlider;
    [SerializeField] Slider _sfxVolumeSlider;
        

    void Start()
    {
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

        // persist
        PlayerSettings.Instance.SaveSFXSetting(newVolume);
    }

    #endregion

}
