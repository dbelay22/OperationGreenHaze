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

        StartCoroutine(LoadLevel(nextSceneIndex));
    }

    public void LoadPreviousLevel()
    {
        int previousSceneIndex = SceneManager.GetActiveScene().buildIndex - 1;

        StartCoroutine(LoadLevel(previousSceneIndex));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        Debug.Log($"(LoadLevel) index:{levelIndex}");

        StartCrossfade();

        Debug.Log($"(LoadLevel) Triggered StartCrossfade, now wait {_transitionTime} seconds");

        yield return new WaitForSeconds(_transitionTime);

        Debug.Log($"(LoadLevel) Time elapsed! load scene now! {levelIndex}");

        SceneManager.LoadScene(levelIndex);
    }

    public void StartCrossfade()
    {
        _transitionAnimator.SetTrigger("StartCrossfade");
    }
}
