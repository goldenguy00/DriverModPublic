using UnityEngine.Networking;

namespace RobDriver.Modules.Components
{
    internal class SyncPickup : NetworkBehaviour
    {
        public bool cutAmmo;
        public bool isNewAmmoType;
        public DriverBulletDef bulletDef = DriverBulletCatalog.Default;

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
            RpcUpdateVisuals(this.bulletDef.index, this.cutAmmo, this.isNewAmmoType);
        }

        [ClientRpc]
        public void RpcUpdateVisuals(ushort bulletIndex, bool cutAmmo, bool isNewAmmoType)
        {
            var weaponPickup = this.gameObject.GetComponentInChildren<WeaponPickup>();
            weaponPickup.UpdateWeaponPickup(DriverBulletCatalog.GetBulletDefFromIndex(bulletIndex), cutAmmo, isNewAmmoType);
        }
    }
}
