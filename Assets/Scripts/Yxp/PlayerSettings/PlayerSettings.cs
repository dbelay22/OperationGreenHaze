using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;


public class PlayerSettings : MonoBehaviour
{
    const float DEFAULT_MUSIC_VOLUME = 0f;
    const float DEFAULT_SFX_VOLUME = 0f;

    public const float MIN_VOLUME_DB = -80f;
    public const float MAX_VOLUME_DB = 0f;

    const string PERSISTANCE_MUSIC_KEY = "music-volume";
    const string PERSISTANCE_SFX_KEY = "sfx-volume";

    Bus _musicBus;
    Bus _sfxBus;

    #region Instance

    private static PlayerSettings _instance;

    public static PlayerSettings Instance { get { return _instance; } }

    void Awake()
    {
        Debug.Log("PlayerSettings] Awake)...");

        _instance = this;
    }

    #endregion

    void Start()
    {
        ApplyPlayerSettingsAudio();
    }

    public void ApplyPlayerSettingsAudio()
    {
        Debug.Log("PlayerSettings] ApplyPlayerSettingsAudio)...");

        AudioController.Instance.SetMusicBusVolume(LoadMusicSetting());
        
        AudioController.Instance.SetSFXBusVolume(LoadSFXSetting());
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
}
