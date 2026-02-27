using RoR2;
using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class DriverWeaponTracker : MonoBehaviour
    {
        public struct StoredWeapon
        {
            public ushort defaultIndex;
            public ushort weaponIndex;
            public ushort bulletIndex;
            public float ammo;
        };

        private StoredWeapon storedWeapon;
        public bool hasWeapon;

        public void StoreWeapon(DriverWeaponDef defaultDef, DriverWeaponDef weaponDef, DriverBulletDef bulletDef, float ammo)
        {
            hasWeapon = true;
            storedWeapon = new StoredWeapon
            {
                defaultIndex = defaultDef.index,
                weaponIndex = weaponDef.index,
                bulletIndex = bulletDef.index,
                ammo = ammo
            };
        }

        public StoredWeapon RetrieveWeapon()
        {
            this.hasWeapon = false;
            return storedWeapon;
        }
    }
}