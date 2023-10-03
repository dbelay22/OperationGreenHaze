using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableOnAwake : MonoBehaviour    
{
    [SerializeField] GameObject _gameObject;

    void Awake()
    {
        _gameObject.SetActive(true);    
    }
}
