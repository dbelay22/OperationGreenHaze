using System.Collections;
using System.Collections.Generic;
using StarterAssets;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ZombieSteps : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] AudioClip[] _walkSounds;

    AudioSource _audioSource;

    bool _isWalking = false;

    int _lastWalkStepIndex = -1;
    
    void Start()
    {
        _lastWalkStepIndex = -1;
        
        _audioSource = GetComponent<AudioSource>();
    }

    public void ZombieStepAnimEvent()
    {
        if (_isWalking == false)
        {
            return;
        }

        int rndIndex;
        do
        {
            rndIndex = GetRandomArrayIndex(_walkSounds);
        }
        while (rndIndex == _lastWalkStepIndex);

        if (PlayAudioClip(_walkSounds[rndIndex]))
        {
            _lastWalkStepIndex = rndIndex;
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

        Debug.Log($"[ZSteps] (PlayAudioClip) step sound now!");

        _audioSource.PlayOneShot(clip);

        return true;
    }
}
