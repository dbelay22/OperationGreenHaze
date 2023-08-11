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
    float _avgStressLevel = 0f;
    float _stressChangeCount = 0f;

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
        Debug.Log($"[Director] Enemy Kill Count: {_enemyKillCount}");
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
        _playerStress = (_meleeAttackCount + _playerDamageCount - _playerEscapeCount - (_enemyKillCount * 0.2f) - _playerPickupMedkitCount - _playerPickupAmmoCount - _playerPickupFlashlightCount) * _stressFactor;

        _playerStress = Mathf.Clamp(_playerStress, 0, 10);

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

            Debug.Log($"[DirectorAI] (OnEvent) Stress Level Update: {_playerStress}, AVG: {_avgStressLevel}");

            _lastReportedStressLevel = _playerStress;
        }

        return _playerStress;
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


}
