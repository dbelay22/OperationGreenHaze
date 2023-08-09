using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ammo))]
[RequireComponent(typeof(PlayerHealth))]
public class Player : MonoBehaviour
{
    const string AMMO_PICKUP_TAG = "AmmoPickup";
    const string MEDKIT_PICKUP_TAG = "MedkitPickup";
    
    [Header("Camera Shake")]
    [SerializeField] CameraShake _cameraShake;

    [Header("SFX")]
    [SerializeField] AudioClip _pickupSFX;

    Ammo _ammo;

    WeaponSwitcher _weaponSwitcher;

    Flashlight _flashlight;

    PlayerHealth _playerHealth;

    AudioSource _audioSource;

    void Start()
    {
        _ammo = GetComponent<Ammo>();

        _playerHealth = GetComponent<PlayerHealth>();

        _audioSource = GetComponent<AudioSource>();

        _weaponSwitcher = GetComponentInChildren<WeaponSwitcher>();
        
        _flashlight = GetComponentInChildren<Flashlight>();
    }

    #region Shooting
    
    public void OnBulletShot(AmmoType ammoType, int amount, bool hitEnemy)
    {
        WeaponShakeData.ShakeProperties shakeProperties = _weaponSwitcher.GetCurrentShakeProperties();
        _cameraShake.Shake(shakeProperties);

        // Notify Ammo-slots manager
        _ammo.OnBulletShot(ammoType, amount, hitEnemy);
    }

    #endregion


    #region Pickups

    void OnTriggerEnter(Collider other)
    {
        GameObject trigger = other.gameObject;

        if (ProcessAmmoPickup(trigger))
        {
        }
        else if (ProcessMedkitPickup(trigger))
        { 
        }        
    }

    bool ProcessMedkitPickup(GameObject trigger)
    {
        bool isMedkit = trigger.CompareTag(MEDKIT_PICKUP_TAG);

        if (isMedkit)
        {
            MedkitPickup medkit = trigger.GetComponent<MedkitPickup>();

            // sound!
            _audioSource.PlayOneShot(_pickupSFX);

            // improve health
            _playerHealth.ImproveByPickup(medkit.HealthAmount);

            // bye pickup
            Destroy(trigger);
        }

        return isMedkit;
    }

    bool ProcessAmmoPickup(GameObject trigger) 
    {
        bool isAmmoPickup = trigger.CompareTag(AMMO_PICKUP_TAG);

        if (isAmmoPickup)
        {
            AmmoPickup ammoPickup = trigger.GetComponent<AmmoPickup>();

            // add ammo to slot
            _ammo.AddAmmoPickup(ammoPickup.AmmoType, ammoPickup.AmmoAmount);

            // bye pickup
            Destroy(trigger);

            // update HUD if current ammo type
            Weapon currentWeapon = _weaponSwitcher.GetCurrentWeapon();
            if (ammoPickup.AmmoType.Equals(currentWeapon.GetAmmoType()))
            {
                HUD.Instance.UpdateAmmoAmount(currentWeapon.GetAmmoLeft());
            }
            
        }

        return isAmmoPickup;
    }

    #endregion

    public bool IsFlashlightOnAndCanBlind()
    {
        if (_flashlight == null)
        {
            return false;
        }

        return _flashlight.IsOnAndCanBlind();
    }

    void OnPlayerDeath()
    {
        GameplayIsOver();
    }

    public void GameplayIsOver()
    {
        // turn of flashlight
        _flashlight.TurnOff();

        // hide weapons
        _weaponSwitcher.HideCurrentWeapon();

        // bye player ?
        Destroy(gameObject);
    }

}
