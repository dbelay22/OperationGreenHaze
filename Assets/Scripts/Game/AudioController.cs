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

    [Header("Mixer")]
    [SerializeField] bool _musicOn;

    List<EventInstance> _eventInstances = new List<EventInstance>();

    EventInstance _musicEventInstance;

    FMOD.Studio.System _fmodStudioSystem;

    #region Instance

    private static AudioController _instance;

    public static AudioController Instance { get { return _instance; } }

    #endregion

    void Awake()
    {
        _instance = this;

        FMODAwake();        
    }

    void FMODAwake()
    {
        /*
        FMOD.Studio.System.create(out _fmodStudioSystem);


        ADVANCEDSETTINGS settings = new ADVANCEDSETTINGS();
        settings.commandqueuesize = 131072;
        _fmodStudioSystem.setAdvancedSettings(settings);

        //_fmodStudioSystem.initialize();
        */

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

    public EventInstance Create3DInstance(EventReference eventReference, Vector3 position)
    {
        EventInstance eventInstance = CreateInstance(eventReference);
        
        eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(position));

        return eventInstance;
    }

    public bool PlayEvent(EventInstance eventInstance, bool forcePlay = false)
    {
        eventInstance.getPlaybackState(out PLAYBACK_STATE playbackState);

        //eventInstance.getDescription(out EventDescription desc);
        //desc.getPath(out string evtPath);
        //Debug.Log($"AudioController] PlayEvent) eventInstance:{evtPath}, playbackState: {playbackState}");

        if (playbackState.Equals(PLAYBACK_STATE.STOPPED) || forcePlay)
        {
            //Debug.Log($"AudioController] PlayEvent) starting event: {evtPath}");
            eventInstance.start();
            
            return true;
        }

        return false;
    }

    public bool Play3DEvent(EventInstance eventInstance, Vector3 position, bool forcePlay = false)
    {
        eventInstance.getPlaybackState(out PLAYBACK_STATE playbackState);

        //eventInstance.getDescription(out EventDescription desc);
        //desc.getPath(out string evtPath);
        //Debug.Log($"AudioController] PlayEvent) eventInstance:{evtPath}, playbackState: {playbackState}");

        if (playbackState.Equals(PLAYBACK_STATE.STOPPED) || forcePlay)
        {
            //Debug.Log($"AudioController] PlayEvent) starting event: {evtPath}");
            eventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(position));

            eventInstance.start();
            
            return true;
        }

        return false;
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

    public void ReleaseEvent(EventInstance eventInstance)
    {
        if (!eventInstance.isValid())
        {            
            Debug.LogWarning("AudioController] ReleaseEvent) eventInstance NOT VALID");
            return;
        }
        
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

#if UNITY_EDITOR
            if (!_musicOn)
            {
                return;                
            }
#endif

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

#if UNITY_EDITOR
        if (!_musicOn)
        {
            return;
        }
#endif

        PauseEvent(_musicEventInstance);
    }

    public void GameplayIntensityUpdate(float intensity)
    {
        _musicEventInstance.setParameterByName(FMODEvents.Instance.TerrorMusicParam, intensity);
    }

    public void GameplayNearZombiesUpdate(float intensity)
    {
        _musicEventInstance.setParameterByName(FMODEvents.Instance.NearZombiesParam, intensity);
    }

    public void GameplayResume()
    {
#if UNITY_EDITOR
        if (!_musicOn)
        {
            return;
        }
#endif
        ResumeEvent(_musicEventInstance);
    }

    public void GameplayStop()
    {
#if UNITY_EDITOR
        if (!_musicOn)
        {
            return;
        }
#endif
        StopEvent(_musicEventInstance);
    }

}
