using System.Collections;
using UnityEngine;
using Yxp.Helpers;
using Yxp.StateMachine;

public class Game : MonoBehaviour
{
    public const string QUADRANT_TAG = "Quadrant";

    GameStateMachine _stateMachine = null;

    [SerializeField] Player _player;

    #region Instance
    
    private static Game _instance;
 
    public static Game Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion


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

    public void ChangeStateToGameOver()
    {
        _stateMachine.TransitionToState(new GameOverState());
    }

    public void ChangeStateToWin()
    {
        StartCoroutine(WinDelayed());
    }

    IEnumerator WinDelayed()
    {
        yield return new WaitForSeconds(2f);

        _stateMachine.TransitionToState(new WinState());

        _player.GameplayIsOver();
    }

    public void TryAgain()
    {
        Debug.Log("Try again");
        SceneHelper.ReloadCurrentScene();
    }

    public GameState GetCurrentState()
    {
        return _stateMachine.GetCurrentState();
    }

    public bool isGameOver()
    {
        bool isGameOver = Game.Instance.GetCurrentState() is GameOverState;
        return isGameOver;
    }

    public bool isGamePlayOver()
    {
        bool isGameOver = Game.Instance.GetCurrentState() is GameOverState;
        bool isWin = Game.Instance.GetCurrentState() is WinState;
        return isGameOver || isWin;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    //=======================================================
}
