using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    const int START_HEALTH = 100;

    [Header("Health")]
    [SerializeField] int _currentHealth;

    void Start()
    {
        _currentHealth = START_HEALTH;

    }

    void Update()
    {
        HUD.Instance.UpdateHealthAmmount(_currentHealth);
    }

    public void Damage(int amount)
    {
        if (Game.Instance.isGameOver()) 
        {
            return;
        }

        _currentHealth -= amount;

        HUD.Instance.UpdateHealthAmmount(_currentHealth);

        HUD.Instance.ShowPlayerDamageVFX();

        //Debug.Log($"[PlayerHealth] _currentHealth:{_currentHealth}");

        if (_currentHealth <= 0)
        {
            //Debug.Log("[PlayerHealth] Oh I'm so dead now x-x");
            Game.Instance.ForceGameOver();
        }
    }
}
