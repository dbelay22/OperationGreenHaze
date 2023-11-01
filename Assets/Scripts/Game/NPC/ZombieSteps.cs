using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ZombieSteps : MonoBehaviour
{
    [Header("Maximum two (2) audio clips")]
    [SerializeField] AudioClip[] _walkSounds;

    AudioSource _audioSource;

    bool _isWalking = false;

    int _lastWalkStepIndex;
    
    void Start()
    {
        _lastWalkStepIndex = -1;
        
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayStepSFX()
    {
        if (_isWalking == false)
        {
            return;
        }

        int index = _lastWalkStepIndex == 0 ? 1 : 0;

        if (PlayAudioClip(_walkSounds[index]))
        {
            _lastWalkStepIndex = index;
        }
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
