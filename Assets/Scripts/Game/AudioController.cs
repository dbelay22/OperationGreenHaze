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
    }

    void Start()
    {        
        FMODStart();
    }

    void FMODStart()
    {
        /*
        FMOD.RESULT createResult = FMOD.Studio.System.create(out _fmodStudioSystem);

        Debug.Log($"AudioController] FMODStart) create result: {createResult}");

        _fmodStudioSystem.getAdvancedSettings(out ADVANCEDSETTINGS fmodSettings);

        Debug.Log($"AudioController] FMODStart) commandqueuesize: {fmodSettings.commandqueuesize}, cbsize:{fmodSettings.cbsize}");

        fmodSettings.commandqueuesize = 131072;        

        FMOD.RESULT result = _fmodStudioSystem.setAdvancedSettings(fmodSettings);

        Debug.Log($"AudioController] FMODStart) AFTER SET result:{result}, commandqueuesize: {fmodSettings.commandqueuesize}");
        */

        _musicEventInstance = CreateInstance(FMODEvents.Instance.GameplayMusicEvent);
    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateInstance(EventReference eventReference)    {        

        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);

        _eventInstances.Add(eventInstance);

        Debug.Log($"AudioController] CreateInstance) NEW instance of path: {eventReference.Path}, list count so far: [{_eventInstances.Count}]");

        return eventInstance;
    }

    public EventInstance Create3DInstance(EventReference eventReference, Vector3 position)
    {
        EventInstance eventInstance = CreateInstance(eventReference);
        
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

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

        string path = GetEventInstancePath(eventInstance);

        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        if (playbackState.Equals(PLAYBACK_STATE.STOPPED) || forcePlay)
        {
            Debug.Log($"AudioController] PlayEvent) starting event: {path}");            

            eventInstance.start();
            
            return true;
        }

        //Debug.Log($"AudioController] PlayEvent) couldnt start event [{path}] - playbackState:{playbackState}");

        return false;
    }

    public void StopEvent(EventInstance eventInstance)
    {
        Debug.Log($"AudioController] StopEvent) event path: {GetEventInstancePath(eventInstance)}");

        eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void StopEventIfPlaying(EventInstance eventInstance)
    {
        eventInstance.getPlaybackState(out PLAYBACK_STATE playbackState);

        if (playbackState.Equals(PLAYBACK_STATE.PLAYING))
        {
            StopEvent(eventInstance);
        }
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

    public string ReleaseEvent(EventInstance eventInstance)
    {
        if (!eventInstance.isValid())
        {
            Debug.LogWarning($"AudioController] ReleaseEvent) eventInstance NOT VALID!");
            return "<invalid instance>";
        }

        string path = GetEventInstancePath(eventInstance);

        StopEvent(eventInstance);
        
        eventInstance.release();

        Debug.Log($"AudioController] ReleaseEvent) Released event path: {path}");

        return path;
    }

    public void DestroyEvent(EventInstance eventInstance)
    {
        if (!eventInstance.isValid())
        {
            return;
        }

        string path = ReleaseEvent(eventInstance);

        if (_eventInstances.Contains(eventInstance))
        {
            _eventInstances.Remove(eventInstance);

            Debug.Log($"AudioController] DestroyEvent) removing instance from list. path: [{path}], _eventInstances.count: {_eventInstances.Count}");
        }
        else
        {
            Debug.LogWarning($"AudioController] DestroyEvent) instance is not in list, will be invalid ??? - path: [{path}]");
        }
    }

    string GetEventInstancePath(EventInstance instance)
    {
        instance.getDescription(out EventDescription description);

        description.getPath(out string path);

        return path;
    }

    void OnDestroy()
    {
        CleanUp();
    }

    void CleanUp()
    {
        Debug.Log($"AudioController] CleanUp)... _eventInstances.count: {_eventInstances.Count}");

        // stop and release any created instances
        foreach (EventInstance eventInstance in _eventInstances)
        {
            ReleaseEvent(eventInstance);
        }
        
        // clear list        
        _eventInstances.Clear();

        // release music
        ReleaseEvent(_musicEventInstance);

        Debug.Log($"AudioController] CleanUp) END OF FUNCTION");        
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
