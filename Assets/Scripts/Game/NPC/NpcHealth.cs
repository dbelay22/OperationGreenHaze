using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcHealth : MonoBehaviour
{
    float _health = 100f;

    void Start()
    {
        _health = 100f;    
    }

    void OnHitByBullet(float damage)
    {
        _health -= damage;
        
        //Debug.Log($"[NpcHealth] (OnHitByBullet) zombie health is now [{_health}]");
        
        BroadcastMessage("OnHealthChange", _health, SendMessageOptions.RequireReceiver);
    }
}
