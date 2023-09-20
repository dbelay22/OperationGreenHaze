using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class JetWaves : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] AudioClip _raidSirenSFX;
    [SerializeField] AudioClip[] _explosions;

    AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayExplosionSFX()
    {
        int rndIndex = Random.Range(0, _explosions.Length);

        _audioSource.PlayOneShot(_explosions[rndIndex]);
    }

    public void PlaySirenSFX()
    {
        _audioSource.PlayOneShot(_raidSirenSFX);
    }

}