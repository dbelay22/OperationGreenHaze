using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesCounter : MonoBehaviour
{
    void Start()
    {
        int zombiesCount = CountZombies();

        //Debug.Log($"[ZombiesCounter] (Start) zombiesCount:{zombiesCount}");

        GameUI.Instance.InitKills(zombiesCount);
    }

    int CountZombies()
    {
        int count = 0;
        
        foreach (Transform t in transform)
        {
            if (t.CompareTag(Tags.ENEMY_GROUP) && t.gameObject.activeSelf)
            {
                count += t.childCount;
            }
        }

        return count;
    }

}
