using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    const int START_HEALTH = 100;
    
    const string TOXIC_ZONE_TRIGGER = "ToxicZone";
    const string FIRE_ZONE_TRIGGER = "FireZone";
    
    const float DAMAGE_ZONE_INTERVAL = 3f;

    [Header("Health")]
    [SerializeField] int _toxicZoneDamage = 15;
    [SerializeField] int _fireZoneDamage = 15;
    [SerializeField] int _currentHealth;
    public float CurrentHealth { get { return _currentHealth; } }
    public float CurrentHealthPercentage { get { return _currentHealth / 100f; } }

    EventInstance _damageByZombieSFX;
    EventInstance _damageByFireSFX;
    EventInstance _damageByGasSFX;
    EventInstance _deathSFX;

    float _timeSinceLastDamage = 0f;

    void Start()
    {
        _currentHealth = START_HEALTH;

        _damageByZombieSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.PlayerDamageByZombie, transform.position);

        _damageByFireSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.PlayerDamageByFire, transform.position);
        
        _damageByGasSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.PlayerDamageByGas, transform.position);

        _deathSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.PlayerDeath, transform.position);
    }

    void Update()
    {
        GameUI.Instance.UpdateHealthAmmount(GetCurrentHealthClamped());
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject trigger = other.gameObject;

        if (!ProcessToxicZoneEnter(trigger))
        {
            ProcessFireZoneEnter(trigger);
        }        
    }

    void OnTriggerStay(Collider other)
    {
        GameObject trigger = other.gameObject;

        if (!ProcessToxicZoneStay(trigger))
        {
            ProcessFireZoneStay(trigger);
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject trigger = other.gameObject;

        if (!ProcessToxicZoneExit(trigger))
        {
            ProcessFireZoneExit(trigger);
        }
    }

    #region ToxicZone

    bool ProcessToxicZoneEnter(GameObject trigger)
    {
        if (!trigger.CompareTag(TOXIC_ZONE_TRIGGER))
        {
            return false;
        }

        // take damage
        HealthUpdate(0 - _toxicZoneDamage);

        // play TOS
        AudioController.Instance.Play3DEvent(_damageByGasSFX, transform.position, true);

        _timeSinceLastDamage = 0f;

        return true;
    }

    bool ProcessToxicZoneStay(GameObject trigger)
    {
        if (!trigger.CompareTag(TOXIC_ZONE_TRIGGER))
        {
            return false;
        }

        _timeSinceLastDamage += Time.deltaTime;

        if (_timeSinceLastDamage >= DAMAGE_ZONE_INTERVAL)
        {
            // take damage
            HealthUpdate(0 - _toxicZoneDamage);

            _timeSinceLastDamage = 0f;
        }

        return true;
    }

    bool ProcessToxicZoneExit(GameObject trigger)
    {
        if (!trigger.CompareTag(TOXIC_ZONE_TRIGGER))
        {
            return false;           
        }

        AudioController.Instance.StopEvent(_damageByGasSFX);

        return true;
    }

    #endregion ToxicZone


    #region FireZone

    bool ProcessFireZoneEnter(GameObject trigger)
    {
        if (!trigger.CompareTag(FIRE_ZONE_TRIGGER))
        {
            return false;
        }

        // take damage
        HealthUpdate(0 - _fireZoneDamage);

        // play sfx
        AudioController.Instance.Play3DEvent(_damageByFireSFX, transform.position, true);

        _timeSinceLastDamage = 0f;

        return true;
    }

    bool ProcessFireZoneStay(GameObject trigger)
    {
        if (!trigger.CompareTag(FIRE_ZONE_TRIGGER))
        {
            return false;
        }

        _timeSinceLastDamage += Time.deltaTime;

        if (_timeSinceLastDamage >= DAMAGE_ZONE_INTERVAL)
        {
            // take damage
            HealthUpdate(0 - _fireZoneDamage);

            _timeSinceLastDamage = 0f;
        }

        return true;
    }

    bool ProcessFireZoneExit(GameObject trigger)
    {
        if (!trigger.CompareTag(FIRE_ZONE_TRIGGER))
        {
            return false;            
        }

        AudioController.Instance.StopEvent(_damageByFireSFX);

        return true;
    }

    #endregion FireZone

    protected internal void Damage(int amount)
    {
        if (!Game.Instance.IsGameplayOn())
        {
            return;
        }

        // take damage
        HealthUpdate(0 - amount);

        PlayHitSFX();

        GameUI.Instance.ShowPlayerDamageVFX();

        Director.Instance.OnEvent(DirectorEvents.Player_Damaged);
    }

    public void ImproveByPickup(int amount)
    {
        if (!Game.Instance.IsGameplayOn())
        {
            return;
        }

        bool wasBadlyHurt = IsBadlyHurt();

        // more health!
        HealthUpdate(amount);

        if (wasBadlyHurt && !IsBadlyHurt())
        {
            GameUI.Instance.HidePlayerBadlyHurt();
        }
    }

    void PlayHitSFX()
    {        
        AudioController.Instance.Play3DEvent(_damageByZombieSFX, transform.position, true);
    }

    void HealthUpdate(int amount)
    {
        if (Game.Instance.IsGodModeOn)
        {
            return;
        }

        _currentHealth += amount;

        // clamp
        int clampedHealth = GetCurrentHealthClamped();
        
        Debug.Log($"[PlayerHealth] _currentHealth:{_currentHealth} -> clamped: {clampedHealth}");

        _currentHealth = clampedHealth;

        // update UI
        GameUI.Instance.UpdateHealthAmmount(clampedHealth);

        if (_currentHealth <= 0)
        {
            BroadcastMessage("OnPlayerDeath", SendMessageOptions.RequireReceiver);

            // sound!
            AudioController.Instance.Play3DEvent(_deathSFX, transform.position, true);

            Game.Instance.ChangeStateToGameOver();
        }
        else if (IsBadlyHurt())
        {
            GameUI.Instance.ShowPlayerBadlyHurt();
        }          
    }

    public bool IsBadlyHurt()
    {
        return CurrentHealthPercentage < 0.5;
    }

    public void HitByExplosion()
    {
        Damage(100);
    }

    int GetCurrentHealthClamped()
    {
        return Mathf.Clamp(_currentHealth, 0, 100);
    }

    void OnPlayerDeath()
    {
        AudioController.Instance.StopEventIfPlaying(_damageByFireSFX);
        
        AudioController.Instance.StopEventIfPlaying(_damageByGasSFX);
    }

}
