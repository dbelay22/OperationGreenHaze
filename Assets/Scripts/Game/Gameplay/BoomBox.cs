using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BoomBox: MonoBehaviour
{
    [Header("Target object")]
    [SerializeField] GameObject _target;
    [SerializeField] float _timeToDissapear = 0.5f;

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
        if (_target == null)
        {
            Debug.LogError("[BoomBox] (Start) target is not set");
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
        if (_target == null)
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

        // play sound
        _audioSource.PlayOneShot(_explosionSFX);

        Vector3 fxPos = new Vector3(transform.position.x, 0, transform.position.z);

        // spawn explosion
        Instantiate(_explosionVFX, fxPos, Quaternion.identity);

        // spawn fire and smoke
        Instantiate(_fireAndSmokeVFX, fxPos, Quaternion.identity);
        _fireZoneTrigger.SetActive(true);

        StartCoroutine(HideTargetDelayed(_timeToDissapear));

        // bye
        StartCoroutine(AutoDestroy());
    }

    IEnumerator HideTargetDelayed(float time)
    {
        yield return new WaitForSeconds(time);

        _target.SetActive(false);
    }

    void ProcessExplosionDamage(Vector3 center, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(center, radius);

        foreach (var collider in colliders)
        {
            if (collider.CompareTag(Tags.PLAYER))
            {
                //Debug.Log($"[BoomBox] (ProcessExplosionDamage) PLAYER affected by explosion");
                ProcessPlayerDamage(collider);
                return;
            }
        }

        foreach (var collider in colliders)
        {
            if (collider.CompareTag(Tags.ENEMY))
            {
                //Debug.Log($"[BoomBox] (ProcessExplosionDamage) ENEMY affected by explosion");
                ProcessEnemyDamage(collider);
            }
            else if (collider.CompareTag(Tags.BOOMBOX))
            {
                //Debug.Log($"[BoomBox] (ProcessExplosionDamage) BOOMBOX affected by explosion");
                ProcessChainReaction(collider);
            }
            else if (collider.CompareTag(Tags.EXIT_BLOCKER))
            {
                //Debug.Log($"[BoomBox] (ProcessExplosionDamage) EXIT_BLOCKER affected by explosion");
                ProcessExitBlocker(collider);
            }
        }
    }

    void ProcessExitBlocker(Collider collider)
    {
        if (collider.TryGetComponent<ExitBlocker>(out ExitBlocker exitBlocker))
        {
            exitBlocker.OnBoomBoxExplosion();
        }
    }

    void ProcessPlayerDamage(Collider collider)
    {
        if (collider.TryGetComponent<PlayerHealth>(out PlayerHealth playerHealth))
        {
            playerHealth.HitByExplosion();
        }
    }

    void ProcessChainReaction(Collider collider)
    {
        if (collider.TryGetComponent<BoomBox>(out BoomBox boombox))
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
        if (collider.TryGetComponent<NpcAI>(out NpcAI npc))
        {
            npc.HitByExplosion(transform);
        }        
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(_lifeTime);

        StopAllCoroutines();

        _fireZoneTrigger.SetActive(false);

        Destroy(_target);

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _damageRadius);
    }

}
