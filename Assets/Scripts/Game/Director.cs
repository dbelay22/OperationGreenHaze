using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DirectorEvents
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
    Player_Pickup_Mission
}

public class Director : MonoBehaviour
{    
    // zombie attack
    public int _meleeAttackCount = 0;
    public int _playerEscapeCount = 0;
    public int _playerDamageCount = 0;

    // pickups
    public int _playerPickupMedkitCount = 0;
    public int _playerPickupAmmoCount = 0;
    public int _playerPickupFlashlightCount = 0;

    // kills
    public int _enemyKillCount = 0;
    public int _enemyKillByHeadshotCount = 0;

    // shot accuracy
    public float _shotAccuracy = 0f;

    bool _statsOnScreen = false;

    #region Instance

    private static Director _instance;    

    public static Director Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion

    void Start()
    {
        _statsOnScreen = false;
    }

    void Update()
    {
        // [T] Show Director Stats
        if (DevDebug.Instance.IsDebugBuild && Input.GetKeyDown(KeyCode.T))
        {
            _statsOnScreen = !_statsOnScreen;
        }
    }

    void OnGUI()
    {
        if (_statsOnScreen == false)
        {
            return;
        }

        GUI.backgroundColor = Color.grey;
        GUI.contentColor = Color.green;

        string stats = DumpStats();

        GUI.TextArea(new Rect(10, 350, 280, 375), stats);
    }    

    public string DumpStats()
    {
        string stats = $"[Director] DUMP......................\n" +
            $"[Director] Elapsed Seconds: {GameUI.Instance.ElapsedSeconds}\n" +
            $"[Director] Shot Accuracy: {_shotAccuracy} %\n" +
            $"[Director] Enemy Kill Count: {_enemyKillCount}\n" +
            $"[Director] Enemy Kill HEADSHOT Count: {_enemyKillByHeadshotCount}\n" +
            
            $"[Director] ..........................\n" +
            $"[Director] Melee Attack Count: {_meleeAttackCount}\n" +
            $"[Director] Player Escape Count: {_playerEscapeCount}\n" +
            $"[Director] Player Damage Count: {_playerDamageCount}\n" +
            
            $"[Director] ..........................\n" +
            $"[Director] Player Pickup Medkit Count: {_playerPickupMedkitCount}\n" +
            $"[Director] Player Pickup Ammo Count: {_playerPickupAmmoCount}\n" +
            
            $"[Director] ..........................\n" +
            $"[Director] Player Ammo Not Used: {Player.Instance.GetUnusedAmmo()}\n" +
            $"[Director] Player Health: {Player.Instance.GetCurrentHealth()}\n" +
            $"[Director] END OF DUMP...............";

        Debug.Log(stats);

        return stats;
    }

    public void OnEvent(DirectorEvents directorEvent, object eventValue = null)
    {
        //Debug.Log($"[Director] OnEvent) [{directorEvent}]");

        switch (directorEvent)
        {
            case DirectorEvents.Enemy_Melee_Attack:
                _meleeAttackCount++;
                break;

            case DirectorEvents.Player_Escape:
                _playerEscapeCount++;
                break;

            case DirectorEvents.Player_Damaged:
                _playerDamageCount++;
                break;

            case DirectorEvents.Player_Pickup_Medkit:
                _playerPickupMedkitCount++;
                break;

            case DirectorEvents.Player_Pickup_Ammo:
                _playerPickupAmmoCount++;
                break;

            case DirectorEvents.Player_Pickup_Flashlight:
                _playerPickupFlashlightCount++;
                break;

            case DirectorEvents.Enemy_Killed:
                _enemyKillCount++;
                break;

            case DirectorEvents.Enemy_Killed_Headshot:
                _enemyKillByHeadshotCount++;
                break;

            case DirectorEvents.Shot_Accuracy_Update:
                _shotAccuracy = (float) eventValue;
                break;
        }        
    }

}
