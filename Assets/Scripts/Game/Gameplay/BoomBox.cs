using FMOD.Studio;
using System.Collections;
using UnityEngine;

public class BoomBox: MonoBehaviour
{
    [Header("Target object")]
    [SerializeField] GameObject _target;
    [SerializeField] float _timeToDissapear = 0.5f;

    [Header("Explosion")]
    [SerializeField] GameObject _explosionVFX;

    [SerializeField] float _damageRadius = 1f;

    [Header("After Explosion")]
    [SerializeField] GameObject _fireAndSmokeVFX;
    [SerializeField] float _lifeTime = 5f;

    GameObject _fireZoneTrigger;

    GameObject _explosionInstance;
    GameObject _fireAndSmokeInstance;

    EventInstance _explosionSFX;

    void Awake()
    {
        _explosionSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.BarrelExplosion, transform.position);
    }

    void Start()
    {
        if (_target == null)
        {
            Debug.LogError("[BoomBox] (Start) target is not set");
            return;
        }

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
        AudioController.Instance.Play3DEvent(_explosionSFX, transform.position, true);

        // VFX
        Vector3 fxPos = new Vector3(transform.position.x, 0, transform.position.z);

        // spawn explosion
        _explosionInstance = Instantiate(_explosionVFX, fxPos, Quaternion.identity);

        // spawn fire and smoke
        _fireAndSmokeInstance = Instantiate(_fireAndSmokeVFX, fxPos, Quaternion.identity);
        
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

        //--------------------------------------------
        // FIRE ZONE
        //Debug.Log("[BoomBox] AutoDestroy) about to move fire zone trigger away");

        // workaround fire trigger exit
        _fireZoneTrigger.transform.position = new Vector3(0, -1000, 0);

        // wait until the exit is processed
        yield return new WaitForFixedUpdate();

        //Debug.Log("[BoomBox] AutoDestroy) about to disable fire zone trigger");

        _fireZoneTrigger.SetActive(false);
        //--------------------------------------------

        AudioController.Instance.ReleaseEvent(_explosionSFX);

        Destroy(_target);

        Destroy(_explosionInstance);

        Destroy(_fireAndSmokeInstance);

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _damageRadius);
    }

}
