using FMOD.Studio;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    public static float LIFETIME_INFINITE = -1f;

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

    [Header("In-game/VFX")]
    [SerializeField] PlayerDamage _playerDamage;
    [SerializeField] float _damageShowTime = 2f;

    [Header("In-game/Interactive")]
    [SerializeField] GameObject _pauseScreen;

    [Header("In-game/Messages")]
    [SerializeField] GameObject _inGameMessagesCanvas;
    [SerializeField] TMP_Text _textInGameMessage;

    [Header("State")]
    [SerializeField] GameObject _gameOverCanvas;
    [SerializeField] GameObject _winCanvas;

    public struct InGameMessage 
    {
        public string MessageKey;
        public float Lifetime;
    }

    Queue<InGameMessage> _queuedMessages;

    int _currentKills;
    int _totalKills;

    float _elapsedSeconds = 0;

    public float ElapsedSeconds { get { return _elapsedSeconds; } }

    int _minutesOfGameplay = 0;

    int _lastSecondUpdate = -1;

    int _lastMinuteUpdate = -1;

    Coroutine _hideMessagesCoroutine;

    bool _timeIsUp = false;

    EventInstance _timerBeepShort;
    EventInstance _timerBeepLong;


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

        _queuedMessages = new Queue<InGameMessage>();

        _timerBeepShort = AudioController.Instance.CreateInstance(FMODEvents.Instance.TimerBeepShort);
        _timerBeepLong = AudioController.Instance.CreateInstance(FMODEvents.Instance.TimerBeepLong);
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

        int intTimeLeftSeconds = (int)Math.Floor(timeLeftSeconds);
        
        int timerMinutes = Mathf.FloorToInt(timeLeftSeconds / 60);
        
        int timerSeconds = Mathf.FloorToInt(timeLeftSeconds - (timerMinutes * 60));

        if (timerSeconds == _lastSecondUpdate)
        {
            // no need to update until next second
            return;
        }

        //Debug.Log($"intTimeLeftSeconds: {intTimeLeftSeconds}");

        // SFX
        if (timeLeftSeconds <= 10)
        {
            // last 10 seconds
            AudioController.Instance.PlayEvent(_timerBeepLong, true);
        }
        else if (timeLeftSeconds <= 30)
        {
            // last minute
            AudioController.Instance.PlayEvent(_timerBeepShort, true);
        }
        else if (intTimeLeftSeconds == 5 * 60)
        {
            Debug.Log($"5 minutoooos");
            RadioTalking.Instance.PlayMessage(RadioTalking.Instance.Time1);
        }
        else if (intTimeLeftSeconds == 3 * 60)
        {
            Debug.Log($"3 minutoooos");
            RadioTalking.Instance.PlayMessage(RadioTalking.Instance.Time2);
        }
        else if (intTimeLeftSeconds == 1 * 60)
        {
            Debug.Log($"1 minutoooos");
            RadioTalking.Instance.PlayMessage(RadioTalking.Instance.Time3);
        }
        else if (timerMinutes != _lastMinuteUpdate && timerMinutes < _minutesOfGameplay - 1)
        {
            // every past minute
            AudioController.Instance.PlayEvent(_timerBeepLong, true);
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

        // Change music
        AudioController.Instance.GameplayDead();

        yield return new WaitForSeconds(2.2f);

        Game.Instance.ChangeStateToGameOver();
    }

    public void ShowGameplay()
    {
        Debug.Log("[GameUI] ShowGameplay)...");

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
        LevelLoader.Instance.LoadWinSceneAsync();

        yield return new WaitForSeconds(2.2f);

        Game.Instance.ShutDown();

        StartCoroutine(LevelLoader.Instance.StartCrossfade());
    }

    IEnumerator LoadLoseSceneDelayed()
    {
        LevelLoader.Instance.LoadLoseSceneAsync();
        
        yield return new WaitForSeconds(2.2f);

        Game.Instance.ShutDown();

        StartCoroutine(LevelLoader.Instance.StartCrossfade());
    }


    public void ShowPause()
    {
        Debug.Log("[GameUI] ShowPause)...");

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

    public void UpdateHealthAmmount(int healthAmount)
    {
        _healthPanel.SetActive(true);

        string healthAmountString = healthAmount.ToString();

        if (_healthLabel.text.Equals(healthAmountString))
        {
            return;
        }

        _healthLabel.text = healthAmountString;
    }

    public void ShowPlayerDamageVFX()
    {
        if (!_playerDamage.IsActive())
        {
            _playerDamage.Show(_damageShowTime);
        }
    }

    public void ShowPlayerBadlyHurt()
    {
        _playerDamage.Show();
    }

    public void HidePlayerBadlyHurt()
    {
        _playerDamage.Hide();
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

    public bool ShowInGameMessage(InGameMessage igMessage)
    {
        return ShowInGameMessage(igMessage.MessageKey, igMessage.Lifetime);
    }

    public bool ShowInGameMessage(string textKey, float lifetime, bool maxPriority = false)
    {
        Debug.Log($"ShowInGameMessage) key:{textKey}, lifetime: {lifetime}");

        if (_inGameMessagesCanvas.activeInHierarchy == true)
        {
            if (_textInGameMessage.text.Equals(textKey))
            {
                Debug.Log($"ShowInGameMessage) already active and same key");
                return false;
            }
            else if (maxPriority)
            {
                // clear all queued messages and dont return, show message
                _queuedMessages.Clear();
            }
            else 
            {
                // enqueue message
                InGameMessage nextMessage;
                
                nextMessage.MessageKey = textKey;
                nextMessage.Lifetime = lifetime;

                _queuedMessages.Enqueue(nextMessage);

                return false;
            }
        }

        _textInGameMessage.text = Localization.Instance.GetTextByKey(textKey);

        _inGameMessagesCanvas.SetActive(true);

        Debug.Log($"ShowInGameMessage) showing text: {_textInGameMessage.text}");

        // negative / zero lifetime = permanent message (will be hidden later)
        // positive lifetime = temporal message
        if (lifetime > 0f)
        {
            _hideMessagesCoroutine = StartCoroutine(HideMessagesDelayed(lifetime));
        }

        return true;
    }

    IEnumerator HideMessagesDelayed(float time)
    {
        yield return new WaitForSeconds(time);

        HideMessagesNow();

        _hideMessagesCoroutine = null;
    }

    public void HideMessagesNow()
    {
        Debug.Log($"[GameUI] (HideMessagesNow)...");

        if (_hideMessagesCoroutine != null)
        {
            StopCoroutine(_hideMessagesCoroutine);
            _hideMessagesCoroutine = null;
        }        

        _textInGameMessage.text = "";
        _inGameMessagesCanvas.SetActive(false);

        if (_queuedMessages.Count > 0)
        {
            InGameMessage nextMessage = _queuedMessages.Dequeue();

            Debug.Log($"HideMessagesNow) Detected next message, showing next: {nextMessage.MessageKey}");

            ShowInGameMessage(nextMessage);
        }
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
