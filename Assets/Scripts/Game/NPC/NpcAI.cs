using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NpcState
{ 
    SearchingPlayer,
    Dead
}

public class NpcAI : MonoBehaviour
{
    [Header("Player Target")]
    [SerializeField] Transform _targetPlayer;
    
    NavMeshAgent _navMeshAgent;

    NpcState _currentState;

    Vector3 _currentTarget;


    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _currentState = NpcState.SearchingPlayer;
    }

    void Update()
    {
        StateUpdate();
    }

    void StateUpdate()
    {
        switch (_currentState)
        {
            case NpcState.SearchingPlayer:
                SearchingPlayerUpdate();
                break;

            case NpcState.Dead:
                break;
        }
    }

    void SearchingPlayerUpdate()
    {
        _navMeshAgent.SetDestination(_targetPlayer.position);
    }


    /*
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseRange);
    }
    */
}
