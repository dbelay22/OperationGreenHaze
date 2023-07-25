using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] AmmoType _ammoType;
    public AmmoType AmmoType { get { return _ammoType; } }

    [SerializeField] int _ammoAmount;
    public int AmmoAmount { get { return _ammoAmount; } }
}
