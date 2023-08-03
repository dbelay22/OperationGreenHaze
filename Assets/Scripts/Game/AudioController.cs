using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Header("Entities with AudioSource")]
    [SerializeField] AudioSource _musicSource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip _gameOverMusic;
    [SerializeField] AudioClip _winMusic;

    #region Instance

    private static AudioController _instance;
    
    public static AudioController Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion

    public void PlayBackgroundMusic()
    {
        _musicSource.Play();
    }

    void StopBackgroundMusic()
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

}
