using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcHealth : MonoBehaviour
{
    const float TOP_HEALTH = 100f;
    
    float _health;

    void Start()
    {
        NpcAI npc = GetComponent<NpcAI>();

        _health = TOP_HEALTH * npc.SizeScale;

        Debug.Log($"[NpcHealth] (Start) npc.SizeScale:{npc.SizeScale}, _health:{_health}");
    }

    void OnHitByBullet(float damage)
    {
        TakeDamage(damage);
    }

    public void HitByExplosion()
    {
        TakeDamage(TOP_HEALTH);
    }

    void TakeDamage(float damage)
    {
        if (_health <= 0)
        {
            // dead can't dead again, does it ?
            return;
        }

        _health -= damage;

        BroadcastMessage("OnHealthChange", _health, SendMessageOptions.RequireReceiver);
    }

}