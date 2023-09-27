using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerHealth : MonoBehaviour
{
    const int START_HEALTH = 100;
    const string TOXIC_ZONE_TRIGGER = "ToxicZone";
    const string FIRE_ZONE_TRIGGER = "FireZone";


    [Header("Health")]
    [SerializeField] int _toxicZoneDamage = 15;
    [SerializeField] int _fireZoneDamage = 15;
    [SerializeField] int _currentHealth;

    public float CurrentHealth { get { return _currentHealth; } }

    public float CurrentHealthPercentage { get { return _currentHealth / 100f; } }


    [Header("Sound FX")]
    [SerializeField] AudioClip _hit1SFX;
    [SerializeField] AudioClip _hit2SFX;
    [SerializeField] AudioClip _tosSFX;
    [SerializeField] AudioClip _dieSFX;

    AudioSource _audioSource;

    void Start()
    {
        _currentHealth = START_HEALTH;

        _audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        GameUI.Instance.UpdateHealthAmmount(GetCurrentHealthClamped());
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject trigger = other.gameObject;

        ProcessToxicZone(trigger);
        
        ProcessFireZone(trigger);
    }

    void ProcessToxicZone(GameObject trigger)
    {
        bool isToxicZone = trigger.CompareTag(TOXIC_ZONE_TRIGGER);

        if (isToxicZone)
        {
            // take damage
            HealthUpdate(0 - _toxicZoneDamage);

            // play TOS
            _audioSource.PlayOneShot(_tosSFX);
        }
    }    
    
    void ProcessFireZone(GameObject trigger)
    {
        bool isFireZone = trigger.CompareTag(FIRE_ZONE_TRIGGER);

        if (isFireZone)
        {
            // take damage
            HealthUpdate(0 - _fireZoneDamage);

            // play sfx
            _audioSource.PlayOneShot(_hit2SFX);
        }
    }

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

        // more health!
        HealthUpdate(amount);
    }

    void PlayHitSFX()
    {
        float randomness = Random.value;
        if (randomness <= 0.33f)
        {
            _audioSource.PlayOneShot(_hit1SFX);
        }
        else if (randomness <= 0.55f)
        {
            _audioSource.PlayOneShot(_hit2SFX);
        }        
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
        //Debug.Log($"[PlayerHealth] _currentHealth:{_currentHealth} -> clamped: {clampedHealth}");

        _currentHealth = clampedHealth;

        // update UI
        GameUI.Instance.UpdateHealthAmmount(clampedHealth);

        if (_currentHealth <= 0)
        {
            BroadcastMessage("OnPlayerDeath", SendMessageOptions.RequireReceiver);

            _audioSource.PlayOneShot(_dieSFX, 3.3f);

            Game.Instance.ChangeStateToGameOver();
        }
    }

    public void HitByExplosion()
    {
        Damage(100);
    }

    int GetCurrentHealthClamped()
    {
        return Mathf.Clamp(_currentHealth, 0, 100);
    }

}
