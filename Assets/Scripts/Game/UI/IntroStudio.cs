using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IntroStudio : MonoBehaviour
{
    [SerializeField] float _introDuration = 3f;
    [SerializeField] TMP_Text _text;

    void Start()
    {
        StartCoroutine(FadeText());
    }

    IEnumerator FadeText()
    {
        float time = 0;

        while (time < _introDuration)
        {
            _text.alpha = Mathf.Lerp(0, 1, time / _introDuration);

            yield return null;

            time += Time.deltaTime;
        }

        LevelLoader.Instance.LoadNextLevel();
    }

}
