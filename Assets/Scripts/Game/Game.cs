using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Yxp.Helpers;
using Yxp.StateMachine;

public class Game : MonoBehaviour
{
    GameStateMachine _stateMachine = null;

    [Header("Player")]
    [SerializeField] Player _player;

    [Header("Gameplay")]
    [SerializeField] int _minutesOfGameplay;

    [Header("Dev")]
    [SerializeField] bool _isDevBuild;

    public bool IsDevBuild { get { return _isDevBuild; } }

    public int MinutesOfGameplay { get { return _minutesOfGameplay; } }

    private bool _isGodModeOn;

    public bool IsGodModeOn { get { return _isGodModeOn; } }

    bool _allEnemiesKilled = false;
    bool _allMissionPickupsCompleted = false;
    bool _exitClear = false;

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
        _isGodModeOn = false;

        _allEnemiesKilled = false;
        _allMissionPickupsCompleted = false;
        _exitClear = false;

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

#region Mission

    public void ReportAllMissionPickupsCollected()
    {
        Debug.Log("[Game] (ReportAllMissionPickupsCompleted)");
        
        _allMissionPickupsCompleted = true;

        ObjectivesPanel.Instance.SetPickupDataComplete();

        if (CheckGameWin() == false)
        {
            GameUI.Instance.ShowInGameMessage("Good job! Project data is now secure", 4f);
        }        
    }

    public void ReportAllEnemiesKilled()
    {
        Debug.Log("[Game] (ReportAllEnemiesKilled)");

        _allEnemiesKilled = true;

        ObjectivesPanel.Instance.SetKillemAllComplete();

        if (CheckGameWin() == false)
        {
            GameUI.Instance.ShowInGameMessage("Does it taste good ? Objective Completed", 4f);
        }
    }

    public void ReportExitClear()
    {
        Debug.Log("[Game] (ReportExitClear)");

        _exitClear = true;

        ObjectivesPanel.Instance.SetFindExitComplete();

        if (CheckGameWin() == false)
        {
            Game.Instance.ChangeStateToGameOver();
        }
    }

    bool CheckGameWin()
    {
        if (_allEnemiesKilled && _allMissionPickupsCompleted && _exitClear)
        {
            Debug.Log("[Game] ALL MISSIONS COMPLETED !! ");
            ChangeStateToWin();
            return true;
        }
        return false;
    }

    void ChangeStateToWin()
    {
        Debug.Log($"[Game] WIN in {Time.realtimeSinceStartup:N0} seconds");
        
        StartCoroutine(WinDelayed());
    }

    IEnumerator WinDelayed()
    {
        yield return new WaitForSeconds(1f);

        _stateMachine.TransitionToState(new WinState());

        _player.GameplayIsOver();
    }

#endregion Mission

    public void ChangeStateToPaused()
    {
        if (IsGameplayOn())
        {
            _stateMachine.TransitionToState(new PauseState());
        }        
    }

    public void ResumeGame()
    {
        if (IsGamePaused())
        {
            _stateMachine.TransitionToState(new PlayState());
        }
    }

    public void TryAgain()
    {
        SceneHelper.ReloadCurrentScene();
    }

    public GameState GetCurrentState()
    {
        return _stateMachine.GetCurrentState();
    }

    public bool IsGameOver()
    {
        bool isGameOver = GetCurrentState() is GameOverState;
        return isGameOver;
    }

    public bool IsGamePlayOver()
    {
        bool isGameOver = GetCurrentState() is GameOverState;
        bool isWin = GetCurrentState() is WinState;
        return isGameOver || isWin;
    }

    public bool IsGamePaused()
    { 
        return GetCurrentState() is PauseState;
    }

    public bool IsGameplayOn()
    {
        return GetCurrentState() is PlayState;
    }

    public void ToggleGodMode()
    {
        _isGodModeOn = !_isGodModeOn;

        Debug.Log($"[Game] GOD MODE ON: {_isGodModeOn}");
    }

    public IEnumerator TimeBend(float slowTimeScale, float waitTime)
    {
        Time.timeScale = slowTimeScale;

        yield return new WaitForSeconds(waitTime);

        Time.timeScale = 1f;
    }

    public void QuitToMainMenu()
    {
        _stateMachine.Shutdown();

        LevelLoader.Instance.LoadMainMenu();
    }    

    //=======================================================
}
