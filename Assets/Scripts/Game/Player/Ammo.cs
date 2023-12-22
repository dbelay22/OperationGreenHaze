using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] AmmoSlot[] _ammoSlots;
        
    [System.Serializable]
    private class AmmoSlot
    {
        public AmmoType _ammoType;
        public int ammoAmount;
    }

    // Accuracy
    float _ammoShooted;
    float _ammoHitEnemy;
    float _ammoHitBoombox;
    float _accuracy;

    public float Accuracy { get { return _accuracy; } }

    private void Start()
    {
        InitAccuracy();
    }

    void InitAccuracy()
    {
        _ammoShooted = 0;
        _ammoHitEnemy = 0;
        _ammoHitBoombox = 0;
        _accuracy = 0;
    }

    void UpdateAccuracy(bool hitEnemy, bool hitBoombox)
    {
        _ammoShooted++;
        
        if (hitEnemy)
        {
            _ammoHitEnemy++;
        }

        if (hitBoombox)
        {
            _ammoHitBoombox++;
        }

        _accuracy = _ammoHitEnemy + _ammoHitBoombox / _ammoShooted * 100;

        Director.Instance.OnEvent(DirectorEvents.Shot_Accuracy_Update, _accuracy);

        Debug.Log($"[Ammo] Accuracy is now {_accuracy} %");
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

    public void OnBulletShot(AmmoType ammoType, int amount, bool hitEnemy, bool hitBoombox)
    {
        // track accuracy
        UpdateAccuracy(hitEnemy, hitBoombox);

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

        // update GameUI
        GameUI.Instance.UpdateAmmoAmount(slot.ammoAmount);
    }

    public void AddAmmoPickup(AmmoType ammoType, int increase)
    {
        //Debug.Log($"[Ammo] (AddAmmoPickup) type:{ammoType}, amount:{increase}");

        AmmoSlot slot = GetAmmoSlotOfType(ammoType);

        slot.ammoAmount += increase;
    }

    public int GetAmmoLeft(AmmoType ammoType)
    {
        AmmoSlot slot = GetAmmoSlotOfType(ammoType);

        return slot.ammoAmount;
    }

    public string GetAllAmmoLeftString()
    {
        string ammoLeft = "";
        
        foreach (AmmoSlot slot in _ammoSlots)
        {
            ammoLeft += "| Type: " + slot._ammoType + ", Count:" + slot.ammoAmount + " |";
        }

        return ammoLeft;
    }

    public int GetAllAmmoLeftCount()
    {
        int ammoLeft = 0;

        foreach (AmmoSlot slot in _ammoSlots)
        {
            ammoLeft += slot.ammoAmount;
        }

        return ammoLeft;
    }

}