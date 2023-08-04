using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ammo))]
public class Player : MonoBehaviour
{
    const string AMMO_PICKUP_TAG = "AmmoPickup";

    [SerializeField] CameraShake _cameraShake;

    Ammo _ammo;

    WeaponSwitcher _weaponSwitcher;

    Flashlight _flashlight;

    void Start()
    {
        _ammo = GetComponent<Ammo>();
        
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

        ProcessAmmoPickup(trigger);
    }

    void ProcessAmmoPickup(GameObject trigger) 
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
