using UnityEngine.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace RobDriver.Modules.Components
{
    internal class SyncStoredWeapon : INetMessage
    {
        private NetworkInstanceId netId;
        private ushort defaultIndex;
        private ushort weaponIndex;
        private ushort bulletIndex;
        private float ammo;

        public SyncStoredWeapon()
        {
        }

        public SyncStoredWeapon(NetworkInstanceId netId, DriverWeaponTracker.StoredWeapon storedWeapon)
        {
            this.netId = netId;
            this.defaultIndex = storedWeapon.defaultIndex;
            this.weaponIndex = storedWeapon.weaponIndex;
            this.bulletIndex = storedWeapon.bulletIndex;
            this.ammo = storedWeapon.ammo;
        }

        public void OnReceived()
        {
            GameObject bodyObject = Util.FindNetworkObject(this.netId);
            if (!bodyObject) return;

            DriverController iDrive = bodyObject.GetComponent<DriverController>();
            if (iDrive)
            {
                iDrive.currentBulletDef = DriverBulletCatalog.GetBulletFromIndex(this.bulletIndex);
                iDrive.defaultWeaponDef = DriverWeaponCatalog.GetWeaponFromIndex(this.defaultIndex);

                iDrive.PickUpWeapon(iDrive.defaultWeaponDef);
                iDrive.PickUpWeapon(DriverWeaponCatalog.GetWeaponFromIndex(this.weaponIndex));
                iDrive.SetBulletAmmo(false, this.ammo);
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.defaultIndex);
            writer.Write(this.weaponIndex);
            writer.Write(this.bulletIndex);
            writer.Write(this.ammo);
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.defaultIndex = reader.ReadUInt16();
            this.weaponIndex = reader.ReadUInt16();
            this.bulletIndex = reader.ReadUInt16();
            this.ammo = reader.ReadSingle();
        }
    }
}