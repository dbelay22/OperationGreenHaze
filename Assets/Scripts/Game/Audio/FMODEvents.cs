using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [Header("Player Steps")]
    public EventReference PlayerFootsteps;

    public string WalkRunParameter;
    
    public string FloorMaterialParameter;
    public int DefaultFloorMaterialValue = 0;

    public string LeftRightParameter;

    #region Instance

    private static FMODEvents _instance;

    public static FMODEvents Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion


}
