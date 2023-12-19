using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AGameByUI : MonoBehaviour
{
    [SerializeField] float _fadeInDuration = 3f;
    [SerializeField] CanvasGroup _textCanvasGroup;

    void Awake()
    {
        HideTextCanvasGroup();

        UICore.LockCursor();
    }

    void HideTextCanvasGroup()
    {
        _textCanvasGroup.alpha = 0;
    }

    void Start()
    {
        StartCoroutine(FadeInText());
    }

    IEnumerator FadeInText()
    {
        float time = 0;

        while (time < _fadeInDuration)
        {
            _textCanvasGroup.alpha = Mathf.Lerp(0, 1, time / _fadeInDuration);

            yield return null;

            time += Time.deltaTime;
        }

        OnFadeInEnd();
    }

    void OnFadeInEnd() 
    {
        LevelLoader.Instance.LoadNextLevel();
    }

}
