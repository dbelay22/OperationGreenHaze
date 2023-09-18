using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioReverbZone))]
[RequireComponent(typeof(BoxCollider))]
public class ReverbZoneTrigger : MonoBehaviour
{
    AudioReverbZone _reverbZone;

    void Start()
    {
        _reverbZone = GetComponent<AudioReverbZone>();
        _reverbZone.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        ProcessReverbZoneEnable(other, true);
    }

    void OnTriggerExit(Collider other)
    {
        ProcessReverbZoneEnable(other, false);
    }

    void ProcessReverbZoneEnable(Collider other, bool enableReverb)
    {
        if (other.transform.CompareTag(Tags.PLAYER_TAG))
        {
            Debug.Log($"[ReverbZoneTrigger] (ProcessReverbZoneEnable) Setting reverb {enableReverb}");

            _reverbZone.enabled = enableReverb;
        }
    }

}
