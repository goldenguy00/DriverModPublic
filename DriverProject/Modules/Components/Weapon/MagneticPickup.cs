using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class MagneticPickup : MonoBehaviour
    {
        private const float ACCELERATION = 30f;
        private const float MAX_SPEED = 100f;

        public Rigidbody rigidbody;
        public WeaponPickup weaponPickup;

        private DriverController gravitateTarget;

        private void Awake()
        {
            this.GetComponent<SphereCollider>().radius = Mathf.Max(1f, Config.pickupRadius.Value);
        }

        private void FixedUpdate()
        {
            if (!Config.enableMagneticPickups.Value || Config.pickupRadius.Value <= 0f)
                return;

            if (CanPickUpWeapon(this.gravitateTarget))
            {
                var speed = (this.gravitateTarget.transform.position - base.transform.position).normalized * MAX_SPEED;
                this.rigidbody.velocity = Vector3.MoveTowards(this.rigidbody.velocity, speed, ACCELERATION);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!this.gravitateTarget)
            {
                var iDrive = other.GetComponent<DriverController>();
                if (iDrive)
                {
                    this.gravitateTarget = iDrive;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (this.gravitateTarget && this.gravitateTarget.gameObject == other.gameObject)
            {
                this.gravitateTarget = null;
            }
        }

        private bool CanPickUpWeapon(DriverController iDrive)
        {
            if (!iDrive)
                return false;

            if (!Config.enableMagenticConditionalPickups.Value || iDrive.passive.isPistolOnly)
                return true;

            if (iDrive.passive.isBullets || (iDrive.passive.isRyan && this.weaponPickup.isNewAmmoType))
            {
                if (iDrive.currentBulletDef == this.weaponPickup.bulletDef)
                    return true;

                return !iDrive.HasSpecialBullets || iDrive.AmmoPercent < 0.1f;
            }

            if (iDrive.weaponDef == this.weaponPickup.weaponDef)
                return true;

            // weapon
            return !iDrive.IsHoldingWeapon;
        }
    }
}
