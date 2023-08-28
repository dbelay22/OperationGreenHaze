using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour, IPointerEnterHandler
{
    [Header("Audio")]
    [SerializeField] AudioSource _audioSource;
    [SerializeField] AudioClip _navigateSFX;
    [SerializeField] AudioClip _optionSelectSFX;

    void Start()
    {
        UnlockCursor();
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    public void Play()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
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

        _audioSource.PlayOneShot(_navigateSFX);
    }    

}
