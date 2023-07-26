using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour
{
    [Header("Decay")]
    [SerializeField] float _lightDecay = 0.2f;
    [SerializeField] float _angleDecay = 0.2f;

    Light _light;

    bool _isOn = false;

    void Start()
    {
        _light = GetComponent<Light>();
        _light.enabled = false;
    }

    void Update()
    {
        if (_isOn)
        {
            DecreaseIntensity();
            DecreaseAngle();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            _isOn = !_isOn;

            _light.enabled = _isOn;
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

}