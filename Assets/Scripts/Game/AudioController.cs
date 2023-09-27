using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Header("Ingame Music AudioSource")]
    [SerializeField] AudioSource _ingameMusicSource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip _gameOverMusic;
    [SerializeField] AudioClip _winMusic;

    bool _audioIsOn = true;

    #region Instance

    private static AudioController _instance;
    
    public static AudioController Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion

    void Start()
    {
        _audioIsOn = true;
        
        SetAudioVolume(_audioIsOn);
    }

    public void GameplayStart()
    {
        PlayerSettings.Instance.ApplyPlayerSettingsAudio();

        _ingameMusicSource.Play();
    }

    public void GameplayPause()
    {
        PlayerSettings.Instance.SetAudioMixerMusicVolume(PlayerSettings.MIN_VOLUME_DB);
        PlayerSettings.Instance.SetAudioMixerSFXVolume(PlayerSettings.MIN_VOLUME_DB);
    }

    void StopBackgroundMusic()
    {
        _ingameMusicSource.Stop();
    }

    public void PlayWinMusic()
    {
        StopBackgroundMusic();

        if (_winMusic != null)
        {
            _ingameMusicSource.PlayOneShot(_winMusic);
        }        
    }

    public void PlayGameOverMusic()
    {
        StopBackgroundMusic();

        if (_gameOverMusic != null)
        {
            _ingameMusicSource.PlayOneShot(_gameOverMusic);
        }
    }

    public void ToggleAudioOnOff()
    {
        _audioIsOn = !_audioIsOn;
        
        SetAudioVolume(_audioIsOn);
    }

    void SetAudioVolume(bool audioIsOn)
    {
        AudioListener.volume = audioIsOn ? 1f : 0f;
    }

}
