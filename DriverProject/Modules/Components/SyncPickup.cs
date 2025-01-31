using RoR2;
using UnityEngine.Networking;

namespace RobDriver.Modules.Components
{
    public class SyncPickup : NetworkBehaviour
    {
        public WeaponPickup weaponPickupComponent;

        [SyncVar]
        public bool cutAmmo;
        [SyncVar]
        public bool isNewAmmoType;
        [SyncVar]
        public ushort bulletIndex;
        [SyncVar]
        public ushort weaponIndex;

        public DriverWeaponDef weaponDef
        {
            get => DriverWeaponCatalog.GetWeaponFromIndex(weaponIndex);
            set => weaponIndex = value?.index ?? DriverWeaponCatalog.Pistol.index;
        }

        public DriverBulletDef bulletDef
        {
            get => DriverBulletCatalog.GetBulletDefFromIndex(bulletIndex);
            set => bulletIndex = value?.index ?? DriverBulletCatalog.Default.index;
        }

        public void SpawnWeapon(TeamIndex teamIndex, DriverWeaponDef weaponDef, DriverBulletDef bulletDef, bool cutAmmo = false, bool isNewAmmoType = false)
        {
            if (NetworkServer.active)
            {
                Log.Warning("Spawn weapon called");
                this.weaponPickupComponent.teamFilter.teamIndex = teamIndex;
                this.weaponDef = weaponDef;
                this.bulletDef = bulletDef;
                this.cutAmmo = cutAmmo;
                this.isNewAmmoType = isNewAmmoType;

                NetworkServer.Spawn(this.gameObject);
            }
        }

        private void Start()
        {
            Log.Warning("Start called " + this.weaponIndex + " | " + this.bulletIndex + " | " + this.cutAmmo + " | " + this.isNewAmmoType);
            this.weaponPickupComponent.UpdateWeaponPickup(this.weaponDef, this.bulletDef, this.cutAmmo, this.isNewAmmoType);
        }
    }
}
