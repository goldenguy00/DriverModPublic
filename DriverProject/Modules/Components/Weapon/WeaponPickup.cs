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

        internal static Vector3? FindSafeTeleportDestination(Vector3 characterFootPosition)
        {
            Vector3? result = null;
            SpawnCard spawnCard = ScriptableObject.CreateInstance<SpawnCard>();
            spawnCard.nodeGraphType = MapNodeGroup.GraphType.Ground;
            spawnCard.prefab = LegacyResourcesAPI.Load<GameObject>("SpawnCards/HelperPrefab");
            GameObject gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
            {
                placementMode = DirectorPlacementRule.PlacementMode.NearestNode,
                position = characterFootPosition
            }, RoR2Application.rng));
            if (!gameObject)
            {
                gameObject = DirectorCore.instance.TrySpawnObject(new DirectorSpawnRequest(spawnCard, new DirectorPlacementRule
                {
                    placementMode = DirectorPlacementRule.PlacementMode.RandomNormalized,
                    position = characterFootPosition
                }, RoR2Application.rng));
            }

            if (gameObject)
            {
                result = gameObject.transform.position;
                UnityEngine.Object.Destroy(gameObject);
            }

            UnityEngine.Object.Destroy(spawnCard);
            return result;
        }

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

            if (this.weaponDef.tier > DriverWeaponTier.Uncommon || this.bulletDef.tier > DriverWeaponTier.Uncommon)
            {
                this.blinker.delayBeforeBeginningBlinking = 285f;
                this.destroyOnTimer.duration = 300f;
            }

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
                CreateModel(Assets.ammoPickupModel, this.bulletDef.bulletName, this.bulletDef.trailColor, this.bulletDef.tier);

                if (this.targetBodyPassive.isPistolOnly)
                    this.textComponent.gameObject.SetActive(false);
            }
            else
            {
                CreateModel(this.weaponDef.pickupPrefab, this.weaponDef.nameToken, this.weaponDef.color, this.weaponDef.tier);
            }
        }

        private void CreateModel(GameObject baseAsset, string nameToken, Color textColor, DriverWeaponTier tier)
        {
            var pickupModelInstance = GameObject.Instantiate(baseAsset, this.modelParent);
            pickupModelInstance.name = "DriverVisuals";

            this.textComponent = pickupModelInstance.GetComponentInChildren<LanguageTextMeshController>();
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