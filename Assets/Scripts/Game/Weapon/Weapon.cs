using Cinemachine;
using StarterAssets;
using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Ammo))]
public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject _player;

    [Header("Shooting")]
    [SerializeField] Camera _fpCamera;
    [SerializeField] float _raycastRange = 250;
    [SerializeField] float _coolDownSeconds = 1f;
    [SerializeField] float _damage = 25;
    [SerializeField] ParticleSystem _muzzleFlashPS;
    [SerializeField] GameObject _hitImpactVFX;
    [SerializeField] int _ammoPerShot = 3;    

    [Header("Zoom")]
    [SerializeField] CinemachineVirtualCamera _virtualCamera;
    [SerializeField] float _fovDefault = 60;
    [SerializeField] float _fovZoom = 20;

    [Header("SFX")]
    [SerializeField] AudioClip _shootSFX;
    [SerializeField] AudioClip _outOfAmmoSFX;

    [Space(10)]
    [SerializeField] FirstPersonController _fpController;

    StarterAssetsInputs _input;

    AudioSource _audioSource;

    bool _canShoot = true;
    
    bool _sniperZoomActive = false;
    bool _zooming = false;

    Ammo _ammo;

    void Start()
    {
        _input = _player.GetComponent<StarterAssetsInputs>();
        _audioSource = GetComponent<AudioSource>();
        _ammo = GetComponent<Ammo>();
        
        _canShoot = true;
    }

    void Update()
    {
        if (Game.Instance.isGameOver())
        {
            return;
        }

        if (_input.shoot && _canShoot)
        {
            if (_ammo.AmmoLeft <= 0)
            {
                PlayOutOfAmmoSFX();

                _canShoot = false;

                StartCoroutine(CoolDown());
            }
            else
            {
                PlayMuzzleFlashVFX();

                PlayShootSFX();

                Shoot();

                BroadcastMessage("OnBulletShot", _ammoPerShot, SendMessageOptions.RequireReceiver);
            }
        }

        if (_input.sniperZoom && !_zooming)
        {
            SniperZoomToggle();
        }
        
    }

    void SniperZoomToggle()
    {
        if (_sniperZoomActive)
        {
            // Go FOV default
            StartCoroutine(ChangeFOV(_virtualCamera, _fovDefault, .4f));
        }
        else
        {
            // Go FOV zoom
            StartCoroutine(ChangeFOV(_virtualCamera, _fovZoom, .5f));
        }

        _sniperZoomActive = !_sniperZoomActive;
    }

    IEnumerator ChangeFOV(CinemachineVirtualCamera cam, float endFOV, float duration)
    {
        _zooming = true;
        float startFOV = cam.m_Lens.FieldOfView;
        float time = 0;
        while (time < duration)
        {
            cam.m_Lens.FieldOfView = Mathf.Lerp(startFOV, endFOV, time / duration);
            yield return null;
            time += Time.deltaTime;
        }
        _zooming = false;

        ChangeMouseSensitivity(endFOV == _fovZoom);
    }

    void ChangeMouseSensitivity(bool zoomed)
    {
        if (zoomed)
        {
            _fpController.RotationSpeed = 1.3f;
        }
        else
        {
            _fpController.RotationSpeed = 1f;
        }
    }

    void PlayShootSFX()
    {
        _audioSource.PlayOneShot(_shootSFX);
    }

    void PlayOutOfAmmoSFX()
    {
        _audioSource.PlayOneShot(_outOfAmmoSFX);
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

        //Debug.DrawRay(ray.origin, ray.direction, Color.red, 10f);

        // Raycast now
        RaycastHit hit;

        bool hitSomething = Physics.Raycast(ray, out hit, _raycastRange);

        if (hitSomething)
        {
            Debug.Log($"[Weapon](ShootUpdate) Just hit {hit.transform.name}, tag: {hit.transform.tag}, distance: {hit.distance}");

            bool hitEnemy = hit.transform.tag.Equals("Enemy");

            if (hitEnemy)
            {
                NpcAI npc = hit.transform.GetComponent<NpcAI>();
                npc.HitByBullet(_damage, hit);
            }
            else
            {
                PlayHitImpactVFX(hit);
            }

        }

        StartCoroutine(CoolDown());
    }

    void PlayHitImpactVFX(RaycastHit hit)
    {
        if (_hitImpactVFX == null)
        {
            return;
        }
        
        GameObject hitImpact = Instantiate(_hitImpactVFX, hit.point, Quaternion.LookRotation(hit.normal));
        
        Destroy(hitImpact, 1.5f);
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(_coolDownSeconds);

        _canShoot = true;
    }

}
