using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

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

    EventInstance _musicEventInstance;

    #region Instance

    private static AudioController _instance;

    public static AudioController Instance { get { return _instance; } }

    #endregion

    void Awake()
    {
        _instance = this;

        _musicEventInstance = CreateInstance(FMODEvents.Instance.GameplayMusicEvent);
    }

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

    public void PlayEvent(EventInstance eventInstance, bool forcePlay = false)
    {
        eventInstance.getPlaybackState(out PLAYBACK_STATE playbackState);

        //Debug.Log($"AudioController] PlayEvent) playbackState: {playbackState}");

        if (playbackState.Equals(PLAYBACK_STATE.STOPPED) || forcePlay)
        {
            //Debug.Log($"AudioController] PlayEvent) starting event");
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

    public void PauseEvent(EventInstance eventInstance)
    {
        eventInstance.setPaused(true);
    }

    public void ResumeEvent(EventInstance eventInstance)
    {
        eventInstance.setPaused(false);
    }

    void ReleaseEvent(EventInstance eventInstance)
    {
        StopEvent(eventInstance);
        eventInstance.release();
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
            ReleaseEvent(eventInstance);
        }

        ReleaseEvent(_musicEventInstance);         

        // stop all of the event emitters, because if we don't they may hang around in other scenes
        /*
        foreach (StudioEventEmitter emitter in _eventEmitters)
        {
            emitter.Stop();
        }
        */
    }

    public void GameplayStart()
    {
        //Debug.Log($"AudioController] GameplayStart)...");

        //PlayerSettings.Instance.ApplyPlayerSettingsAudio();

        _musicEventInstance.getPlaybackState(out PLAYBACK_STATE playbackState);
        //Debug.Log($"AudioController] GameplayStart) playbackState: {playbackState}");

        if (playbackState.Equals(PLAYBACK_STATE.STOPPED))
        {
            _musicEventInstance.setParameterByName(FMODEvents.Instance.MusicPartsParam, 0);

            _musicEventInstance.setParameterByName(FMODEvents.Instance.TerrorMusicParam, 0);

            PlayEvent(_musicEventInstance);
        }
    }

    public void GameplayFindExit()
    {
        _musicEventInstance.setParameterByName(FMODEvents.Instance.MusicPartsParam, 1);
    }

    public void GameplayDead()
    {
        int musicPart = Game.Instance.PlayerNeedsToClearExitNow() ? 3 : 2;

        Debug.Log($"[AudioController] GameplayDead) musicPart:{musicPart}");

        _musicEventInstance.setParameterByName(FMODEvents.Instance.MusicPartsParam, musicPart);
    }
    public void GameplayPause()
    {
        Debug.Log($"AudioController] GameplayPause)...");

        //PlayerSettings.Instance.SetAudioMixerMusicVolume(PlayerSettings.MIN_VOLUME_DB);
        //PlayerSettings.Instance.SetAudioMixerSFXVolume(PlayerSettings.MIN_VOLUME_DB);

        PauseEvent(_musicEventInstance);
    }

    public void GameplayIntensityUpdate(float intensity)
    {
        _musicEventInstance.setParameterByName(FMODEvents.Instance.TerrorMusicParam, intensity);
    }

    public void GameplayResume()
    {
        ResumeEvent(_musicEventInstance);
    }

    public void GameplayStop()
    {
        StopEvent(_musicEventInstance);
    }

}
