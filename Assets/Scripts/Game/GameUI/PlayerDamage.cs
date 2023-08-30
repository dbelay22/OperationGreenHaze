using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamage : MonoBehaviour
{
    [SerializeField] float _durationSeconds = 2.2f;

    void OnEnable()
    {
        StartCoroutine(EndVFX());
    }

    IEnumerator EndVFX()
    {
        yield return new WaitForSeconds(_durationSeconds);

        gameObject.SetActive(false);
    }

    
}
