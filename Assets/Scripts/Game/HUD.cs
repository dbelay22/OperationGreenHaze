using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
    [SerializeField] GameObject _gameOverCanvas;

    #region Instance
    private static HUD _instance;
    public static HUD Instance
    {
        get { return _instance; }
    }
    #endregion

    private void Awake()
    {
        _instance = this;
    }

    public void Reset()
    {
        HideGameOver();
    }

    public void ShowGameOver()
    {
        _gameOverCanvas.SetActive(true);
    }

    public void HideGameOver()
    {
        _gameOverCanvas.SetActive(false);
    }
}
