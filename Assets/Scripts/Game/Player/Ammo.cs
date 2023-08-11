using System;
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

    // Accuracy
    float _ammoShooted;
    float _ammoHitEnemy;
    float _accuracy;

    public float Accuracy { get { return _accuracy; } }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        InitAccuracy();
    }

    void InitAccuracy()
    {
        _ammoShooted = 0;
        _ammoHitEnemy = 0;
        _accuracy = 0;
    }

    void UpdateAccuracy(bool hitEnemy)
    {
        _ammoShooted++;
        
        if (hitEnemy)
        {
            _ammoHitEnemy++;
        }

        _accuracy = _ammoHitEnemy / _ammoShooted * 100;

        DirectorAI.Instance.OnEvent(DirectorEvent.Shot_Accuracy_Update, _accuracy);

        //Debug.Log($"[Ammo] Accuracy is now {_accuracy} %");
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

    public void OnBulletShot(AmmoType ammoType, int amount, bool hitEnemy)
    {
        // track accuracy
        UpdateAccuracy(hitEnemy);

        // get slot
        AmmoSlot slot = GetAmmoSlotOfType(ammoType);

        // reduce ammo
        slot.ammoAmount -= amount;

        // clamp to zero
        if (slot.ammoAmount < 0)
        {
            slot.ammoAmount = 0;
        }

        //Debug.Log($"[Ammo] (OnBulletShot) ammo left: {slot.ammoAmount}, ammo type: {slot._ammoType}");

        // update HUD
        HUD.Instance.UpdateAmmoAmount(slot.ammoAmount);
    }

    public void AddAmmoPickup(AmmoType ammoType, int increase)
    {
        //Debug.Log($"[Ammo] (AddAmmoPickup) type:{ammoType}, amount:{increase}");

        AmmoSlot slot = GetAmmoSlotOfType(ammoType);

        slot.ammoAmount += increase;

        _audioSource.PlayOneShot(slot.pickupSFX);
    }

    public int GetAmmoLeft(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlotOfType(ammoType);

        return slot.ammoAmount;
    }

}