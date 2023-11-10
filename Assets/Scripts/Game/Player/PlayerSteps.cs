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

    void StepSoundUpdate()
    {
        bool _isPlayerHurt = _playerHealth.CurrentHealthPercentage < 0.5;

        if (_input.move != Vector2.zero)
        {
            if (_input.sprint && _isPlayerHurt == false)
            {
                PlayStepSFX(isRunning: true);
            }
            else
            {
                PlayStepSFX(isRunning: false);
            }
        }
        else
        {
            // stop audio
            StopStepSFX();
        }
    }

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
        //////////////////////////////

        AudioController.Instance.PlayEvent(_footstepEventInstance);
    }
    
    void StopStepSFX()
    {
        AudioController.Instance.StopEvent(_footstepEventInstance);
    }
}
