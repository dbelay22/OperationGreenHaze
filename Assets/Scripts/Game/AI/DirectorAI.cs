using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectorEvent
{ 
    Enemy_Melee_Attack,
    Player_Escape,
    Player_Damaged,
    Enemy_Killed,
    Player_Pickup_Medkit,
    Player_Pickup_Ammo,
    Player_Pickup_Flashlight
}

public class DirectorAI : MonoBehaviour
{
    [SerializeField] float _stressFactor = 0.2f;

    float _playerStress = 0;
    float _lastReportedStressLevel = 0f;

    // stress - enemies
    int _meleeAttackCount = 0;
    int _playerEscapeCount = 0;
    int _playerDamageCount = 0;
    int _playerPickupMedkitCount = 0;
    int _playerPickupAmmoCount = 0;
    int _playerPickupFlashlightCount = 0;
    int _enemyKillCount = 0;

    #region Instance

    private static DirectorAI _instance;
    

    public static DirectorAI Instance { get { return _instance; } }    

    void Awake()
    {
        _instance = this;
    }

    #endregion

    void Start()
    {
        _playerStress = 0f;
        _lastReportedStressLevel = 0f;
    }

    void Update()
    {
        StressLevelUpdate();
    }

    public void OnEvent(DirectorEvent directorEvent)
    {
        switch (directorEvent)
        {
            case DirectorEvent.Enemy_Melee_Attack:                
                _meleeAttackCount++;
                break;

            case DirectorEvent.Player_Escape:
                _playerEscapeCount++;
                break;

            case DirectorEvent.Player_Damaged:
                _playerDamageCount++;
                break;

            case DirectorEvent.Player_Pickup_Medkit:
                _playerPickupMedkitCount++;
                break;

            case DirectorEvent.Player_Pickup_Ammo:
                _playerPickupAmmoCount++;
                break;

            case DirectorEvent.Player_Pickup_Flashlight:
                _playerPickupFlashlightCount++;
                break;

            case DirectorEvent.Enemy_Killed:
                _enemyKillCount++;
                break;
        }
    }

    

    float StressLevelUpdate()
    {
        _playerStress = (_meleeAttackCount + _playerDamageCount - _playerEscapeCount - (_enemyKillCount * 0.2f) - _playerPickupMedkitCount - _playerPickupAmmoCount - _playerPickupFlashlightCount) * _stressFactor;

        _playerStress = Mathf.Clamp(_playerStress, 0, 10);

        if (_lastReportedStressLevel != _playerStress)
        {
            Debug.Log($"[DirectorAI] (OnEvent) Stress Level Update: {_playerStress}");
            _lastReportedStressLevel = _playerStress;
        }

        return _playerStress;
    }


}
