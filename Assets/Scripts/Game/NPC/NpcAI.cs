using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NpcState
{ 
    Hungry,
    GoingToFood,
    Eating,
    Satisfied
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

        _currentState = NpcState.Hungry;
    }

    void Update()
    {
        ProcessState();
    }

    void ProcessState()
    {
        switch (_currentState)
        {
            case NpcState.Hungry:
                break;

            case NpcState.GoingToFood:
                break;

            case NpcState.Eating:
                break;

            case NpcState.Satisfied:
                break;
        }
    }
    
    /*
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _chaseRange);
    }
    */
}
