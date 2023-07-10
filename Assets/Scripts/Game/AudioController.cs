using UnityEngine;

public class AudioController : MonoBehaviour
{
    [Header("Entities with AudioSource")]
    [SerializeField] GameObject _musicSource;

    AudioSource _musicAS;

    #region Instance
    private static AudioController _instance;
    public static AudioController Instance
    {
        get { return _instance; }
    }
    #endregion

    void Awake()
    {
        _instance = this;
        _musicAS = GetAudioSourceFromGO(_musicSource);
    }

    AudioSource GetAudioSourceFromGO(GameObject go)
    {
        return go.GetComponent<AudioSource>();
    }
    
    public void PlayBackgroundMusic()
    {
        _musicAS.Play();
    }

    public void StopBackgroundMusic()
    {
        _musicAS.Stop();
    }
    

}
