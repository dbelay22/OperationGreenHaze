using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectorAI : MonoBehaviour
{

    #region Instance
    
    private static DirectorAI _instance;
    
    public static DirectorAI Instance
    {
        get { return _instance; }
    }

    void Awake()
    {
        _instance = this;
    }

    #endregion

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
