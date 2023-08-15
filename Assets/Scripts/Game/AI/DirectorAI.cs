using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectorEvent
{ 
    Enemy_Melee_Attack,
    Player_Escape,
    Player_Damaged,
    Enemy_Killed,
    Enemy_Killed_Headshot,
    Player_Pickup_Medkit,
    Player_Pickup_Ammo,
    Player_Pickup_Flashlight,
    Shot_Accuracy_Update,
}

public class DirectorAI : MonoBehaviour
{
    [SerializeField] float _stressFactor = 0.2f;

    // stress
    float _playerStress = 0;
    float _lastReportedStressLevel = 0f;
    float _avgStressLevel = 0f;
    float _stressChangeCount = 0f;

    // zombie attack
    int _meleeAttackCount = 0;
    int _playerEscapeCount = 0;
    int _playerDamageCount = 0;

    // pickups
    int _playerPickupMedkitCount = 0;
    int _playerPickupAmmoCount = 0;
    int _playerPickupFlashlightCount = 0;

    // kills
    int _enemyKillCount = 0;
    int _enemyKillByHeadshotCount = 0;

    // shot accuracy
    float _shotAccuracy = 0f;

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
        StressStart();
    }

    void Update()
    {
        StressLevelUpdate();
    }

    public void DumpStats()
    {
        Debug.Log($"[Director] DUMP......................");
        Debug.Log($"[Director] Elapsed Seconds: {HUD.Instance.ElapsedSeconds}");
        Debug.Log($"[Director] Shot Accuracy: {_shotAccuracy} %");
        Debug.Log($"[Director] Enemy Kill Count: {_enemyKillCount}");
        Debug.Log($"[Director] Enemy Kill HEADSHOT Count: {_enemyKillByHeadshotCount}");

        Debug.Log($"[Director] ..........................");        
        Debug.Log($"[Director] AVG Stress Level: {_avgStressLevel}");
        Debug.Log($"[Director] Last Reported Stress Level: {_lastReportedStressLevel}");
        Debug.Log($"[Director] Stress Change Count: {_stressChangeCount}");
        
        Debug.Log($"[Director] ..........................");        
        Debug.Log($"[Director] Melee Attack Count: {_meleeAttackCount}");
        Debug.Log($"[Director] Player Escape Count: {_playerEscapeCount}");
        Debug.Log($"[Director] Player Damage Count: {_playerDamageCount}");
        
        Debug.Log($"[Director] ..........................");        
        Debug.Log($"[Director] Player Pickup Medkit Count: {_playerPickupMedkitCount}");
        Debug.Log($"[Director] Player Pickup Ammo Count: {_playerPickupAmmoCount}");      
        Debug.Log($"[Director] END OF DUMP...............");
    }

    void StressStart()
    {
        _playerStress = 0f;
        _lastReportedStressLevel = 0f;
        _avgStressLevel = 0f;
        _stressChangeCount = 0f;
    }

    float StressLevelUpdate()
    {
        _playerStress = (_meleeAttackCount + _playerDamageCount + _playerEscapeCount + (_enemyKillByHeadshotCount + _enemyKillCount * 0.2f) - _playerPickupMedkitCount - _playerPickupAmmoCount - _playerPickupFlashlightCount) * _stressFactor;

        //Debug.Log($"[DirectorAI] (OnEvent) Stress Level Update - Stress Brute: {_playerStress} - _lastReportedStressLevel: {_lastReportedStressLevel}");

        _playerStress = Mathf.Clamp(_playerStress, 0, 10);

        //Debug.Log($"[DirectorAI] (OnEvent) Stress Level Update - Stress Clamp: {_playerStress} - _lastReportedStressLevel: {_lastReportedStressLevel}");

        if (_lastReportedStressLevel != _playerStress)
        {
            _stressChangeCount++;

            // AVG
            if (_avgStressLevel == 0f)
            {
                // init value
                _avgStressLevel = _playerStress;
            }
            else
            {
                _avgStressLevel = (_avgStressLevel + _playerStress) / 2;
            }

            //Debug.Log($"[DirectorAI] (OnEvent) Stress Level Update - AVG: {_avgStressLevel}");

            _lastReportedStressLevel = _playerStress;
        }

        return _playerStress;
    }

    public void OnEvent(DirectorEvent directorEvent, object eventValue = null)
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

            case DirectorEvent.Enemy_Killed_Headshot:
                _enemyKillByHeadshotCount++;
                break;

            case DirectorEvent.Shot_Accuracy_Update:
                _shotAccuracy = (float) eventValue;
                break;
        }
    }


}
