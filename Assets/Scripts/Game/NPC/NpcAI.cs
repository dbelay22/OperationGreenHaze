using FMOD.Studio;
using System.Collections;
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
    const float MAX_SPEED = 1.1f;

    const float AUDIO_STEP_MIN_DISTANCE = 12f;
    const float AUDIO_STEP_STOP_DISTANCE = 2f;

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

    [Header("Minimap")]
    [SerializeField] GameObject _minimapIcon;

    float _distanceToTarget;

    NavMeshAgent _navMeshAgent;
    
    NpcState _previousState;
    NpcState _currentState;

    Animator _animator;

    Player _player;

    BoxCollider _bodyCollider;
    CapsuleCollider _headCollider;

    bool _reportedAttack = false;
    bool _reportedPlayerEscape = false;

    float _currentSizeScale = 1;   

    float _currentHealth;

    EventInstance _zombieAppearSFX;
    EventInstance _zombieAttackSFX;
    EventInstance _zombieBlindedSFX;
    EventInstance _zombieBlindedLoopSFX;
    EventInstance _zombieDamageSFX;
    EventInstance _zombieDieSFX;
    EventInstance _zombieFallSFX;
    EventInstance _zombieHeadshotSFX;

    bool _deadByHeadshot = false;

    GameObject _model3D;

    void Awake()    
    {
        _deadByHeadshot = false;
        
        _reportedAttack = false;
        
        _reportedPlayerEscape = false;

        _headCollider = GetComponent<CapsuleCollider>();
        
        _bodyCollider = GetComponent<BoxCollider>();

        _navMeshAgent = GetComponent<NavMeshAgent>();

        _animator = GetComponent<Animator>();

        _player = _targetPlayer.GetComponent<Player>();

        _previousState = NpcState.Idle;
        _currentState = NpcState.Idle;

        RandomizeMaterial();

        RandomizeSizeScale();
        
        RandomizeMissingBodyParts();

        _model3D = transform.Find("Model").gameObject;
    }

    void Start()
    {
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
            ChangeStateFromIdleToProvoked();
        }
    }

    void ChangeStateFromIdleToProvoked()
    {
        InitializeAudioInstances();

        SetCurrentStateTo(NpcState.Provoked);
    }

    void InitializeAudioInstances()
    {   
        _zombieAppearSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.ZombieAppear, transform.position);
        _zombieAttackSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.ZombieAttack, transform.position);
        _zombieBlindedSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.ZombieBlinded, transform.position);
        _zombieBlindedLoopSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.ZombieBlindedLoop, transform.position);
        _zombieDamageSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.ZombieDamage, transform.position);
        _zombieDieSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.ZombieDie, transform.position);
        _zombieFallSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.ZombieFall, transform.position);
        _zombieHeadshotSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.ZombieHeadshot, transform.position);
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
        AudioController.Instance.Play3DEvent(_zombieBlindedSFX, transform.position, true);

        // set anim trigger
        _animator.SetTrigger("Blinded Trigger");

        // disable colliders so player can walk over
        SetCollidersActive(false);

        StartCoroutine(WakeUpFromBlinded());
    }

    IEnumerator WakeUpFromBlinded()
    {
        yield return new WaitForSeconds(_blindedTimeout);

        AudioController.Instance.StopEventIfPlaying(_zombieBlindedLoopSFX);

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

            //Debug.Log($"Zombie navigationSpeed:{navigationSpeed}, animSpeedFactor:{animSpeedFactor}, scaleSpeed: {scaleSpeed}");

            // set animator speed factor
            _animator.SetFloat("SpeedFactor", animSpeedFactor);

            StartMoving();
        }

        _animator.SetTrigger("Move Trigger");

        _navMeshAgent.SetDestination(_targetPlayer.position);

        PlayChaseSFX();
    }

    float _lastTimeChaseSFX = 0;
    
    const float CHASE_SFX_INTERVAL = 5f;

    void PlayChaseSFX()
    {
        if (isHeadless())
        {
            // no head, no screaming
            return;
        }

        if (Time.time < _lastTimeChaseSFX + CHASE_SFX_INTERVAL)
        {
            // wait, not yet
            return;
        }

        if (_distanceToTarget <= AUDIO_STEP_MIN_DISTANCE && Random.value > 0.5)
        {
            AudioController.Instance.Play3DEvent(_zombieAppearSFX, transform.position);
            
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
        if (_distanceToTarget <= AUDIO_STEP_MIN_DISTANCE && _distanceToTarget > AUDIO_STEP_STOP_DISTANCE)
        {
            _zombieSteps.PlayStepSFX();
        }
        else 
        {
            //Debug.Log($"[NpcAI] WONT PLAY STEP audio because distance ({_distanceToTarget}) is not between {AUDIO_STEP_STOP_DISTANCE} and ({AUDIO_STEP_MIN_DISTANCE})");
            _zombieSteps.StopStepSFX();
        }
    }

    public void OnZombieFallAnimEvent()
    {
        if (_distanceToTarget < AUDIO_STEP_MIN_DISTANCE)
        {
            AudioController.Instance.Play3DEvent(_zombieFallSFX, transform.position, true);

            if (_currentState.Equals(NpcState.Blinded))
            {
                AudioController.Instance.Play3DEvent(_zombieBlindedLoopSFX, transform.position, true);
            }
        }
    }

    public void OnAttackStartAnimEvent()
    {
        if (!Game.Instance.IsGameplayOn())
        {
            return;
        }

        if (!isHeadless())
        {
            AudioController.Instance.Play3DEvent(_zombieAttackSFX, transform.position, true);
        }        
    }

    public void OnAttackHitAnimEvent()
    {
        if (!Game.Instance.IsGameplayOn())
        {
            return;
        }

        _player.Damage(EatBrainDamage);
    }

    public void HitByExplosion(Transform explosionTransform)
    {
        // compute explosion force
        Vector3 forceDirection = transform.position - explosionTransform.position;
        
        forceDirection.Normalize();

        float EXPLOSION_FORCE = 20f;

        Vector3 forceVector = forceDirection * EXPLOSION_FORCE;

        Rigidbody rb = _model3D.GetComponent<Rigidbody>();

        // vuela, vuela
        rb.AddForceAtPosition(forceVector, transform.position, ForceMode.Impulse);

        // hurt
        TakeDamage(HEALTH_SCALE_ONE);
    }

    public void HitByBullet(float damage, RaycastHit hit, bool isHeadshot = false)
    {
        if (_currentState == NpcState.Idle)
        {
            ChangeStateFromIdleToProvoked();
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

        TakeDamage(damage, isHeadshot);

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
        PlayHitByBulletSFX(isHeadshot);

        PlayHitByBulletVFX(hit, isHeadshot);
    }

    void PlayHitByBulletAnim()
    {
        _animator.SetTrigger("BulletHit Trigger");
    }

    void PlayHitByBulletSFX(bool isHeadshot)
    {
        if (isHeadless() || isHeadshot)
        {
            // no head, no screaming
            return;
        }

        AudioController.Instance.Play3DEvent(_zombieDamageSFX, transform.position);
    }

    void PlayHitByBulletVFX(RaycastHit hit, bool isHeadshot = false)
    {
        if (isHeadshot)
        {
            GameObject vfx = Instantiate(_hitBulletVFX, hit.point, Quaternion.LookRotation(Vector3.zero, Vector3.up));
            Destroy(vfx, 0.4f);

            GameObject vfx2 = Instantiate(_headshotVFX, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(vfx2, 0.3f);
        }
        else
        {
            GameObject vfx = Instantiate(_hitBulletVFX, hit.point, Quaternion.LookRotation(hit.normal));
            Destroy(vfx, 0.7f);
        }
    }

    void TakeDamage(float damage, bool isHeadshot = false)
    {
        if (_currentHealth <= 0)
        {
            // dead can't dead again, does it ?
            return;
        }

        _currentHealth -= damage;

        if (_currentHealth <= 0f)
        {
            _deadByHeadshot = isHeadshot;

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

    bool isHeadless()
    {
        return _headCollider == null;
    }

    void ChangeStateToDead()
    {
        // change state
        SetCurrentStateTo(NpcState.Dead);

        StopMoving();

        _minimapIcon.SetActive(false);

        if (_deadByHeadshot)
        {
            Invoke(nameof(HideHead), 0.15f);
        }

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

    void HideHead()
    {
        GameObject head = _bodyParts[0];

        head.SetActive(false);
    }

    IEnumerator HideNPC()
    {
        yield return new WaitForSeconds(0.7f);

        _shadow.SetActive(false);

        yield return new WaitForSeconds(22f);

        _model3D.SetActive(false);

        Destroy(gameObject, 3f);
    }

    void PlayDeathSFX()
    {
        if (_deadByHeadshot)
        {
            // headshot
            AudioController.Instance.Play3DEvent(_zombieHeadshotSFX, transform.position, true);
        }
        else if (!isHeadless())
        {
            // a regular zombie with a head
            AudioController.Instance.Play3DEvent(_zombieDieSFX, transform.position, true);
        }        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseRange);
    }

    void OnDestroy()
    {
        AudioController.Instance.DestroyEvent(_zombieAppearSFX);
        AudioController.Instance.DestroyEvent(_zombieAttackSFX);
        AudioController.Instance.DestroyEvent(_zombieBlindedSFX);
        AudioController.Instance.DestroyEvent(_zombieDamageSFX);
        AudioController.Instance.DestroyEvent(_zombieDieSFX);
        AudioController.Instance.DestroyEvent(_zombieFallSFX);
        AudioController.Instance.DestroyEvent(_zombieHeadshotSFX);
    }
}
