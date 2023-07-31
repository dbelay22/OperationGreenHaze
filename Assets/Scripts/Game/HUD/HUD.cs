using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
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


    [Header("VFX")]
    [SerializeField] GameObject _playerDamageCanvas;

    [Header("Interactive")]
    [SerializeField] GameObject _gameOverCanvas;

    int _currentKills;
    int _totalKills;

    #region Instance
    private static HUD _instance;
    public static HUD Instance
    {
        get { return _instance; }
    }
    #endregion

    void Awake()
    {
        _instance = this;
    }

    public void ShowGameplay()
    {
        // hide game-over
        _gameOverCanvas.SetActive(false);

        // show in-game
        _hudCanvas.SetActive(true);
        
        // show reticle
        _gunReticleCanvas.SetActive(true);

        // hide vfx
        _playerDamageCanvas.SetActive(false);
    }

    public void ShowGameOver()
    {
        // hide in-game hud
        _gunReticleCanvas.SetActive(false);
        _hudCanvas.SetActive(false);

        // hide vfx
        _playerDamageCanvas.SetActive(false);

        // show game over
        _gameOverCanvas.SetActive(true);
    }

    public void UpdateAmmoAmount(int amount)
    {
        //Debug.Log($"[HUD] (UpdateAmmoAmount) amount: {amount}");

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
            Debug.Log($"[HUD] WIN in {Time.realtimeSinceStartup / 60f}");
        }
    }

    string GetLabelKillsOverTotal(int kills, int total)
    {
        return string.Format("{0} / {1}", kills, total);
    }

}
