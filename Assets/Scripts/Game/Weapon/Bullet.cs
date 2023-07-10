using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Hit")]
    [SerializeField] GameObject _hitFx;

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[Bullet] collisioned with {collision.collider.name}");
        
        ShowHitEffect(collision.GetContact(0));
        
        StartCoroutine(DestroyDelayed());
    }

    void ShowHitEffect(ContactPoint contactPoint)
    {
        GameObject hitFxInstance = Instantiate(_hitFx, contactPoint.point, Quaternion.LookRotation(contactPoint.normal));

        Destroy(hitFxInstance, 2f);
    }

    IEnumerator DestroyDelayed()
    {
        yield return new WaitForFixedUpdate();
        Destroy(gameObject);
    }
}
