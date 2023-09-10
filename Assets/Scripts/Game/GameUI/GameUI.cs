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

    [Header("In-game/HUD/Timer")]
    [SerializeField] TMP_Text _timerLabel;

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

    Coroutine _hideMessagesCoroutine;

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
        TimerUpdate();
    }

    void TimerInit()
    {
        _elapsedSeconds = 0;
        _minutesOfGameplay = Game.Instance.MinutesOfGameplay;
    }

    void TimerUpdate()
    {
        _elapsedSeconds += Time.deltaTime;

        float timeLeftSeconds = (_minutesOfGameplay * 60) - _elapsedSeconds;

        int timerMinutes = Mathf.FloorToInt(timeLeftSeconds / 60);
        int timerSeconds = Mathf.FloorToInt(timeLeftSeconds - (timerMinutes * 60));

        _timerLabel.text = GetTimeElapsedLabel(timerMinutes, timerSeconds);

        if (timeLeftSeconds <= 0)
        {
            Game.Instance.ChangeStateToGameOver();
        }
        
    }

    public void ShowGameplay()
    {
        // hide states
        _gameOverCanvas.SetActive(false);
        _winCanvas.SetActive(false);
        _pauseScreen.SetActive(false);

        // show in-game
        _hudCanvas.SetActive(true);
        
        // show reticle
        _gunReticleCanvas.SetActive(true);

        // hide vfx
        _playerDamageCanvas.SetActive(false);
    }

    public void ShowGameOver()
    {
        // hide in-game canvases
        _gunReticleCanvas.SetActive(false);
        _hudCanvas.SetActive(false);
        _playerDamageCanvas.SetActive(false);

        // show game over
        _gameOverCanvas.SetActive(true);
    }

    public void ShowWin()
    {
        // hide in-game canvases
        _gunReticleCanvas.SetActive(false);
        _hudCanvas.SetActive(false);
        _playerDamageCanvas.SetActive(false);

        // show win
        _winCanvas.SetActive(true);
    }

    public void ShowPause()
    {
        // hide in-game canvases
        _gunReticleCanvas.SetActive(false);
        _hudCanvas.SetActive(false);
        _playerDamageCanvas.SetActive(false);

        // show pause
        _pauseScreen.SetActive(true);       
    }

    public void UpdateAmmoAmount(int amount)
    {
        //Debug.Log($"[GameUI] (UpdateAmmoAmount) amount: {amount}");

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
