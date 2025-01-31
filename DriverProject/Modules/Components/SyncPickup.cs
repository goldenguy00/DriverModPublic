using RoR2;
using UnityEngine.Networking;

namespace RobDriver.Modules.Components
{
    public class SyncPickup : NetworkBehaviour
    {
        public WeaponPickup weaponPickupComponent;

        public bool cutAmmo;
        public bool isNewAmmoType;
        public DriverBulletDef bulletDef = DriverBulletCatalog.Default;
        public DriverWeaponDef weaponDef = DriverWeaponCatalog.Pistol;

        public void SpawnWeapon(TeamIndex teamIndex, DriverWeaponDef weaponDef, DriverBulletDef bulletDef, bool cutAmmo = false, bool isNewAmmoType = false)
        {
            if (NetworkServer.active)
            {
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
            if (NetworkServer.active && this.isClient)
            {
                CmdUpdateVisuals();
            }
        }

        [Command]
        public void CmdUpdateVisuals()
        {
            RpcUpdateVisuals(this.weaponDef.index, this.bulletDef.index, this.cutAmmo, this.isNewAmmoType);
        }

        [ClientRpc]
        public void RpcUpdateVisuals(ushort weaponIndex, ushort bulletIndex, bool cutAmmo, bool isNewAmmoType)
        {
            var weaponDef = DriverWeaponCatalog.GetWeaponFromIndex(weaponIndex);
            var bulletDef = DriverBulletCatalog.GetBulletDefFromIndex(bulletIndex);

            this.weaponPickupComponent.UpdateWeaponPickup(weaponDef, bulletDef, cutAmmo, isNewAmmoType);
        }
    }
}
