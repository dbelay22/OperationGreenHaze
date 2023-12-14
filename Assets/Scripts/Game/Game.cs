using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Yxp.Helpers;
using Yxp.StateMachine;

[RequireComponent(typeof(DevDebug))]
public class Game : MonoBehaviour
{
    GameStateMachine _stateMachine = null;

    [SerializeField] GameObject _helicopterExitSound;

    [Header("DontDestroyOnLoadInstances")]
    [SerializeField] GameObject _playerSettingsPrefab;
    [SerializeField] GameObject _localizationPrefab;
    [SerializeField] GameObject _audioControllerPrefab;

    [Header("Player")]
    [SerializeField] Player _player;

    [Header("Gameplay")]
    [SerializeField] int _minutesOfGameplay;

    [Header("PostProcessingFX")]
    [SerializeField] Volume _ppVolume;

    public int MinutesOfGameplay { get { return _minutesOfGameplay; } }

    private bool _isGodModeOn;
    public bool IsGodModeOn { get { return _isGodModeOn; } }

    private bool _isPPFxOn;

    bool _allEnemiesKilled = false;
    bool _allMissionPickupsCompleted = false;
    bool _exitClear = false;

    #region Instance

    private static Game _instance;

    public static Game Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
        
        CheckAudioControllerInstance();

        CheckPlayerSettingsInstance();

        CheckLocalizationInstance();
    }

    #endregion


    void Start()
    {
        _isGodModeOn = false;
        _isPPFxOn = true;

        _allEnemiesKilled = false;
        _allMissionPickupsCompleted = false;
        _exitClear = false;       

        _stateMachine = new GameStateMachine();
        _stateMachine.TransitionToState(new PlayState());
    }

    void CheckAudioControllerInstance()
    {
#if UNITY_EDITOR
        if (AudioController.Instance == null)
        {
            Debug.LogWarning("[Game] Start) No AudioController instance found, creating one.");
            Instantiate(_audioControllerPrefab);
        }
#endif    
    }

    void CheckPlayerSettingsInstance()
    {
#if UNITY_EDITOR
        if (PlayerSettings.Instance == null)
        {
            Debug.LogWarning("[Game] Start) No player settings instance found, creating one.");
            Instantiate(_playerSettingsPrefab);
        }
#endif
    }

    void CheckLocalizationInstance()
    {
#if UNITY_EDITOR
        if (Localization.Instance == null)
        {
            Debug.LogWarning("[Game] Start) No localization instance found, creating one.");
            Instantiate(_localizationPrefab);
        }
#endif
    }

    void Update()
    {
        _stateMachine.Update();
    }

    //========
    // Cheats/Debug Shortcuts Helpers

    public void UpdateCameraFOV(float step)
    {
        Camera.main.fieldOfView += step;
#if UNITY_EDITOR
        Debug.Log($"New [FOV] is: {Camera.main.fieldOfView}");
#endif
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

    public void ReportExitDangerZoneEnter()
    {
        if (PlayerNeedsToClearExitNow())
        {
            GameUI.Instance.ShowInGameMessage("ig_exit_danger", GameUI.LIFETIME_INFINITE, true);
        }
    }

    public void ReportExitDangerZoneExit()
    {
        Debug.Log("Game] ReportExitDangerZoneExit)");

        if (PlayerNeedsToClearExitNow())
        {
            GameUI.Instance.HideMessagesNow();
        }
    }

    public bool PlayerNeedsToClearExitNow()
    {
        return _allEnemiesKilled && _allMissionPickupsCompleted;
    }

    public void ReportAllMissionPickupsCollected()
    {
#if UNITY_EDITOR
        Debug.Log("[Game] (ReportAllMissionPickupsCompleted)");
#endif
        
        _allMissionPickupsCompleted = true;

        ObjectivesPanel.Instance.SetPickupDataComplete();

        ShowObjectiveCompletedMessage();

        CheckGameWin();
    }

    void ShowObjectiveCompletedMessage()
    {
        GameUI.Instance.ShowInGameMessage("ig_objective_completed", 3f);
    }

    public void ReportAllEnemiesKilled()
    {
#if UNITY_EDITOR
        Debug.Log("[Game] (ReportAllEnemiesKilled)");
#endif

        _allEnemiesKilled = true;

        ObjectivesPanel.Instance.SetKillemAllComplete();

        ShowKillsCompletedMessage();

        CheckGameWin();
    }

    void ShowKillsCompletedMessage()
    {
        GameUI.Instance.ShowInGameMessage("ig_kills_completed", 3f);
    }

    public void ReportExitClear()
    {
#if UNITY_EDITOR
        Debug.Log("[Game] (ReportExitClear)");
#endif
        
        _exitClear = true;

        if (!CheckGameWin())
        {
            ChangeStateToGameOver();
            return;
        }        

        ObjectivesPanel.Instance.SetFindExitComplete();
    }

    bool CheckGameWin()
    {
        if (PlayerNeedsToClearExitNow())
        {
            if (_exitClear)
            {
                ChangeStateToWin();
                return true;
            }
            else
            {
                _helicopterExitSound.SetActive(true);
                
                GameUI.Instance.ShowInGameMessage("ig_find_exit", 4f);

                AudioController.Instance.GameplayFindExit();
                
                return false;
            }
        }
        return false;
    }

    void ChangeStateToWin()
    {
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
#if UNITY_EDITOR
        Debug.Log($"[Game] GOD MODE ON: {_isGodModeOn}");
#endif
    }

    public void TogglePPFx()
    {
        _isPPFxOn = !_isPPFxOn;
#if UNITY_EDITOR
        Debug.Log($"[Game] PP-FX MODE ON: {_isPPFxOn}");
#endif

        _ppVolume.profile.TryGet(out ChromaticAberration chromaticAberration);
        _ppVolume.profile.TryGet(out DepthOfField depthOfField);
        _ppVolume.profile.TryGet(out Tonemapping tonemapping);

        chromaticAberration.active = _isPPFxOn;
        depthOfField.active = _isPPFxOn;
        tonemapping.active = _isPPFxOn;
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
