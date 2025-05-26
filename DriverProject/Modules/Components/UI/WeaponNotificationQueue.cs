using UnityEngine;
using RoR2;
using System;

namespace RobDriver.Modules.Components.UI
{
    public class WeaponNotificationQueue : MonoBehaviour
    {
        private CharacterMasterNotificationQueue.TimedNotificationInfo _notification;

        public float CurrentNotificationTime => this._notification is null ? 0f : (Run.instance.fixedTime - this._notification.startTime) / this._notification.duration;

        public CharacterMasterNotificationQueue.NotificationInfo CurrentNotification => this._notification?.notification;

        public event Action<WeaponNotificationQueue> onCurrentNotificationChanged;

        public static WeaponNotificationQueue GetNotificationQueueForMaster(CharacterMaster master)
        {
            if (master != null)
            {
                var characterMasterNotificationQueue = master.GetComponent<WeaponNotificationQueue>();
                if (!characterMasterNotificationQueue)
                {
                    characterMasterNotificationQueue = master.gameObject.AddComponent<WeaponNotificationQueue>();
                }
                return characterMasterNotificationQueue;
            }
            return null;
        }

        private void FixedUpdate()
        {
            if (this.CurrentNotificationTime > 1f)
            {
                this._notification = null;
                this.onCurrentNotificationChanged?.Invoke(this);
            }
        }

        public void PushWeaponNotification(CharacterMaster characterMaster, DriverWeaponDef weaponDef)
        {
            if (!characterMaster.hasAuthority)
                return;

            this._notification = new CharacterMasterNotificationQueue.TimedNotificationInfo
            {
                notification = new CharacterMasterNotificationQueue.NotificationInfo(weaponDef, null),
                startTime = Run.instance.fixedTime,
                duration = 3f
            };

            this.onCurrentNotificationChanged?.Invoke(this);
        }

        public void PushWeaponNotification(CharacterMaster characterMaster, DriverBulletDef bulletDef)
        {
            if (!characterMaster.hasAuthority)
                return;

            this._notification = new CharacterMasterNotificationQueue.TimedNotificationInfo
            {
                notification = new CharacterMasterNotificationQueue.NotificationInfo(bulletDef, null),
                startTime = Run.instance.fixedTime,
                duration = 3f
            };

            this.onCurrentNotificationChanged?.Invoke(this);
        }
    }
}