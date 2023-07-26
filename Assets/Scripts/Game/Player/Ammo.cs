using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Ammo : MonoBehaviour
{
    [SerializeField] AmmoSlot[] _ammoSlots;
        
    [System.Serializable]
    private class AmmoSlot
    {
        public AmmoType _ammoType;
        public int ammoAmount;
        public AudioClip pickupSFX;
    }

    AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    AmmoSlot GetAmmoSlotOfType(AmmoType ammoType)
    {
        foreach (AmmoSlot slot in _ammoSlots)
        {
            if (slot._ammoType.Equals(ammoType))
            {
                return slot;
            }
        }

        return null;
    }

    public void OnBulletShot(AmmoType ammoType, int amount)
    {
        AmmoSlot slot = GetAmmoSlotOfType(ammoType);

        slot.ammoAmount -= amount;

        if (slot.ammoAmount < 0)
        {
            slot.ammoAmount = 0;
        }

        //Debug.Log($"[Ammo] (OnBulletShot) ammo left: {slot.ammoAmount}, ammo type: {slot._ammoType}");

        HUD.Instance.ShowAmmoAmount(slot.ammoAmount);
    }

    public void AddAmmoPickup(AmmoType ammoType, int increase)
    {
        //Debug.Log($"[Ammo] (AddAmmoPickup) type:{ammoType}, amount:{increase}");

        AmmoSlot slot = GetAmmoSlotOfType(ammoType);

        slot.ammoAmount += increase;

        _audioSource.PlayOneShot(slot.pickupSFX);

        HUD.Instance.ShowAmmoAmount(slot.ammoAmount);
    }

    public int GetAmmoLeft(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlotOfType(ammoType);

        return slot.ammoAmount;
    }

}