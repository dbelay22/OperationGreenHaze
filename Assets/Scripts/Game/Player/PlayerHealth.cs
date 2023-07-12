using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    const float START_HEALTH = 100f;

    [Header("Health")]
    [SerializeField] float _currentHealth;


    void Start()
    {
        _currentHealth = START_HEALTH;
    }

    public void Damage(float amount)
    {
        _currentHealth -= amount;

        Debug.Log($"[PlayerHealth] _currentHealth:{_currentHealth}");

        if (_currentHealth < 0)
        {
            Debug.Log("[PlayerHealth] Oh I'm so dead now x-x");
        }
    }
}
