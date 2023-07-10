using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NpcState
{ 
    Idle,
    SearchingPlayer,
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
            case NpcState.SearchingPlayer:
                SearchingPlayerUpdate();
                break;
            case NpcState.Dead:
                break;
        }
    }

    void CalcDistanceToPlayer()
    {
        _distanceToTarget = Vector3.Distance(transform.position, _targetPlayer.position);
    }

    void IdleUpdate()
    {
        CalcDistanceToPlayer();
        
        Debug.Log($"[NPC] distance to player is {_distanceToTarget}");

        if (_distanceToTarget < _chaseRange)
        {
            _currentState = NpcState.SearchingPlayer;
        }
    }

    void SearchingPlayerUpdate()
    {
        _navMeshAgent.SetDestination(_targetPlayer.position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseRange);
    }
}
