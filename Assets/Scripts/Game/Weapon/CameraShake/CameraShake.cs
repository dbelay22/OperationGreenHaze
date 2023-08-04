using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] CinemachineVirtualCamera _virtualCamera;

    float _currentFoV;
    float _originalFoV;

    Vector3 _originalPosition;
    
    float _currentShakeMagnitude;

    Coroutine _shakeCoroutine;

    void Start()
    {
        _originalPosition = transform.localPosition;
    }

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

        // Calculate target FoV
        float _originalFoV = _virtualCamera.m_Lens.FieldOfView;        
        float targetFoV = _originalFoV + Random.Range(-maxFoVChange, maxFoVChange);
        _currentFoV = _originalFoV;

        //Debug.Log($"[CameraShake] (ShakeCoroutine) ************************");
        //Debug.Log($"[CameraShake] (ShakeCoroutine) _originalFoV={_originalFoV}, targetFoV={targetFoV}");

        while (elapsedTime < duration)
        {
            float percentComplete = elapsedTime / duration;

            //Debug.Log($"[CameraShake] (ShakeCoroutine) (................................");
            //Debug.Log($"[CameraShake] (ShakeCoroutine) percentComplete={percentComplete}");

            #region Local Position change

            _currentShakeMagnitude = Mathf.Lerp(0f, magnitude, percentComplete);
            //Debug.Log($"[CameraShake] (ShakeCoroutine) _currentShakeMagnitude={_currentShakeMagnitude}");

            Vector3 randomOffset = Random.insideUnitSphere * _currentShakeMagnitude;
            randomOffset.z = _originalPosition.z;
            //Debug.Log($"[CameraShake] (ShakeCoroutine) randomOffset:{randomOffset}");
            
            transform.localPosition = _originalPosition + randomOffset;

            #endregion

            #region FOV change

            _currentFoV = Mathf.Lerp(_currentFoV, targetFoV, percentComplete);
            
            //Debug.Log($"[CameraShake] (ShakeCoroutine) _currentFoV={_currentFoV}");
            //Debug.Log($"[CameraShake] (ShakeCoroutine) ........................)");

            _virtualCamera.m_Lens.FieldOfView = _currentFoV;

            //_virtualCamera.m_Lens.FieldOfView = Mathf.SmoothDamp(_currentFoV, targetFoV, ref _currentShakeMagnitude, _smoothDampTime); ;

            #endregion

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        _currentShakeMagnitude = 0f;

        transform.localPosition = _originalPosition;

        _virtualCamera.m_Lens.FieldOfView = _originalFoV;
    }

}
