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
    [Header("Attack")]
    [SerializeField] int EatBrainDamage = 10;

    [Header("Speed")]
    [SerializeField] [Range(0f, 5f)] float _minSpeed = 0f;
    [SerializeField] [Range(0f, 5f)] float _maxSpeed = 5f;

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
    [SerializeField] AudioClip _synthSFX;
    [SerializeField] AudioClip _bulletHitSFX;
    [SerializeField] AudioClip _death01SFX;
    [SerializeField] AudioClip _death02SFX;
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

    bool _reportedAttack = false;
    bool _reportedPlayerEscape = false;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        
        _animator = GetComponent<Animator>();

        _player = _targetPlayer.GetComponent<Player>();
        _playerHealth = _targetPlayer.GetComponent<PlayerHealth>();

        _audioSource = GetComponent<AudioSource>();

        _currentState = NpcState.Idle;

        _reportedAttack = false;
        _reportedPlayerEscape = false;
    }

    void Update()
    {
        if (!Game.Instance.isGameplayOn())
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
        _navMeshAgent.isStopped = true;

        CalcDistanceToTarget();

        if (_distanceToTarget < _chaseRange)
        {
            //Debug.Log($"[NPC] Oh too close, I feel PROVOKED yummy!");
            _currentState = NpcState.Provoked;
        }
    }

    void ProvokedUpdate()
    {
        SetCollidersActive(true);

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
        // nothing here
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

        // disable colliders so player can walk over
        //SetCollidersActive(false);

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

        SetCollidersActive(true);
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
            // Report melee attack to Director
            if (_reportedPlayerEscape == false && _reportedAttack == true)
            {
                _reportedAttack = false;
                _reportedPlayerEscape = true;
                Director.Instance.OnEvent(DirectorEvents.Player_Escape);
            }

            ChaseTarget();
        }
        else if (_distanceToTarget <= _navMeshAgent.stoppingDistance)
        {            
            // Report melee attack to Director
            if (_reportedAttack == false)
            {
                _reportedAttack = true;
                _reportedPlayerEscape = false;
                Director.Instance.OnEvent(DirectorEvents.Enemy_Melee_Attack);
            }
            
            // ATTACK !!
            AttackTarget();
        }
    }

    void ChaseTarget()
    {
        if (_navMeshAgent.isStopped == true)
        {
            // Randomize speed
            float rndSpeed = Random.Range(_minSpeed, _maxSpeed);
            
            _navMeshAgent.speed = rndSpeed;
            
            //Debug.Log($"[NPC] <{transform.name}> Moving at rndSpeed: {rndSpeed}, _navMeshAgent.speed:{_navMeshAgent.speed}");

            // Go!
            _navMeshAgent.isStopped = false;            
        }

        _animator.SetTrigger("Move Trigger");

        _navMeshAgent.SetDestination(_targetPlayer.position);
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
        if (!Game.Instance.isGameplayOn())
        {
            return;
        }

        //Debug.Log("[NPC] I'm eating your brain now jojo");
        _playerHealth.Damage(EatBrainDamage);


        float rndHit = Random.value;
        if (rndHit < 0.3f)
        {
            _audioSource.PlayOneShot(_punchSFX);
        }
        else if (rndHit < 0.6f)
        {
            _audioSource.PlayOneShot(_synthSFX);
        } 
        else if (rndHit < 0.9f)
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

        PlayHitByBulletFX(hit);
    }

    void PlayHitByBulletFX(RaycastHit hit)
    {
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

    void SetCollidersActive(bool active)
    {
        GetComponent<CapsuleCollider>().enabled = active;
        GetComponent<BoxCollider>().enabled = active;
    }

    void ChangeStateToDead()
    {
        // change state
        _currentState = NpcState.Dead;

        _animator.SetTrigger("Dead Trigger");

        PlayDeathSFX();

        //Debug.Log($"[NPC] I'm a dead zombie dead, deja v�");
        _navMeshAgent.isStopped = true;

        // Disable colliders so we can't shoot after dead
        SetCollidersActive(false);
        
        // update HUD
        HUD.Instance.IncreaseKills();

        Director.Instance.OnEvent(DirectorEvents.Enemy_Killed);

        Destroy(gameObject, 1.5f);
    }

    void PlayDeathSFX()
    {
        if (Random.value < 0.4)
        {
            return;
        }

        // play SFX
        _audioSource.PlayOneShot(Random.value < 0.5 ? _death01SFX : _death02SFX);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseRange);
    }
}
