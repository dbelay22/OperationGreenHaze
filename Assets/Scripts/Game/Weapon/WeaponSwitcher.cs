using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class WeaponSwitcher : MonoBehaviour
{
    [Header("SFX")]
    [SerializeField] AudioClip _switchSFX;

    [Header("Weapon Shake DB")]
    [SerializeField] WeaponShakeData _shakeData;

    WeaponShakeData.ShakeProperties _currentShakeProperties;

    List<Weapon> _weapons;

    AudioSource _audioSource;

    bool _canScrollToNextWeapon = true;
    
    Weapon _activeWeapon;

    int _currentWeaponIdx = 0;

    void Start()
    {
        _canScrollToNextWeapon = true;

        InitializeWeapons();

        _audioSource = GetComponent<AudioSource>();

        StartCoroutine(SetCurrentWeaponActiveDelayed(0.5f));
    }

    void InitializeWeapons()
    {
        _weapons = new List<Weapon>();
        
        foreach (Transform child in transform)
        {
            Weapon weapon = child.gameObject.GetComponent<Weapon>();
            if (weapon != null) {
                _weapons.Add(weapon);
            }            
        }
    }

    IEnumerator SetCurrentWeaponActiveDelayed(float time)
    {
        yield return new WaitForSeconds(time);

        SetCurrentWeaponActive();
    }

    void Update()
    {
        if (!Game.Instance.IsGameplayOn())
        {
            return;
        }

        float mouseScrollY = Mouse.current.scroll.ReadValue().y;

        if (mouseScrollY != 0 && _canScrollToNextWeapon)
        {
            CycleToNextWeapon();
        }

        ProcessKeyboardInput();
    }

    void ProcessKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetCurrentWeapon(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetCurrentWeapon(1);
        }
    }

    void SetCurrentWeapon(int index)
    {
        if (index == _currentWeaponIdx)
        {
            return;
        }

        _currentWeaponIdx = index;
        
        SetCurrentWeaponActive();
    }

    void SetCurrentWeaponActive()
    {
        if (_switchSFX != null)
        {
            _audioSource.PlayOneShot(_switchSFX);
        }

        int weaponIndex = 0;

        foreach (Transform child in transform)
        {
            Weapon weapon = child.gameObject.GetComponent<Weapon>();
            
            bool foundWeapon = weaponIndex == _currentWeaponIdx;
            
            if (foundWeapon)
            {
                _activeWeapon = weapon;
                UpdateCurrentShakeProperties(weapon);
            }            

            weapon.gameObject.SetActive(foundWeapon);
            
            weaponIndex++;            
        }
    }

    void UpdateCurrentShakeProperties(Weapon weapon)
    {
        AmmoType currentAmmoType = weapon.GetAmmoType();

        foreach (WeaponShakeData.WeaponShakeDataset shakeDataset in _shakeData.weaponShakeDataList)
        {
            if (shakeDataset.ammoType.Equals(currentAmmoType))
            {
                _currentShakeProperties = shakeDataset.shakeProperties;
            }
        }
    }

    void CycleToNextWeapon()
    {
        _canScrollToNextWeapon = false;

        if (_currentWeaponIdx < _weapons.Count - 1)
        {
            _currentWeaponIdx++;
        }
        else
        {
            _currentWeaponIdx = 0;
        }        
        
        SetCurrentWeaponActive();

        StartCoroutine(CoolDownScroll());
    }

    IEnumerator CoolDownScroll()
    {
        yield return new WaitForSeconds(0.5f);
        
        _canScrollToNextWeapon = true;
    }

    public Weapon GetCurrentWeapon() {
        return _weapons[_currentWeaponIdx];
    }

    public void HideCurrentWeapon()
    {
        _activeWeapon.gameObject.SetActive(false);
    }

    public WeaponShakeData.ShakeProperties GetCurrentShakeProperties()
    {
        return _currentShakeProperties;
    }
}
