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
        _footstepEventInstance = AudioController.Instance.CreateInstance(FMODEvents.Instance.PlayerFootsteps);
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
        
        // position
        _footstepEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(_playerPosition.position));

        // walk / run 
        _footstepEventInstance.setParameterByName(FMODEvents.Instance.WalkRunParameter, isRunning ? 1 : 0);
        
        // floor material
        _footstepEventInstance.setParameterByName(FMODEvents.Instance.FloorMaterialParameter, FMODEvents.Instance.DefaultFloorMaterialValue);

        _footstepEventInstance.setParameterByName(FMODEvents.Instance.LeftRightParameter, _stepLeft ? 0 : 1);
        //////////////////////////////

        AudioController.Instance.PlayEvent(_footstepEventInstance);

        _stepLeft = !_stepLeft;
    }
    
    void StopStepSFX()
    {
        AudioController.Instance.StopEvent(_footstepEventInstance);
    }
}
