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
        LevelLoader.Instance.LoadNextLevelAsync();

        AudioController.Instance.StopFadeEvent(_menuMusic);

        LevelLoader.Instance.ReadyToStartNextLevel();
    }

    public void Quit()
    {
        StartCoroutine(LevelLoader.Instance.StartCrossfade());

        StartCoroutine(QuitDelayed(LevelLoader.Instance.TransitionTime));            
    }

    IEnumerator QuitDelayed(float time)
    {
        yield return new WaitForSeconds(time);

        Application.Quit();
    }

}
