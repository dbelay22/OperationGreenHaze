using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using FMOD.Studio;

public class OptionsMenu : MonoBehaviour
{
    [Header("Slider range [0, 1]")]
    [SerializeField] Slider _musicVolumeSlider;
    [SerializeField] Slider _sfxVolumeSlider;

    EventInstance _sfxSample;

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
        AudioController.Instance.SetMusicBusVolume(newVolume);
        
        // persist
        PlayerSettings.Instance.SaveMusicSetting(newVolume);
    }

    public void OnSFXVolumeChange()
    {
        float newVolume = _sfxVolumeSlider.value;

        // mixer
        AudioController.Instance.SetSFXBusVolume(newVolume);

        // play sample
        AudioController.Instance.PlayInstanceOrCreate(_sfxSample, FMODEvents.Instance.MenuSelect, out _sfxSample, false);

        // persist
        PlayerSettings.Instance.SaveSFXSetting(newVolume);
    }

    #endregion

}
