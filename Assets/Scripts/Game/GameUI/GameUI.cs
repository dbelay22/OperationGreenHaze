using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("In-game")]
    [SerializeField] GameObject _hudCanvas;
    [SerializeField] GameObject _gunReticleCanvas;

    [Header("In-game/HUD/Ammo")]
    [SerializeField] GameObject _ammoPanel;
    [SerializeField] TMP_Text _ammoLeftLabel;

    [Header("In-game/HUD/Health")]
    [SerializeField] GameObject _healthPanel;
    [SerializeField] TMP_Text _healthLabel;   

    [Header("In-game/HUD/Kills")]
    [SerializeField] GameObject _killsPanel;
    [SerializeField] TMP_Text _killsLabel;

    [Header("In-game/HUD/MiniMap")]
    [SerializeField] GameObject _minimapCanvas;

    [Header("In-game/HUD/Timer")]
    [SerializeField] TMP_Text _timerLabel;
    [SerializeField] AudioSource _timerAudioSource;
    [SerializeField] AudioClip _timerBeepShort;
    [SerializeField] AudioClip _timerBeepLong;

    [Header("In-game/VFX")]
    [SerializeField] GameObject _playerDamageCanvas;

    [Header("In-game/Interactive")]
    [SerializeField] GameObject _pauseScreen;

    [Header("In-game/Messages")]
    [SerializeField] GameObject _inGameMessagesCanvas;
    [SerializeField] TMP_Text _textInGameMessage;

    [Header("State")]
    [SerializeField] GameObject _gameOverCanvas;
    [SerializeField] GameObject _winCanvas;


    int _currentKills;
    int _totalKills;

    float _elapsedSeconds = 0;

    public float ElapsedSeconds { get { return _elapsedSeconds; } }

    int _minutesOfGameplay = 0;

    int _lastSecondUpdate = -1;

    int _lastMinuteUpdate = -1;

    Coroutine _hideMessagesCoroutine;

    bool _timeIsUp = false;

    #region Instance

    private static GameUI _instance;
    

    public static GameUI Instance { get { return _instance; } }    

    void Awake()
    {
        _instance = this;
    }

    #endregion

    void Start()
    {
        TimerInit();

        _hideMessagesCoroutine = null;
    }

    void Update()
    {
        if (_timeIsUp ||Game.Instance.IsGameplayOn() == false)
        {
            return;
        }

        TimerUpdate();
    }

    void TimerInit()
    {
        _timeIsUp = false;
        _elapsedSeconds = 0;
        _lastSecondUpdate = -1;
        _lastMinuteUpdate = -1;
        _minutesOfGameplay = Game.Instance.MinutesOfGameplay;
    }

    void TimerUpdate()
    {
        _elapsedSeconds += Time.deltaTime;

        float timeLeftSeconds = (_minutesOfGameplay * 60) - _elapsedSeconds;
        
        int timerMinutes = Mathf.FloorToInt(timeLeftSeconds / 60);
        
        int timerSeconds = Mathf.FloorToInt(timeLeftSeconds - (timerMinutes * 60));

        if (timerSeconds == _lastSecondUpdate)
        {
            // no need to update until next second
            return;
        }       

        // SFX
        if (timeLeftSeconds <= 10 && _timerAudioSource.isPlaying == false)
        {
            if (_timerAudioSource.isPlaying == false)
            {
                // last 10 seconds
                _timerAudioSource.PlayOneShot(_timerBeepLong);
            }
        } 
        else if (timeLeftSeconds <= 59)
        {
            if (_timerAudioSource.isPlaying == false) 
            {
                // last minute
                _timerAudioSource.PlayOneShot(_timerBeepShort);
            }
        } 
        else if (timerMinutes != _lastMinuteUpdate && timerMinutes < _minutesOfGameplay-1)
        {
            // every past minute
            _timerAudioSource.PlayOneShot(_timerBeepLong);
        }

        // Check timeout
        if (timerMinutes == 0 && timerSeconds == 0)
        {
            _timerLabel.text = "Time's Up";

            StartCoroutine(OnTimeOut());
            
            return;
        }

        // update label
        _timerLabel.text = GetTimeElapsedLabel(timerMinutes, timerSeconds);

        // update values shown
        _lastMinuteUpdate = timerMinutes;
        _lastSecondUpdate = timerSeconds;
    }

    IEnumerator OnTimeOut()
    {
        _timeIsUp = true;

        yield return new WaitForSeconds(1f);

        Game.Instance.ChangeStateToGameOver();
    }

    public void ShowGameplay()
    {
        // hide states
        _gameOverCanvas.SetActive(false);
        _winCanvas.SetActive(false);
        _pauseScreen.SetActive(false);

        ShowHUD(true);
    }

    void ShowHUD(bool active)
    {
        _gunReticleCanvas.SetActive(active);
        _hudCanvas.SetActive(active);        
        _minimapCanvas.SetActive(active);

        _playerDamageCanvas.SetActive(false);
    }

    public void ShowGameOver()
    {
        // hide in-game canvases
        ShowHUD(false);

        // hide in-game messages
        HideMessagesNow();

        // show game over
        _gameOverCanvas.SetActive(true);

        StartCoroutine(LoadLoseSceneDelayed());
    }    

    public void ShowWin()
    {
        // hide in-game canvases
        ShowHUD(false);

        // hide in-game messages
        HideMessagesNow();

        // show win
        _winCanvas.SetActive(true);

        StartCoroutine(LoadWinSceneDelayed());
    }

    IEnumerator LoadWinSceneDelayed()
    {
        yield return new WaitForSeconds(1f);

        LevelLoader.Instance.LoadWinScene();
    }

    IEnumerator LoadLoseSceneDelayed()
    {
        yield return new WaitForSeconds(1f);

        LevelLoader.Instance.LoadLoseScene();
    }


    public void ShowPause()
    {
        // hide in-game canvases
        ShowHUD(false);

        // show pause
        _pauseScreen.SetActive(true);       
    }

    public void UpdateAmmoAmount(int amount)
    {
        //Debug.Log($"[GameUI] (UpdateAmmoAmount) amount: {amount}");
        if (_ammoPanel == null)
        {
            Debug.LogError($"[GameUI] UpdateAmmoAmount) - Error: _ammoPanel is null - amount:{amount}");
            return;
        }
        
        _ammoPanel.SetActive(true);
        
        _ammoLeftLabel.text = amount.ToString();
    }

    public void UpdateHealthAmmount(int amount)
    {
        _healthPanel.SetActive(true);
        _healthLabel.text = amount.ToString();
    }

    public void ShowPlayerDamageVFX()
    {
        _playerDamageCanvas.SetActive(true);
    }

    public void InitKills(int total)
    {
        _currentKills = 0;
        _totalKills = total;

        _killsPanel.SetActive(true);
        _killsLabel.text = GetLabelKillsOverTotal(_currentKills, _totalKills);
    }

    public void IncreaseKills()
    {
        _currentKills++;
        
        _killsLabel.text = GetLabelKillsOverTotal(_currentKills, _totalKills);

        if (_currentKills >= _totalKills)
        {
            Game.Instance.ReportAllEnemiesKilled();
        }
    }

    public void ShowInGameMessage(string message, float lifetime)
    {
        //Debug.Log($"[GameUI] (ShowInGameMessage) showing message: {message}, lifetime: {lifetime}");

        _textInGameMessage.text = message;

        _inGameMessagesCanvas.SetActive(true);

        _hideMessagesCoroutine = StartCoroutine(HideMessagesDelayed(lifetime));
    }

    IEnumerator HideMessagesDelayed(float time)
    {
        yield return new WaitForSeconds(time);

        HideMessagesNow();

        _hideMessagesCoroutine = null;
    }

    public void HideMessagesNow()
    {
        //Debug.Log($"[GameUI] (HideMessagesNow)...");

        if (_hideMessagesCoroutine != null)
        {
            StopCoroutine(_hideMessagesCoroutine);
            _hideMessagesCoroutine = null;
        }        

        _textInGameMessage.text = "";
        _inGameMessagesCanvas.SetActive(false);        
    }


    string GetLabelKillsOverTotal(int kills, int total)
    {
        return string.Format("{0} / {1}", kills, total);
    }

    string GetTimeElapsedLabel(int minutes, int seconds)
    {
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

}
