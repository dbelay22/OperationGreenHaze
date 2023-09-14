using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcHealth : MonoBehaviour
{
    const float HEALTH_SCALE_ONE = 100f;
    
    float _currentHealth;

    void Start()
    {
        NpcAI npc = GetComponent<NpcAI>();

        _currentHealth = HEALTH_SCALE_ONE * npc.SizeScale;

        //Debug.Log($"[NpcHealth] (Start) npc.SizeScale:{npc.SizeScale}, _health:{_currentHealth}");
    }

    void OnHitByBullet(float damage)
    {
        TakeDamage(damage);
    }

    public void HitByExplosion()
    {
        TakeDamage(HEALTH_SCALE_ONE);
    }

    void TakeDamage(float damage)
    {
        if (_currentHealth <= 0)
        {
            // dead can't dead again, does it ?
            return;
        }

        _currentHealth -= damage;

        BroadcastMessage("OnHealthChange", _currentHealth, SendMessageOptions.RequireReceiver);
    }

}