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
    const string FLASHLIGHT_PICKUP_TAG = "FlashlightPickup";
    const string MISSION_PICKUP_TAG = "MissionPickup";

    [Header("Camera Shake")]
    [SerializeField] CameraShake _cameraShake;

    [Header("SFX")]
    [SerializeField] AudioClip _pickupSFX;
    [SerializeField] AudioClip _errorSFX;

    Ammo _ammo;

    WeaponSwitcher _weaponSwitcher;

    Flashlight _flashlight;

    PlayerHealth _playerHealth;

    AudioSource _audioSource;


    #region Instance

    private static Player _instance;


    public static Player Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion

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
            Director.Instance.OnEvent(DirectorEvents.Player_Pickup_Ammo);
        }
        else if (ProcessMedkitPickup(trigger))
        {
            Director.Instance.OnEvent(DirectorEvents.Player_Pickup_Medkit);
        }
        else if (ProcessFlashlightPickup(trigger))
        {
            Director.Instance.OnEvent(DirectorEvents.Player_Pickup_Flashlight);
        }
        else if (ProcessMissionPickup(trigger))
        {
            Director.Instance.OnEvent(DirectorEvents.Player_Pickup_Mission);
        }
    }

    bool ProcessMissionPickup(GameObject trigger)
    {
        bool isMissionPickup = trigger.CompareTag(MISSION_PICKUP_TAG);

        if (isMissionPickup)
        {
            // sound!
            _audioSource.PlayOneShot(_pickupSFX);

            MissionSystem.Instance.OnMissionItemPickup(trigger);

            Destroy(trigger);

            return true;
        }

        return false;
    }

    bool ProcessFlashlightPickup(GameObject trigger)
    {
        bool isFlashlight = trigger.CompareTag(FLASHLIGHT_PICKUP_TAG);

        if (isFlashlight)
        {
            // sound!
            _audioSource.PlayOneShot(_pickupSFX);

            _flashlight.ReportPickUp();

            // bye pickup
            Destroy(trigger);
        }

        return isFlashlight;
    }

    bool ProcessMedkitPickup(GameObject trigger)
    {
        bool isMedkit = trigger.CompareTag(MEDKIT_PICKUP_TAG);

        if (isMedkit)
        {
            if (_playerHealth.CurrentHealthPercentage < 1)
            {
                MedkitPickup medkit = trigger.GetComponent<MedkitPickup>();

                // sound!
                _audioSource.PlayOneShot(_pickupSFX);

                // improve health
                _playerHealth.ImproveByPickup(medkit.HealthAmount);

                // bye pickup
                Destroy(trigger);

                return true;
            }
            else
            {
                _audioSource.PlayOneShot(_errorSFX);
                return false;
            }
        }

        return false;
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

            // update GameUI if current ammo type
            Weapon currentWeapon = _weaponSwitcher.GetCurrentWeapon();
            if (ammoPickup.AmmoType.Equals(currentWeapon.GetAmmoType()))
            {
                GameUI.Instance.UpdateAmmoAmount(currentWeapon.GetAmmoLeft());
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

    public string GetUnusedAmmo()
    {
        return _ammo.GetAllAmmoLeftString();
    }

    public float GetCurrentHealth()
    {
        return _playerHealth.CurrentHealth;
    }

}
