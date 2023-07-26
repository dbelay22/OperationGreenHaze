using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Ammo))]
public class Player : MonoBehaviour
{
    const string AMMO_PICKUP_TAG = "AmmoPickup";

    Ammo _ammo;

    void Start()
    {
        _ammo = GetComponent<Ammo>();
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

            _ammo.AddAmmoPickupToSlot(ammoPickup.AmmoType, ammoPickup.AmmoAmount);
        }
    }

}
