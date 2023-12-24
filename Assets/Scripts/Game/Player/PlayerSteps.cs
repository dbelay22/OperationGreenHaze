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
    [SerializeField] FirstPersonController _fpController;

    [Header("Health")]
    [SerializeField] PlayerHealth _playerHealth;

    [Header("Player position")]
    [SerializeField] Transform _playerPosition;

    EventInstance _footstepSFX;
    EventInstance _jumpSFX;
    EventInstance _landSFX;

    void Start()
    {
        _footstepSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.PlayerFootsteps, _playerPosition.position);
        _jumpSFX = AudioController.Instance.CreateInstance(FMODEvents.Instance.PlayerJump);
        _landSFX = AudioController.Instance.CreateInstance(FMODEvents.Instance.PlayerLand);
    }

    void Update()
    {
        if (!Game.Instance.IsGameplayOn() || _playerHealth.CurrentHealth <= 0)
        {
            return;
        }

        StepSoundUpdate();

        JumpSoundUpdate();
    }

    bool IsMoving()
    {
        return _input.move != Vector2.zero;
    }

    bool _wasRunning = false;

    void StepSoundUpdate()
    {
        bool _isPlayerHurt = _playerHealth.CurrentHealthPercentage < 0.5;

        if (IsMoving() && _fpController.Grounded)
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
            StopStepSFX();
        }
    }

    bool _isJumping = false;

    void JumpSoundUpdate()
    {
        bool justJumped = _input.jump == true && _fpController.Grounded == false;
        
        bool justGrounded = _input.jump == false && _fpController.Grounded == true;

        if (justJumped)
        {
            StopStepSFX();

            AudioController.Instance.PlayEvent(_jumpSFX, true);

            _isJumping = true;
        }
        else if (justGrounded && _isJumping)
        {
            _isJumping = false;

            int landStillMoving = IsMoving() ? 1 : 0;

            _landSFX.setParameterByName(FMODEvents.Instance.LandMovementParam, landStillMoving);

            AudioController.Instance.PlayEvent(_landSFX, true);            
        }
    }

    bool _stepLeft = true;

    void PlayStepSFX(bool isRunning)
    {
        //////////////////////////////
        // Parameters
        // walk / run 
        _footstepSFX.setParameterByName(FMODEvents.Instance.WalkRunParameter, isRunning ? 1 : 0);

        // floor material
        _footstepSFX.setParameterByName(FMODEvents.Instance.FloorMaterialParameter, FMODEvents.Instance.DefaultFloorMaterialValue);

        _footstepSFX.setParameterByName(FMODEvents.Instance.LeftRightParameter, _stepLeft ? 0 : 1);
        //////////////////////////////       
        
        bool stepPlayed = AudioController.Instance.Play3DEvent(_footstepSFX, _playerPosition.position);

        //Debug.Log($"[PlayerSteps] PlayStepSFX) Playing footstep ? stepPlayed: {stepPlayed} - running: {isRunning}, stepLeft: {_stepLeft}");

        _stepLeft = !_stepLeft;
    }   

    void OnPlayerDeath()
    {
        OnGameplayOver();
    }

    void OnGameplayOver()
    {
        StopStepSFX();
    }
    
    void StopStepSFX()
    {
        AudioController.Instance.StopEventIfPlaying(_footstepSFX);
    }
}
