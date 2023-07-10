using StarterAssets;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] GameObject _player;

    StarterAssetsInputs _input;

    void Start()
    {
        _input = _player.GetComponent<StarterAssetsInputs>();
    }

    void Update()
    {
        Debug.Log($"[Weapon] shoot:{_input.shoot}");
        if (_input.shoot)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        //Debug.Log($"[Weapon] Shooting");
    }

}
