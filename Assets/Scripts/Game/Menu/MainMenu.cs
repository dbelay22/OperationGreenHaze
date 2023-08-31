using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MenuBase 
{
    public void Play()
    {
        LevelLoader.Instance.LoadNextLevel();
    }

    public void Quit()
    {
        Application.Quit();
    }   

}
