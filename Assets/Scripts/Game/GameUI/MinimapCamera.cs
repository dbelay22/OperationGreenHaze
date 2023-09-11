using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] Vector3 _offset;

    void Update()
    {
        transform.position = _player.position + _offset;

        // rotation
        Vector3 r = new Vector3(90, _player.eulerAngles.y, 0);

        transform.rotation = Quaternion.Euler(r);
    }
}
