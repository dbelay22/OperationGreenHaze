using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] CinemachineVirtualCamera _virtualCamera;

    float _currentFoV;
    float _currentShakeMagnitude;

    Coroutine _shakeCoroutine;

    public void Shake(WeaponShakeData.ShakeProperties props)
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
        }

        _shakeCoroutine = StartCoroutine(ShakeCoroutine(props.magnitude, props.duration, props.maxFOVChange));
    }

    IEnumerator ShakeCoroutine(float magnitude, float duration, float maxFoVChange)
    {
        float elapsedTime = 0f;

        // FOV
        float originalFoV = _virtualCamera.m_Lens.FieldOfView;

        float targetFoV = originalFoV + Random.Range(-maxFoVChange, maxFoVChange);
        
        _currentFoV = originalFoV;

        // Rotation
        Vector3 originalRotation = transform.localEulerAngles;

        while (elapsedTime < duration)
        {
            float percentComplete = elapsedTime / duration;

            #region Local Rotation

            // interpolation in time
            _currentShakeMagnitude = Mathf.Lerp(0f, magnitude, percentComplete);

            // random rotation
            Vector3 rotOffset = Random.insideUnitSphere * _currentShakeMagnitude;

            // apply
            transform.localEulerAngles = originalRotation + rotOffset;

            #endregion

            #region FOV change

            _currentFoV = Mathf.Lerp(_currentFoV, targetFoV, percentComplete);
            
            //Debug.Log($"[CameraShake] (ShakeCoroutine) _currentFoV={_currentFoV}");
            //Debug.Log($"[CameraShake] (ShakeCoroutine) ........................)");

            _virtualCamera.m_Lens.FieldOfView = _currentFoV;

            #endregion

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _currentShakeMagnitude = 0f;

        // back to previous state of FOV and rotation
        transform.localEulerAngles = originalRotation;
        _virtualCamera.m_Lens.FieldOfView = originalFoV;
    }

}
