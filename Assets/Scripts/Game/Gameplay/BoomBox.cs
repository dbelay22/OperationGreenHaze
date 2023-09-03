using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BoomBox: MonoBehaviour
{
    const string ENEMY_TAG = "Enemy";
    const string BOOMBOX_TAG = "BoomBox";
    const string PLAYER_TAG = "Player";

    [SerializeField] GameObject _object;

    [Header("Explosion")]
    [SerializeField] GameObject _explosionVFX;
    [SerializeField] AudioClip _explosionSFX;
    [SerializeField] float _damageRadius = 1f;

    [Header("After Explosion")]
    [SerializeField] GameObject _fireAndSmokeVFX;
    [SerializeField] float _lifeTime = 5f;

    AudioSource _audioSource;
    
    GameObject _fireZoneTrigger;
    
    void Start()
    {
        if (_object == null)
        {
            Debug.LogError("[BoomBox] (Start) object is not set");
            return;
        }

        _audioSource = GetComponent<AudioSource>();

        FireZoneStart();
    }

    void FireZoneStart()
    {
        Transform t = transform.Find("FireZone");
        
        if (t == null)
        {
            Debug.LogError($"[BoomBox] (FireZoneStart) Can't find the FireZone: {transform.name}");
            return;
        }
        
        _fireZoneTrigger = t.gameObject;

        _fireZoneTrigger.SetActive(false);
    }

    public void BoomNow()
    {
        if (_object == null)
        {
            Debug.LogError("[BoomBox] (BoomNow) object is not set");
            return;
        }

        if (_explosionVFX == null)
        {
            Debug.LogError("[BoomBox] (BoomNow) explosion is not set");
            return;
        }

        // deactivate box collider
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.enabled = false;

        ProcessExplosionDamage(transform.position, _damageRadius);

        // object destroy
        Destroy(_object);

        // play sound
        _audioSource.PlayOneShot(_explosionSFX);

        // spawn explosion
        Instantiate(_explosionVFX, transform);

        // spawn fire and smoke
        Instantiate(_fireAndSmokeVFX, transform);
        _fireZoneTrigger.SetActive(true);

        // bye
        StartCoroutine(AutoDestroy());
    }

    void ProcessExplosionDamage(Vector3 center, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(center, radius);

        //Debug.Log($"[BoomBox] (ProcessExplosionDamage) colliders affected by explosion: {colliders.Length}");

        //bool shouldBendTime = false;

        foreach (var collider in colliders)
        {
            if (collider.CompareTag(ENEMY_TAG))
            {
                ProcessEnemyDamage(collider);
                //shouldBendTime = true;
            }
            else if (collider.CompareTag(BOOMBOX_TAG))
            {
                ProcessChainReaction(collider);
            }
            else if (collider.CompareTag(PLAYER_TAG))
            {
                ProcessPlayerDamage(collider);
                //shouldBendTime = true;
            }
            else
            {
                Debug.Log($"[BoomBox] (ProcessExplosionDamage) object {collider.name} affected by explosion");
            }            
        }

        /*
        if (shouldBendTime)
        {
            StartCoroutine(TimeBend());
        } 
        */
    }

    /*
    IEnumerator TimeBend()
    {
        Time.timeScale = 0.11f;

        yield return new WaitForSeconds(0.11f);

        Time.timeScale = 1f;
    }
    */

    void ProcessPlayerDamage(Collider collider)
    {
        PlayerHealth playerHealth;

        collider.TryGetComponent<PlayerHealth>(out playerHealth);

        if (playerHealth != null)
        {
            playerHealth.HitByExplosion();
        }
    }

    void ProcessChainReaction(Collider collider)
    {
        BoomBox boombox;

        collider.TryGetComponent<BoomBox>(out boombox);

        if (boombox != null)
        {
            StartCoroutine(ChainReactionDelayed(boombox));
        }
    }

    IEnumerator ChainReactionDelayed(BoomBox bbox)
    {
        float reactionTime = Random.Range(0.2f, 0.8f);

        yield return new WaitForSeconds(reactionTime);
        
        bbox.BoomNow();
    }

    void ProcessEnemyDamage(Collider collider)
    {
        NpcHealth npcHealth;        

        collider.TryGetComponent<NpcHealth>(out npcHealth);

        if (npcHealth != null)
        {
            npcHealth.HitByExplosion();            
        }
        
        NpcAI npc;
        collider.TryGetComponent<NpcAI>(out npc);

        if (npc != null)
        {
            npc.HitByExplosion(transform);
        }
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(_lifeTime);

        StopAllCoroutines();

        _fireZoneTrigger.SetActive(false);

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _damageRadius);
    }

}
