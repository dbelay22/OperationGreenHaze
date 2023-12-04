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

    EventInstance _footstepEventInstance;
    
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
                
        _footstepEventInstance = AudioController.Instance.Create3DInstance(FMODEvents.Instance.ZombieFootsteps, transform.position);
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
        _footstepEventInstance.setParameterByName(FMODEvents.Instance.LeftRightParameter, _stepLeft ? 0 : 1);
        //////////////////////////////

        AudioController.Instance.Play3DEvent(_footstepEventInstance, transform.position);

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

    void OnDestroy()
    {
        AudioController.Instance.ReleaseEvent(_footstepEventInstance);    
    }
}
