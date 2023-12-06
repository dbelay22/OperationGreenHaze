using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [Header("Gameplay Music")]
    public EventReference GameplayMusicEvent;
    public string TerrorMusicParam;
    public string MusicPartsParam;
    public string NearZombiesParam;
    
    [Header("Player")]
    public EventReference PlayerDamage;
    public EventReference PlayerDeath;

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

    #region Instance

    private static FMODEvents _instance;

    public static FMODEvents Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion


}
