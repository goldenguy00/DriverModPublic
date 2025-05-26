using UnityEngine;
using RoR2;
using RoR2.UI;

namespace RobDriver.Modules.Components.UI
{
    public class WeaponNotificationUIController : MonoBehaviour
    {
        public HUD hud;
        public CharacterMaster targetMaster;

        public GenericNotification currentNotification;
        public WeaponNotificationQueue notificationQueue;

        public void OnEnable()
        {
            CharacterMaster.onCharacterMasterLost += this.OnCharacterMasterLost;
        }

        public void OnDisable()
        {
            CharacterMaster.onCharacterMasterLost -= this.OnCharacterMasterLost;
            this.CleanUpCurrentMaster();
        }

        private void OnCharacterMasterLost(CharacterMaster master)
        {
            if (master == this.targetMaster)
                this.CleanUpCurrentMaster();
        }

        public void Update()
        {
            if (this.hud.targetMaster != this.targetMaster)
                this.SetTargetMaster(this.hud.targetMaster);

            if (this.currentNotification && this.notificationQueue)
                this.currentNotification.SetNotificationT(this.notificationQueue.CurrentNotificationTime);
        }

        private void ShowCurrentNotification(WeaponNotificationQueue notificationQueue)
        {
            this.DestroyCurrentNotification();

            var notificationInfo = notificationQueue.CurrentNotification;
            if (notificationInfo != null)
            {
                this.currentNotification = Instantiate(Assets.weaponNotificationPrefab).GetComponent<GenericNotification>();

                if (notificationInfo.data is DriverWeaponDef weaponDef)
                {
                    this.currentNotification.titleText.token = weaponDef.nameToken;
                    this.currentNotification.descriptionText.token = weaponDef.descriptionToken;
                    this.currentNotification.iconImage.texture = weaponDef.icon.texture;
                    this.currentNotification.titleTMP.color = weaponDef.color;
                }
                else if (notificationInfo.data is DriverBulletDef bulletDef)
                {
                    this.currentNotification.titleText.token = bulletDef.bulletName;
                    this.currentNotification.descriptionText.token = bulletDef.bulletName;
                    this.currentNotification.iconImage.texture = Assets.bulletSprite.texture;
                    this.currentNotification.iconImage.color = bulletDef.trailColor;
                    this.currentNotification.titleTMP.color = Helpers.GetColorForTier(bulletDef.tier);
                }

                this.currentNotification.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>(), false);
            }
        }

        private void SetTargetMaster(CharacterMaster newMaster)
        {
            this.DestroyCurrentNotification();
            this.CleanUpCurrentMaster();

            this.targetMaster = newMaster;
            if (newMaster)
            {
                this.notificationQueue = WeaponNotificationQueue.GetNotificationQueueForMaster(newMaster);
                this.notificationQueue.onCurrentNotificationChanged += this.ShowCurrentNotification;
                this.ShowCurrentNotification(this.notificationQueue);
            }
        }

        private void DestroyCurrentNotification()
        {
            if (this.currentNotification)
            {
                Destroy(this.currentNotification.gameObject);
                this.currentNotification = null;
            }
        }

        private void CleanUpCurrentMaster()
        {
            if (this.notificationQueue)
                this.notificationQueue.onCurrentNotificationChanged -= this.ShowCurrentNotification;
            this.notificationQueue = null;
            this.targetMaster = null;
        }
    }
}