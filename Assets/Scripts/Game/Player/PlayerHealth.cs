using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerHealth : MonoBehaviour
{
    const int START_HEALTH = 100;
    const string TOXIC_ZONE_TRIGGER = "ToxicZone";

    [Header("Health")]
    [SerializeField] int _toxicZoneDamage = 15;
    [SerializeField] int _currentHealth;
    

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
        HUD.Instance.UpdateHealthAmmount(_currentHealth);
    }

    void OnTriggerEnter(Collider other)
    {
        GameObject trigger = other.gameObject;

        ProcessToxicZone(trigger);
    }

    void ProcessToxicZone(GameObject trigger)
    {
        bool isToxicZone = trigger.CompareTag(TOXIC_ZONE_TRIGGER);

        if (isToxicZone)
        {
            // take damage
            _currentHealth -= _toxicZoneDamage;

            // update UI
            HUD.Instance.UpdateHealthAmmount(_currentHealth);

            // play TOS
            _audioSource.PlayOneShot(_tosSFX);
        }
    }

    public void Damage(int amount)
    {
        if (Game.Instance.isGameOver()) 
        {
            return;
        }

        // take damage
        _currentHealth -= amount;

        // update UI
        HUD.Instance.UpdateHealthAmmount(_currentHealth);

        PlayHitSFX();

        HUD.Instance.ShowPlayerDamageVFX();

        //Debug.Log($"[PlayerHealth] _currentHealth:{_currentHealth}");

        if (_currentHealth <= 0)
        {
            //Debug.Log("[PlayerHealth] Oh I'm so dead now x-x");

            BroadcastMessage("OnPlayerDeath", SendMessageOptions.RequireReceiver);
                        
            _audioSource.PlayOneShot(_dieSFX, 3.3f);

            Game.Instance.ForceGameOver();
        }
    }

    void PlayHitSFX()
    {
        float randomness = Random.value;
        if (randomness <= 0.3f)
        {
            _audioSource.PlayOneShot(_hit1SFX);
        }
        else if (randomness <= 0.6f)
        {
            _audioSource.PlayOneShot(_hit2SFX);
        }        
    }
}
