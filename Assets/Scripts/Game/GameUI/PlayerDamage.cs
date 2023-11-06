using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    float _durationSeconds = 0f;

    public void Show(float duration = 0f)
    {
        // show
        gameObject.SetActive(true);

        // should hide after some time ?
        if (duration > 0)
        {
            _durationSeconds = duration;

            StartCoroutine(EndVFX());
        }
    }

    IEnumerator EndVFX()
    {
        yield return new WaitForSeconds(_durationSeconds);

        Hide();
    }


    public void Hide()
    {
        gameObject.SetActive(false);
    }


    public bool IsActive()
    {
        return gameObject.activeInHierarchy;
    }
}
