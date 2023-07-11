using StarterAssets;
using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject _player;

    [Header("Shooting")]
    [SerializeField] float _raycastRange = 250;
    [SerializeField] float _coolDownSeconds = 1f;
    [SerializeField] float _damage = 25;
    [SerializeField] ParticleSystem _muzzleFlashPS;

    [Space(10)]
    [SerializeField] Camera _fpCamera;

    StarterAssetsInputs _input;

    AudioSource _shootFXSource;

    bool _canShoot = true;

    void Start()
    {
        _input = _player.GetComponent<StarterAssetsInputs>();
        _shootFXSource = GetComponent<AudioSource>();
        _canShoot = true;
    }

    void Update()
    {
        if (_input.shoot == false || _canShoot == false)
        {
            return;
        }

        PlayMuzzleFlashVFX();
        PlayShootFX();
        Shoot();
    }

    void PlayShootFX()
    {
        _shootFXSource.Play();
    }

    void PlayMuzzleFlashVFX()
    {
        if (_muzzleFlashPS == null)
        {
            return;
        }

        _muzzleFlashPS.Play();
    }

    private void Shoot()
    {
        _canShoot = false;

        // Ray from screen center
        Ray ray = _fpCamera.ScreenPointToRay(new Vector3(_fpCamera.pixelWidth / 2, _fpCamera.pixelHeight / 2, 0f));

        Debug.DrawRay(ray.origin, ray.direction, Color.red, 10f);

        // Raycast now
        RaycastHit hit;

        bool hitSomething = Physics.Raycast(ray, out hit, _raycastRange);

        if (hitSomething == true && hit.transform.tag.Equals("Enemy"))
        {
            //Debug.Log($"[Weapon](ShootUpdate) Just hit {hit.transform.name}, distance: {hit.distance}, hit point: {hit.point}");
            EnemyHealth health = hit.transform.GetComponent<EnemyHealth>();
            health.Shoot(_damage);
        }

        StartCoroutine(CoolDown());
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(_coolDownSeconds);

        _canShoot = true;
    }

}
