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
    [SerializeField] GameObject _hitImpactVFX;

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
        if (Game.Instance.isGameOver())
        {
            return;
        }

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
