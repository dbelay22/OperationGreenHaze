using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using FMOD.Studio;

public class MainMenu : MenuBase 
{
    EventInstance _menuMusic;

    void Awake()
    {
        UICore.UnlockCursor();

        AudioController.Instance.PlayInstanceOrCreate(_menuMusic, FMODEvents.Instance.MenuMusic, out _menuMusic, true);
    }

    public void Play()
    {
        AudioController.Instance.StopFadeEvent(_menuMusic);

        LevelLoader.Instance.LoadNextLevel();
    }

    public void Quit()
    {
        LevelLoader.Instance.StartCrossfade();
        
        StartCoroutine(QuitDelayed());
    }

    IEnumerator QuitDelayed()
    {
        yield return new WaitForSeconds(0.5f);

        Application.Quit();
    }

}
