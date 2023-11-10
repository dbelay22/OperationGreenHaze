using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioController : MonoBehaviour
{
    /*
    [Header("Ingame Music AudioSource")]
    [SerializeField] AudioSource _ingameMusicSource;

    [Header("Audio Clips")]
    [SerializeField] AudioClip _gameOverMusic;
    [SerializeField] AudioClip _winMusic;
    */

    List<EventInstance> _eventInstances = new List<EventInstance>();

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
        //RuntimeManager.LoadBank("Master");
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        
        _eventInstances.Add(eventInstance);
        
        return eventInstance;
    }

    public void PlayEvent(EventInstance eventInstance)
    {
        eventInstance.getPlaybackState(out PLAYBACK_STATE playbackState);

        //Debug.Log($"AudioController] PlayEvent) playbackState: {playbackState}");

        if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
        {
            eventInstance.start();
        }        
    }

    public void StopEvent(EventInstance eventInstance)
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void StopFadeEvent(EventInstance eventInstance)
    {
        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }    

    void OnDestroy()
    {
        CleanUp();
    }

    void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in _eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);

            eventInstance.release();
        }

        // stop all of the event emitters, because if we don't they may hang around in other scenes
        /*
        foreach (StudioEventEmitter emitter in _eventEmitters)
        {
            emitter.Stop();
        }
        */
    }

    /*
    public void GameplayStart()
    {
        PlayerSettings.Instance.ApplyPlayerSettingsAudio();

        _ingameMusicSource.Play();
    }
    */

    /*    
    public void GameplayPause()
    {
        PlayerSettings.Instance.SetAudioMixerMusicVolume(PlayerSettings.MIN_VOLUME_DB);
        PlayerSettings.Instance.SetAudioMixerSFXVolume(PlayerSettings.MIN_VOLUME_DB);
    }
    */

}
