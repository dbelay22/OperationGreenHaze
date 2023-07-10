using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NpcState
{ 
    Idle,
    Provoked,
    Dead
}

public class NpcAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] Transform _targetPlayer;
    [SerializeField] float _chaseRange = 17;

    float _distanceToTarget;

    NavMeshAgent _navMeshAgent;

    NpcState _currentState;


    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _currentState = NpcState.Idle;
    }

    void Update()
    {
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
            case NpcState.Dead:
                DeadUpdate();
                break;
        }
    }

    void DeadUpdate()
    {
        Debug.Log($"[NPC] I'm a dead zombie dead, deja vú");
    }

    void IdleUpdate()
    {
        _distanceToTarget = Vector3.Distance(transform.position, _targetPlayer.position);
        
        Debug.Log($"[NPC] distance to player is {_distanceToTarget}");

        if (_distanceToTarget < _chaseRange)
        {
            Debug.Log($"[NPC] Oh too close, I feel PROVOKED yummy!");
            _currentState = NpcState.Provoked;
        }
    }

    void ProvokedUpdate()
    {
        EngageTartet();
    }

    void EngageTartet()
    {
        _navMeshAgent.SetDestination(_targetPlayer.position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseRange);
    }
}
