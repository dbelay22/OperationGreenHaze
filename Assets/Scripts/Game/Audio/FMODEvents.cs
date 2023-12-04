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

    [Header("Player Steps")]
    public EventReference PlayerFootsteps;

    public string WalkRunParameter;
    
    public string FloorMaterialParameter;
    public int DefaultFloorMaterialValue = 0;

    public string LeftRightParameter;

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

    [Header("Player")]
    public EventReference PlayerDamage;

    #region Instance

    private static FMODEvents _instance;

    public static FMODEvents Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion


}
