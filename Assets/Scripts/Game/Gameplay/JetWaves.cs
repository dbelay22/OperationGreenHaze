using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetWaves : MonoBehaviour
{
    EventInstance _explosionsSFX;
    EventInstance _raidSirenSFX;

    void Awake()
    {
        _explosionsSFX = AudioController.Instance.CreateInstance(FMODEvents.Instance.JetBombExplosions);
        _raidSirenSFX = AudioController.Instance.CreateInstance(FMODEvents.Instance.RaidSiren);
    }

    public void PlayExplosionSFX()
    {
        AudioController.Instance.PlayEvent(_explosionsSFX);
    }

    public void PlaySirenSFX()
    {
        AudioController.Instance.PlayEvent(_raidSirenSFX);
    }

}