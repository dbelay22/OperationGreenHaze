using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mail;
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
    EventInstance _healthSFX;    

    float _timeSinceLastDamage = 0f;

    void Start()
    {
        _currentHealth = START_HEALTH;

        _damageByZombieSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.PlayerDamageByZombie, transform.position);

        _damageByFireSFX = AudioController.Instance.CreateInstance(FMODEvents.Instance.PlayerDamageByFire);
        
        _damageByGasSFX = AudioController.Instance.CreateInstance(FMODEvents.Instance.PlayerDamageByGas);

        _deathSFX = AudioController.Instance.CreateInstance(FMODEvents.Instance.PlayerDeath);

        _healthSFX = AudioController.Instance.CreateInstance(FMODEvents.Instance.PlayerHealth);
        
        _healthSFX.setParameterByName(FMODEvents.Instance.HealthParamName, _currentHealth);

        AudioController.Instance.PlayEvent(_healthSFX);
    }

    void Update()
    {
        if (!Game.Instance.IsGameplayOn())        
        {
            return;
        }

        AudioController.Instance.PlayEvent(_healthSFX);

        GameUI.Instance.UpdateHealthAmmount(GetCurrentHealthClamped());

#if UNITY_EDITOR
        // [O] Force Player Kill
        if (Input.GetKeyDown(KeyCode.O))
        {
            HealthUpdate(-999);
        }
#endif

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
        if (!Game.Instance.IsGameplayOn())
        {
            return;
        }

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

        _timeSinceLastDamage = 0f;

        return true;
    }

    bool ProcessToxicZoneStay(GameObject trigger)
    {
        if (!trigger.CompareTag(TOXIC_ZONE_TRIGGER))
        {
            return false;
        }

        if (GetCurrentHealthClamped() > 0 && !AudioController.Instance.IsEventPlaying(_damageByGasSFX))
        {
            AudioController.Instance.PlayEvent(_damageByGasSFX, true);
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

        StartCoroutine(StopFadeDamageByGasSFX(3f));

        return true;
    }

    IEnumerator StopFadeDamageByGasSFX(float time)
    {
        yield return new WaitForSeconds(time);

        AudioController.Instance.StopEvent(_damageByGasSFX);
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

        _timeSinceLastDamage = 0f;

        return true;
    }

    bool ProcessFireZoneStay(GameObject trigger)
    {
        if (!trigger.CompareTag(FIRE_ZONE_TRIGGER))
        {
            return false;
        }

        // play sfx
        if (GetCurrentHealthClamped() > 0 && !AudioController.Instance.IsEventPlaying(_damageByFireSFX))
        {
            AudioController.Instance.PlayEvent(_damageByFireSFX);
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

        StartCoroutine(StopFadeDamageByFireSFX(3f));

        return true;
    }

    IEnumerator StopFadeDamageByFireSFX(float time)
    {
        yield return new WaitForSeconds(time);

        AudioController.Instance.StopFadeEvent(_damageByFireSFX);
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
        if (Game.Instance.IsGodModeOn || _currentHealth <= 0)
        {
            return;
        }

        _currentHealth += amount;

        // clamp
        int clampedHealth = GetCurrentHealthClamped();
        
        //Debug.Log($"[PlayerHealth] _currentHealth:{_currentHealth} -> clamped: {clampedHealth}");

        _currentHealth = clampedHealth;

        // update UI
        GameUI.Instance.UpdateHealthAmmount(clampedHealth);

        // update SFX
        //Debug.Log($"PlayerHealth] HealthUpdate) Sent health param value: {_currentHealth}");
        _healthSFX.setParameterByName(FMODEvents.Instance.HealthParamName, _currentHealth);

        if (_currentHealth <= 0)
        {
            BroadcastMessage("OnPlayerDeath", SendMessageOptions.RequireReceiver);

            Game.Instance.ChangeStateToGameOver();
        }
        else if (IsVeryBadlyHurt())
        {
            GameUI.Instance.ShowPlayerVeryBadlyHurt();

            RadioTalking.Instance.PlayUseMedkit(maxPriority: true);
        }
        else if (IsBadlyHurt())
        {
            GameUI.Instance.ShowPlayerBadlyHurt();

            RadioTalking.Instance.PlayUseMedkit();
        }
    }

    public bool IsBadlyHurt()
    {
        return CurrentHealthPercentage <= 0.5;
    }

    public bool IsVeryBadlyHurt()
    {
        return CurrentHealthPercentage <= 0.35;
    }

    public void HitByExplosion()
    {
        Damage(100);
    }

    int GetCurrentHealthClamped()
    {
        return Mathf.Clamp(_currentHealth, 0, 100);
    }

    void OnGameplayOver()
    {
        //Debug.Log("PlayerHealth] OnGameplayOver)...");

        StopAllHealthSFX();
    }

    void OnPlayerDeath()
    {
        //Debug.Log("PlayerHealth] OnPlayerDeath)...");

        StopAllHealthSFX();

        AudioController.Instance.PlayEvent(_deathSFX, true);
    }

    void StopAllHealthSFX()
    {
        //Debug.Log("PlayerHealth] StopAllHealthSFX)...");

        AudioController.Instance.StopEvent(_healthSFX);

        AudioController.Instance.StopEvent(_damageByFireSFX);

        AudioController.Instance.StopEvent(_damageByGasSFX);
    }

}
