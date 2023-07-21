using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ammo : MonoBehaviour
{
    [SerializeField] int _ammoStart = 30;

    
    int _ammoLeft;
    public int AmmoLeft { get { return _ammoLeft; } }

    
    private void Start()
    {
        _ammoLeft = _ammoStart;
    }

    void OnBulletShot(int amount)
    {
        _ammoLeft -= amount;

        if (_ammoLeft < 0)
        {
            _ammoLeft = 0;
        }

        Debug.Log($"[Ammo] ammo left: {_ammoLeft}");
    }
}
