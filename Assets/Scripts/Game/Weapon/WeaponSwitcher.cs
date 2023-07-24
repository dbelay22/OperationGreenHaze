using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] int _currentWeaponIdx = 0;
    
    Weapon[] _weapons;

    void Start()
    {
        _weapons = GetComponentsInChildren<Weapon>();
        SetCurrentWeaponActive();
    }

    void Update()
    {
        float mouseScrollY = Mouse.current.scroll.ReadValue().y;

        if (mouseScrollY != 0)
        {
            CycleToNextWeapon();
        }
    }

    void SetCurrentWeaponActive()
    {
        int weaponIndex = 0;

        foreach (Weapon weapon in _weapons)
        {
            weapon.gameObject.SetActive(weaponIndex == _currentWeaponIdx);
            weaponIndex++;
        }
    }

    void CycleToNextWeapon()
    {
        //Debug.Log($"[WeaponSwitcher] CycleToNextWeapon _currentWeaponIdx:{_currentWeaponIdx}");

        if (_currentWeaponIdx < _weapons.Length - 1)
        {
            _currentWeaponIdx++;
        }
        else
        {
            _currentWeaponIdx = 0;
        }        
        
        SetCurrentWeaponActive();
    }
}
