using UnityEngine;
using UnityEngine.Networking;

namespace RobDriver.Modules.Components
{
    public class SyncPickup : NetworkBehaviour
    {
        private Rigidbody rigidBody;

        [SyncVar]
        public bool cutAmmo;
        [SyncVar]
        public bool isNewAmmoType;
        [SyncVar]
        public ushort bulletIndex;
        [SyncVar]
        public ushort weaponIndex;

        public DriverWeaponDef WeaponDef
        {
            get => DriverWeaponCatalog.GetWeaponFromIndex(weaponIndex);
            set => weaponIndex = value?.index ?? DriverWeaponCatalog.Pistol.index;
        }

        public DriverBulletDef BulletDef
        {
            get => DriverBulletCatalog.GetBulletFromIndex(bulletIndex);
            set => bulletIndex = value?.index ?? DriverBulletCatalog.Default.index;
        }

        public WeaponPickup weaponPickup;

        public void SpawnWeapon(DriverWeaponDef weaponDef, DriverBulletDef bulletDef, bool cutAmmo, bool isNewAmmoType)
        {
            if (NetworkServer.active)
            {
                this.WeaponDef = weaponDef;
                this.BulletDef = bulletDef;
                this.cutAmmo = cutAmmo;
                this.isNewAmmoType = isNewAmmoType;

                NetworkServer.Spawn(this.gameObject);
            }
        }

        private void Awake()
        {
            this.rigidBody = GetComponent<Rigidbody>();
        }

        private void Start()
        {
            this.weaponPickup.UpdateWeaponPickup(this.WeaponDef, this.BulletDef, this.cutAmmo, this.isNewAmmoType);
            this.rigidBody.AddForce(Vector3.up * 8f, ForceMode.Impulse);
        }
    }
}
