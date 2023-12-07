using Cinemachine;
using FMODUnity;
using StarterAssets;
using System;
using System.Collections;
using UnityEngine;
using FMOD.Studio;

public class Weapon : MonoBehaviour
{
    private enum BullletImpactTarget
    {       
        Concrete = 0,
        Steel = 1,
        Zombie = 2
    }

    [SerializeField] GameObject _playerGO;

    [Header("Shooting")]
    [SerializeField] Camera _fpCamera;
    [SerializeField] float _raycastRange = 250;
    [SerializeField] float _coolDownSeconds = 1f;
    [SerializeField] float _damage = 25;
    [SerializeField] float _headshotDamage = 200;
    [SerializeField] ParticleSystem _muzzleFlashPS;
    [SerializeField] GameObject _hitImpactVFX;

    [Header("Ammo")]
    [SerializeField] AmmoType _ammoType;
    [SerializeField] int _ammoPerShot = 1;    

    [Header("Zoom")]
    [SerializeField] CinemachineVirtualCamera _virtualCamera;
    [SerializeField] float _fovDefault = 60;
    [SerializeField] float _fovZoom = 20;

    [Header("SFX")]
    [SerializeField] EventReference _shootEventRef;
    [SerializeField] EventReference _outOfAmmoEventRef;

    EventInstance _shootSFX;    
    EventInstance _bulletImpactSFX;
    EventInstance _outOfAmmoSFX;

    [Space(10)]
    [SerializeField] FirstPersonController _fpController;

    StarterAssetsInputs _input;

    bool _canShoot = true;
    
    bool _sniperZoomActive = false;
    bool _zooming = false;

    Ammo _ammo;

    Player _player;

    void Awake()
    {
        _shootSFX = AudioController.Instance.CreateInstance(_shootEventRef);

        _outOfAmmoSFX = AudioController.Instance.CreateInstance(_outOfAmmoEventRef);
                
        _bulletImpactSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.BulletImpact, transform.position);
    }

    void Start()
    {
        _player = _playerGO.GetComponent<Player>();
        _input = _playerGO.GetComponent<StarterAssetsInputs>();
        _ammo = _playerGO.GetComponent<Ammo>();
        
        _canShoot = true;

        GameUI.Instance.UpdateAmmoAmount(GetAmmoLeft());
    }

    void OnEnable()
    {
        _canShoot = true;

        ZoomOut();

        GameUI.Instance.UpdateAmmoAmount(GetAmmoLeft());
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    void Update()
    {
        if (!Game.Instance.IsGameplayOn())
        {
            return;
        }

        if (_input.shoot && _canShoot)
        {
            int ammoLeft = GetAmmoLeft();

            if (ammoLeft <= 0)
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
            }
        }

        if (_input.sniperZoom && !_zooming && _fovDefault != _fovZoom)
        {
            SniperZoomToggle();
        }
        
    }

    void SniperZoomToggle()
    {
        //Debug.Log($"SniperZoomToggle _sniperZoomActive:{_sniperZoomActive}");

        if (_sniperZoomActive)
        {
            // Go FOV default
            ZoomOut();
        }
        else
        {
            _player.OnWeaponZoomIn();

            // Go FOV zoom
            ZoomIn();
        }
    }

    void ZoomOut()
    {
        StartCoroutine(ChangeFOV(_virtualCamera, _fovDefault, .4f));
        _sniperZoomActive = false;
    }

    void ZoomIn()
    {
        StartCoroutine(ChangeFOV(_virtualCamera, _fovZoom, .5f));
        _sniperZoomActive = true;
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
        AudioController.Instance.PlayEvent(_shootSFX, true);
    }

    void PlayOutOfAmmoSFX()
    {
        AudioController.Instance.PlayEvent(_outOfAmmoSFX);
    }

    void PlayMuzzleFlashVFX()
    {
        if (_muzzleFlashPS == null)
        {
            return;
        }

        _muzzleFlashPS.Play();
    }

    void Shoot()
    {
        _canShoot = false;

        // Ray from screen center
        Ray ray = _fpCamera.ScreenPointToRay(new Vector3(_fpCamera.pixelWidth / 2, _fpCamera.pixelHeight / 2, 0f));

        //Debug.DrawRay(ray.origin, ray.direction, Color.red, 10f);

        // Raycast now
        bool hitSomething = Physics.Raycast(ray, out RaycastHit hit, _raycastRange);

        bool hitEnemy = false;

        // Did I Hit ?
        if (hitSomething)
        {
            Debug.Log($"[Weapon](ShootUpdate) Just hit {hit.transform.name}, tag: {hit.transform.tag}, distance: {hit.distance}");

            hitEnemy = ProcessHitEnemy(hit);

            if (!hitEnemy)
            {
                bool hitBoombox = ProcessHitBoomBox(hit);

                if (!hitBoombox) 
                {
                    if (!ProcessHitSteel(hit))
                    {
                        ProcessHitConcrete(hit);
                    }
                }
            }
        }

        // Notify Player
        _player.OnBulletShot(_ammoType, _ammoPerShot, hitEnemy);

        // cool down if still active
        if (gameObject.activeInHierarchy) 
        {
            StartCoroutine(CoolDown());
        }
    }

    bool ProcessHitBoomBox(RaycastHit hit)
    {
        bool hitBoomBox = hit.transform.CompareTag(Tags.BOOMBOX);

        if (hitBoomBox)
        {
            BoomBox bbox = hit.transform.GetComponent<BoomBox>();
            
            bbox.BoomNow();

            PlayHitImpactSFX(hit, BullletImpactTarget.Steel);
        }

        return hitBoomBox;
    }

    bool ProcessHitConcrete(RaycastHit hit)
    {
        bool hitConcrete = hit.transform.CompareTag(Tags.MATERIAL_CONCRETE);

        if (hitConcrete)
        {
            PlayHitImpactVFX(hit);

            PlayHitImpactSFX(hit, BullletImpactTarget.Concrete);
        }

        return hitConcrete;
    }

    bool ProcessHitSteel(RaycastHit hit)
    {
        bool hitSteel = hit.transform.CompareTag(Tags.MATERIAL_STEEL);

        if (hitSteel)
        {
            PlayHitImpactVFX(hit);

            PlayHitImpactSFX(hit, BullletImpactTarget.Steel);
        }

        return hitSteel;
    }

    bool ProcessHitEnemy(RaycastHit hit)
    {
        bool hitEnemy = hit.transform.CompareTag(Tags.ENEMY);

        // F*ck you zombie
        if (hitEnemy)
        {
            NpcAI npc = hit.transform.GetComponent<NpcAI>();

            Type colliderType = hit.collider.GetType();

            bool isHeadshot = colliderType == typeof(CapsuleCollider);

            npc.HitByBullet(isHeadshot ? _headshotDamage : _damage, hit, isHeadshot);

            if (isHeadshot)
            {
                Director.Instance.OnEvent(DirectorEvents.Enemy_Killed_Headshot);
            }

            PlayHitImpactSFX(hit, BullletImpactTarget.Zombie);
        }        

        return hitEnemy;
    }

    IEnumerator CoolDown()
    {
        yield return new WaitForSeconds(_coolDownSeconds);

        _canShoot = true;
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

    void PlayHitImpactSFX(RaycastHit hit, BullletImpactTarget impactTarget)
    {
        Debug.Log($"[Weapon] PlayHitImpactSFX) impact target: {impactTarget}");

        _bulletImpactSFX.setParameterByName(FMODEvents.Instance.ImpactMaterialParameter, (int) impactTarget);

        AudioController.Instance.Play3DEvent(_bulletImpactSFX, hit.point, true);
    }

    public AmmoType GetAmmoType()
    {
        return _ammoType;
    }

    public int GetAmmoLeft()
    {
        if (_ammo == null)
        {
            return 0;
        }

        return _ammo.GetAmmoLeft(_ammoType);
    }

}
