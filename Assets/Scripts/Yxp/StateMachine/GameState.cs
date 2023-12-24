namespace Yxp.StateMachine
{
    public abstract class GameState
    {
        public abstract void EnterState();
        public abstract void ExitState(bool isShuttingDown);
        public abstract void Update();
    }
}
