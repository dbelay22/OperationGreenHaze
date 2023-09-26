using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;


public class PlayerSettings : MonoBehaviour
{
    [SerializeField] AudioMixer _audioMixer;

    const float DEFAULT_MUSIC_VOLUME = 0;
    const float DEFAULT_SFX_VOLUME = 0f;

    const string PERSISTANCE_MUSIC_KEY = "music-volume";
    const string PERSISTANCE_SFX_KEY = "sfx-volume";

    #region Instance

    private static PlayerSettings _instance;

    public static PlayerSettings Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion

    void Start()
    {
        ApplyPlayerSettingsAudio();
    }

    void ApplyPlayerSettingsAudio()
    {
        SetAudioMixerMusicVolume(LoadMusicSetting());
        SetAudioMixerSFXVolume(LoadSFXSetting());
    }

    #region Audio

    public void SetAudioMixerMusicVolume(float volume)
    {
        // Music on Audio Mixer
        _audioMixer.SetFloat("MusicVolume", volume);
    }

    public bool SetAudioMixerSFXVolume(float volume)
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

    public void SaveMusicSetting(float value)
    {
        PlayerPrefs.SetFloat(PERSISTANCE_MUSIC_KEY, value);
    }

    public void SaveSFXSetting(float value)
    {
        PlayerPrefs.SetFloat(PERSISTANCE_SFX_KEY, value);
    }

    public float LoadMusicSetting()
    {
        if (PlayerPrefs.HasKey(PERSISTANCE_MUSIC_KEY) == false)
        {
            // first run
            SaveMusicSetting(DEFAULT_MUSIC_VOLUME);

            return DEFAULT_MUSIC_VOLUME;
        }

        return PlayerPrefs.GetFloat(PERSISTANCE_MUSIC_KEY);
    }

    public float LoadSFXSetting()
    {
        if (PlayerPrefs.HasKey(PERSISTANCE_SFX_KEY) == false)
        {
            // first run
            SaveSFXSetting(DEFAULT_SFX_VOLUME);

            return DEFAULT_SFX_VOLUME;
        }

        return PlayerPrefs.GetFloat(PERSISTANCE_SFX_KEY);
    }

    #endregion

    #endregion
}
