using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [Header("In-game")]
    [SerializeField] GameObject _hudCanvas;

    [Header("Ammo")]
    [SerializeField] GameObject _ammoPanel;
    [SerializeField] TMP_Text _ammoLeftLabel;


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

    void Start()
    {
        ShowGameplay();
    }

    public void ShowGameplay()
    {
        // hide game-over
        _gameOverCanvas.SetActive(false);

        // show in-game
        _hudCanvas.SetActive(true);
        _ammoPanel.SetActive(false);

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

    public void ShowAmmoAmount(int amount)
    {
        _ammoPanel.SetActive(true);
        _ammoLeftLabel.text = amount.ToString();
    }

}
