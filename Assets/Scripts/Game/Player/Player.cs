using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ammo))]
public class Player : MonoBehaviour
{
    const string AMMO_PICKUP_TAG = "AmmoPickup";

    Ammo _ammo;

    WeaponSwitcher _weaponSwitcher;

    Flashlight _flashlight;

    void Start()
    {
        _ammo = GetComponent<Ammo>();
        _weaponSwitcher = GetComponentInChildren<WeaponSwitcher>();
        _flashlight = GetComponentInChildren<Flashlight>();
    }


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

    public bool IsFlashlightOnAndCanBlind()
    {
        if (_flashlight == null)
        {
            return false;
        }

        return _flashlight.IsOnAndCanBlind();
    }

}
