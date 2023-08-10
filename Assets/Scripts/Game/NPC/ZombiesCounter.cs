using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesCounter : MonoBehaviour
{
    void Start()
    {
        int zombiesCount = CountZombies();

        Debug.Log($"[ZombiesCounter] (Start) zombiesCount:{zombiesCount}");

        HUD.Instance.InitKills(zombiesCount);
    }

    int CountZombies()
    {
        int count = 0;
        
        foreach (Transform t in transform)
        {
            if (t.tag.Equals(Game.QUADRANT_TAG) && t.gameObject.activeSelf)
            {
                foreach (Transform tchild in t.transform)
                {
                    if (tchild.tag.Equals(Game.ENEMY_GROUP_TAG) && tchild.gameObject.activeSelf)
                    {
                        count += tchild.childCount;
                    }
                }
            }
        }

        return count;
    }

}
