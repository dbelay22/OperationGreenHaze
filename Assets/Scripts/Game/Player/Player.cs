using FMOD.Studio;
using FMODUnity;
using System;
using UnityEngine;

[RequireComponent(typeof(Ammo))]
[RequireComponent(typeof(PlayerHealth))]
public class Player : MonoBehaviour
{ 
    [Header("Camera Shake")]
    [SerializeField] CameraShake _cameraShake;

    EventInstance _pickupAmmoSFX;
    EventInstance _pickupMedkitSFX;
    EventInstance _missionPickup1SFX;
    EventInstance _missionPickup2SFX;
    EventInstance _flashlightSFX;
    EventInstance _cantUseSFX;
    EventInstance _weaponZoomIBreathSFX;

    Ammo _ammo;

    WeaponSwitcher _weaponSwitcher;

    Flashlight _flashlight;

    PlayerHealth _playerHealth;
    
    int _flashlightAsWeaponMessageCount = 0;

    Animator _animator;    

    #region Instance

    private static Player _instance;


    public static Player Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion

    void Start()
    {
        _flashlightAsWeaponMessageCount = 0;

        _animator = GetComponentInChildren<Animator>();
        _animator.enabled = false;

        _ammo = GetComponent<Ammo>();

        _playerHealth = GetComponent<PlayerHealth>();

        _weaponSwitcher = GetComponentInChildren<WeaponSwitcher>();
        
        _flashlight = GetComponentInChildren<Flashlight>();

        InitializeAudioInstances();
    }

    void InitializeAudioInstances()
    {
        _pickupAmmoSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.PickupAmmo, transform.position);
        _pickupMedkitSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.PickupMedkit, transform.position);
        _missionPickup1SFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.PickupMission1, transform.position);
        _missionPickup2SFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.PickupMission2, transform.position);
        _flashlightSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.FlashlighToggle, transform.position);
        _cantUseSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.PickupCantUse, transform.position);
        _weaponZoomIBreathSFX = AudioController.Instance.CreateInstance(FMODEvents.Instance.PlayerZoomInBreath);

    }

    void Update()
    {
        if (!Game.Instance.IsGameplayOn())
        {
            return;
        }

        UpdateMusicIntensity();
    }

    void UpdateMusicIntensity()
    {
        int acc = 0;

        //Debug.Log($"[Player] UpdateMusicIntensity) ---------");

        int enemyCountTooClose = HowManyEnemiesAtDistance(2f);
        acc += enemyCountTooClose;

        int enemyCountNear = HowManyEnemiesAtDistance(12f) - acc;
        acc += enemyCountNear;

        int enemyCountMid = HowManyEnemiesAtDistance(25f) - acc;

        //Debug.Log($"[Player] UpdateMusicIntensity) Close: {enemyCountTooClose}");
        //Debug.Log($"[Player] UpdateMusicIntensity) Near: {enemyCountNear}");
        //Debug.Log($"[Player] UpdateMusicIntensity) Mid: {enemyCountMid}");

        //----
        float intensity = ((enemyCountTooClose / 3f) * 0.50f) + ((enemyCountNear / 3f) * 0.25f) + ((enemyCountMid / 3f) * 0.25f);
        float clampIntensity = Math.Clamp(intensity, 0f, 1f);
        
        //Debug.Log($"[Player] UpdateMusicIntensity) intensity: {intensity}, clamp:{clampIntensity}");
        
        AudioController.Instance.GameplayIntensityUpdate(clampIntensity);

        //----
        float nearZombiesFactor = ((enemyCountTooClose / 2f) * 0.5f) + ((enemyCountNear / 2f) * 0.5f);
        float clampNearZombiesFactor = Math.Clamp(nearZombiesFactor, 0f, 1f);
        
        //Debug.Log($"[Player] UpdateMusicIntensity) nearZombiesFactor: {nearZombiesFactor}, clampNearZombiesFactor:{clampNearZombiesFactor}");                

        AudioController.Instance.GameplayNearZombiesUpdate(clampNearZombiesFactor);
    }

    int HowManyEnemiesAtDistance(float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        int amount = 0;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag(Tags.ENEMY) && collider.GetType().Equals(typeof(BoxCollider)))
            {
                amount++;
            }
        }

        return amount;
    }

    /*
    float DistanceToZombie(Transform enemyTransform)
    {
        float distance = Vector3.Distance(transform.position, enemyTransform.position);
        
        return distance;
    }
    */

    #region Shooting

    public void OnBulletShot(AmmoType ammoType, int amount, bool hitEnemy)
    {
        WeaponShakeData.ShakeProperties shakeProperties = _weaponSwitcher.GetCurrentShakeProperties();
        _cameraShake.Shake(shakeProperties);

        // Notify Ammo-slots manager
        _ammo.OnBulletShot(ammoType, amount, hitEnemy);
    }

    #endregion


    #region Pickups

    void OnTriggerEnter(Collider other)
    {
        GameObject trigger = other.gameObject;

        if (ProcessAmmoPickup(trigger))
        {
            Director.Instance.OnEvent(DirectorEvents.Player_Pickup_Ammo);
        }
        else if (ProcessMedkitPickup(trigger))
        {
            Director.Instance.OnEvent(DirectorEvents.Player_Pickup_Medkit);
        }
        else if (ProcessFlashlightPickup(trigger))
        {
            Director.Instance.OnEvent(DirectorEvents.Player_Pickup_Flashlight);
        }
        else if (ProcessMissionPickup(trigger))
        {
            Director.Instance.OnEvent(DirectorEvents.Player_Pickup_Mission);
        }
        else if (ProcessExitDanger(trigger))
        {
            Game.Instance.ReportExitDangerZoneEnter();
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject trigger = other.gameObject;

        if (ProcessExitDanger(trigger))
        {
            Game.Instance.ReportExitDangerZoneExit();
        }
    }

    bool ProcessExitDanger(GameObject trigger)
    {
        bool isExitDanger = trigger.CompareTag(Tags.EXIT_DANGER);
        
        return isExitDanger;
    }

    bool _firstMissionPickup = true;

    bool ProcessMissionPickup(GameObject trigger)
    {
        bool isMissionPickup = trigger.CompareTag(Tags.MISSION_PICKUP);

        if (isMissionPickup)
        {
            // sound!
            AudioController.Instance.Play3DEvent(_firstMissionPickup ? _missionPickup1SFX : _missionPickup2SFX, transform.position, true);

            _firstMissionPickup = false;

            MissionPickups.Instance.OnMissionItemPickup(trigger);

            Destroy(trigger);

            return true;
        }

        return false;
    }

    bool ProcessFlashlightPickup(GameObject trigger)
    {
        bool isFlashlight = trigger.CompareTag(Tags.FLASHLIGHT_PICKUP);

        if (isFlashlight)
        {
            // sound!
            const int FLASHLIGHT_ON = 0;            
            _flashlightSFX.setParameterByName(FMODEvents.Instance.FlashlightOnOffParam, FLASHLIGHT_ON);
            AudioController.Instance.Play3DEvent(_flashlightSFX, transform.position, true);

            _flashlight.ReportPickUp();

            // bye pickup
            Destroy(trigger);
        }

        return isFlashlight;
    }

    public void OnWeaponZoomIn()
    {
        if (_flashlight != null && _flashlight.IsPickedUp && _flashlight.IsOn())
        {
            _flashlight.TurnOff();
        }

        AudioController.Instance.PlayEvent(_weaponZoomIBreathSFX);
    }

    public void OnWeaponZoomOut()
    {
        AudioController.Instance.StopEvent(_weaponZoomIBreathSFX);
    }

    bool ProcessMedkitPickup(GameObject trigger)
    {
        bool isMedkit = trigger.CompareTag(Tags.MEDKIT_PICKUP);

        if (isMedkit)
        {
            if (_playerHealth.CurrentHealthPercentage < 1)
            {
                // sound!
                AudioController.Instance.Play3DEvent(_pickupMedkitSFX, transform.position, true);

                MedkitPickup medkit = trigger.GetComponent<MedkitPickup>();                

                // improve health
                _playerHealth.ImproveByPickup(medkit.HealthAmount);

                // bye pickup
                Destroy(trigger);

                return true;
            }
            else
            {
                AudioController.Instance.Play3DEvent(_cantUseSFX, transform.position, true);
                return false;
            }
        }

        return false;
    }

    bool ProcessAmmoPickup(GameObject trigger) 
    {
        bool isAmmoPickup = trigger.CompareTag(Tags.AMMO_PICKUP);

        if (isAmmoPickup)
        {
            // sound!
            AudioController.Instance.Play3DEvent(_pickupAmmoSFX, transform.position, true);

            AmmoPickup ammoPickup = trigger.GetComponent<AmmoPickup>();

            // add ammo to slot
            _ammo.AddAmmoPickup(ammoPickup.AmmoType, ammoPickup.AmmoAmount);

            // bye pickup
            Destroy(trigger);

            // update GameUI if current ammo type
            Weapon currentWeapon = _weaponSwitcher.GetCurrentWeapon();
            if (ammoPickup.AmmoType.Equals(currentWeapon.GetAmmoType()))
            {
                GameUI.Instance.UpdateAmmoAmount(currentWeapon.GetAmmoLeft());
            }
            
        }

        return isAmmoPickup;
    }

    #endregion
    
    public bool IsFlashlightOnAndCanBlind()
    {
        if (_flashlight == null)
        {
            return false;
        }

        return _flashlight.IsOnAndCanBlind();
    }

    void OnPlayerDeath()
    {
        Debug.Log("Player] OnPlayerDeath)...");

        GameUI.Instance.ShowPlayerDamageVFX();

        AudioController.Instance.GameplayDead();

        PlayDeathAnimation();
        
        GameplayIsOver();

        StartCoroutine(Game.Instance.TimeBend(0.2f, 1f));
    }

    void PlayDeathAnimation()
    {
        _animator.enabled = true;

        // random death animation
        float rndValue = UnityEngine.Random.value;

        string animationTrigger = "PlayerDeathBack";
        
        if (rndValue < 0.34f)
        {
            animationTrigger = "PlayerDeathBackRight";
        }
        else if (rndValue < 0.67f)
        {
            animationTrigger = "PlayerDeathBackLeft";
        }
        
        _animator.SetTrigger(animationTrigger);
    }

    public void GameplayIsOver()
    {
        Debug.Log("Player] GameplayIsOver)...");

        BroadcastMessage("OnGameplayOver", SendMessageOptions.RequireReceiver);

        OnWeaponZoomOut();

        // turn of flashlight
        _flashlight.TurnOff();

        // hide weapons
        _weaponSwitcher.HideCurrentWeapon();
    }

    public string GetUnusedAmmo()
    {
        return _ammo.GetAllAmmoLeftString();
    }

    public float GetCurrentHealth()
    {
        return _playerHealth.CurrentHealth;
    }

    public void Damage(int amount)
    {
        _playerHealth.Damage(amount);

        ProcessUseFlashlightAsWeapon();
    }

    private void ProcessUseFlashlightAsWeapon()
    {
        if (_flashlightAsWeaponMessageCount >= 3)
        {
            return;
        }

        bool canUseFlashlightToDefend = _flashlight.IsPickedUp && _flashlight.IsOnAndCanBlind() == false;

        if (canUseFlashlightToDefend && _ammo.GetAllAmmoLeftCount() < 1)
        {
            bool messageShown = GameUI.Instance.ShowInGameMessage("ig_use_the_flashlight", 3);

            if (messageShown)
            {
                _flashlightAsWeaponMessageCount++;
            }
        }
    }
}
