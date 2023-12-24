using System.Collections;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    float _durationSeconds = 0f;

    public void Show(float duration = 0f)
    {
        // show
        gameObject.SetActive(true);

        // should hide after some time ?
        if (duration > 0f)
        {
            _durationSeconds = duration;

            StartCoroutine(EndVFX());
        }
        else
        {
            StopAllCoroutines();
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
        return gameObject.activeSelf;
    }
}
