using System;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [Header("In-game")]
    [SerializeField] GameObject _hudCanvas;

    [Header("Gameplay")]
    [SerializeField] GameObject _gunReticleCanvas;

    [Header("Ammo")]
    [SerializeField] GameObject _ammoPanel;
    [SerializeField] TMP_Text _ammoLeftLabel;

    [Header("Health")]
    [SerializeField] GameObject _healthPanel;
    [SerializeField] TMP_Text _healthLabel;

    [Header("Kills")]
    [SerializeField] GameObject _killsPanel;
    [SerializeField] TMP_Text _killsLabel;

    [Header("Timer")]
    [SerializeField] TMP_Text _timerLabel;

    [Header("VFX")]
    [SerializeField] GameObject _playerDamageCanvas;

    [Header("Interactive")]
    [SerializeField] GameObject _gameOverCanvas;
    [SerializeField] GameObject _winCanvas;
    [SerializeField] GameObject _pauseScreen;

    int _currentKills;
    int _totalKills;

    float _elapsedSeconds = 0;

    public float ElapsedSeconds { get { return _elapsedSeconds; } }

    int _minutesOfGameplay = 0;

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
            Debug.Log($"[GameUI] WIN in {Time.realtimeSinceStartup.ToString("N0")} seconds");
            
            Game.Instance.ChangeStateToWin();
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
