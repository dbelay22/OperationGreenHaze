﻿using FMOD.Studio;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
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

    EventInstance _helicopterGoneSFX;

    #region Instance

    private static Game _instance;

    public static Game Instance { get { return _instance; } }

    #endregion

    void Awake()
    {
        _instance = this;

        CheckAudioControllerInstance();

        CheckPlayerSettingsInstance();

        CheckLocalizationInstance();
    }

    void Start()
    {
        _isGodModeOn = false;
        _isPPFxOn = true;

        _allEnemiesKilled = false;
        _allMissionPickupsCompleted = false;
        _exitClear = false;       

        _stateMachine = new GameStateMachine();
        _stateMachine.TransitionToState(new PlayState());

        AudioController.Instance.PlayInstanceOrCreate(_helicopterGoneSFX, FMODEvents.Instance.Helicopter_Gone, out _helicopterGoneSFX, true);

        Invoke(nameof(PlayRadioWelcome), 10f);
    }

    void PlayRadioWelcome()
    {
        RadioTalking.Instance.PlayMessage(RadioTalking.Instance.Start);
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

    #region Mission

    public void ReportExitDangerZoneEnter()
    {
        if (PlayerNeedsToClearExitNow())
        {
            RadioTalking.Instance.PlayMessage(RadioTalking.Instance.TooClose, maxPriority: true);
        }
    }

    public void ReportExitDangerZoneExit()
    {
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
        RadioTalking.Instance.PlayMessage(RadioTalking.Instance.MissionObj2, maxPriority: true);
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
        RadioTalking.Instance.PlayMessage(RadioTalking.Instance.KillComplete, maxPriority: true);
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

    #endregion Mission

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

                RadioTalking.Instance.PlayMessage(RadioTalking.Instance.FindExit, maxPriority: true);

                AudioController.Instance.GameplayFindExit();

                return false;
            }
        }
        return false;
    }

    void GameplayIsOver()
    {
        Debug.Log("Game] GameplayIsOver)...");

        GameUI.Instance.HideMessagesNow();

        RadioTalking.Instance.ShutDown();

        if (_helicopterExitSound.activeInHierarchy)
        {
            _helicopterExitSound.SetActive(false);
        }

        _player.GameplayIsOver();
    }

    void ChangeStateToWin()
    {
        _stateMachine.TransitionToState(new WinState());
        
        GameplayIsOver();
    }

    public void ChangeStateToGameOver()
    {
        _stateMachine.TransitionToState(new GameOverState());

        GameplayIsOver();
    }

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

        LevelLoader.Instance.LoadMainMenuAsync(true);
    }

    public void ShutDown()
    {
        _stateMachine.Shutdown();
    }

    //=======================================================
}
