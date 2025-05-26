using System;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RobDriver.Modules.Components.UI;
using RoR2;
using RoR2.Stats;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace RobDriver.Modules.Components
{
    public class DriverController : MonoBehaviour 
    {
        // state
        public bool isReloading;
        public bool needReload;
        public float maxWeaponTimer;
        public float weaponTimer;
        public float chargeValue;

        // rav compat
        public bool isWallClinging;
        public bool clingReady;
        public float featherTimer;

        // effects
        public ParticleSystem machineGunVFX;
        public PrimarySkillShurikenBehavior shurikenComponent;

        // weapons
        public DriverWeaponDef weaponDef;
        public DriverBulletDef currentBulletDef;

        private DriverWeaponDef _defaultWeapon;

        // common components
        public DriverPassive passive;

        private CharacterBody characterBody;
        private ModelSkinController skinController;
        private ChildLocator childLocator;
        private CharacterModel characterModel;
        private Animator animator;
        private SkillLocator skillLocator;
        private DriverArsenal arsenal;
        private Inventory inventory;
        private PlayerStatsComponent statsComponent;

        // effects
        private GameObject muzzleTrail;
        private GameObject hammerEffectInstance;
        private BackWeaponComponent weaponEffectInstance;
        private WeaponNotificationQueue notificationQueue;

        // shells
        private readonly int maxShellCount = 12;
        private bool fakeHolster;
        private float jamTimer;
        private int currentShell;
        private int currentSlug;
        private GameObject[] shellObjects;
        private GameObject[] slugObjects;

        // common properties
        public bool HolsteredWeapon => this.fakeHolster || this.weaponEffectInstance != null;
        public bool IsHoldingWeapon => this.weaponDef != this.defaultWeaponDef;
        public bool HasSpecialBullets => this.currentBulletDef != DriverBulletCatalog.Default;
        public DamageTypeCombo DamageType => this.currentBulletDef.damageType;
        public float AmmoPercent => this.maxWeaponTimer > 0f ? this.weaponTimer / this.maxWeaponTimer : 1f;

        public DriverWeaponDef defaultWeaponDef
        {
            get => _defaultWeapon;
            set
            {
                if (_defaultWeapon != value)
                {
                    this.skillLocator.primary.UnsetWeaponSkill(_defaultWeapon.primarySkillDef, GenericSkill.SkillOverridePriority.Upgrade);
                    this.skillLocator.secondary.UnsetWeaponSkill(_defaultWeapon.secondarySkillDef, GenericSkill.SkillOverridePriority.Upgrade);

                    _defaultWeapon = value;

                    if (_defaultWeapon != DriverWeaponCatalog.Pistol)
                    {
                        this.skillLocator.primary.SetWeaponSkill(_defaultWeapon.primarySkillDef, GenericSkill.SkillOverridePriority.Upgrade);
                        this.skillLocator.secondary.SetWeaponSkill(_defaultWeapon.secondarySkillDef, GenericSkill.SkillOverridePriority.Upgrade);
                    }
                }
            }
        }

        private DriverWeaponTracker weaponTracker
        {
            get
            {
                var masterObj = this.characterBody ? this.characterBody.masterObject : null;

                return masterObj 
                    ? masterObj.GetComponent<DriverWeaponTracker>() ?? masterObj.AddComponent<DriverWeaponTracker>()
                    : null;
            }
        }

        // events
        public Action onConsumeAmmo;
        public Action onWeaponUpdate;
        public Action<DriverWeaponDef> onWeaponChanged;

        private void Awake()
        {
            this.arsenal = this.GetComponent<DriverArsenal>();
            this.passive = this.GetComponent<DriverPassive>();
            this.characterBody = this.GetComponent<CharacterBody>();
            this.skillLocator = this.GetComponent<SkillLocator>();
            
            Transform modelTransform = this.GetComponent<ModelLocator>().modelTransform;
            this.childLocator = modelTransform.GetComponentInChildren<ChildLocator>();
            this.childLocator.GetComponent<DriverAnimationEvents>().iDrive = this;
            this.animator = modelTransform.GetComponentInChildren<Animator>();
            this.characterModel = modelTransform.GetComponentInChildren<CharacterModel>();
            this.skinController = modelTransform.GetComponent<ModelSkinController>();

            this.machineGunVFX = this.childLocator.FindChildComponent<ParticleSystem>("MachineGunVFX");

            this.currentBulletDef = DriverBulletCatalog.Default;
            this._defaultWeapon = DriverWeaponCatalog.Pistol;
            this.weaponDef = DriverWeaponCatalog.Pistol;
        }

        private void Start()
        {
            this.skillLocator.special.AddOneStock();

            this.InitShells();
            this.Invoke(nameof(SetInventoryHook), 0.5f);
            this.Invoke(nameof(CheckForUpgrade), 3f);
        }

        public void OnEnable() => InstanceTracker.Add(this);
        public void OnDisable() => InstanceTracker.Remove(this);

        private void SetInventoryHook()
        {
            this.inventory = this.characterBody.inventory;
            this.inventory.onItemAddedClient += this.Inventory_onItemAddedClient;
            this.inventory.onInventoryChanged += this.Inventory_onInventoryChanged;

            // modelskinswapper compat
            // i hate this as much as you do.
            this.childLocator.FindChildGameObject("PistolModel").SetActive(true);
            this.childLocator.FindChildGameObject("KnifeModel").SetActive(false);
            this.childLocator.FindChildGameObject("ButtonModel").SetActive(false);
            this.childLocator.FindChildGameObject("SyringeModel").SetActive(false);
            this.childLocator.FindChildGameObject("SkateboardModel").SetActive(false);
            this.childLocator.FindChildGameObject("SkateboardBackModel").SetActive(this.skillLocator.utility.skillDef == Skills.skateboardSkillDef);
            this.childLocator.FindChildGameObject("AltWeaponModel").SetActive(false);

            // enable all the renderers...
            for (int i = 0; i < this.characterModel.baseRendererInfos.Length; i++)
            {
                this.characterModel.baseRendererInfos[i].renderer.enabled = true;
            }

            // enable sword as knife
            if (this.skillLocator.special.skillDef == Skills.scepterKnifeSkillDef || (Config.enableRevengence.Value && this.skillLocator.special.skillDef == Skills.knifeSkillDef))
            {
                this.childLocator.FindChildGameObject("AltWeaponModel").SetActive(true);

                if (this.skillLocator.special.skillDef == Skills.scepterKnifeSkillDef)
                {
                    var knifeRenderer = this.childLocator.FindChildComponent<SkinnedMeshRenderer>("KnifeModel");
                    knifeRenderer.sharedMaterial = Weapons.NemKatana.instance.weaponDef.material;
                    knifeRenderer.sharedMesh = Weapons.NemKatana.instance.weaponDef.mesh;

                    for (int i = 0; i < this.characterModel.baseRendererInfos.Length; i++)
                    {
                        ref var info = ref this.characterModel.baseRendererInfos[i];

                        if (info.renderer == knifeRenderer)
                            info.defaultMaterial = Weapons.NemKatana.instance.weaponDef.material;
                    }
                }
            }

            this.defaultWeaponDef = this.arsenal.LoadoutWeapon;
            this.PickUpWeapon(this.defaultWeaponDef);
            this.ServerGetStoredWeapon();

            this.onWeaponChanged += this.OnWeaponChanged;
        }

        private void OnWeaponChanged(DriverWeaponDef newWeapon)
        {
            this.TryPickupNotification(newWeapon);
            this.TryCallout(newWeapon);
            this.TryUnlockServer(newWeapon);
            this.TrySetDefault(newWeapon);
        }

        private void CheckForUpgrade()
        {
            if (!Config.enablePistolUpgrade.Value || !DriverWeaponCatalog.IsWeaponPistol(this.defaultWeaponDef)) 
                return;

            // upgrade your pistol for run-ending bosses; this is more interesting than just injecting weapon drops imo
            string currentScene = SceneManager.GetActiveScene().name;
            GameObject effectPrefab = null;
            DriverWeaponDef upgradeDef = null;
            switch (currentScene)
            {
                case "moon" or "moon2" or "limbo" or "mysteryspace":
                    upgradeDef = DriverWeaponCatalog.LunarPistol;
                    effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarGolem/LunarGolemTwinShotExplosion.prefab").WaitForCompletion();
                    break;

                case "voidraid" or "voidstage" or "arena":
                    upgradeDef = DriverWeaponCatalog.VoidPistol;
                    effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterExplosion.prefab").WaitForCompletion();
                    break;

                case "goldshores":
                    upgradeDef = DriverWeaponCatalog.PyriteGun;
                    effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/Elites/EliteAurelionite/AffixAurelioniteStrikeEffect.prefab").WaitForCompletion();
                    break;

                case "meridian":
                    upgradeDef = Weapons.FalsePistol.instance.weaponDef;
                    effectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/FalseSonBoss/LunarRainExplosionVFX.prefab").WaitForCompletion();
                    break;
            }

            if (upgradeDef)
            {
                if (this.IsHoldingWeapon)
                {
                    this.TryPickupNotification(upgradeDef);
                    this.TryUnlockServer(upgradeDef);
                }
                else
                {
                    this.PickUpWeapon(upgradeDef);
                }

                this.defaultWeaponDef = upgradeDef;

                EffectManager.SpawnEffect(Assets.upgradeEffectPrefab, new EffectData
                {
                    origin = this.childLocator.FindChild("PistolMuzzle").position,
                    rotation = Quaternion.identity
                }, false);

                if (effectPrefab)
                {
                    EffectManager.SpawnEffect(effectPrefab, new EffectData
                    {
                        origin = this.childLocator.FindChild("Pistol").position,
                        rotation = Quaternion.identity,
                        scale = 1f
                    }, false);
                }
            }
        }

        private void Inventory_onInventoryChanged() => this.shurikenComponent = this.GetComponent<PrimarySkillShurikenBehavior>();

        private void Inventory_onItemAddedClient(ItemIndex itemIndex)
        {
            if (itemIndex == DLC1Content.Items.EquipmentMagazineVoid.itemIndex)
            {
                this.skillLocator.special.AddOneStock();
            }

            if (DriverPlugin.ScepterInstalled && this.skillLocator.special.baseSkill == Skills.knifeSkillDef && DriverPlugin.IsItemScepter(itemIndex))
            {
                this.childLocator.FindChildGameObject("AltWeaponModel").SetActive(true);

                var knifeRenderer = this.childLocator.FindChildComponent<SkinnedMeshRenderer>("KnifeModel");
                knifeRenderer.sharedMaterial = Weapons.NemKatana.instance.weaponDef.material;
                knifeRenderer.sharedMesh = Weapons.NemKatana.instance.weaponDef.mesh;

                for (int i = 0; i < this.characterModel.baseRendererInfos.Length; i++)
                {
                    ref var info = ref this.characterModel.baseRendererInfos[i];

                    if (info.renderer == knifeRenderer)
                        info.defaultMaterial = Weapons.NemKatana.instance.weaponDef.material;
                }
            }

            // quit resetting my shit
            if (this.passive.isBullets || this.passive.isPistolOnly) 
                return;

            if (DriverPlugin.LitInstalled && DriverPlugin.IsItemGoldenGun(itemIndex)) // funny compat :-)
            {
                this.ServerPickUpWeapon(DriverWeaponCatalog.GoldenGun);
            }

            if (DriverPlugin.ClassicItemsInstalled && DriverPlugin.IsItemGoldenGun2(itemIndex)) // not funny anymore
            {
                this.ServerPickUpWeapon(DriverWeaponCatalog.GoldenGun);
            }

            if (itemIndex == RoR2Content.Items.Behemoth.itemIndex)
            {
                this.ServerPickUpWeapon(DriverWeaponCatalog.Behemoth);
            }

            if (itemIndex == RoR2Content.Items.LunarPrimaryReplacement.itemIndex)
            {
                this.ServerPickUpWeapon(DriverWeaponCatalog.Needler);
            }
        }

        public void ConsumeAmmo(float multiplier = 1f, bool scaleWithAttackSpeed = true)
        {
            if (this.maxWeaponTimer <= 0f)
                return;

            if (!this.characterBody || !this.inventory)
                return;

            if (this.characterBody.HasBuff(RoR2Content.Buffs.NoCooldowns))
                return;

            if (scaleWithAttackSpeed)
            {
                int alienHeadCount = this.inventory.GetItemCount(RoR2Content.Items.AlienHead);
                alienHeadCount += this.inventory.GetItemCount(RoR2Content.Items.LunarBadLuck);
                if (alienHeadCount > 0)
                {
                    for (int i = 0; i < alienHeadCount; i++)
                    {
                        if (DriverPlugin.GreenAlienHeadInstalled)
                        {
                            multiplier *= 0.85f;
                        }
                        else
                        {
                            multiplier *= 0.75f;
                        }
                    }
                }

                this.weaponTimer -= multiplier / this.characterBody.attackSpeed;
            }
            else
            {
                this.weaponTimer -= multiplier;
            }

            // notify Hud
            this.onConsumeAmmo?.Invoke();
        }

        public bool AddJamBuildup(bool jammed = false)
        {
            this.jamTimer += 3f;

            if (this.jamTimer >= 10f)
            {
                this.jamTimer = 0f;
                jammed = true;
            }

            return jammed;
        }

        private void FixedUpdate()
        {
            this.jamTimer = Mathf.Clamp(this.jamTimer - (2f * Time.fixedDeltaTime), 0f, Mathf.Infinity);
            
            if (this.weaponTimer <= 0f && this.maxWeaponTimer > 0f)
            {
                if (this.HasSpecialBullets)
                {
                    this.currentBulletDef = DriverBulletCatalog.Default;

                    if (this.muzzleTrail)
                        GameObject.Destroy(muzzleTrail);
                }

                if (this.IsHoldingWeapon)
                {
                    this.PickUpWeapon(this.defaultWeaponDef);
                }
                else if (!this.needReload)
                {
                    this.needReload = true;
                    this.skillLocator.primary.SetWeaponSkill(Skills.pistolReloadSkillDef, GenericSkill.SkillOverridePriority.Contextual);
                }
            }
        }

        /// <summary>
        /// Decides what to do for dropped weapons with each passive
        /// </summary>
        public void PickUpWeaponDrop(DriverWeaponDef newWeapon, DriverBulletDef newBullet, bool cutAmmo, bool isNewAmmoType)
        {
            if (this.passive.isDefault)
            {
                this.PickUpWeapon(newWeapon, cutAmmo);
            }
            else if (this.passive.isPistolOnly)
            {
                this.FinishReload();
            }
            else if (this.passive.isBullets || isNewAmmoType)
            {
                TryPickupNotification(newBullet);
                this.currentBulletDef = newBullet;
                this.SetBulletAmmo(cutAmmo);
            }
            else
            {
                float? ammo = null;

                if (this.HasSpecialBullets && this.weaponDef != newWeapon)
                {
                    // get rid of remaining shots if low
                    if (this.AmmoPercent > 0.2f)
                    {
                        ammo = newWeapon.shotCount;
                        if (Config.backupMagExtendDuration.Value && !this.passive.isPistolOnly)
                            ammo += this.inventory?.GetItemCount(RoR2Content.Items.SecondarySkillMagazine) ?? 0;

                        ammo *= this.AmmoPercent;
                    }
                    else
                    {
                        currentBulletDef = DriverBulletCatalog.Default;
                    }
                }

                PickUpWeapon(newWeapon, cutAmmo, ammo);
            }
        }

        /// <summary>
        /// Changes weapon, does not change ammo type
        /// </summary>
        public void PickUpWeapon(DriverWeaponDef newWeapon, bool cutAmmo = false, float? ammo = null)
        {
            if (this.weaponDef != newWeapon)
                this.onWeaponChanged?.Invoke(newWeapon);

            this.skillLocator.UnsetWeaponSkills(this.weaponDef);
            if (newWeapon != this.defaultWeaponDef && newWeapon != DriverWeaponCatalog.Pistol)
                this.skillLocator.SetWeaponSkills(newWeapon);

            this.characterBody._defaultCrosshairPrefab = newWeapon.crosshairPrefab;
            this.weaponDef = newWeapon;

            this.SetSkinnedWeaponModel(newWeapon);
            this.SetBulletAmmo(cutAmmo, ammo);
        }

        public void FinishReload() => this.SetBulletAmmo();

        /// <summary>
        /// Resets ammo for current weapon
        /// </summary>
        public void SetBulletAmmo(bool cutAmmo = false, float? ammo = null)
        {
            this.skillLocator.primary.UnsetWeaponSkill(Skills.pistolReloadSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            if (this.HasSpecialBullets)
            {
                var muzzleTransform = this.weaponDef.animationSet == DriverWeaponDef.AnimationSet.Default
                    ? this.childLocator.FindChild("PistolMuzzle")
                    : this.childLocator.FindChild("ShotgunMuzzle");

                if (!this.muzzleTrail)
                    this.muzzleTrail = GameObject.Instantiate(Assets.defaultMuzzleTrail, muzzleTransform);

                var color = this.currentBulletDef.trailColor.RGBMultiplied(0.5f).AlphaMultiplied(0.5f);
                var renderer = this.muzzleTrail.GetComponent<TrailRenderer>();
                renderer.startColor = color;
                renderer.endColor = color;
            }
            else if (this.muzzleTrail)
            {
                GameObject.Destroy(this.muzzleTrail);
            }

            // infinite ammo
            if (this.weaponDef.shotCount == 0 || (DriverWeaponCatalog.IsWeaponPistol(this.weaponDef) && !this.HasSpecialBullets))
            {
                this.weaponTimer = 0f;
                this.maxWeaponTimer = 0f;
            }
            else
            {
                this.maxWeaponTimer = this.weaponDef.shotCount;

                if (Config.backupMagExtendDuration.Value)
                {
                    this.maxWeaponTimer += this.inventory?.GetItemCount(RoR2Content.Items.SecondarySkillMagazine) ?? 0;
                }

                if (ammo.HasValue)
                {
                    this.weaponTimer = ammo.Value;
                }
                else
                {
                    this.weaponTimer = this.maxWeaponTimer;

                    if (cutAmmo)
                        this.weaponTimer *= 0.5f;
                }
            }

            this.needReload = false;
            this.isReloading = false;

            // notify hud
            this.onWeaponUpdate?.Invoke();
            this.onConsumeAmmo?.Invoke();
        }

        public void SetSkinnedWeaponModel(DriverWeaponDef newWeapon)
        {
            var modelSwapInfo = DriverWeaponSkinCatalog.GetModelSwapInfoForWeapon(this.skinController, newWeapon);

            for (int i = 0; i < modelSwapInfo.Length; i++)
            {
                ref var info = ref modelSwapInfo[i];
                SetMeshRenderer(info.childName, info.material, info.mesh);
            }

            // animator layer
            this.animator.SetLayerWeight((int)DriverWeaponDef.AnimationSet.TwoHanded, 0f);
            this.animator.SetLayerWeight((int)DriverWeaponDef.AnimationSet.BigMelee, 0f);

            if (newWeapon.animationSet != DriverWeaponDef.AnimationSet.Default)
                this.animator.SetLayerWeight((int)newWeapon.animationSet, 1f);

            UpdateHammerVfx(newWeapon);
        }

        private void SetMeshRenderer(string childName, Material material, Mesh mesh)
        {
            var childTransform = this.childLocator.FindChild(childName);

            for (int i = 0; i < this.characterModel.baseRendererInfos.Length; i++)
            {
                ref var info = ref this.characterModel.baseRendererInfos[i];

                if (info.renderer?.transform != childTransform)
                    continue;

                info.defaultMaterial = material;
                info.renderer.sharedMaterial = material;

                if (info.renderer is SkinnedMeshRenderer skinRenderer)
                    skinRenderer.sharedMesh = mesh;
                else if (info.renderer.TryGetComponent<MeshFilter>(out var filter))
                    filter.sharedMesh = mesh;
                else
                    Log.Error("no skinned mesh renderer or mesh filter found for " + childName);
            }
        }

        public void SetHolsteredWeaponInstance(DriverWeaponDef modelDef)
        {
            if (modelDef == this.weaponEffectInstance?.weaponDef)
                return;

            this.DestroyHolsteredWeaponInstance();
            this.childLocator.FindChildGameObject("PistolModel").SetActive(modelDef.disableHolster);

            if (modelDef.disableHolster)
            {
                this.fakeHolster = true;
            }
            else
            {
                var modelSwapInfo = DriverWeaponSkinCatalog.GetModelSwapInfoForWeapon(this.skinController, modelDef);
                var parent = modelDef.animationSet == DriverWeaponDef.AnimationSet.Default
                        ? this.childLocator.FindChild("ThighHolster")
                        : this.childLocator.FindChild("BackHolster");

                this.weaponEffectInstance = GameObject.Instantiate(Assets.backWeaponEffect, parent).GetComponent<BackWeaponComponent>();
                this.weaponEffectInstance.weaponDef = modelDef;
                this.weaponEffectInstance.mesh = modelSwapInfo[0].mesh;
                this.weaponEffectInstance.material = modelSwapInfo[0].material;
            }
        }

        public void DestroyHolsteredWeaponInstance()
        {
            this.fakeHolster = false;

            if (this.weaponEffectInstance)
            {
                this.weaponEffectInstance.gameObject.SetActive(false);
                GameObject.Destroy(this.weaponEffectInstance);
                this.weaponEffectInstance = null;
            }
        }

        private void TryPickupNotification(DriverWeaponDef newWeapon)
        {
            if (!Config.enablePickupNotifications.Value || newWeapon == this.defaultWeaponDef)
                return;

            // attempt to add the component if it's not there
            if (!this.notificationQueue)
            {
                var master = this.characterBody ? this.characterBody.master : null;
                if (master)
                    this.notificationQueue = master.GetComponent<WeaponNotificationQueue>();
            }

            if (this.notificationQueue)
            {
                this.notificationQueue.PushWeaponNotification(this.characterBody.master, newWeapon);
            }
        }

        private void TryPickupNotification(DriverBulletDef newBullet)
        {
            if (!Config.enablePickupNotifications.Value || newBullet == DriverBulletCatalog.Default || newBullet == this.currentBulletDef)
                return;

            // attempt to add the component if it's not there
            if (!this.notificationQueue)
            {
                var master = this.characterBody ? this.characterBody.master : null;
                if (master)
                    this.notificationQueue = master.GetComponent<WeaponNotificationQueue>();
            }

            if (this.notificationQueue)
            {
                this.notificationQueue.PushWeaponNotification(this.characterBody.master, newBullet);
            }
        }

        private void TryCallout(DriverWeaponDef newWeapon)
        {
            if (!Config.weaponCallouts.Value || newWeapon == this.defaultWeaponDef)
                return;

            if (!string.IsNullOrEmpty(newWeapon.calloutSoundString))
            {
                Util.PlaySound(newWeapon.calloutSoundString, this.gameObject);
            }
        }

        private void TryUnlockServer(DriverWeaponDef newWeapon)
        {
            if (!NetworkServer.active)
                return;

            this.statsComponent ??= PlayerStatsComponent.FindBodyStatsComponent(this.gameObject);
            if (this.statsComponent && newWeapon.unlockableDef)
            {
                var netUser = this.statsComponent.playerCharacterMasterController ? this.statsComponent.playerCharacterMasterController.networkUser : null;
                if (netUser && !netUser.unlockables.Contains(newWeapon.unlockableDef))
                {
                    this.statsComponent.currentStats.AddUnlockable(newWeapon.unlockableDef);
                    this.statsComponent.ForceNextTransmit();
                }
            }
        }

        private void TrySetDefault(DriverWeaponDef newWeapon)
        {
            // force hammer over anything else fuck it
            if (newWeapon == DriverWeaponCatalog.LunarHammer || this.defaultWeaponDef == DriverWeaponCatalog.LunarHammer)
            {
                this.defaultWeaponDef = newWeapon;
                return;
            }

            if (newWeapon == DriverWeaponCatalog.Needler || this.defaultWeaponDef == DriverWeaponCatalog.Needler)
            {
                this.defaultWeaponDef = newWeapon;
                return;
            }

            // pistol upgrades
            if (DriverWeaponCatalog.IsWeaponPistol(newWeapon) && this.defaultWeaponDef == DriverWeaponCatalog.Pistol)
                this.defaultWeaponDef = newWeapon;
        }

        private void UpdateHammerVfx(DriverWeaponDef newWeapon)
        {
            if (newWeapon != DriverWeaponCatalog.LunarHammer)
            {
                if (this.hammerEffectInstance)
                    Destroy(this.hammerEffectInstance);

                return;
            }

            if (!this.hammerEffectInstance)
            {
                this.hammerEffectInstance = GameObject.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/BrotherBody")
                    .GetComponentInChildren<ChildLocator>().FindChild("Phase3HammerFX").gameObject);

                var transform = this.hammerEffectInstance.transform;
                transform.parent = this.childLocator.FindChild("GunR");
                transform.localScale = Vector3.one * 0.0002f;
                transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 90f));
                transform.localPosition = new Vector3(0f, 1.6f, 0.05f);

                transform.Find("Amb_Fire_Ps, Left").localScale = Vector3.one * 0.6f;
                transform.Find("Amb_Fire_Ps, Right").localScale = Vector3.one * 0.6f;
                transform.Find("Core, Light").localScale = Vector3.one * 0.1f;
                transform.Find("Blocks, Spinny").localScale = Vector3.one * 0.4f;
                transform.Find("Sparks").localScale = Vector3.one * 0.4f;

                /*
                this.hammerEffectInstance2 = GameObject.Instantiate(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/LunarWispBody").GetComponentInChildren<CharacterModel>().transform.Find("Amb_Fire_Ps").gameObject);
                this.hammerEffectInstance2.transform.parent = this.childLocator.FindChild("HandL");
                this.hammerEffectInstance2.transform.localPosition = Vector3.zero;
                this.hammerEffectInstance2.transform.localRotation = Quaternion.identity;
                this.hammerEffectInstance2.transform.localScale *= 0.25f;
                */
                //this.hammerEffectInstance2.SetActive(false);
            }

            this.hammerEffectInstance.SetActive(true);
        }

        private void InitShells()
        {
            this.currentShell = 0;
            this.shellObjects = new GameObject[this.maxShellCount + 1];
            for (int i = 0; i < this.maxShellCount; i++)
            {
                this.shellObjects[i] = InitShell(Assets.shotgunShell, 1.1f);
            }

            this.currentSlug = 0;
            this.slugObjects = new GameObject[this.maxShellCount + 1];
            for (int i = 0; i < this.maxShellCount; i++)
            {
                this.slugObjects[i] = InitShell(Assets.shotgunSlug, 1.2f);
            }

            GameObject InitShell(GameObject desiredShell, float scale)
            {
                var shellObject = GameObject.Instantiate(desiredShell, this.childLocator.FindChild("Pistol"), false);
                shellObject.transform.localScale = Vector3.one * scale;
                shellObject.SetActive(false);
                shellObject.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;

                shellObject.layer = LayerIndex.ragdoll.intVal;
                shellObject.transform.GetChild(0).gameObject.layer = LayerIndex.ragdoll.intVal;

                return shellObject;
            }
        }

        public void DropShell(Vector3 force)
        {
            if (this.shellObjects?[this.currentShell] == null) 
                return;

            DropShell(this.shellObjects[this.currentShell], force);

            this.currentShell++;
            if (this.currentShell >= this.maxShellCount) 
                this.currentShell = 0;
        }

        public void DropSlug(Vector3 force)
        {
            if (this.slugObjects?[this.currentSlug] == null)
                return;

            DropShell(this.slugObjects[this.currentSlug], force);

            this.currentSlug++;
            if (this.currentSlug >= this.maxShellCount) 
                this.currentSlug = 0;
        }

        public void DropShell(GameObject shell, Vector3 force)
        {
            shell.SetActive(false);

            shell.transform.position = this.childLocator.FindChild("Pistol").position;
            shell.transform.SetParent(null);

            shell.SetActive(true);

            Rigidbody rb = shell.GetComponent<Rigidbody>();
            if (rb) rb.velocity = force;
        }

        #region Server
        public void ServerResetTimer()
        {
            bool cutAmmo = this.maxWeaponTimer > 0 && this.weaponTimer / this.maxWeaponTimer < 0.5f;
            this.ServerPickUpWeapon(this.weaponDef, this.currentBulletDef, cutAmmo, false);
        }

        public void ServerPickUpWeapon(DriverWeaponDef newWeapon) => this.ServerPickUpWeapon(newWeapon, this.currentBulletDef, false, false);
        public void ServerPickUpWeapon(DriverWeaponDef newWeapon, DriverBulletDef newBullet, bool cutAmmo, bool isNewAmmoType)
        {
            if (NetworkServer.active && this.TryGetComponent<NetworkIdentity>(out var identity))
            {
                new SyncWeapon(identity.netId, newWeapon.index, newBullet.index, cutAmmo, isNewAmmoType).Send(NetworkDestination.Clients);
            }
        }

        private void ServerGetStoredWeapon()
        {
            if (NetworkServer.active && this.TryGetComponent<NetworkIdentity>(out var identity))
            {
                DriverWeaponTracker weaponTracker = this.weaponTracker;
                if (weaponTracker && weaponTracker.hasWeapon)
                {
                    new SyncStoredWeapon(identity.netId, weaponTracker.RetrieveWeapon()).Send(NetworkDestination.Clients);
                }
            }
        }
        #endregion

        private void OnDestroy()
        {
            if (NetworkServer.active)
                this.weaponTracker?.StoreWeapon(this.defaultWeaponDef, this.weaponDef, this.currentBulletDef, this.weaponTimer);

            this.onWeaponChanged -= this.OnWeaponChanged;

            if (this.inventory)
            {
                this.inventory.onItemAddedClient -= this.Inventory_onItemAddedClient;
                this.inventory.onInventoryChanged -= this.Inventory_onInventoryChanged;
            }

            if (this.weaponEffectInstance) 
                Destroy(this.weaponEffectInstance);

            if (this.muzzleTrail)
                Destroy(this.muzzleTrail);

            if (this.shellObjects != null && this.shellObjects.Length > 0)
            {
                for (int i = 0; i < this.shellObjects.Length; i++)
                {
                    if (this.shellObjects[i]) Destroy(this.shellObjects[i]);
                }
            }

            if (this.slugObjects != null && this.slugObjects.Length > 0)
            {
                for (int i = 0; i < this.slugObjects.Length; i++)
                {
                    if (this.slugObjects[i]) Destroy(this.slugObjects[i]);
                }
            }
        }
    }
}
