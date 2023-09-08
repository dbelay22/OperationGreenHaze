using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [Serializable]
    private class TutorialStep
    {
        public string _message;
        public float _lifeTime;
    }

    [SerializeField] TutorialStep[] _tutorialSteps;

    int _currentStepIndex = 0;

    bool _shouldExecuteNextStep = true;

    void Start()
    {
        _currentStepIndex = 0;

        _shouldExecuteNextStep = true;

        ShowCurrentStep();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _shouldExecuteNextStep = false;
        }
    }

    void ShowCurrentStep()
    {
        Debug.Log($"[Tutorial] (ShowCurrentStep) _currentStepIndex:{_currentStepIndex}, _tutorialSteps.Length: {_tutorialSteps.Length}");

        if (_currentStepIndex >= _tutorialSteps.Length)
        {
            // no more steps viva!
            return;
        }

        TutorialStep step = _tutorialSteps[_currentStepIndex];

        if (step == null) 
        {
            Debug.LogError($"[Tutorial] (ShowCurrentStep) step at index: {_currentStepIndex} is null");
            return;
        }

        GameUI.Instance.ShowInGameMessage(step._message, step._lifeTime);

        if (_shouldExecuteNextStep == true)
        {
            float secondsToNextStep = step._lifeTime + 1;

            StartCoroutine(ShowNextStep(secondsToNextStep));
        }
    }

    IEnumerator ShowNextStep(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        _currentStepIndex += 1;

        ShowCurrentStep();
    }
       
}
