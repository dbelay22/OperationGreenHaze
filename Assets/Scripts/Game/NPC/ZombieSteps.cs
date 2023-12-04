using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;
using FMOD.Studio;

[RequireComponent(typeof(AudioSource))]
public class ZombieSteps : MonoBehaviour
{
    [Header("Maximum two (2) audio clips")]
    [SerializeField] AudioClip[] _walkSounds;

    AudioSource _audioSource;

    bool _isWalking = false;

    int _lastWalkStepIndex;

    EventInstance _footstepEventInstance;
    
    void Start()
    {
        _lastWalkStepIndex = -1;
        
        _audioSource = GetComponent<AudioSource>();
                
        _footstepEventInstance = AudioController.Instance.CreateInstance(FMODEvents.Instance.ZombieFootsteps);
        _footstepEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));
    }

    bool _stepLeft = true;

    public void PlayStepSFX()
    {
        if (_isWalking == false)
        {
            return;
        }

        //////////////////////////////
        // Parameters

        // position
        _footstepEventInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform.position));

        _footstepEventInstance.setParameterByName(FMODEvents.Instance.LeftRightParameter, _stepLeft ? 0 : 1);
        //////////////////////////////

        AudioController.Instance.PlayEvent(_footstepEventInstance);

        _stepLeft = !_stepLeft;
    }

    public void OnNPCStartWalking()
    {
        _isWalking = true;
    }

    public void OnNPCStoppedWalking()
    {
        _isWalking = false;
        
        _audioSource.Stop();
        
        _lastWalkStepIndex = -1;
    }

    bool PlayAudioClip(AudioClip clip)
    {
        if (_audioSource.isPlaying || Game.Instance.IsGamePlayOver())
        {
            return false;
        }

        _audioSource.PlayOneShot(clip);

        return true;
    }
}
