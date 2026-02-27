using RoR2;
using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class RocketRotation : MonoBehaviour
    {
        private Rigidbody rb;

        private void Awake()
        {
            this.rb = this.GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            this.transform.rotation = Util.QuaternionSafeLookRotation(this.rb.velocity.normalized);
        }
    }
}