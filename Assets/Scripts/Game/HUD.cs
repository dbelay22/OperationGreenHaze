using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour
{
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
        UpdateHUD();
    }

    void UpdateHUD() 
    {
         
    }
}
