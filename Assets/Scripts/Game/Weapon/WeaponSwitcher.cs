using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AudioSource))]
public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] int _currentWeaponIdx = 0;
    [SerializeField] AudioClip _switchSFX;
    
    List<Weapon> _weapons;

    AudioSource _audioSource;

    void Start()
    {
        InitializeWeapons();

        _audioSource = GetComponent<AudioSource>();

        StartCoroutine(SetCurrentWeaponActiveDelayed(0.3f));
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
        if (Game.Instance.isGameOver())
        {
            return;
        }

        float mouseScrollY = Mouse.current.scroll.ReadValue().y;

        if (mouseScrollY != 0)
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
            weapon.gameObject.SetActive(weaponIndex == _currentWeaponIdx);
            weaponIndex++;
        }
    }

    void CycleToNextWeapon()
    {
        if (_currentWeaponIdx < _weapons.Count - 1)
        {
            _currentWeaponIdx++;
        }
        else
        {
            _currentWeaponIdx = 0;
        }        
        
        SetCurrentWeaponActive();
    }

    public Weapon GetCurrentWeapon() {
        return _weapons[_currentWeaponIdx];
    }
}
