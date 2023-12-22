using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayingSkillValues
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

    [Header("Rating")]
    [SerializeField] int _ratePlayingElapsedSeconds;
    [SerializeField] int _ratePlayingMaxCount;
    [SerializeField] int _ratePlayingFrequencySeconds;

    [Header("Use Medkit")]
    [SerializeField] int _minTimeBetweenMessageSeconds;

    List<PlayingSkillValues> SkillMessages;

    int _ratePlayingCount = 0;
    
    float _lastRatedTimeSeconds = 0f;

    bool _isPlayingMessage = false;

    #region Instance

    private static RadioTalking _instance;

    public static RadioTalking Instance { get { return _instance; } }

    #endregion

    void Awake()
    {
        _instance = this;

        SkillMessages = new List<PlayingSkillValues>()
        {
            { new PlayingSkillValues(Instance.PlayVGood, 60, 10, 5) },
            { new PlayingSkillValues(Instance.PlayGood, 40, 5, 1) },
            { new PlayingSkillValues(Instance.PlayBad, 0, 0, 0) },
        };

        _ratePlayingCount = 0;
        
        _lastRatedTimeSeconds = 0f;

        _isPlayingMessage = false;
    }

    float _lastMessageTimeSeconds = 0f;

    void Update()
    {
        if (_isPlayingMessage)
        {
            if (_currentMessage.isValid())
            {
                _isPlayingMessage = AudioController.Instance.IsEventPlaying(_currentMessage);

                bool justStopped = !_isPlayingMessage;

                if (justStopped)
                {
                    _currentMessage = new EventInstance();
                }
            }
        }
    }    

    public void ProcessRatePlaying()
    {
        Debug.Log($"ProcessRatePlaying) _ratePlayingCount:{_ratePlayingCount}");

        if (_ratePlayingCount > _ratePlayingMaxCount)
        {
            // max count reached
            return;
        }

        int elapsed = (int) Math.Floor(GameUI.Instance.ElapsedSeconds);

        //Debug.Log($"ProcessRatePlaying) elapsed:{elapsed}, _ratePlayingElapsedSeconds:{_ratePlayingElapsedSeconds}");

        if (elapsed < _ratePlayingElapsedSeconds)
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

        Debug.Log($"ProcessRatePlaying) Auditing now...");

        foreach (PlayingSkillValues skillValues in SkillMessages)
        {
            bool gotSKill = Director.Instance._shotAccuracy >= skillValues.ShotAccuracy && 
                Director.Instance._enemyKillCount >= skillValues.KillCount && 
                Director.Instance._enemyKillByHeadshotCount >= skillValues.KillHeadshotCount;

            Debug.Log($"RadioTalking] Evaluating skill: {skillValues.EventRef}, gotSKill: {gotSKill}");

            if (gotSKill)
            {
                PlayMessage(skillValues.EventRef);
                
                _ratePlayingCount++;

                _lastRatedTimeSeconds = Time.time;

                break;
            }
        }
    }


    float _lastUseMedkitTime = 0f;

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

    EventInstance _currentMessage;

    public void PlayMessage(EventReference eventRef)
    {
        float elapsed = Time.time - _lastMessageTimeSeconds;

        if (elapsed < 22)
        {
            Debug.LogWarning($"RadioTalking] Sorry, another message played soon: {elapsed} seconds");
            return;
        }

        if (_isPlayingMessage)
        {
            Debug.LogWarning($"RadioTalking] Already playing, missed message: {eventRef}");
            return;
        }

        _isPlayingMessage = true;

        AudioController.Instance.PlayInstanceOrCreate(_currentMessage, eventRef, out _currentMessage, true);

        _lastMessageTimeSeconds = Time.time;
    }

}