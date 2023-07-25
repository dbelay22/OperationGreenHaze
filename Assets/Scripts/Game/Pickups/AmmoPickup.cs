using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AmmoPickup : MonoBehaviour
{
    [SerializeField] AmmoType _ammoType;
    public AmmoType AmmoType { get { return _ammoType; } }

    [SerializeField] int _ammoAmount;
    public int AmmoAmount { get { return _ammoAmount; } }

    [SerializeField] AudioClip _pickupSFX;

    AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    public void PlayPickupSFX() 
    {
        _audioSource.PlayOneShot(_pickupSFX);
    }
}
