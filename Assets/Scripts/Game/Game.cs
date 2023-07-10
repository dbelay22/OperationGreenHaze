using UnityEngine;

public class Game : MonoBehaviour
{
    GameStateMachine _stateMachine = null;
    
    #region Instance
    private static Game _instance;
    public static Game Instance
    {
        get { return _instance; }
    }
    #endregion

    void Awake() 
    {
        _instance = this; 
    }

    void Start()
    {
        _stateMachine = new GameStateMachine();
        _stateMachine.TransitionToState(new PlayState());
    }

    void Update()
    {
        _stateMachine.Update();
    }



    //=======================================================    
    // TODO Refactor, move

    //========
    // Cheats/Debug Shortcuts Helpers

    public void UpdateCameraFOV(float step)
    {
        Camera.main.fieldOfView += step;
        Debug.Log($"New [FOV] is: {Camera.main.fieldOfView}");
    }

    public void SetCameraFov(float value)
    {
        Camera.main.fieldOfView = value;
    }

    public void ForceGameOver()
    {
        _stateMachine.TransitionToState(new GameOverState());
    }

    //=======================================================
}
