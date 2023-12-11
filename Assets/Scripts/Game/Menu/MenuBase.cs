using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuBase : MonoBehaviour, IPointerEnterHandler
{
    EventInstance _menuHover;
    EventInstance _menuSelect;

    void Awake()
    {
        _menuHover = AudioController.Instance.CreateInstance(FMODEvents.Instance.MenuHover);
        _menuSelect = AudioController.Instance.CreateInstance(FMODEvents.Instance.MenuSelect);
    }

    void Start()
    {
        //Debug.Log($"[MenuBase] I'm {this.name}");
        UnlockCursor();
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }
    
    // When mouse enters the menu
    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayNavigateSFX();
    }

    // Option is Selected (keyboard only ?)
    public void OptionSelected()
    {
        PlayNavigateSFX();
    }

    public void PlayOptionSelectSFX()
    {
        AudioController.Instance.PlayEvent(_menuSelect);
    }

    public void PlayNavigateSFX()
    {
        AudioController.Instance.PlayEvent(_menuHover, true);
    }

}
