using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [Header("Gameplay Music")]
    public EventReference GameplayMusicEvent;
    public string TerrorMusicParam;
    public string MusicPartsParam;
    public string NearZombiesParam;

    [Header("Other music")]
    public EventReference MenuMusic;
    public EventReference BriefingMusic;
    public EventReference WinMusic;
    public EventReference LoseMusic;

    [Header("Ambience")]
    public EventReference AmbienceSoundscape;

    [Header("Player")]
    public EventReference PlayerDamageByZombie;
    public EventReference PlayerDamageByFire;
    public EventReference PlayerDamageByGas;
    public EventReference PlayerDeath;

    [Header("PlayerHealth")]
    public EventReference PlayerHealth;
    public string HealthParamName = "Health_Medium-Low";

    [Header("Player Steps")]
    public EventReference PlayerFootsteps;

    public string WalkRunParameter;
    
    public string FloorMaterialParameter;
    public int DefaultFloorMaterialValue = 0;

    public string LeftRightParameter;

    [Header("Pickups")]
    public EventReference PickupAmmo;
    public EventReference PickupMedkit;
    public EventReference PickupMission1;
    public EventReference PickupMission2;
    public EventReference PickupCantUse;

    [Header("Weapons")]
    public EventReference ChangeWeaponSMG;
    public EventReference ChangeWeaponPistol;

    [Header("Flashlight")]
    public EventReference FlashlighToggle;
    public string FlashlightOnOffParam;

    [Header("Bullet")]
    public EventReference BulletImpact;
    public string ImpactMaterialParameter;

    [Header("Zombie")]
    public EventReference ZombieAppear;

    [Header("Zombie steps")]
    public EventReference ZombieFootsteps;
    public string ZombieLeftRightParam;

    [Header("Zombie Attack")]
    public EventReference ZombieAttack;

    [Header("Zombie Damaged")]
    public EventReference ZombieFall;
    public EventReference ZombieDie;
    public EventReference ZombieDamage;
    public EventReference ZombieBlinded;
    public EventReference ZombieHeadshot;

    [Header("Jet waves")]
    public EventReference Jet;

    [Header("UI")]
    public EventReference MenuHover;
    public EventReference MenuSelect;
    public EventReference Typewriter;

    [Header("Explosions")]
    public EventReference BarrelExplosion;

    #region Instance

    private static FMODEvents _instance;

    public static FMODEvents Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion


}
