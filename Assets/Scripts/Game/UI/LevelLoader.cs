using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] Animator _transitionAnimator;

    [SerializeField] float _transitionTime = 1f;

    #region Instance

    private static LevelLoader _instance;


    public static LevelLoader Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;
    }

    #endregion

    public void LoadNextLevel()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        StartCoroutine(CrossfadeToSceneIndex(nextSceneIndex, _transitionTime));
    }    

    public void LoadPreviousLevel()
    {
        int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;

        StartCoroutine(CrossfadeToSceneIndex(previousSceneIndex, _transitionTime));
    }

    public void LoadMainMenu()
    {
        StartCoroutine(CrossfadeToSceneName("Menu", _transitionTime));
    }

    public void LoadWinScene()
    {
        StartCoroutine(CrossfadeToSceneName("Win", _transitionTime));
    }

    IEnumerator CrossfadeToSceneIndex(int sceneIndex, float time)
    {
        StartCrossfade();

        yield return new WaitForSeconds(time);

        SceneManager.LoadScene(sceneIndex);
    }

    IEnumerator CrossfadeToSceneName(string sceneName, float time)
    {
        //Debug.Log($"[LevelLoader] CrossfadeToSceneName) sceneName:{sceneName}");

        StartCrossfade();

        yield return new WaitForSeconds(time);

        SceneManager.LoadScene(sceneName);
    }

    public void StartCrossfade()
    {
        _transitionAnimator.SetTrigger("StartCrossfade");
    }
}
