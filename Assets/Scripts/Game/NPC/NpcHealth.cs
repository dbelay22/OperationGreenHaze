using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcHealth : MonoBehaviour
{
    const float TOP_HEALTH = 100f;
    
    float _health;

    void Start()
    {
        _health = TOP_HEALTH;    
    }

    void OnHitByBullet(float damage)
    {
        TakeDamage(damage);
    }

    public void HitByExplosion()
    {
        Debug.Log($"[NpcHealth] (HitByExplosion) !!!");
        TakeDamage(TOP_HEALTH);
    }

    void TakeDamage(float damage)
    {
        _health -= damage;

        BroadcastMessage("OnHealthChange", _health, SendMessageOptions.RequireReceiver);
    }

}
