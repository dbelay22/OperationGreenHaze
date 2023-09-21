using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSteps : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] StarterAssetsInputs _input;

    [Header("Health")]
    [SerializeField] PlayerHealth _playerHealth;


    [Header("SFX")]
    [SerializeField] AudioClip[] _walkHurtSounds;
    [SerializeField] AudioClip[] _walkSounds;
    [SerializeField] AudioClip[] _sprintSounds;

    AudioSource _audioSource;

    int _lastWalkStepIndex = -1;
    int _lastSprintStepIndex = -1;

    void Start()
    {
        _lastWalkStepIndex = -1;
        _lastSprintStepIndex = 1;

        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Game.Instance.IsGameplayOn() == false)
        {
            return;
        }

        StepSoundUpdate();
    }

    void StepSoundUpdate()
    {
        bool _isPlayerHurt = _playerHealth.CurrentHealthPercentage < 0.5;

        if (_input.move != Vector2.zero)
        {
            if (_input.sprint && _isPlayerHurt == false)
            {
                PlaySprintSound();
            }
            else
            {
                PlayWalkSound();
            }
        }
        else
        {
            _audioSource.Stop();
            
            _lastSprintStepIndex = -1;
            
            _lastWalkStepIndex = -1;
        }
    }

    void PlayWalkSound()
    {
        int rndIndex;
        AudioClip[] soundsArray = _playerHealth.CurrentHealthPercentage < 0.5 ? _walkHurtSounds : _walkSounds;
        do
        {
            rndIndex = GetRandomArrayIndex(soundsArray);
        }
        while (rndIndex == _lastWalkStepIndex);

        //Debug.Log($"[Player] (Update) moving:{_input.move} sound rndIndex:{rndIndex}, _lastStepIndex:{_lastWalkStepIndex}");

        if (PlayAudioClip(soundsArray[rndIndex]))
        {
            _lastWalkStepIndex = rndIndex;
        }
    }

    void PlaySprintSound()
    {
        int rndIndex;
        do
        {
            rndIndex = GetRandomArrayIndex(_sprintSounds);
        }
        while (rndIndex == _lastSprintStepIndex);

        if (PlayAudioClip(_sprintSounds[rndIndex]))
        {
            _lastSprintStepIndex = rndIndex;
        }
    }

    int GetRandomArrayIndex(AudioClip[] sounds)
    {
        return Random.Range(0, sounds.Length);
    }

    bool PlayAudioClip(AudioClip clip)
    {
        if (_audioSource.isPlaying)
        {
            return false;
        }

        _audioSource.PlayOneShot(clip);

        return true;
    }
}
