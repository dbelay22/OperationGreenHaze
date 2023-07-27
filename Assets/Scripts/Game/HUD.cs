using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [Header("In-game")]
    [SerializeField] GameObject _hudCanvas;

    [Header("Ammo")]
    [SerializeField] GameObject _ammoPanel;
    [SerializeField] TMP_Text _ammoLeftLabel;

    [Header("Health")]
    [SerializeField] GameObject _healthPanel;
    [SerializeField] TMP_Text _healthLabel;

    [SerializeField] GameObject _gunReticleCanvas;

    [Header("Other")]
    [SerializeField] GameObject _gameOverCanvas;    

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
        
        _gunReticleCanvas.SetActive(true);
    }

    public void ShowGameOver()
    {
        // hide in-game hud
        _gunReticleCanvas.SetActive(false);
        _hudCanvas.SetActive(false);

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

}
