using FMOD.Studio;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using System.Collections;

public class AudioController : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] string _musicBusPath;
    [SerializeField] string _sfxBusPath;

    Bus _musicBus;
    Bus _sfxBus;

    List<EventInstance> _eventInstances = new List<EventInstance>();

    EventInstance _musicEventInstance;
    EventInstance _ambienceSoundscape;

    int _maxInstancesCount = 0;

    #region Instance

    private static AudioController _instance;
    
    public static AudioController Instance { get { return _instance; } }

    #endregion

    void Awake()
    {
        _instance = this;

        _maxInstancesCount = 0;

        FMODAwake();
    }

    void FMODAwake()
    {
        // get buses
        _musicBus = RuntimeManager.GetBus(_musicBusPath);        
        _sfxBus = RuntimeManager.GetBus(_sfxBusPath);
    }

    public bool SetMusicBusVolume(float volume)
    {
        return SetBusVolume(_musicBus, volume);
    }

    public bool SetSFXBusVolume(float volume)
    {
        return SetBusVolume(_sfxBus, volume);
    }

    private bool SetBusVolume(Bus bus, float volume)
    {
        FMOD.RESULT result = bus.setVolume(volume);

        if (!result.Equals(FMOD.RESULT.OK))
        {
            Debug.LogWarning($"AudioController] SetMusicBusVolume) result is NOT OK: {result}");
            return false;
        }

        return true;
    }


    #region FMOD_Events

    public EventInstance CreateInstance(EventReference eventReference) {

        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);

        _eventInstances.Add(eventInstance);

        if (_eventInstances.Count > _maxInstancesCount)
        {
            _maxInstancesCount = _eventInstances.Count;
        }

        string path = GetEventInstancePath(eventInstance);

        //Debug.Log($"AudioController] CreateInstance) NEW instance of path: {path}, list count so far: [{_eventInstances.Count}]");

        return eventInstance;
    }

    public EventInstance Create3DInstance(EventReference eventReference, Vector3 position)
    {
        EventInstance eventInstance = CreateInstance(eventReference);
        
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        return eventInstance;
    }

    public EventInstance PlayFromListOrCreate(EventReference eventReference, bool forcePlay = false)
    {
        EventInstance instance = getInstanceFromList(eventReference);

        if (!instance.isValid())
        {
            //Debug.Log($"AudioController] PlayFromListOrCreate) instance NOT FOUND in list path: {eventReference.Guid}, creating one.");
            instance = CreateInstance(eventReference);
        }
        else
        {
            //Debug.Log($"AudioController] PlayFromListOrCreate) instance FOUND in list path: {eventReference.Guid}, using it.");
        }

        PlayEvent(instance, forcePlay);

        return instance;
    }

    EventInstance getInstanceFromList(EventReference eventReference)
    {
        foreach (EventInstance eventInstance in _eventInstances)
        {
            eventInstance.getDescription(out EventDescription description);

            description.getID(out FMOD.GUID instanceGuid);

            if (instanceGuid.Equals(eventReference.Guid))
            {
                description.getPath(out string path);

                //Debug.Log($"AudioController] getInstanceFromList) found instance in list path: {path}, using it.");                

                return eventInstance;
            }
        }

        return new EventInstance();
    }

    public void PlayInstanceOrCreate(EventInstance instance, EventReference reference, out EventInstance outInstance, bool forcePlay = false)
    {
        if (instance.isValid())
{
            //Debug.Log($"AudioController] PlayInstanceOrCreate) instance is valid, playing it.");

            PlayEvent(instance, forcePlay);
            
            outInstance = instance;
        }
        else
        {
            outInstance = PlayFromListOrCreate(reference, forcePlay);

            //Debug.Log($"AudioController] PlayInstanceOrCreate) instance created/found in list, playing...");
        }
    }

    public bool PlayEvent(EventInstance eventInstance, bool forcePlay = false)
    {
        //Debug.Log($"AudioController] PlayEvent)");

        if (forcePlay || !IsEventPlaying(eventInstance))
        {
            string path = GetEventInstancePath(eventInstance);

            //Debug.Log($"AudioController] PlayEvent) starting event: {path}");

            eventInstance.start();
            
            return true;
        }

        return false;
    }

    public bool Play3DEvent(EventInstance eventInstance, Vector3 position, bool forcePlay = false)
    {
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(position));

        string path = GetEventInstancePath(eventInstance);

        if (forcePlay || !IsEventPlaying(eventInstance))
        {
            //Debug.Log($"AudioController] Play3DEvent) starting event: {path}");  

            eventInstance.start();
            
            return true;
        }

        //Debug.Log($"AudioController] PlayEvent) couldnt start event [{path}] - playbackState:{playbackState}");

        return false;
    }

    public void StopEvent(EventInstance eventInstance)
    {
        //Debug.Log($"AudioController] StopEvent) event path: {GetEventInstancePath(eventInstance)}");

        eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }

    public void StopEventIfPlaying(EventInstance eventInstance)
    {
        if (eventInstance.isValid() && IsEventPlaying(eventInstance))
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

        //Debug.Log($"AudioController] ReleaseEvent) Released event path: {path}");

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

            //Debug.Log($"AudioController] DestroyEvent) removing instance from list. path: [{path}], _eventInstances.count: {_eventInstances.Count}");
        }
        else
        {
            Debug.LogWarning($"AudioController] DestroyEvent) instance is not in list, will be invalid ??? - path: [{path}]");
        }
    }

    public string GetEventInstancePath(EventInstance instance)
    {
        instance.getDescription(out EventDescription description);

        description.getPath(out string path);

        return path;
    }

    PLAYBACK_STATE GetPlaybackState(EventInstance instance)
    {
        if (!instance.isValid())
        {
            Debug.LogWarning($"AudioController] GetPlaybackState) instance is not valid");
            return PLAYBACK_STATE.STOPPED;
        }

        instance.getPlaybackState(out PLAYBACK_STATE playbackState);

        return playbackState;
    }

    public bool IsEventPlaying(EventInstance instance)
    {
        PLAYBACK_STATE playbackState = GetPlaybackState(instance);

        return playbackState.Equals(PLAYBACK_STATE.PLAYING);
    }

    #endregion

    #region GameplayStates

    public void GameplayStart()
    {
        Debug.Log($"AudioController] GameplayStart)...");

        PlayGameplayMusic();

        PlayAmbience();
    }

    public void GameplayPause()
    {
        Debug.Log($"AudioController] GameplayPause)...");

        _musicBus.setPaused(true);
        
        _sfxBus.setPaused(true);
    }    

    public void GameplayResume()
    {
        Debug.Log($"AudioController] GameplayResume)...");

        _musicBus.setPaused(false);

        _sfxBus.setPaused(false);
    }

    public void GameplayStop()
    {
        Debug.Log($"AudioController] GameplayStop)...");

        _sfxBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

        _musicBus.stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);

        CleanUp();
    }

    public void GameplayExitFromPauseScreen()
    {
        Debug.Log($"AudioController] GameplayExitFromPauseScreen)...");

        _sfxBus.setPaused(false);
        _sfxBus.setMute(true);

        _musicBus.setPaused(false);
        _musicBus.setMute(true);

        GameplayStop();

        _sfxBus.setMute(false);
        _musicBus.setMute(false);
    }

    public void GameplayFindExit()
    {
        _musicEventInstance.setParameterByName(FMODEvents.Instance.MusicPartsParam, 1);
    }

    public void GameplayDead()
    {
        Debug.Log("AudioController] GameplayDead)...");

        int musicPart = Game.Instance.PlayerNeedsToClearExitNow() ? 3 : 2;

        _musicEventInstance.setParameterByName(FMODEvents.Instance.MusicPartsParam, musicPart);
    }

    public void GameplayIntensityUpdate(float intensity)
    {
        _musicEventInstance.setParameterByName(FMODEvents.Instance.TerrorMusicParam, intensity);
    }

    public void GameplayNearZombiesUpdate(float intensity)
    {
        _musicEventInstance.setParameterByName(FMODEvents.Instance.NearZombiesParam, intensity);
    }

    void PlayAmbience()
    {
        if (_ambienceSoundscape.isValid() && IsEventPlaying(_ambienceSoundscape))
        {
            Debug.LogWarning("AudioController] PlayAmbience) AmbienceSoundscape is already playing...");
            return;
        }

        _ambienceSoundscape = CreateInstance(FMODEvents.Instance.AmbienceSoundscape);

        PlayEvent(_ambienceSoundscape, true);
    }

    void PlayGameplayMusic()
    {
        if (_musicEventInstance.isValid() && IsEventPlaying(_musicEventInstance))
        {
            Debug.LogWarning("AudioController] PlayGameplayMusic) music is already playing...");
            return;
        }

        PlayInstanceOrCreate(_musicEventInstance, FMODEvents.Instance.GameplayMusicEvent, out _musicEventInstance, true);

        _musicEventInstance.setParameterByName(FMODEvents.Instance.MusicPartsParam, 0);

        _musicEventInstance.setParameterByName(FMODEvents.Instance.TerrorMusicParam, 0);
    }

    #endregion GameplayStates

    void CleanUp()
    {
        Debug.Log($"AudioController] CleanUp)... _eventInstances.count: {_eventInstances.Count}, max: {_maxInstancesCount}");

        // stop and release any created instances
        foreach (EventInstance eventInstance in _eventInstances)
        {
            ReleaseEvent(eventInstance);
        }

        // clear list        
        _eventInstances.Clear();

        _maxInstancesCount = 0;

        Debug.Log($"AudioController] CleanUp) END OF FUNCTION");
    }

    void OnDestroy()
    {
        CleanUp();
    }

}
