using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NpcState
{ 
    Idle,
    IdleWaitingReaction,
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

    [Header("Provoked")]
    [SerializeField] [Range(0f, 10f)] float _maxProvokedReactionTime = 0f;

    [Header("Blinded")]
    [SerializeField] float _blindedTimeout = 7f;

    [Header("VFX")]
    [SerializeField] GameObject _hitBulletVFX;
    [SerializeField] GameObject _headshotVFX;
    [SerializeField] GameObject _deadVFX;

    [Header("SFX")]
    [SerializeField] AudioClip _growlSFX;
    [SerializeField] AudioClip _punchSFX;
    [SerializeField] AudioClip _synthSFX;
    [SerializeField] AudioClip _bulletHitSFX;
    [SerializeField] AudioClip _death01SFX;
    [SerializeField] AudioClip _death02SFX;
    [SerializeField] AudioClip _blindedSFX;

    [Header("Minimap")]
    [SerializeField] GameObject _minimapIcon;

    [Header("Debug")]
    [SerializeField] bool _showLogs = false;

    float _distanceToTarget;

    NavMeshAgent _navMeshAgent;

    NpcState _currentState;

    Animator _animator;

    Player _player;
    PlayerHealth _playerHealth;

    AudioSource _audioSource;

    bool _reportedAttack = false;
    bool _reportedPlayerEscape = false;

    float _currentSizeScale = 1;

    readonly float[] DEFAULT_SIZE_SCALE_RANGE = { 1f, 1f };
    readonly float[] BIG_SIZE_SCALE_RANGE = { 1.2f, 1.35f };

    public float SizeScale { get { return _currentSizeScale; } }

    void Awake()
    {
        RandomizeSizeScale();
    }

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

        _minimapIcon.SetActive(true);
    }

    void RandomizeSizeScale()
    {
        if (Random.value < 0.6f)
        {
            _currentSizeScale = Random.Range(DEFAULT_SIZE_SCALE_RANGE[0], DEFAULT_SIZE_SCALE_RANGE[1]);
        }
        else         
        {
            _currentSizeScale = Random.Range(BIG_SIZE_SCALE_RANGE[0], BIG_SIZE_SCALE_RANGE[1]);
        }

        //Debug.Log($"[NPC] (RandomizeScale) _sizeScale: {_currentSizeScale}");

        transform.localScale = new Vector3(_currentSizeScale, _currentSizeScale, _currentSizeScale);
    }

    void Update()
    {
        if (!Game.Instance.IsGameplayOn())
        {
            return;
        }

        StateUpdate();
    }

    void StateUpdate()
    {
        if (_showLogs)
        {
            Debug.Log($"[Npc] _currentState={_currentState}");
        } 
        
        switch (_currentState)
        {
            case NpcState.Idle:
                IdleUpdate();
                break;
            case NpcState.IdleWaitingReaction:
                // nothing, just wait
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
            StartCoroutine(ChangeStateToProvokedDelayed());
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

    IEnumerator ChangeStateToProvokedDelayed()
    {
        _currentState = NpcState.IdleWaitingReaction;

        yield return null;

        if (_maxProvokedReactionTime > 0)
        {
            float time = Random.Range(0f, _maxProvokedReactionTime);

            //Debug.Log($"[NPC] (ChangeStateToProvokedDelayed) [{transform.parent.name}/{transform.name}] time: {time}");

            yield return new WaitForSeconds(time);
        }

        ChangeStateToProvokedNow();
    }

    void ChangeStateToProvokedNow()
    {
        _currentState = NpcState.Provoked;
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
        SetCollidersActive(false);

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

        ChangeStateToProvokedNow();

        SetCollidersActive(true);
    }

    void CalcDistanceToTarget()
    {
        _distanceToTarget = Vector3.Distance(transform.position, _targetPlayer.position);
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
            // big zombies are a little slower
            float scaleSpeed = _currentSizeScale >= BIG_SIZE_SCALE_RANGE[0] ? 0.85f : 1f;

            // Randomize speed
            float rndSpeed = Random.Range(_minSpeed * scaleSpeed, _maxSpeed * scaleSpeed);

            //Debug.Log($"[NPC] rndSpeed: {rndSpeed}, scaleSpeed: {scaleSpeed}, _sizeScale: {_sizeScale}");

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
        _navMeshAgent.isStopped = true;

        _animator.SetTrigger("Attack Trigger");

        if (_player.IsFlashlightOnAndCanBlind())
        {
            ChangeStateToBlinded();
        }
    }

    public void AttackHitAnimEvent()
    {
        if (!Game.Instance.IsGameplayOn())
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

    public void HitByExplosion(Transform explosionTransform)
    {
        Vector3 forceDirection = transform.position - explosionTransform.position;
        
        forceDirection.Normalize();

        float EXPLOSION_FORCE = 20f;

        Vector3 forceVector = forceDirection * EXPLOSION_FORCE;

        Rigidbody rb = Get3DModel().GetComponent<Rigidbody>();

        // vuela, vuela
        rb.AddForceAtPosition(forceVector, transform.position, ForceMode.Impulse);
    }

    public void HitByBullet(float damage, RaycastHit hit, bool isHeadshot = false)
    {
        if (_currentState == NpcState.Idle)
        {
            ChangeStateToProvokedNow();
        }

        BroadcastMessage("OnHitByBullet", damage, SendMessageOptions.RequireReceiver);

        PlayHitByBulletFX(hit, isHeadshot);
    }

    void PlayHitByBulletFX(RaycastHit hit, bool isHeadshot = false)
    {
        PlayHitByBulletSFX();

        PlayHitByBulletVFX(hit, isHeadshot);
    }

    void PlayHitByBulletSFX()
    {
        _audioSource.PlayOneShot(_bulletHitSFX);
    }

    void PlayHitByBulletVFX(RaycastHit hit, bool isHeadshot = false)
    {
        GameObject prefabVfx = isHeadshot ? _headshotVFX : _hitBulletVFX;

        GameObject vfx = Instantiate(prefabVfx, hit.point, Quaternion.LookRotation(hit.normal));
                
        Destroy(vfx, 0.7f);
    }

    void OnHealthChange(float health)
    {
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

        _minimapIcon.SetActive(false);

        _animator.SetTrigger("Dead Trigger");

        PlayDeathSFX();

        //Debug.Log($"[NPC] I'm a dead zombie dead, deja vú");
        _navMeshAgent.isStopped = true;

        // Disable colliders so we can't shoot after dead
        SetCollidersActive(false);
        
        // update GameUI
        GameUI.Instance.IncreaseKills();

        Director.Instance.OnEvent(DirectorEvents.Enemy_Killed);

        StartCoroutine(HideNPC());
    }

    IEnumerator HideNPC()
    {
        yield return new WaitForSeconds(2.2f);

        Get3DModel().SetActive(false);

        Destroy(gameObject, 3f);
    }

    GameObject Get3DModel() 
    {
        return transform.Find("Model").gameObject;
    }

    void PlayDeathSFX()
    {
        if (Random.value < 0.5)
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
