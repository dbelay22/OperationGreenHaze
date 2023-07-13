using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    float _health = 100f;

    public float Health { get { return _health; } }

    void Start()
    {
        _health = 100f;    
    }

    public void Shoot(float damage)
    {
        _health -= damage;
        
        Debug.Log($"[EnemyHealth] shooted! zombie health is now [{_health}]");
    }
}
