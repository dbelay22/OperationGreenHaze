using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCounter : MonoBehaviour
{
    [SerializeField] string _counterName = "noname";

    int _childCount;

    void Start()
    {
        _childCount = transform.childCount;

        Debug.Log($"[{_counterName}] (Start) _childCount:{_childCount}");        
    }

    void Update()
    {
        int childCount = transform.childCount;
        
        if (childCount != _childCount)
        {
            _childCount = childCount;

            Debug.Log($"[{_counterName}] _childCount now: {_childCount}");
        }
    }
}
