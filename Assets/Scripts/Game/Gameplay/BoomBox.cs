using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BoomBox: MonoBehaviour
{
    [SerializeField] GameObject _object;

    [Header("Explosion")]
    [SerializeField] GameObject _explosionVFX;
    [SerializeField] AudioClip _explosionSFX;
    [SerializeField] float _durationSeconds = 1f;

    [Header("After Explosion")]
    [SerializeField] GameObject _fireAndSmokeVFX;
    [SerializeField] float _lifeTime = 5f;

    AudioSource _audioSource;

    
    
    void Start()
    {
        if (_object == null)
        {
            Debug.LogError("[BoomBox] (Start) object is not set");
            return;
        }

        _audioSource = GetComponent<AudioSource>();

        // set in the same place as the object
        //transform.position = _object.transform.position;
    }

    public void BoomNow()
    {
        if (_object == null)
        {
            Debug.LogError("[BoomBox] (BoomNow) object is not set");
            return;
        }

        if (_explosionVFX == null)
        {
            Debug.LogError("[BoomBox] (BoomNow) explosion is not set");
            return;
        }

        // deactivate box collider
        BoxCollider collider = GetComponent<BoxCollider>();
        collider.enabled = false;

        // object destroy
        Destroy(_object);

        // play sound
        _audioSource.PlayOneShot(_explosionSFX);

        // spawn explosion
        Instantiate(_explosionVFX, transform);

        // spawn fire and smoke
        Instantiate(_fireAndSmokeVFX, transform);

        // bye
        StartCoroutine(AutoDestroy());
    }


    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(_lifeTime);

        StopAllCoroutines();        

        Destroy(gameObject);
    }

}
