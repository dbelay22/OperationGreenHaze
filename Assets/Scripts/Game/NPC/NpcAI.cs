using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NpcState
{ 
    Idle,
    Provoked,
    Blinded,
    Dead
}

public class NpcAI : MonoBehaviour
{
    [SerializeField] int EatBrainDamage = 10;

    [Header("Target")]
    [SerializeField] Transform _targetPlayer;
    [SerializeField] float _chaseRange = 17;
    [SerializeField] float _faceTargetSpeed = 3f;

    [Header("Blinded")]
    [SerializeField] float _blindedTimeout = 7f;

    [Header("VFX")]
    [SerializeField] GameObject _hitEnemyVFX;

    [Header("SFX")]
    [SerializeField] AudioClip _growlSFX;
    [SerializeField] AudioClip _punchSFX;
    [SerializeField] AudioClip _bulletHitSFX;
    [SerializeField] AudioClip _deathSFX;
    [SerializeField] AudioClip _blindedSFX;

    [Header("Debug")]
    [SerializeField] bool _showLogs = false;

    float _distanceToTarget;

    NavMeshAgent _navMeshAgent;

    NpcState _currentState;

    Animator _animator;

    Player _player;
    PlayerHealth _playerHealth;

    GameObject _lastBulletHitInstance;

    AudioSource _audioSource;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        
        _animator = GetComponent<Animator>();

        _player = _targetPlayer.GetComponent<Player>();
        _playerHealth = _targetPlayer.GetComponent<PlayerHealth>();

        _audioSource = GetComponent<AudioSource>();

        _currentState = NpcState.Idle;
    }

    void Update()
    {
        if (Game.Instance.isGameOver())
        {
            return;
        }

        StateUpdate();
    }

    void StateUpdate()
    {
        if (_showLogs)
        {
            Debug.Log($"[Npc] _currentState={_currentState.ToString()}");
        } 
        
        switch (_currentState)
        {
            case NpcState.Idle:
                IdleUpdate();
                break;
            case NpcState.Provoked:
                ProvokedUpdate();
                break;
            case NpcState.Blinded:
                BlindedUpdate();
                break;
            case NpcState.Dead:
                DeadUpdate();
                break;
        }
    }
    
    void IdleUpdate()
    {
        CalcDistanceToTarget();

        if (_distanceToTarget < _chaseRange)
        {
            //Debug.Log($"[NPC] Oh too close, I feel PROVOKED yummy!");
            _currentState = NpcState.Provoked;
        }
    }

    void ProvokedUpdate()
    {
        EngageTarget();
    }

    void BlindedUpdate() {
        if (_showLogs) 
        {
            Debug.Log("I can't see shit you asshole");
        }
    }

    void DeadUpdate()
    {
        //Debug.Log($"[NPC] I'm a dead zombie dead, deja vú");
        _navMeshAgent.isStopped = true;

        _animator.SetTrigger("Dead Trigger");

        Destroy(gameObject, 3f);
    }

    void ChangeStateToBlinded()
    {
        if (_currentState == NpcState.Blinded)
        {
            return;
        }

        if (_showLogs)
        {
            Debug.Log("[NPC] (ChangeStateToBlinded)");
        }

        // set state
        _currentState = NpcState.Blinded;

        // stop moving
        _navMeshAgent.isStopped = true;

        // play sfx
        _audioSource.PlayOneShot(_blindedSFX);

        // set anim trigger
        _animator.SetTrigger("Blinded Trigger");

        StartCoroutine(WakeUpFromBlinded());
    }

    IEnumerator WakeUpFromBlinded()
    {
        if (_showLogs)
        {
            Debug.Log($"[NPC] I'm gonna be blind for {_blindedTimeout} seconds");
        }

        yield return new WaitForSeconds(_blindedTimeout);

        if (_showLogs)
        {
            Debug.Log($"[NPC] I CAN SEEEEEEE");
        }

        _currentState = NpcState.Provoked;
    }

    void CalcDistanceToTarget()
    {
        _distanceToTarget = Vector3.Distance(transform.position, _targetPlayer.position);
        //Debug.Log($"[NPC] distance to player is {_distanceToTarget}");
    }
    
    void EngageTarget()
    {
        FaceTarget();
        
        CalcDistanceToTarget();

        if (_distanceToTarget > _navMeshAgent.stoppingDistance)
        {
            ChaseTarget();
        }
        else if (_distanceToTarget <= _navMeshAgent.stoppingDistance)
        {
            AttackTarget();
        }
    }

    void ChaseTarget()
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(_targetPlayer.position);

        _animator.SetTrigger("Move Trigger");
    }

    void FaceTarget()
    {
        Vector3 lookDirection = (_targetPlayer.transform.position - transform.position).normalized;
        
        Quaternion targetRotation = Quaternion.LookRotation(lookDirection);

        Quaternion nextRotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _faceTargetSpeed);
        
        transform.rotation = nextRotation;
    }

    void AttackTarget()
    {
        //Debug.Log($"[NPC] Attacking player!");
        _navMeshAgent.isStopped = true;

        _animator.SetTrigger("Attack Trigger");

        if (_player.IsFlashlightOnAndCanBlind())
        {
            ChangeStateToBlinded();
        }
    }

    public void AttackHitAnimEvent()
    {
        //Debug.Log("[NPC] I'm eating your brain now jojo");
        _playerHealth.Damage(EatBrainDamage);

        if (Random.value < 0.6f)
        {
            _audioSource.PlayOneShot(_punchSFX);
        }

        if (Random.value < 0.3f)
        {
            _audioSource.PlayOneShot(_growlSFX);
        }
    }

    public void HitByBullet(float damage, RaycastHit hit)
    {
        if (_currentState == NpcState.Idle)
        {
            //Debug.Log($"[NpcAI] (HitByBullet) was IDLE, now PROVOKED");
            _currentState = NpcState.Provoked;
        }

        BroadcastMessage("OnHitByBullet", damage, SendMessageOptions.RequireReceiver);

        PlayHitEnemySFX();

        PlayHitEnemyVFX(hit);
    }

    void PlayHitEnemySFX()
    {
        _audioSource.PlayOneShot(_bulletHitSFX);
    }

    void PlayHitEnemyVFX(RaycastHit hit)
    {
        if (_hitEnemyVFX == null)
        {
            return;
        }

        GameObject vfx = Instantiate(_hitEnemyVFX, hit.point, Quaternion.LookRotation(hit.normal));
                
        Destroy(vfx, 0.7f);
    }

    void OnHealthChange(float health)
    {
        //Debug.Log($"[NpcAI] OnHealthChange [{health}]");

        if (health <= 0f)
        {
            ChangeStateToDead();
        }
    }

    void ChangeStateToDead()
    {
        // Disable colliders so we can't shoot after dead
        GetComponent<CapsuleCollider>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;

        // play SFX
        _audioSource.PlayOneShot(_deathSFX);

        // update HUD
        HUD.Instance.IncreaseKills();

        // change state
        _currentState = NpcState.Dead;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseRange);
    }
}
