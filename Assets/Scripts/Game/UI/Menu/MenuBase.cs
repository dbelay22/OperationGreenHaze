using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuBase : MonoBehaviour, IPointerEnterHandler
{
    EventInstance _menuHoverSFX;
    EventInstance _menuSelectSFX;

    void Start()
    {
        //Debug.Log($"[MenuBase] I'm {this.name}");
        UICore.UnlockCursor();
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
        AudioController.Instance.PlayInstanceOrCreate(_menuSelectSFX, FMODEvents.Instance.MenuSelect, out _menuSelectSFX);
    }

    public void PlayNavigateSFX()
    {
        AudioController.Instance.PlayInstanceOrCreate(_menuHoverSFX, FMODEvents.Instance.MenuHover, out _menuHoverSFX);
    }

}
