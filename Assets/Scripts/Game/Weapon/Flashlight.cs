using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
[RequireComponent(typeof(AudioSource))]
public class Flashlight : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] GameObject _prefab;

    [Header("Decay")]
    [SerializeField] float _lightDecay = 0.2f;
    [SerializeField] float _angleDecay = 0.2f;

    [Header("In-game message")]
    [SerializeField] float _inGameMessageLifetime = 4f;

    float _timeOfPickup;

    Light _light;

    bool _isOn = false;
    float _minIntensityToBlind;
    
    bool _isPickedUp = false;

    public bool IsPickedUp { get { return _isPickedUp; } }

    AudioSource _audioSource;

    void Start()
    {
        _light = GetComponent<Light>();
        _audioSource = GetComponent<AudioSource>();

        _isPickedUp = false;
        
        TurnOff();

        // min half intensity to blind a zombie
        _minIntensityToBlind = _light.intensity / 2;
    }

    void Update()
    {
        _light.enabled = _isOn;
        _prefab.SetActive(_isOn);

        if (_isPickedUp == false)
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
            ToggleOnOff();
            PlaySFX();
        }
    }

    void PlaySFX()
    {
        _audioSource.Play();
    }

    void ToggleOnOff()
    {
        if (_isPickedUp == false)
        {
            return;
        }

        float curTime = Time.time;
        float timeEndMessage = _timeOfPickup + _inGameMessageLifetime;

        if (curTime < timeEndMessage)
        {
            GameUI.Instance.HideMessagesNow();
        }

        _isOn = !_isOn;
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
        // show in-game message
        GameUI.Instance.ShowInGameMessage("Press 'F' to toggle Flashlight", _inGameMessageLifetime);

        _isPickedUp = true;
        _timeOfPickup = Time.time;

        _isOn = true;
    }
}
