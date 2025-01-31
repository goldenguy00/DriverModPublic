using RobDriver.Modules.Survivors;
using RoR2;
using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class MagneticPickup : MonoBehaviour
    {
        private const float ACCELERATION = 50f;
        private const float MAX_SPEED = 100f;

        [Tooltip("The rigidbody to set the velocity of.")]
        public Rigidbody rigidbody;

        [Tooltip("The TeamFilter which controls which team can activate this trigger.")]
        public TeamFilter teamFilter;

        public Transform gravitateTarget;

        private void OnTriggerEnter(Collider other)
        {
            if (gravitateTarget || teamFilter.teamIndex == TeamIndex.None)
                return;

            var teamComponent = other.GetComponent<TeamComponent>();
            if (teamComponent && teamComponent.teamIndex == teamFilter.teamIndex && teamComponent.body && teamComponent.body.bodyIndex == Driver.bodyIndex)
            {
                var iDrive = teamComponent.body.GetComponent<DriverController>();
                if (iDrive && (!Config.enableMagenticConditionalPickups.Value || (!iDrive.HasSpecialBullets && iDrive.HasLoadoutWeapon)))
                {
                    gravitateTarget = other.transform;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.transform == gravitateTarget)
                gravitateTarget = null;
        }

        private void FixedUpdate()
        {
            if (!Config.enableMagneticPickups.Value || Config.pickupRadius.Value <= 0f)
                return;

            if (gravitateTarget)
            {
                rigidbody.velocity = Vector3.MoveTowards(rigidbody.velocity, (gravitateTarget.transform.position - base.transform.position).normalized * MAX_SPEED, ACCELERATION);
            }
        }
    }
}
