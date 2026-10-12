using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using RoR2.UI;
using RoR2.Navigation;

namespace RobDriver.Modules.Components
{
    public class WeaponPickup : MonoBehaviour
    {
        public GameObject baseObject;
        public Transform modelParent;
        public LanguageTextMeshController textComponent;

        public BeginRapidlyActivatingAndDeactivating blinker;
        public DestroyWeaponOnTimer destroyOnTimer;
        public ParticleSystem[] systems;
        public Light light;

        // weapon info
        public DriverWeaponDef weaponDef;
        public DriverBulletDef bulletDef;
        public bool cutAmmo;
        public bool isNewAmmoType;

        private bool alive;
        private LocalUser localUser;
        private DriverPassive targetBodyPassive;

        private void Awake()
        {
            weaponDef ??= DriverWeaponCatalog.Pistol;
            bulletDef ??= DriverBulletCatalog.Default;

            localUser = LocalUserManager.GetFirstLocalUser();
            if (localUser != null)
            {
                localUser.onBodyChanged += LocalUser_onBodyChanged;
            }

            LocalUser_onBodyChanged();
        }

        private void LocalUser_onBodyChanged()
        {
            // prio local/spectated driver
            var targetBody = localUser?.cameraRigController ? localUser.cameraRigController.targetBody : null;
            if (targetBody && targetBody.TryGetComponent<DriverPassive>(out var passive))
            {
                this.targetBodyPassive = passive;
                UpdateVisuals();
            }
            else if (!Config.sharedPickupVisuals.Value)
            {
                targetBodyPassive = null;
                UpdateVisuals();
            }
        }

        private void OnTriggerStay(Collider collider)
        {
            if (this.alive)
            {
                var iDrive = collider.GetComponent<DriverController>();
                if (iDrive)
                {
                    this.alive = false;

                    if (!iDrive.passive.isPistolOnly)
                    {
                        Achievements.DriverPistolPassiveAchievement.weaponPickedUp = true;
                        Achievements.DriverGodslingPassiveAchievement.weaponPickedUpHard = true;
                    }

                    if (NetworkServer.active)
                    {
                        iDrive.ServerPickUpWeapon(this.weaponDef, this.bulletDef, this.cutAmmo, this.isNewAmmoType);
                        EffectManager.SimpleEffect(Assets.weaponPickupEffect, this.transform.position, Quaternion.identity, true);
                    }

                    Destroy(this.baseObject);
                }
            }
        }

        private void OnDestroy()
        {
            if (localUser != null)
                localUser.onBodyChanged -= LocalUser_onBodyChanged;
        }

        public void UpdateWeaponPickup(DriverWeaponDef weaponDef, DriverBulletDef bulletDef, bool cutAmmo, bool isNewAmmoType)
        {
            this.weaponDef = weaponDef;
            this.bulletDef = bulletDef;
            this.cutAmmo = cutAmmo;
            this.isNewAmmoType = isNewAmmoType;

            // make sure this is called before handling the collider logic
            this.alive = true;

            float duration = 60f;
            float blinkDelay = 15f;
            if (this.weaponDef.tier > DriverWeaponTier.Uncommon || this.bulletDef.tier > DriverWeaponTier.Uncommon)
            {
                duration = 180f;
                blinkDelay = 30f;
            }
            this.blinker.delayBeforeBeginningBlinking = duration - blinkDelay;
            this.destroyOnTimer.duration = duration;

            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (!this.alive || !this.modelParent)
                return;

            var modelChild = this.modelParent.Find("DriverVisuals");
            if (modelChild)
                GameObject.DestroyImmediate(modelChild.gameObject);

            // ammo pickup visuals
            if (this.targetBodyPassive && (this.targetBodyPassive.isPistolOnly || this.targetBodyPassive.isBullets || (this.targetBodyPassive.isRyan && this.isNewAmmoType)))
            {
                CreateModel(Assets.ammoModel, this.bulletDef.bulletName, this.bulletDef.trailColor, this.bulletDef.tier, true);
            }
            else
            {
                CreateModel(this.weaponDef.pickupPrefab, this.weaponDef.nameToken, this.weaponDef.color, this.weaponDef.tier, false);
            }
        }

        private void CreateModel(GameObject baseAsset, string nameToken, Color textColor, DriverWeaponTier tier, bool isBullet)
        {
            var pickupModelInstance = GameObject.Instantiate(baseAsset, this.modelParent);
            pickupModelInstance.name = "DriverVisuals";

            if (isBullet)
            {
                if (this.targetBodyPassive && this.targetBodyPassive.isPistolOnly)
                {
                    this.textComponent.gameObject.SetActive(false);
                }
                else
                {
                    pickupModelInstance.transform.Find("ammoBox/meshShellsColored").GetComponent<MeshRenderer>().material.color = textColor;
                }
            }

            this.textComponent.token = nameToken;
            this.textComponent.textMeshPro.color = textColor;
            this.textComponent.textMeshPro.isOverlay = true;

            var color = Helpers.GetColorForTier(tier);

            this.light.range = 10f + (5f * (int)tier);
            this.light.color = color;
#pragma warning disable CS0618 // Type or member is obsolete
            systems[0].startColor = color;
            systems[1].startColor = color;
#pragma warning restore CS0618 // Type or member is obsolete
        }
    }
}