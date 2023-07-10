namespace Yxp.StateMachine
{
    public interface IGameStateMachine
    {
        public bool TransitionToState(GameState state);
        GameState GetCurrentState();
    }
}