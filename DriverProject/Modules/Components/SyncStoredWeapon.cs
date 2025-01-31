using UnityEngine.Networking;
using R2API.Networking.Interfaces;
using UnityEngine;
using RoR2;

namespace RobDriver.Modules.Components
{
    internal class SyncStoredWeapon : INetMessage
    {
        private NetworkInstanceId netId;
        private ushort weaponIndex;
        private ushort bulletIndex;
        private long ammo;

        public SyncStoredWeapon()
        {
        }

        public SyncStoredWeapon(NetworkInstanceId netId, ushort augh, ushort ough, float ammo)
        {
            this.netId = netId;
            this.weaponIndex = augh;
            this.bulletIndex = ough;
            this.ammo = Mathf.CeilToInt(ammo * 100f);
        }

        public void Deserialize(NetworkReader reader)
        {
            this.netId = reader.ReadNetworkId();
            this.weaponIndex = reader.ReadUInt16();
            this.bulletIndex = reader.ReadUInt16();
            this.ammo = reader.ReadInt64();
        }

        public void OnReceived()
        {
            GameObject bodyObject = Util.FindNetworkObject(this.netId);
            if (!bodyObject) return;

            DriverController iDrive = bodyObject.GetComponent<DriverController>();
            if (iDrive)
            {
                DriverWeaponDef weaponDef = DriverWeaponCatalog.GetWeaponFromIndex(this.weaponIndex);
                DriverBulletDef bulletDef = DriverBulletCatalog.GetBulletDefFromIndex(this.bulletIndex);

                // yes, this is a dumb way to do it
                float ammoVal = this.ammo;
                if (ammoVal != -1) ammoVal *= 0.01f;
                iDrive.PickUpWeaponDrop(weaponDef, bulletDef, ammoVal, false, false /*isNewAmmo*/);
                iDrive.PickUpWeaponDrop(weaponDef, bulletDef, ammoVal, false, true /*isNewAmmo*/);
            }
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(this.netId);
            writer.Write(this.weaponIndex);
            writer.Write(this.bulletIndex);
            writer.Write(this.ammo);
        }
    }
}