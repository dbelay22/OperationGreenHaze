using FMOD.Studio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JetFighter : MonoBehaviour
{
    EventInstance _jetSFX;

    void Awake()
    {
        _jetSFX = AudioController.Instance.Create3DInstance(FMODEvents.Instance.Jet, transform.position);
    }

    void Start()
    {
        AudioController.Instance.Play3DEvent(_jetSFX, transform.position, true);
    }

}
