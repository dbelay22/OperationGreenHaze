using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    const string AMMO_PICKUP_TAG = "AmmoPickup";
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
            WeaponSwitcher weaponSwitcher = GetComponentInChildren<WeaponSwitcher>();

            AmmoPickup ammoPickup = trigger.GetComponent<AmmoPickup>();

            // Tell the switcher to add this ammo type to the corresponding slot
            weaponSwitcher.AddAmmoPickupToSlot(ammoPickup.AmmoType, ammoPickup.AmmoAmount, ammoPickup);
        }
    }
}
