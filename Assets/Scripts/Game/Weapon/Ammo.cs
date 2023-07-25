using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] int _ammoStart = 30;
    [SerializeField] AmmoType _ammoTypeSerialized;

    AmmoSlot _ammoSlot;
        
    public int AmmoLeft { get { return _ammoSlot.ammoAmount; } }

    private class AmmoSlot
    {
        AmmoType _ammoType;
        public AmmoType AmmoType { get { return _ammoType; } }

        public int ammoAmount;

        public AmmoSlot(AmmoType ammoType)
        {
            _ammoType = ammoType;
        }        
    }
    
    void Start()
    {
        _ammoSlot = new AmmoSlot(_ammoTypeSerialized);
        _ammoSlot.ammoAmount = _ammoStart;
    }

    public AmmoType GetAmmoType()
    {
        return _ammoSlot.AmmoType;
    }

    void OnBulletShot(int amount)
    {
        _ammoSlot.ammoAmount -= amount;

        if (_ammoSlot.ammoAmount < 0)
        {
            _ammoSlot.ammoAmount = 0;
        }

        Debug.Log($"[Ammo] ammo left: {_ammoSlot.ammoAmount}, ammo type: {_ammoSlot.AmmoType}");
    }

    public void IncreaseAmmo(int increase)
    {
        Debug.Log($"[Ammo] ammo increase: {increase}");

        _ammoSlot.ammoAmount += increase;
    }
}