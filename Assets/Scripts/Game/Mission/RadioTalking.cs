using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioTalking : MonoBehaviour
{
    [Header("FMOD Events")]
    public EventReference Start;
    public EventReference PlayVGood;
    public EventReference PlayGood;
    public EventReference PlayBad;
    public EventReference Damage;
    public EventReference UseFlashlight;
    public EventReference OutOfAmmo;
    public EventReference MissionObj1;
    public EventReference MissionObj2;
    public EventReference KillComplete;
    public EventReference FindExit;
    public EventReference TooClose;
    public EventReference Time1;
    public EventReference Time2;
    public EventReference Time3;

    #region Instance

    private static RadioTalking _instance;

    public static RadioTalking Instance { get { return _instance; } }

    #endregion

    void Awake()
    {
        _instance = this;
    }

    public void PlayMessage(EventReference eventRef)
    {
        AudioController.Instance.PlayFromListOrCreate(eventRef, true);
    }

}