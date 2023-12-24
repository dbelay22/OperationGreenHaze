using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoad : MonoBehaviour
{
    [SerializeField] string _instanceTAG = null;

    void Awake()
    {
        GameObject[] currentInstances = GetCurrentInstances();

        if (currentInstances != null && currentInstances.Length > 1)
        {
            Debug.LogWarning($"DontDestroyOnLoad] Awake) found ({currentInstances.Length}) instances of {_instanceTAG}, destroying one.");
            Destroy(currentInstances[0]);
        }        

        DontDestroyOnLoad(gameObject);
    }

    GameObject[] GetCurrentInstances()
    {
        if (_instanceTAG == null || _instanceTAG.Length < 1)
        {
            Debug.LogError($"[DontDestroyOnLoad] GetCurrentInstances) _instanceTAG is null or empty for object '{this.gameObject.name}'.");
            return null;
        }

        return GameObject.FindGameObjectsWithTag(_instanceTAG);
        
    }
}