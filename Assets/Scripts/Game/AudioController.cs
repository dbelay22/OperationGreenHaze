using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Header("BG Music AudioSource")]
    [SerializeField] AudioSource _musicSource;

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

    public void PlayBackgroundMusic()
    {
        _musicSource.Play();
    }

    public void StopBackgroundMusic()
    {
        _musicSource.Stop();
    }

    public void PlayWinMusic()
    {
        StopBackgroundMusic();

        if (_winMusic != null)
        {
            _musicSource.PlayOneShot(_winMusic);
        }        
    }

    public void PlayGameOverMusic()
    {
        StopBackgroundMusic();

        if (_gameOverMusic != null)
        {
            _musicSource.PlayOneShot(_gameOverMusic);
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
