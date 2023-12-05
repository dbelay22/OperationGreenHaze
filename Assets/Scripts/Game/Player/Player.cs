using System;
using UnityEngine;

[RequireComponent(typeof(Ammo))]
[RequireComponent(typeof(PlayerHealth))]
public class Player : MonoBehaviour
{ 
    [Header("Camera Shake")]
    [SerializeField] CameraShake _cameraShake;

    /*
    [Header("SFX")]
    [SerializeField] AudioClip _pickupSFX;
    [SerializeField] AudioClip _missionPickupSFX;
    [SerializeField] AudioClip _errorSFX;
    */

    Ammo _ammo;

    WeaponSwitcher _weaponSwitcher;

    Flashlight _flashlight;

    PlayerHealth _playerHealth;

    //AudioSource _audioSource;

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

        //_audioSource = GetComponent<AudioSource>();

        _weaponSwitcher = GetComponentInChildren<WeaponSwitcher>();
        
        _flashlight = GetComponentInChildren<Flashlight>();
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
        float nearZombiesFactor = ((enemyCountTooClose / 2f) * 1f);
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

    float DistanceToZombie(Transform enemyTransform)
    {
        float distance = Vector3.Distance(transform.position, enemyTransform.position);
        
        return distance;
    }

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

    bool ProcessMissionPickup(GameObject trigger)
    {
        bool isMissionPickup = trigger.CompareTag(Tags.MISSION_PICKUP);

        if (isMissionPickup)
        {
            // TODO: Trigger mission pickup SFX
            // sound!
            //PlayAudioClip(_missionPickupSFX, true);

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
            // TODO: Trigger flashlight pickup SFX
            // sound!
            //PlayAudioClip(_pickupSFX);

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
    }

    bool ProcessMedkitPickup(GameObject trigger)
    {
        bool isMedkit = trigger.CompareTag(Tags.MEDKIT_PICKUP);

        if (isMedkit)
        {
            if (_playerHealth.CurrentHealthPercentage < 1)
            {
                MedkitPickup medkit = trigger.GetComponent<MedkitPickup>();

                // TODO: Trigger medkit pickup SFX
                // sound!
                //PlayAudioClip(_pickupSFX);

                // improve health
                _playerHealth.ImproveByPickup(medkit.HealthAmount);

                // bye pickup
                Destroy(trigger);

                return true;
            }
            else
            {
                // TODO: Trigger medkit cant use pickup SFX
                //PlayAudioClip(_errorSFX);
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

    /*
    bool PlayAudioClip(AudioClip clip, bool forcePlay = false)
    {
        if (_audioSource.isPlaying)
        {
            if (forcePlay == false)
            {
                return false;
            }
            else
            {
                _audioSource.Stop();
            }            
        }

        _audioSource.PlayOneShot(clip);

        return true;
    }
    */

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
        // turn of flashlight
        _flashlight.TurnOff();

        // hide weapons
        _weaponSwitcher.HideCurrentWeapon();

        // bye player ?
        Destroy(gameObject, 3f);
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
