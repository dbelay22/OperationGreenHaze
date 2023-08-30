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
    Player_Pickup_Mission,
}

[RequireComponent(typeof(AudioSource))]
public class Director : MonoBehaviour
{
    [Header("Raid Siren")]
    [SerializeField] AudioClip _raidSirenSFX;

    [Header("Explosions")]
    [SerializeField] AudioClip[] _explosions;

    AudioSource _audioSource;

    // stress
    float _playerStress = 0;
    float _maxStress = 0;

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
        _audioSource = GetComponent<AudioSource>();
        
        _statsOnScreen = false;
        
        StressStart();
    }

    void Update()
    {
        StressLevelUpdate();

        // [T] Show Director Stats
        if (Input.GetKeyDown(KeyCode.T))
        {
            _statsOnScreen = !_statsOnScreen;
        }
    }

    void OnGUI()
    {
        // Make a background box
        //GUI.Box(new Rect(10, 300, 300, 550), "Director Stats");

        if (_statsOnScreen == false)
        {
            return;
        }

        GUI.backgroundColor = Color.grey;
        GUI.contentColor = Color.green;

        string stats = DumpStats();

        GUI.TextArea(new Rect(10, 350, 280, 375), stats);
    }

    public void PlayExplosionSFX()
    {
        int rndIndex = Random.Range(0, _explosions.Length);

        Debug.Log($"[Director] (Explosion SFX) rndIndex: {rndIndex}");

        _audioSource.PlayOneShot(_explosions[rndIndex]);
    }

    public void PlaySirenSFX() 
    {
        _audioSource.PlayOneShot(_raidSirenSFX);
    }

    public string DumpStats()
    {
        string stats = $"[Director] DUMP......................\n" +
            $"[Director] Elapsed Seconds: {GameUI.Instance.ElapsedSeconds}\n" +
            $"[Director] Shot Accuracy: {_shotAccuracy} %\n" +
            $"[Director] Enemy Kill Count: {_enemyKillCount}\n" +
            $"[Director] Enemy Kill HEADSHOT Count: {_enemyKillByHeadshotCount}\n" +
            
            $"[Director] ..........................\n" +
            $"[Director] Stress Level: {_playerStress}\n" +
            
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

        //Debug.Log(stats);

        return stats;
    }

    void StressStart()
    {
        _playerStress = 0f;
        _maxStress = 0f;
    }

    float StressLevelUpdate()
    {
        _playerStress = Mathf.Round(
            _meleeAttackCount + _playerDamageCount
            - (_playerEscapeCount * 0.2f) - (_enemyKillByHeadshotCount * 0.1f) - (_playerPickupAmmoCount * 0.2f) - (_playerPickupFlashlightCount * 0.2f)
         );

        //Debug.Log($"[DirectorAI] (OnEvent) Stress Level Update - Stress Brute: {_playerStress} - _lastReportedStressLevel: {_lastReportedStressLevel}");

        _playerStress = Mathf.Clamp(_playerStress, 0, 100);

        _maxStress = Mathf.Max(_playerStress, _maxStress);

        //Debug.Log($"[DirectorAI] (OnEvent) Stress Level Update - Stress Clamp: {_playerStress} - _lastReportedStressLevel: {_lastReportedStressLevel}");

        return _playerStress;
    }

    public void OnEvent(DirectorEvents directorEvent, object eventValue = null)
    {
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

    public void GameIsOver()
    {
        StopAllCoroutines();
    }

}
