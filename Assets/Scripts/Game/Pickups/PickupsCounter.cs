using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupsCounter : MonoBehaviour
{
    int _pickupsLeft;

    void Start()
    {
        _pickupsLeft = transform.childCount;

        Debug.Log($"[PickupsCounter] (Start) _pickupsLeft:{_pickupsLeft}");        
    }

    void Update()
    {
        int pickupCount = transform.childCount;
        
        if (pickupCount != _pickupsLeft)
        {
            _pickupsLeft = pickupCount;

            Debug.Log($"[PickupsCounter] pickups left now: {_pickupsLeft}");
        }
    }
}
