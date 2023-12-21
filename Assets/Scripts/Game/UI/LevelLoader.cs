using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] Animator _transitionAnimator;

    [SerializeField] public float _transitionTime = 1f;

    public AsyncOperation _currentLoading;

    public bool _loadingAsync = false;

    public bool _nextLevelReady = false;

    public bool _canStartNextLevel = false;

    public Scene _previousScene;

    public float _currentProgress;

    #region Instance

    private static LevelLoader _instance;

    public static LevelLoader Instance { get { return _instance; } }

    void Awake()
    {
        _instance = this;

        ResetLoader();        
    }

    #endregion

    void Start()
    {
        HideProgressBar();
    }

    public void ResetLoader()
    {
        _nextLevelReady = false;

        _loadingAsync = false;

        _canStartNextLevel = false;

        _currentProgress = 0f;
    }

    public void PreloadLevelAsync()
    {
        _previousScene = SceneManager.GetActiveScene();

        Debug.Log($"LevelLoader] PreloadLevelAsync)... _previousScene: {_previousScene.name}");

        _loadingAsync = true;

        _nextLevelReady = false;

        _canStartNextLevel = false;

        _currentProgress = 0f;
    }

    public void LoadNextLevelAsync()
    {
        Debug.Log($"LevelLoader] LoadNextLevelAsync)...");

        PreloadLevelAsync();

        int nextSceneIndex = GetNextLevelIndex();

        Debug.Log($"LevelLoader] LoadNextLevelAsync) nextSceneIndex: {nextSceneIndex}");

        _currentLoading = SceneManager.LoadSceneAsync(nextSceneIndex, LoadSceneMode.Single);

        _currentLoading.allowSceneActivation = false;
    }

    public void LoadLevelAsync(string nextSceneName)
    {
        Debug.Log($"LevelLoader] LoadLevelAsync)... nextSceneName:{nextSceneName}");

        PreloadLevelAsync();

        _currentLoading = SceneManager.LoadSceneAsync(nextSceneName, LoadSceneMode.Single);

        _currentLoading.allowSceneActivation = false;
    }

    public void ReadyToStartNextLevel()
    {
        Debug.Log("LevelLoader] ReadyToLoadNextLevel)...");
        
        _canStartNextLevel = true;
    }

    void Update()
    {
        if (_loadingAsync && _currentLoading != null && !_currentLoading.isDone)
        {
            Debug.Log($"LevelLoader] Update) _nextLevelReady:{_nextLevelReady}, _canStartNextLevel:{_canStartNextLevel}");

            _currentProgress = _currentLoading.progress;

            ShowProgressBar(_currentProgress);

            _nextLevelReady = _currentProgress >= 0.9f;

            if (_nextLevelReady)
            {
                Debug.Log("LevelLoader] Level is ready...");

                _loadingAsync = false;

                StartCoroutine(WaitUntilUIReady());
            }
        }
    }

    public bool IsNextLevelReady()
    {
        return _nextLevelReady;
    }


    IEnumerator WaitUntilUIReady()
    {
        while (!_canStartNextLevel)
        {
            //Debug.Log("Waiting signal to start next level...");
            yield return new WaitForEndOfFrame();
        }

        Debug.Log("allowSceneActivation NOW");
        _currentLoading.allowSceneActivation = true;

        ResetLoader();

        ShowProgressBar(1f);
    }

    void ShowProgressBar(float progress)
    {
        Debug.Log($"LevelLoader] ShowProgressBar value:{progress}...");
    }

    void HideProgressBar()
    { 
    }

    public void LoadMainMenuAsync(bool crossFadeAndStartMenu = false)
    {
        LoadLevelAsync("Menu");

        if (crossFadeAndStartMenu)
        {
            StartCoroutine(StartCrossfade());
        }
    }

    public void LoadWinSceneAsync()
    {
        LoadLevelAsync("Win");
    }

    public void LoadLoseSceneAsync()
    {
        LoadLevelAsync("Lose");
    }

    public IEnumerator StartCrossfade()
    {
        Debug.Log($"[LevelLoader] Crossfade)");

        _transitionAnimator.SetTrigger("StartCrossfade");

        yield return new WaitForSeconds(_transitionTime);

        ReadyToStartNextLevel();
    }

    int GetNextLevelIndex()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        return nextSceneIndex;
    }

}
