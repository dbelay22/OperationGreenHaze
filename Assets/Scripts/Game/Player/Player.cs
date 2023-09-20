using UnityEngine;

[RequireComponent(typeof(Ammo))]
[RequireComponent(typeof(PlayerHealth))]
public class Player : MonoBehaviour
{ 
    [Header("Camera Shake")]
    [SerializeField] CameraShake _cameraShake;

    [Header("SFX")]
    [SerializeField] AudioClip _pickupSFX;
    [SerializeField] AudioClip _errorSFX;   

    Ammo _ammo;

    WeaponSwitcher _weaponSwitcher;

    Flashlight _flashlight;

    PlayerHealth _playerHealth;

    AudioSource _audioSource;

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
        _ammo = GetComponent<Ammo>();

        _playerHealth = GetComponent<PlayerHealth>();

        _audioSource = GetComponent<AudioSource>();

        _weaponSwitcher = GetComponentInChildren<WeaponSwitcher>();
        
        _flashlight = GetComponentInChildren<Flashlight>();
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
    }

    bool ProcessMissionPickup(GameObject trigger)
    {
        bool isMissionPickup = trigger.CompareTag(Tags.MISSION_PICKUP_TAG);

        if (isMissionPickup)
        {
            // sound!
            PlayAudioClip(_pickupSFX);

            MissionPickups.Instance.OnMissionItemPickup(trigger);

            Destroy(trigger);

            return true;
        }

        return false;
    }

    bool ProcessFlashlightPickup(GameObject trigger)
    {
        bool isFlashlight = trigger.CompareTag(Tags.FLASHLIGHT_PICKUP_TAG);

        if (isFlashlight)
        {
            // sound!
            PlayAudioClip(_pickupSFX);

            _flashlight.ReportPickUp();

            // bye pickup
            Destroy(trigger);
        }

        return isFlashlight;
    }

    bool ProcessMedkitPickup(GameObject trigger)
    {
        bool isMedkit = trigger.CompareTag(Tags.MEDKIT_PICKUP_TAG);

        if (isMedkit)
        {
            if (_playerHealth.CurrentHealthPercentage < 1)
            {
                MedkitPickup medkit = trigger.GetComponent<MedkitPickup>();

                // sound!
                PlayAudioClip(_pickupSFX);

                // improve health
                _playerHealth.ImproveByPickup(medkit.HealthAmount);

                // bye pickup
                Destroy(trigger);

                return true;
            }
            else
            {
                PlayAudioClip(_errorSFX);
                return false;
            }
        }

        return false;
    }

    bool ProcessAmmoPickup(GameObject trigger) 
    {
        bool isAmmoPickup = trigger.CompareTag(Tags.AMMO_PICKUP_TAG);

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

    bool PlayAudioClip(AudioClip clip)
    {
        if (_audioSource.isPlaying)
        {
            return false;
        }

        _audioSource.PlayOneShot(clip);

        return true;
    }

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
        
        GameplayIsOver();

        StartCoroutine(Game.Instance.TimeBend(0.19f, 1f));
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

}
