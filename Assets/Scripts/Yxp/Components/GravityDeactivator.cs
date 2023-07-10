using UnityEngine;


namespace Yxp.Behaviours
{
    [RequireComponent(typeof(Rigidbody))]
    public class GravityDeactivator : MonoBehaviour
    {
        void Start()
        {
            // deactivate gravity for the obstacle
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
        }
    }
}
