using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RadioTalking : MonoBehaviour
{
    [Header("FMOD Events")]
    public EventReference Start;
    public EventReference PlayVGood;
    public EventReference PlayGood;
    public EventReference PlayBad;
    public EventReference UseMedkit;
    public EventReference UseFlashlight;
    public EventReference OutOfAmmo;
    public EventReference MissionObj1;
    public EventReference MissionObj2;
    public EventReference KillComplete;
    public EventReference FindExit;
    public EventReference TooClose;
    public EventReference Time1;
    public EventReference Time2;
    public EventReference Time3;

    [Header("Frequency")]
    [SerializeField] int _minFrequencyBetweenMessagesSeconds;

    [Header("Rating")]
    [SerializeField] int _startRatePlayingElapsedSeconds;
    [SerializeField] int _ratePlayingMaxCount;
    [SerializeField] int _ratePlayingFrequencySeconds;

    [Header("Use Medkit")]
    [SerializeField] int _minTimeBetweenMessageSeconds;

    private struct PlayingSkillValues
    {
        public EventReference EventRef;
        public float ShotAccuracy;
        public int KillCount;
        public int KillHeadshotCount;

        public PlayingSkillValues(EventReference eventRef, float shotAccuracy, int killCount, int killHeadshotCount)
        {
            EventRef = eventRef;
            ShotAccuracy = shotAccuracy;
            KillCount = killCount;
            KillHeadshotCount = killHeadshotCount;
        }
    }

    private struct IngameMessageDuration
    {
        public string MessageKey;
        public float Duration;

        public IngameMessageDuration(string key, float duration)
        {
            MessageKey = key;
            Duration = duration;
        }
    }

    List<PlayingSkillValues> SkillMessages;

    Dictionary<EventReference, IngameMessageDuration> AudioTextMessage;

    EventInstance _currentMessage;

    int _ratePlayingCount = 0;
    
    float _lastRatedTimeSeconds = 0f;

    bool _isPlayingMessage = false;

    float _lastMessageTimeSeconds = -1f;

    bool _alreadyRateVGood = false;

    float _lastUseMedkitTime = 0f;

    bool _needToShowIGMessages = false;

    #region Instance

    private static RadioTalking _instance;

    public static RadioTalking Instance { get { return _instance; } }

    #endregion

    void Awake()
    {
        _instance = this;

        SkillMessages = new List<PlayingSkillValues>()
        {
            { new PlayingSkillValues(Instance.PlayVGood, 0.6f, 10, 3) },
            { new PlayingSkillValues(Instance.PlayGood, 0.4f, 5, 0) },
            { new PlayingSkillValues(Instance.PlayBad, 0f, 0, 0) },
        };

        AudioTextMessage = new Dictionary<EventReference, IngameMessageDuration>()
        {
            { Instance.Start, new IngameMessageDuration("ig_radio_start", 5f) },
            { Instance.PlayVGood, new IngameMessageDuration("ig_radio_playvgood", 5f) },
            { Instance.PlayGood, new IngameMessageDuration("ig_radio_playgood", 5f) },
            { Instance.PlayBad, new IngameMessageDuration("ig_radio_playbad", 5f) },
            { Instance.TooClose, new IngameMessageDuration("ig_radio_tooclose", GameUI.LIFETIME_INFINITE) }
        };

        _ratePlayingCount = 0;
        
        _lastRatedTimeSeconds = 0f;

        _isPlayingMessage = false;

        _alreadyRateVGood = false;

        _lastMessageTimeSeconds = -1f;

        _lastUseMedkitTime = 0f;

        _needToShowIGMessages = Localization.Instance.CurrentLanguage.Equals(SystemLanguage.English);
    }

    void Update()
    {
        CurrentMessagePlayingUpdate();
    }

    void CurrentMessagePlayingUpdate()
    {
        if (_isPlayingMessage && _currentMessage.isValid())
        {
            _isPlayingMessage = AudioController.Instance.IsEventPlaying(_currentMessage);

            if (!_isPlayingMessage)
            {
                // just stopped
                _currentMessage = new EventInstance();
            }
        }
    }

    
    public void ProcessRatePlaying()
    {
        if (_ratePlayingCount > _ratePlayingMaxCount)
        {
            // max count reached
            Debug.Log($"RadioTalkin] ProcessRatePlaying) Max rate playing count reached:{_ratePlayingMaxCount}");
            return;
        }

        int elapsed = (int) Math.Floor(GameUI.Instance.ElapsedSeconds);

        //Debug.Log($"ProcessRatePlaying) elapsed:{elapsed}, _ratePlayingElapsedSeconds:{_ratePlayingElapsedSeconds}");

        if (elapsed < _startRatePlayingElapsedSeconds)
        {
            // not yet
            return;
        }

        float elapsedSecondsSinceLastRating = Time.time - _lastRatedTimeSeconds;

        if (elapsedSecondsSinceLastRating < _ratePlayingFrequencySeconds)
        {
            // let's wait a little
            return;
        }

        Debug.Log($"RadioTalkin] ProcessRatePlaying) _ratePlayingCount:{_ratePlayingCount} auditing now...");

        foreach (PlayingSkillValues skillValues in SkillMessages)
        {
            bool gotSKill = Director.Instance._shotAccuracy >= skillValues.ShotAccuracy && 
                Director.Instance._enemyKillCount >= skillValues.KillCount && 
                Director.Instance._enemyKillByHeadshotCount >= skillValues.KillHeadshotCount;

            Debug.Log($"RadioTalking] Evaluating skill: {skillValues.EventRef}, gotSKill: {gotSKill}");

            if (gotSKill)
            {
                EventReference ratingMessage = skillValues.EventRef;

                if (ratingMessage.Equals(Instance.PlayVGood))
                {
                    // NATURAL TALENT!

                    if (_alreadyRateVGood)
                    {
                        // don't be so cheesy
                        ratingMessage = Instance.PlayGood;
                    }
                    else
                    {
                        _alreadyRateVGood = true;
                    }                    
                }

                PlayMessage(ratingMessage);
                
                _ratePlayingCount++;

                _lastRatedTimeSeconds = Time.time;

                break;
            }
        }
    } 

    public void PlayUseMedkit()
    {
        float elapsedSecondsFromLast = Time.time - _lastUseMedkitTime;

        bool noMedkitPickedUP = Director.Instance._playerPickupMedkitCount < 1;

        bool firstBadlyHurt = _lastUseMedkitTime < 1f;

        bool needAnotherWarning = !firstBadlyHurt && noMedkitPickedUP;

        bool isTimeOfAnotherWarning = needAnotherWarning && elapsedSecondsFromLast > _minTimeBetweenMessageSeconds;        

        bool shouldPlayMessage = firstBadlyHurt || isTimeOfAnotherWarning;

        Debug.Log($"PlayUseMedkit) elapsedSecondsFromLast: {elapsedSecondsFromLast}, noMedkitPickedUP: {noMedkitPickedUP}, firstBadlyHurt:{firstBadlyHurt}, needAnotherWarning: {needAnotherWarning}, isTimeOfAnotherWarning:{isTimeOfAnotherWarning}, shouldPlayMessage:{shouldPlayMessage}");

        if (shouldPlayMessage)
        {
            PlayMessage(Instance.UseMedkit);

            _lastUseMedkitTime = Time.time;
        }
    }

    

    public void PlayMissionMessage(EventReference eventReference)
    {
        PlayMessage(eventReference, maxPriority: true);
    }


    public void PlayMessage(EventReference eventRef, bool maxPriority = false)
    {
        if (maxPriority)
        {
            AudioController.Instance.StopEventIfPlaying(_currentMessage);
        }
        else
        {
            float elapsed = Time.time - _lastMessageTimeSeconds;

            if (_lastMessageTimeSeconds > 0 && elapsed < _minFrequencyBetweenMessagesSeconds)
            {
                Debug.LogWarning($"RadioTalking] Sorry, another message played soon: {elapsed} seconds. Max: {_minFrequencyBetweenMessagesSeconds}");
                return;
            }

            if (_isPlayingMessage)
            {
                Debug.LogWarning($"RadioTalking] Already playing, missed message: {eventRef}");
                return;
            }
        }
        
        _isPlayingMessage = true;        

        AudioController.Instance.PlayInstanceOrCreate(_currentMessage, eventRef, out _currentMessage, true);

        Debug.LogWarning($"RadioTalking] PlayMessage) _currentMessage: {AudioController.Instance.GetEventInstancePath(_currentMessage)}");

        _lastMessageTimeSeconds = Time.time;

        if (_needToShowIGMessages)
        {
            ShowInGameText(eventRef, maxPriority);
        }        
    }

    void ShowInGameText(EventReference eventRef, bool maxPriority = false)
    {
        Debug.Log($"RadioTalking] ShowInGameText) eventRef:{eventRef.Path}");

        AudioTextMessage.TryGetValue(eventRef, out IngameMessageDuration igMessage);

        Debug.Log($"RadioTalking] ShowInGameText) igMessage key is null: {igMessage.MessageKey == null}");

        if (igMessage.MessageKey == null || igMessage.MessageKey.Length < 1)
        {
            Debug.LogError($"There's no in-game-message text key for event path: {eventRef.Path}");
            return;
        }

        GameUI.Instance.ShowInGameMessage(igMessage.MessageKey, igMessage.Duration, maxPriority);
    }

}