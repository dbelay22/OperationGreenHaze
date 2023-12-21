using Cinemachine;
using FMODUnity;
using StarterAssets;
using System;
using System.Collections;
using UnityEngine;
using FMOD.Studio;
using System.Collections.Generic;

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

    // pool of shoots
    readonly int ShootPoolCount = 15;
    List<EventInstance> _shootSFXList;

    EventInstance _bulletImpactSFX;
    EventInstance _outOfAmmoSFX;
    EventInstance _zoomInSFX;
    EventInstance _zoomOutSFX;

    [Space(10)]
    [SerializeField] FirstPersonController _fpController;

    StarterAssetsInputs _input;

    bool _canShoot = true;
    
    bool _sniperZoomActive = false;
    bool _zooming = false;

    Ammo _ammo;

    Player _player;

    GameObject _model3D;

    void Awake()
    {
        _shootSFXList = new List<EventInstance>();

        for (int i = 0; i < ShootPoolCount; i++)
        {
            EventInstance shootInstance = AudioController.Instance.CreateInstance(_shootEventRef);
            
            _shootSFXList.Add(shootInstance);
        }    

        _outOfAmmoSFX = AudioController.Instance.CreateInstance(_outOfAmmoEventRef);
                
        _bulletImpactSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.BulletImpact, transform.position);

        _zoomInSFX = AudioController.Instance.CreateInstance(FMODEvents.Instance.WeaponZoomIn);
        
        _zoomOutSFX = AudioController.Instance.CreateInstance(FMODEvents.Instance.WeaponZoomOut);

        _model3D = transform.Find("Model").gameObject;
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

        ShowWeapon();

        GameUI.Instance.UpdateAmmoAmount(GetAmmoLeft());
    }

    public void WillBeDisabled()
    {
        //Debug.Log("Weapon] WillBeDisabled)...");

        if (_sniperZoomActive)
        {
            ZoomOut(0f);
        }
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
            // Go FOV zoom
            ZoomIn();
        }
    }

    void ZoomOut(float time = 0.4f)
    {
        //Debug.Log($"Weapon] ZoomOut) time: {time}");

        _player.OnWeaponZoomOut();

        AudioController.Instance.PlayEvent(_zoomOutSFX);

        if (time <= 0f)
        {
            ZoomOutImmediate();
        }
        else
        {
            StartCoroutine(ChangeFOV(_virtualCamera, _fovDefault, time));
        }

        _sniperZoomActive = false;
    }

    void ZoomOutImmediate()
    {
        //Debug.Log($"Weapon] ZoomOutImmediate)...");
        _virtualCamera.m_Lens.FieldOfView = _fovDefault;

        AfterFOVChange(_fovDefault);
    }

    void ZoomIn()
    {
        _player.OnWeaponZoomIn();

        _model3D.SetActive(false);

        AudioController.Instance.PlayEvent(_zoomInSFX);

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

        AfterFOVChange(endFOV);
    }

    void AfterFOVChange(float endFOV)
    {
        bool zoomedIn = endFOV == _fovZoom;

        ChangeMouseSensitivity(zoomedIn);

        if (!zoomedIn)
        {
            ShowWeapon();
        }        
    }

    void ShowWeapon()
    {
        if (!Game.Instance.IsGameplayOn()) {
            return;
        }

        _model3D.SetActive(true);
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
        //Debug.Log("Weapon] PlayShootSFX)...");

        int playingShoots = 0;

        foreach (EventInstance shootInstance in _shootSFXList)
        {
            if (AudioController.Instance.IsEventPlaying(shootInstance))            
            {
                playingShoots++;
                continue;
            }
            else
            {
                //Debug.Log($"Weapon] PlayShootSFX) Found available instance, playing NOW.");

                AudioController.Instance.PlayEvent(shootInstance, true);

                playingShoots++;
                
                break;
            }
        }
        
        //Debug.Log($"Weapon] PlayShootSFX) There are {playingShoots} playing shoots right now.");

        if (playingShoots >= ShootPoolCount)
        {
            Debug.LogWarning($"Weapon] PlayShootSFX) Increase the ShootPoolCount!");
        }
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
            //Debug.Log($"[Weapon](ShootUpdate) Just hit {hit.transform.name}, tag: {hit.transform.tag}, distance: {hit.distance}");

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
        //Debug.Log($"[Weapon] PlayHitImpactSFX) impact target: {impactTarget}");

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
