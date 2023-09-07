using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] GameObject _prefab;

    [Header("Decay")]
    [SerializeField] float _lightDecay = 0.2f;
    [SerializeField] float _angleDecay = 0.2f;

    Light _light;

    bool _isOn = false;
    float _minIntensityToBlind;
    bool _flashlightPickedUp = false;

    void Start()
    {
        _light = GetComponent<Light>();

        _flashlightPickedUp = false;
        
        TurnOff();

        // min half intensity to blind a zombie
        _minIntensityToBlind = _light.intensity / 2;
    }

    void Update()
    {
        _light.enabled = _isOn;
        _prefab.SetActive(_isOn);

        if (_flashlightPickedUp == false)
        {
            return;
        }

        if (_isOn)
        {
            DecreaseIntensity();
            DecreaseAngle();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            _isOn = !_isOn;            
        }
    }

    void DecreaseIntensity()
    {
        _light.intensity -= Time.deltaTime * _lightDecay;
    }

    void DecreaseAngle()
    {
        float amount = Time.deltaTime * _angleDecay;
        
        if (amount < 0)
        {
            amount = 0;
        }

        _light.innerSpotAngle -= amount;
        _light.spotAngle -= amount;
    }
        
    public bool IsOn()
    {
        return _isOn;
    }

    public bool IsOnAndCanBlind()
    {
        if (_isOn == false)
        {
            return false;
        }

        bool canBlind = _light.intensity >= _minIntensityToBlind;

        return canBlind;
    }

    public void TurnOff()
    {
        _isOn = false;
    }

    public void ReportPickUp()
    {
        _flashlightPickedUp = true;
        _isOn = false;
    }
}
