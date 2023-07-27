using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesCounter : MonoBehaviour
{
    void Start()
    {
        int zombiesCount = transform.childCount;

        Debug.Log($"[ZombiesCounter] (Start) zombiesCount:{zombiesCount}");

        HUD.Instance.InitKills(zombiesCount);
    }

}
