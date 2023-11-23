using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum NpcState
{ 
    Idle,
    Provoked,
    Blinded,
    HitHyBullet,
    Dead
}

public class NpcAI : MonoBehaviour
{
    const float HEALTH_SCALE_ONE = 100f;
    const float DEFAULT_ANIM_SPEED = 0.5f;

    const float MIN_SPEED = 0.5f;
    const float MAX_SPEED = 1.25f;

    readonly float[] DEFAULT_SIZE_SCALE_RANGE = { 1f, 1f };
    readonly float[] BIG_SIZE_SCALE_RANGE = { 1.2f, 1.35f };

    const string HEAD_GO_NAME = "Z_Head";

    [Header("Components")]
    [SerializeField] ZombieSteps _zombieSteps;    

    [Header("Missing parts")]
    [SerializeField] GameObject[] _bodyParts;

    [Header("Material randomization")]
    [SerializeField] Material[] _materials;
    [SerializeField] Renderer[] _renderers;

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
    [SerializeField] GameObject _shadow;
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

    float _distanceToTarget;

    NavMeshAgent _navMeshAgent;
    
    NpcState _previousState;
    NpcState _currentState;

    Animator _animator;

    Player _player;

    AudioSource _audioSource;

    BoxCollider _bodyCollider;
    CapsuleCollider _headCollider;

    bool _reportedAttack = false;
    bool _reportedPlayerEscape = false;

    float _currentSizeScale = 1;   

    float _currentHealth;

    void Awake()
    {
        _headCollider = GetComponent<CapsuleCollider>();
        
        _bodyCollider = GetComponent<BoxCollider>();

        RandomizeMaterial();

        RandomizeSizeScale();
        
        RandomizeMissingBodyParts();        
    }

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _animator = GetComponent<Animator>();

        _player = _targetPlayer.GetComponent<Player>();

        _audioSource = GetComponent<AudioSource>();

        _previousState = NpcState.Idle;
        _currentState = NpcState.Idle;

        _reportedAttack = false;
        _reportedPlayerEscape = false;

        _minimapIcon.SetActive(true);
    }

    void SetCurrentStateTo(NpcState state)
    {
        if (_currentState.Equals(NpcState.Dead))
        {
            return;
        }       

        _previousState = _currentState;

        _currentState = state;

        //Debug.Log($"SetCurrentStateTo) _currentState: {_currentState}, _previousState:{_previousState}");
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

        transform.localScale = new Vector3(_currentSizeScale, _currentSizeScale, _currentSizeScale);

        _currentHealth = HEALTH_SCALE_ONE * _currentSizeScale;
    }

    void RandomizeMissingBodyParts()
    {
        if (Random.value < 0.5f)
        {
            // no missing parts for you
            return;
        }

        int partIndex = Random.Range(0, _bodyParts.Length);

        GameObject bodyPart = _bodyParts[partIndex];

        bodyPart.SetActive(false);

        if (bodyPart.name.Equals(HEAD_GO_NAME))
        {
            Destroy(_headCollider);

            _headCollider = null;
        }
    }

    void RandomizeMaterial()
    {
        if (Random.value < 0.3f)
        {
            // no material change for you
            return;
        }

        int index = Random.Range(0, _materials.Length);

        Material material = _materials[index];

        foreach (Renderer r in _renderers)
        {
            r.material = material;
        }
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
        switch (_currentState)
        {
            case NpcState.Idle:
                IdleUpdate();
                break;
            case NpcState.Provoked:
                ProvokedUpdate();
                break;
        }
    }

    void IdleUpdate()
    {
        StopMoving();

        CalcDistanceToTarget();

        if (_distanceToTarget < _chaseRange)
        {
            SetCurrentStateTo(NpcState.Provoked);
        }
    }

    void ProvokedUpdate()
    {
        SetCollidersActive(true);

        EngageTarget();
    }

    void ChangeStateToBlinded()
    {
        if (_currentState == NpcState.Blinded)
        {
            return;
        }

        // set state
        SetCurrentStateTo(NpcState.Blinded);

        StopMoving();

        // play sfx
        PlayAudioClip(_blindedSFX);

        // set anim trigger
        _animator.SetTrigger("Blinded Trigger");

        // disable colliders so player can walk over
        SetCollidersActive(false);

        StartCoroutine(WakeUpFromBlinded());
    }

    IEnumerator WakeUpFromBlinded()
    {
        yield return new WaitForSeconds(_blindedTimeout);

        SetCurrentStateTo(NpcState.Provoked);

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
            // Report "player escape" to Director
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
            // Report "enemy melee attack" to Director
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
        if (!IsMoving())
        {
            // big zombies are a little slower
            float scaleSpeed = _currentSizeScale >= BIG_SIZE_SCALE_RANGE[0] ? 0.9f : 1f;

            // Randomize speed // IGNORE custom values
            //float navigationSpeed = Random.Range(_minSpeed * scaleSpeed, _maxSpeed * scaleSpeed);
            float navigationSpeed = Random.Range(MIN_SPEED * scaleSpeed, MAX_SPEED * scaleSpeed);

            // set navigation speed
            _navMeshAgent.speed = navigationSpeed;

            // animation speed factor
            float animSpeedFactor = navigationSpeed / DEFAULT_ANIM_SPEED;

            Debug.Log($"Zombie navigationSpeed:{navigationSpeed}, animSpeedFactor:{animSpeedFactor}, scaleSpeed: {scaleSpeed}");

            // set animator speed factor
            _animator.SetFloat("SpeedFactor", animSpeedFactor);

            StartMoving();
        }

        _animator.SetTrigger("Move Trigger");

        _navMeshAgent.SetDestination(_targetPlayer.position);

        PlayChaseSFX();
    }

    float _lastTimeChaseSFX = 0;
    
    const float CHASE_SFX_INTERVAL = 7f;

    void PlayChaseSFX()
    {

        if (Time.time < _lastTimeChaseSFX + CHASE_SFX_INTERVAL)
        {
            // wait, not yet
            return;
        }

        if (Random.value > 0.5)
        {
            float rndSound = Random.value;

            PlayAudioClip(rndSound > 0.7 ? _synthSFX : _growlSFX);
            
            _lastTimeChaseSFX = Time.time;
        }
    }

    bool IsMoving()
    {
        return (_navMeshAgent.isStopped == false);
    }

    void StartMoving()
    {
        _navMeshAgent.isStopped = false;

        _zombieSteps.OnNPCStartWalking();
    }

    void StopMoving()
    {
        _navMeshAgent.isStopped = true;

        _zombieSteps.OnNPCStoppedWalking();
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
        StopMoving();

        _animator.SetTrigger("Attack Trigger");

        if (_player.IsFlashlightOnAndCanBlind())
        {
            ChangeStateToBlinded();
        }
    }

    public void OnZombieStepAnimEvent()
    {
        _zombieSteps.PlayStepSFX();
    }

    public void OnAttackHitAnimEvent()
    {
        if (!Game.Instance.IsGameplayOn())
        {
            return;
        }

        _player.Damage(EatBrainDamage);

        float rndHit = Random.value;
        if (rndHit < 0.3f)
        {
            PlayAudioClip(_punchSFX);
        }
        else if (rndHit < 0.6f)
        {
            PlayAudioClip(_synthSFX);
        } 
        else if (rndHit < 0.9f)
        {
            PlayAudioClip(_growlSFX);
        }
    }

    public void HitByExplosion(Transform explosionTransform)
    {
        // compute explosion force
        Vector3 forceDirection = transform.position - explosionTransform.position;
        
        forceDirection.Normalize();

        float EXPLOSION_FORCE = 20f;

        Vector3 forceVector = forceDirection * EXPLOSION_FORCE;

        Rigidbody rb = Get3DModel().GetComponent<Rigidbody>();

        // vuela, vuela
        rb.AddForceAtPosition(forceVector, transform.position, ForceMode.Impulse);

        // hurt
        TakeDamage(HEALTH_SCALE_ONE);
    }

    public void HitByBullet(float damage, RaycastHit hit, bool isHeadshot = false)
    {
        if (_currentState == NpcState.Idle)
        {
            SetCurrentStateTo(NpcState.Provoked);
        }
        else if (_currentState == NpcState.Dead) 
        {
            return;
        } 
        else
        {
            ChangeStateToHitByBullet(damage, hit, isHeadshot);
        }
    }



    void ChangeStateToHitByBullet(float damage, RaycastHit hit, bool isHeadshot = false)
    {
        StopMoving();

        PlayHitByBulletAnim();

        PlayHitByBulletFX(hit, isHeadshot);

        SetCurrentStateTo(NpcState.HitHyBullet);

        TakeDamage(damage);

        StartCoroutine(ChangeStateDelayed(0.5f, NpcState.Provoked));
    }

    IEnumerator ChangeStateDelayed(float time, NpcState nextState)
    {
        if (!_currentState.Equals(NpcState.Dead))
        {
            yield return new WaitForSeconds(time);

            SetCurrentStateTo(nextState);
        }
        else
        {
            //Debug.Log($"ChangeStateDelayed) ALREADY DEAD, will NOT change to: {nextState}");

            yield return null;
        }
    }

    void PlayHitByBulletFX(RaycastHit hit, bool isHeadshot = false)
    {
        PlayHitByBulletSFX();

        PlayHitByBulletVFX(hit, isHeadshot);
    }

    void PlayHitByBulletAnim()
    {
        _animator.SetTrigger("BulletHit Trigger");
    }

    void PlayHitByBulletSFX()
    {
        PlayAudioClip(_bulletHitSFX);
    }

    void PlayHitByBulletVFX(RaycastHit hit, bool isHeadshot = false)
    {
        GameObject prefabVfx = isHeadshot ? _headshotVFX : _hitBulletVFX;

        GameObject vfx = Instantiate(prefabVfx, hit.point, Quaternion.LookRotation(hit.normal));
                
        Destroy(vfx, 0.7f);
    }

    void TakeDamage(float damage)
    {
        if (_currentHealth <= 0)
        {
            // dead can't dead again, does it ?
            return;
        }

        _currentHealth -= damage;

        if (_currentHealth <= 0f)
        {
            ChangeStateToDead();
        }
    }

    void SetCollidersActive(bool active)
    {
        if (_headCollider != null) 
        {
            _headCollider.enabled = active;
        }
        _bodyCollider.enabled = active;
    }

    void ChangeStateToDead()
    {
        // change state
        SetCurrentStateTo(NpcState.Dead);

        StopMoving();

        _minimapIcon.SetActive(false);

        string rndDeadTrigger = Random.value < 0.5f ? "Dead Trigger" : "Dead Fwd Trigger";
        _animator.SetTrigger(rndDeadTrigger);

        PlayDeathSFX();

        // Disable colliders so we can't shoot after dead
        SetCollidersActive(false);
        
        // update GameUI
        GameUI.Instance.IncreaseKills();

        Director.Instance.OnEvent(DirectorEvents.Enemy_Killed);

        StartCoroutine(HideNPC());
    }

    IEnumerator HideNPC()
    {
        yield return new WaitForSeconds(0.7f);

        _shadow.SetActive(false);

        yield return new WaitForSeconds(22f);

        Get3DModel().SetActive(false);

        Destroy(gameObject, 3f);
    }

    GameObject Get3DModel() 
    {
        return transform.Find("Model").gameObject;
    }

    void PlayDeathSFX()
    {
        if (Random.value > 0.3)
        {
            PlayAudioClip(Random.value > 0.5 ? _death01SFX : _death02SFX);
        }
    }

    bool PlayAudioClip(AudioClip clip)
    {
        if (_audioSource.isPlaying)
        {
            return false;
        }

        _audioSource.PlayOneShot(clip);

        return true;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseRange);
    }
}
