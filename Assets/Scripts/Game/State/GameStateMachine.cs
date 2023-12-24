using UnityEngine;
using Yxp.StateMachine;

public class GameStateMachine : IGameStateMachine
{
    GameState _currentState;

    public bool TransitionToState(GameState state)
    {
        if (state == null) 
        {
            Debug.LogWarning("[GSM] (TransitionToState) Can't transition to a null state");
            return false;
        }

        if (_currentState != null && state != null && _currentState.GetType() == state.GetType())
        {
            Debug.LogWarning("[GSM] (TransitionToState) Can't transition to the same state type");
            return false;
        }

        ExitCurrentState(false);

        Debug.Log($"*** GSM Entering state {state.GetType()} ***");

        // enter new state
        state.EnterState();

        // new is the current state
        _currentState = state;

        return true;
    }

    public void Update()
    {
        if (_currentState == null)
        {
            return;
        }
        _currentState.Update();
    }

    public GameState GetCurrentState()
    {
        return _currentState;
    }

    public void Shutdown()
    {
        ExitCurrentState(true);
    }

    bool ExitCurrentState(bool isShuttingDown = false)
    {
        if (_currentState != null)
        {
            _currentState.ExitState(isShuttingDown);
            return true;
        }

        return false;
    }
}
