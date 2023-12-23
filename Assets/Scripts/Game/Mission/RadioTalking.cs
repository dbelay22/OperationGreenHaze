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
    
    bool _isRadioDisabled = false;

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
            { Instance.UseMedkit, new IngameMessageDuration("ig_radio_medkit", 4f) },
            { Instance.UseFlashlight, new IngameMessageDuration("ig_radio_flashlight", 4f) },
            { Instance.OutOfAmmo, new IngameMessageDuration("ig_radio_outofammo", 4f) },
            { Instance.MissionObj1, new IngameMessageDuration("ig_radio_missionobj1", 5f) },
            { Instance.MissionObj2, new IngameMessageDuration("ig_radio_missionobj2", 5f) },
            { Instance.KillComplete, new IngameMessageDuration("ig_radio_killcomplete", 5f) },
            { Instance.FindExit, new IngameMessageDuration("ig_radio_findexit", 10f) },
            { Instance.TooClose, new IngameMessageDuration("ig_radio_tooclose", GameUI.LIFETIME_INFINITE) },
            { Instance.Time1, new IngameMessageDuration("ig_radio_time1", 5f) },
            { Instance.Time2, new IngameMessageDuration("ig_radio_time2", 5f) },
            { Instance.Time3, new IngameMessageDuration("ig_radio_time3", 5f) }
        };

        _ratePlayingCount = 0;
        
        _lastRatedTimeSeconds = 0f;

        _isPlayingMessage = false;

        _alreadyRateVGood = false;

        _lastMessageTimeSeconds = -1f;

        _lastUseMedkitTime = 0f;

        _needToShowIGMessages = Localization.Instance.CurrentLanguage.Equals(SystemLanguage.English);

        _isRadioDisabled = false;
    }

    void Update()
    {
        CheckCurrentMessageState();
    }

    void CheckCurrentMessageState()
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
        if (_isRadioDisabled)
        {
            return;
        }

        if (_ratePlayingCount > _ratePlayingMaxCount)
        {
            // max count reached
            Debug.LogWarning($"RadioTalking] ProcessRatePlaying) Max rate playing count reached:{_ratePlayingMaxCount}");
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

        /*
        Debug.Log($"RadioTalking] ProcessRatePlaying) _ratePlayingCount:{_ratePlayingCount} auditing now...");        
        Debug.LogWarning($"RadioTalking] ProcessRatePlaying) accuracy:{Director.Instance._shotAccuracy}");
        Debug.LogWarning($"RadioTalking] ProcessRatePlaying) _enemyKillCount:{Director.Instance._enemyKillCount}");
        Debug.LogWarning($"RadioTalking] ProcessRatePlaying) _enemyKillByHeadshotCount:{Director.Instance._enemyKillByHeadshotCount}");
        */

        foreach (PlayingSkillValues skillValues in SkillMessages)
        {
            bool gotSKill = Director.Instance._shotAccuracy >= skillValues.ShotAccuracy && 
                Director.Instance._enemyKillCount >= skillValues.KillCount && 
                Director.Instance._enemyKillByHeadshotCount >= skillValues.KillHeadshotCount;

            //Debug.Log($"RadioTalking] Evaluating skill: {skillValues.EventRef}, gotSKill: {gotSKill}");

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

    public void PlayUseMedkit(bool maxPriority = false)
    {
        if (_isRadioDisabled)
        {
            return;
        }

        bool firstBadlyHurt = _lastUseMedkitTime < 1f;

        bool noMedkitPickedUP = Director.Instance._playerPickupMedkitCount < 1;

        float elapsedSecondsFromLast = Time.time - _lastUseMedkitTime;

        bool isTimeOfAnotherWarning = elapsedSecondsFromLast > _minTimeBetweenMessageSeconds;
                
        bool needAnotherWarning = maxPriority || (noMedkitPickedUP && !firstBadlyHurt);

        bool shouldPlayMessage = firstBadlyHurt || (needAnotherWarning && isTimeOfAnotherWarning);
        
        /*
        Debug.Log($"RadioTalking] PlayUseMedkit)----------------------------------------------------------------------------------------------------------------");
        Debug.Log($"RadioTalking] PlayUseMedkit) firstBadlyHurt: {firstBadlyHurt}, noMedkitPickedUP:{noMedkitPickedUP}, isTimeOfAnotherWarning:{isTimeOfAnotherWarning}, elapsedSecondsFromLast:{elapsedSecondsFromLast}, needAnotherWarning: {needAnotherWarning}");
        Debug.Log($"RadioTalking] PlayUseMedkit) isTimeOfAnotherWarning:{isTimeOfAnotherWarning}, elapsedSecondsFromLast:{elapsedSecondsFromLast}");
        Debug.Log($"RadioTalking] PlayUseMedkit) needAnotherWarning: {needAnotherWarning}, shouldPlayMessage:{shouldPlayMessage}, maxPriority:{maxPriority}");
        */

        if (shouldPlayMessage)
        {
            PlayMessage(Instance.UseMedkit, maxPriority);

            _lastUseMedkitTime = Time.time;
        }
    }   

    public void PlayMissionMessage(EventReference eventReference)
    {
        PlayMessage(eventReference, maxPriority: true);
    }

    public void PlayMessage(EventReference eventRef, bool maxPriority = false)
    {
        if (_isRadioDisabled)
        {
            return;
        }

        if (maxPriority)
        {
            //Debug.Log($"RadioTalking] PlayMessage) maxPriority!");

            StopAllMessagesNow();
        }
        else
        {
            if (_isPlayingMessage)
            {
                //Debug.LogWarning($"RadioTalking] Already playing, missed message: {eventRef}");
                return;
            }
            float elapsed = Time.time - _lastMessageTimeSeconds;

            if (_lastMessageTimeSeconds > 0 && elapsed < _minFrequencyBetweenMessagesSeconds)
            {
                //Debug.LogWarning($"RadioTalking] PlayMessage) Sorry, another message played soon: {elapsed} seconds. Min frequency is: {_minFrequencyBetweenMessagesSeconds}");
                return;
            }       
        }
        
        _isPlayingMessage = true;

        _currentMessage = AudioController.Instance.CreateInstance(eventRef);
        
        AudioController.Instance.PlayEvent(_currentMessage);

        Debug.Log($"RadioTalking] PlayMessage) _currentMessage: {AudioController.Instance.GetEventInstancePath(_currentMessage)}");

        _lastMessageTimeSeconds = Time.time;

        if (_needToShowIGMessages)
        {
            ShowInGameText(eventRef, maxPriority);
        }        
    }

    void ShowInGameText(EventReference eventRef, bool maxPriority = false)
    {
        if (_isRadioDisabled)
        {
            return;
        }

#if UNITY_EDITOR
        //Debug.Log($"RadioTalking] ShowInGameText) eventRef:{eventRef.Path}");
#endif

        AudioTextMessage.TryGetValue(eventRef, out IngameMessageDuration igMessage);

        //Debug.Log($"RadioTalking] ShowInGameText) igMessage key is null: {igMessage.MessageKey == null}");

        if (igMessage.MessageKey == null || igMessage.MessageKey.Length < 1)
        {
#if UNITY_EDITOR
            Debug.LogError($"RadioTalking] ShowInGameText) There's no in-game-message text key for event path: {eventRef.Path}");
#endif
            return;
        }

        GameUI.Instance.ShowInGameMessage(igMessage.MessageKey, igMessage.Duration, maxPriority);
    }

    public void StopAllMessagesNow()
    {
        //Debug.Log($"RadioTalking] StopAllMessagesNow)...");

        AudioController.Instance.StopEvent(_currentMessage);
    }

    public void ShutDown()
    {
        //Debug.Log($"RadioTalking] Shutdown)...");

        StopAllMessagesNow();

        _isRadioDisabled = true;
    }


}