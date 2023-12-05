using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using FMOD.Studio;
using UnityEngine.Rendering;

public class PlayerSteps : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] StarterAssetsInputs _input;

    [Header("Health")]
    [SerializeField] PlayerHealth _playerHealth;

    [Header("Player position")]
    [SerializeField] Transform _playerPosition;

    EventInstance _footstepEventInstance;

    void Start()
    {
        _footstepEventInstance = AudioController.Instance.Create3DInstance(FMODEvents.Instance.PlayerFootsteps, _playerPosition.position);
    }

    void Update()
    {
        if (Game.Instance.IsGameplayOn() == false)
        {
            return;
        }

        StepSoundUpdate();
    }

    bool _wasRunning = false;

    void StepSoundUpdate()
    {
        bool _isPlayerHurt = _playerHealth.CurrentHealthPercentage < 0.5;

        if (_input.move != Vector2.zero)
        {
            if (_input.sprint && _isPlayerHurt == false)
            {
                if (!_wasRunning)
                {
                    StopStepSFX();
                    _wasRunning = true;
                }
                PlayStepSFX(isRunning: true);
            }
            else
            {
                if (_wasRunning)
                {
                    StopStepSFX();
                    _wasRunning = false;
                }
                PlayStepSFX(isRunning: false);
            }
        }
        else
        {
            // stop audio
            StopStepSFX();
        }
    }

    bool _stepLeft = true;

    void PlayStepSFX(bool isRunning)
    {
        //////////////////////////////
        // Parameters
        // walk / run 
        _footstepEventInstance.setParameterByName(FMODEvents.Instance.WalkRunParameter, isRunning ? 1 : 0);

        // floor material
        _footstepEventInstance.setParameterByName(FMODEvents.Instance.FloorMaterialParameter, FMODEvents.Instance.DefaultFloorMaterialValue);

        _footstepEventInstance.setParameterByName(FMODEvents.Instance.LeftRightParameter, _stepLeft ? 0 : 1);
        //////////////////////////////       
        
        bool stepPlayed = AudioController.Instance.Play3DEvent(_footstepEventInstance, _playerPosition.position);

        //Debug.Log($"[PlayerSteps] PlayStepSFX) Playing footstep ? stepPlayed: {stepPlayed} - running: {isRunning}, stepLeft: {_stepLeft}");

        _stepLeft = !_stepLeft;
    }

    

    void OnPlayerDeath()
    {
        Debug.Log("PlayerSteps] OnPlayerDeath) Stopping step sfx");
        StopStepSFX();
    }
    
    void StopStepSFX()
    {
        AudioController.Instance.StopEventIfPlaying(_footstepEventInstance);
    }
}
