using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitcher : MonoBehaviour
{
    readonly float[] SWITCH_WEAPON_TIMES = { 1.3f, 0.8f };

    [Header("Weapon Shake DB")]
    [SerializeField] WeaponShakeData _shakeData;

    WeaponShakeData.ShakeProperties _currentShakeProperties;

    List<Weapon> _weapons;

    bool _canScrollToNextWeapon = true;
    
    Weapon _activeWeapon;

    int _currentWeaponIdx = 0;

    EventInstance _switchSMGSFX;
    EventInstance _switchPistolSFX;

    void Start()
    {
        _canScrollToNextWeapon = true;

        InitializeWeapons();

        InitializeAudioInstances();

        StartCoroutine(SetCurrentWeaponActiveDelayed());
    }

    void InitializeAudioInstances()
    {
        _switchSMGSFX = AudioController.Instance.CreateInstance(FMODEvents.Instance.ChangeWeaponSMG);
        _switchPistolSFX = AudioController.Instance.CreateInstance(FMODEvents.Instance.ChangeWeaponPistol);
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

    IEnumerator SetCurrentWeaponActiveDelayed()
    {
        if (_activeWeapon != null)
        {
            _activeWeapon.gameObject.SetActive(false);
        }

        transform.localScale = new Vector3(0,0,0);

        AudioController.Instance.PlayEvent(_currentWeaponIdx == 0 ? _switchSMGSFX : _switchPistolSFX);

        float time = SWITCH_WEAPON_TIMES[_currentWeaponIdx];

        //Debug.Log($"WeaponSwitcher] SetCurrentWeaponActiveDelayed) time: {time}, _currentWeaponIdx:{_currentWeaponIdx}");

        yield return new WaitForSeconds(time);

        SetCurrentWeaponActive();

        transform.localScale = new Vector3(1, 1, 1);
    }

    void SetCurrentWeaponActive()
    {
        int weaponIndex = 0;

        foreach (Transform child in transform)
        {
            //Debug.Log($"WeaponSwitcher] SetCurrentWeaponActive) child: {child.name}");

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

        StartCoroutine(SetCurrentWeaponActiveDelayed());
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

        StartCoroutine(SetCurrentWeaponActiveDelayed());

        StartCoroutine(CoolDownScroll());
    }

    IEnumerator CoolDownScroll()
    {
        yield return new WaitForSeconds(0.25f);
        
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
