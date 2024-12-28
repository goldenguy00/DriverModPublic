using RobDriver.Modules.Survivors;
using RoR2;
using UnityEngine;

namespace RobDriver.Modules.Components
{
    internal class MagneticPickup : MonoBehaviour
    {
        // stole this code from MagneticPickups mod
        private void FixedUpdate()
        {
            if (!Config.enableMagneticPickups.Value)
                return;

            if (IsDriverPlayerNearby(this.transform.position, out var driverPosition))
            {
                MovePickupTowardsPlayer(driverPosition);
            }
        }

        // stole this code from MagneticPickups mod
        private bool IsDriverPlayerNearby(Vector3 thisPosition, out Vector3 driverPosition)
        {
            driverPosition = Vector3.zero;
            var lowestDistance = float.PositiveInfinity;
            foreach (var pcmc in PlayerCharacterMasterController.instances)
            {
                if (pcmc && pcmc.body && pcmc.body.baseNameToken == Driver.bodyNameToken)
                {
                    var distance = (pcmc.body.footPosition - thisPosition).sqrMagnitude;
                    if (distance < lowestDistance)
                    {
                        if (!Config.enableMagenticConditionalPickups.Value || (pcmc.body.TryGetComponent<DriverController>(out var iDrive) 
                            && iDrive && !iDrive.HasSpecialBullets && iDrive.weaponDef == iDrive.defaultWeaponDef))
                        {
                            driverPosition = pcmc.body.footPosition;
                            lowestDistance = distance;
                        }
                    }
                }
            }

            return lowestDistance < (Config.pickupRadius.Value * Config.pickupRadius.Value);
        }

        // stole this code from MagneticPickups mod
        private void MovePickupTowardsPlayer(Vector3 playerLocation)
        {
            var rigidBody = this.GetComponent<Rigidbody>();

            // Set the velocity to 0 to begin with to remove any gravity.
            rigidBody.velocity = Vector3.zero;

            // Move the pickup upwards, if it is not already high above the ground and there's nothing directly above it.
            var didHitUp = Physics.Raycast(
                this.transform.position,
                this.transform.up,
                out var upwardsHit
            );
            var didHitDown = Physics.Raycast(
                this.transform.position,
                -this.transform.up,
                out var downwardsHit
            );

            var hasSpaceUpwards = !didHitUp || upwardsHit.distance > 10f;
            var hasSpaceDownwards = didHitDown && downwardsHit.distance < 250f;

            // Only move upwards if the pickup has enough space.
            if (hasSpaceUpwards && hasSpaceDownwards)
            {
                rigidBody.velocity += Vector3.up * 1.25f;
            }

            var speed = Vector3.MoveTowards(
                rigidBody.velocity,
                (this.transform.position - playerLocation).normalized * 100f,
                50f
            );

            rigidBody.velocity -= speed;
        }
    }
}
