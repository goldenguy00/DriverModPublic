using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using RoR2.UI;

namespace RobDriver.Modules.Components
{
    public class WeaponPickup : MonoBehaviour
    {
        [Tooltip("The base object to destroy when this pickup is consumed.")]
        public GameObject baseObject;

        [Tooltip("The team filter object which determines who can pick up this pack.")]
        public TeamFilter teamFilter;

        [Tooltip("Parent of the model object.")]
        public Transform modelParent;

        [Tooltip("Blinking thing")]
        public BeginRapidlyActivatingAndDeactivating blinker;

        [Tooltip("DestroyTimer thing")]
        public DestroyOnTimer destroyOnTimer;

        // visuals
        private GameObject pickupModelInstance;

        // weapon info
        private DriverWeaponDef weaponDef = DriverWeaponCatalog.Pistol;
        private DriverBulletDef bulletDef = DriverBulletCatalog.Default;
        private bool cutAmmo;
        private bool isNewAmmoType;

        private bool alive;

        private void OnTriggerStay(Collider collider)
        {
            if (NetworkServer.active && this.alive)
            {
                var iDrive = collider.GetComponent<DriverController>();
                if (iDrive)
                {
                    this.alive = false;

                    Achievements.DriverPistolPassiveAchievement.weaponPickedUp = true;
                    Achievements.DriverGodslingPassiveAchievement.weaponPickedUpHard = true;

                    iDrive.ServerPickUpWeapon(iDrive, this.weaponDef, this.bulletDef, this.cutAmmo, this.isNewAmmoType);
                    EffectManager.SimpleEffect(Assets.weaponPickupEffect, this.transform.position, Quaternion.identity, true);
                    Destroy(this.baseObject);
                }
            }
        }

        private void OnDestroy()
        {
            if (this.alive)
            {
                Achievements.SupplyDropAchievement.weaponHasDespawned = true;
            }
        }

        public void UpdateWeaponPickup(DriverWeaponDef weaponDef, DriverBulletDef bulletDef, bool cutAmmo, bool isNewAmmoType)
        {
            this.weaponDef = weaponDef;
            this.bulletDef = bulletDef;
            this.cutAmmo = cutAmmo;
            this.isNewAmmoType = isNewAmmoType;

            // make sure this is called before handling the collider logic
            alive = true;

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            DriverController iDrive = null;
            foreach (var localUser in LocalUserManager.readOnlyLocalUsersList)
            {
                if (localUser?.cachedBody && localUser.cachedBody.hasEffectiveAuthority)
                    iDrive ??= localUser.cachedBody.GetComponent<DriverController>();
            }

            if (!Config.sharedPickupVisuals.Value && !iDrive)
            {
                modelParent.gameObject.SetActive(false);
                Destroy(blinker);
            }

            // ammo pickup visuals
            if (iDrive && (iDrive.passive.isPistolOnly || iDrive.passive.isBullets || (iDrive.passive.isRyan && this.isNewAmmoType)))
            {
                CreateModel(Assets.ammoPickupModel, this.bulletDef.nameToken, this.bulletDef.tier);

                var textComponent = pickupModelInstance.GetComponentInChildren<LanguageTextMeshController>();
                textComponent.textMeshPro.outlineColor = this.bulletDef.trailColor;
                textComponent.textMeshPro.outlineWidth *= 0.75f;
            }
            else
            {
                // normal visuals
                var baseAsset = weaponDef.tier switch
                {
                    DriverWeaponTier.Legendary => Assets.legendaryPickupModel,
                    DriverWeaponTier.Unique => Assets.uniquePickupModel,
                    DriverWeaponTier.Void => Assets.legendaryPickupModel,
                    DriverWeaponTier.Lunar => Assets.lunarPickupModel,
                    _ => Assets.commonPickupModel,
                };

                CreateModel(baseAsset, this.weaponDef.nameToken, this.weaponDef.tier);
            }
        }

        private void CreateModel(GameObject baseAsset, string nameToken, DriverWeaponTier tier)
        {
            pickupModelInstance = GameObject.Instantiate(baseAsset, modelParent);
            pickupModelInstance.transform.localPosition = Vector3.zero;
            pickupModelInstance.transform.localRotation = Quaternion.identity;

            // always use weapon tier, fuck it
            if (weaponDef.tier > DriverWeaponTier.Uncommon)
            {
                blinker.delayBeforeBeginningBlinking = 285f;
                destroyOnTimer.duration = 300f;
            }

            Color color = Helpers.badColor;
            if (!this.cutAmmo)
            {
                color = tier switch
                {
                    DriverWeaponTier.Common => Helpers.whiteItemColor,
                    DriverWeaponTier.Uncommon => Helpers.greenItemColor,
                    DriverWeaponTier.Legendary => Helpers.redItemColor,
                    DriverWeaponTier.Unique => Helpers.yellowItemColor,
                    DriverWeaponTier.Lunar => Helpers.lunarItemColor,
                    DriverWeaponTier.Void => Helpers.voidItemColor,
                    _ => Helpers.badColor,
                };
            }

            var textComponent = pickupModelInstance.GetComponentInChildren<LanguageTextMeshController>();
            textComponent.token = nameToken;
            textComponent.textMeshPro.color = color;
        }
    }
}