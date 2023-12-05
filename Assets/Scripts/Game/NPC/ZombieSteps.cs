using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using FMOD.Studio;

[RequireComponent(typeof(AudioSource))]
public class ZombieSteps : MonoBehaviour
{
    bool _isWalking = false;

    bool _stepLeft = true;
    
    EventInstance _footstepEventInstance;


    void Update()
    {
        if (Game.Instance.IsGamePaused())
        {
            AudioController.Instance.StopEventIfPlaying(_footstepEventInstance);
        }
    }

    public void PlayStepSFX()
    {
        if (_isWalking == false)
        {
            // must be dead
            return;
        }

        //////////////////////////////
        // Parameters

        // position
        _footstepEventInstance.setParameterByName(FMODEvents.Instance.LeftRightParameter, _stepLeft ? 0 : 1);
        //////////////////////////////

        AudioController.Instance.Play3DEvent(_footstepEventInstance, transform.position);

        _stepLeft = !_stepLeft;
    }

    public void OnNPCStartWalking()
    {
        Debug.Log($"ZombieSteps] NPC Started Walking...");

        _isWalking = true;

        if (!_footstepEventInstance.isValid())
        {
            Debug.Log("[ZombieSteps] OnNPCStartWalking) footstep instance not valid, must be first time");

            _footstepEventInstance = AudioController.Instance.Create3DInstance(FMODEvents.Instance.ZombieFootsteps, transform.position);
        }
    }

    public void OnNPCStoppedWalking()
    {
        //Debug.Log($"ZombieSteps] NPC Stopped Walking...");

        _isWalking = false;

        AudioController.Instance.StopEventIfPlaying(_footstepEventInstance);
    }

    void OnDestroy()
    {
        Debug.Log($"[ZombieSteps] OnDestroy) Destroying audio instance: footstep");
        AudioController.Instance.DestroyEvent(_footstepEventInstance);    
    }
}
