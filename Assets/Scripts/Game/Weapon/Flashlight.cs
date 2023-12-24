using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class Flashlight : MonoBehaviour
{
    [System.Serializable]
    private struct FlashlightDecay
    {
        public float light;
        public float angle;
    }

    [Header("Model")]
    [SerializeField] GameObject _prefab;

    [Header("Decay")]
    [SerializeField] FlashlightDecay decayValues;


    [Header("In-game message")]
    [SerializeField] float _inGameMessageLifetime = 4f;

    float _timeOfPickup;

    Light _light;

    bool _isOn = false;
    float _minIntensityToBlind;
    
    bool _isPickedUp = false;

    public bool IsPickedUp { get { return _isPickedUp; } }

    EventInstance _flashlightSFX;

    void Start()
    {
        _light = GetComponent<Light>();

        _flashlightSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.FlashlighToggle, transform.position);

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
            
            PlaySFX(_isOn);
        }
    }

    void PlaySFX(bool isOn)
    {
        _flashlightSFX.setParameterByName(FMODEvents.Instance.FlashlightOnOffParam, isOn ? 0 : 1);
        AudioController.Instance.Play3DEvent(_flashlightSFX, transform.position, true);
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
        _light.intensity -= Time.deltaTime * decayValues.light;
    }

    void DecreaseAngle()
    {
        float amount = Time.deltaTime * decayValues.angle;
        
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
        GameUI.Instance.ShowInGameMessage("ig_flashlight_pickup", _inGameMessageLifetime);

        _isPickedUp = true;
        _timeOfPickup = Time.time;

        _isOn = true;
    }

}
