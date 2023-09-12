using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuBase : MonoBehaviour, IPointerEnterHandler
{
    [Header("Audio")]
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _navigateSFX;
    [SerializeField] AudioClip _optionSelectSFX;

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
        if (_audioSource == null)
        {
            Debug.LogError("[MainMenu] (PlayOptionSelectSFX) AudioSource is null");
            return;
        }

        _audioSource.PlayOneShot(_optionSelectSFX);
    }

    public void PlayNavigateSFX()
    {
        if (_audioSource == null)
        {
            Debug.LogError("[MainMenu] (OnPointerEnter) AudioSource is null");
            return;
        }

        _audioSource.Stop();
        _audioSource.PlayOneShot(_navigateSFX);
    }

}
