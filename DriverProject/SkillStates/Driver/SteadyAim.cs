using UnityEngine;
using RoR2;
using EntityStates;
using RobDriver.Modules;
using static RoR2.CameraTargetParams;
using UnityEngine.Networking;
using RoR2.HudOverlay;
using UnityEngine.AddressableAssets;
using RobDriver.SkillStates.BaseStates;
using RobDriver.Modules.Components.UI;
using RoR2.UI;

namespace RobDriver.SkillStates.Driver
{
    public class SteadyAim : BaseDriverShootState
    {
        public static float _damageCoefficient = 5f;

        private static float baseShotDuration = 0.3f;
        private static float baseChargeDuration = 0.2f;

        private static GameObject gunLight = Modules.Assets.mainAssetBundle.LoadAsset<GameObject>("GunLight");
        internal static GameObject weakPoint = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Common/VFX/WeakPointProcEffect.prefab").WaitForCompletion();

        protected override string showProp => "PistolSight";
        protected override float damageCoefficient => this.wasCharged ? SteadyAim._damageCoefficient : Shoot._damageCoefficient;
        protected override float animationDuration => 1.5f * (SteadyAim.baseShotDuration / this.attackSpeedStat);
        protected override float maxBulletSpread => this.wasCharged ? 1.5f : 0.35f;
        protected override string shootSoundString
        {
            get
            {
                string soundString = this.baseShootSound;

                if (this.isCrit)
                    soundString += "_critical";
                else if (this.wasCharged)
                    soundString += "_charged";

                return soundString;
            }
        }

        protected override string animationString
        {
            get
            {
                string animString = this.baseShootAnimation;

                if (this.wasCharged)
                    animString += "Charged";
                if (this.isCrit)
                    animString += "Critical";

                return animString;
            }
        }

        protected override DamageTypeCombo damageType => this.iDrive.DamageType;
        protected override GameObject tracerPrefab => this.isCrit ? Shoot.critTracerEffectPrefab : Shoot.tracerEffectPrefab;
        protected override GameObject muzzleFlashPrefab => EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab;
        protected override GameObject hitEffectPrefab => EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab;
        protected virtual bool isPiercing => false;

        public bool skipAnim;
        public CameraParamsOverrideHandle camParamsOverrideHandle;

        protected bool wasCharged;
        protected string baseShootSound = "sfx_driver_pistol_shoot";
        protected string baseShootAnimation = "SteadyAimFire";
        protected string enterAnimation = "SteadyAim";
        protected string exitAnimation = "SteadyAimEnd";

        private bool isCharged;
        private float chargeTimer;
        private float chargeDuration;
        private float cachedShotTimer;
        private uint cachedShots;
        private float shotCooldown;
        private bool autoFocus;
        private bool jamFlag; // fired shortly after entering state

        private OverlayController overlayController;
        private GameObject lightEffectInstance;
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;

        public override void OnEnter()
        {
            base.procCoefficient = 1f;
            base.bulletCount = 1;
            base.dropShells = 0;
            base.ammoConsumption = 1f;
            base.useAttackSpeed = false;

            base.hitMask = LayerIndex.CommonMasks.bullet;
            base.bulletFalloff = BulletAttack.FalloffModel.DefaultBullet;
            base.stopperMask = this.isPiercing ? LayerIndex.world.mask : LayerIndex.CommonMasks.bullet;
            base.damageColorIndex = DamageColorIndex.Default;

            base.bulletRange = 2000f;
            base.bulletThiccness = 1f;
            base.bulletForce = 100f;
            base.selfForce = 0f;

            // hacky shit but w/e
            base.baseDuration = float.PositiveInfinity;
            base.earlyExitFraction = 1f;
            base.fireDelayFraction = 0.05f;

            base.visualRecoilAmplitude = 1f;
            base.visualRecoilVertical = 2f;
            base.visualRecoilHorizontal = 0.5f;
            base.spreadBloom = 1.25f;
            base.aimTimer = 2f;

            base.aimPitchString = "SteadyAimPitch";
            base.playbackRateString = "Action.playbackRate";
            base.muzzleString = "PistolMuzzle";

            base.OnEnter();

            if (NetworkServer.active) 
                this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            this.autoFocus = Config.autoFocus.Value;
            this.chargeDuration = SteadyAim.baseChargeDuration / this.attackSpeedStat;
            if (Config.adaptiveFocus.Value && this.chargeDuration <= 0.1f)
                this.autoFocus = true;

            this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, Modules.Assets.pistolAimCrosshairPrefab, CrosshairUtils.OverridePriority.PrioritySkill);
            this.overlayController = HudOverlayManager.AddOverlay(this.gameObject, new OverlayCreationParams
            {
                prefab = Modules.Assets.headshotOverlay,
                childLocatorEntry = "ScopeContainer"
            });

            this.lightEffectInstance = GameObject.Instantiate(gunLight);
        }

        protected override void PlayEntryAnimation()
        {
            var animator = this.GetModelAnimator();
            var layerIndex = animator.GetLayerIndex("AltPistol, Override");
            animator.SetLayerWeight(layerIndex, Config.defaultPistolAnims.Value ? 0f : 1f);

            base.PlayAnimation("AimPitch", "SteadyAimPitch");
            if (!this.skipAnim)
            {
                this.camParamsOverrideHandle = CameraParams.OverrideCameraParams(base.cameraTargetParams, DriverCameraParams.AIM_PISTOL, 0.5f);
                base.PlayAnimation("Gesture, Override", this.enterAnimation, this.playbackRateString, 0.25f);
                Util.PlaySound("sfx_driver_aim_foley", this.gameObject);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.cancelling)
                return;

            this.UpdateStats();
            this.UpdateLightEffects();
            this.UpdateCharge();


            if (!this.inputBank.skill2.down)
            {
                if (base.isAuthority)
                {
                    this.outer.SetNextStateToMain();

                    // add jam buildup
                    if (this.jamFlag && this.shotCooldown > 0f)
                    {
                        if (this.iDrive.AddJamBuildup())
                            this.outer.SetNextState(new JammedGun());
                    }
                }
            }
            else if (this.inputBank.skill1.down && this.shotCooldown <= 0f)
            {
                if (this.iDrive.weaponTimer <= 0f && this.iDrive.maxWeaponTimer > 0f)
                {
                    if (base.isAuthority)
                        this.outer.SetNextState(new Reload());
                }
                else if (!this.autoFocus || this.skillLocator.secondary.stock == 0 || this.isCharged)
                {
                    this.FireBullet();
                }
            }

            if (this.cachedShots > 0 && base.fixedAge >= this.cachedShotTimer)
                this.FireCachedBullet();

        }

        private void UpdateStats()
        {
            this.characterBody.SetAimTimer(0.2f);
            this.chargeDuration = SteadyAim.baseChargeDuration / this.attackSpeedStat;
            this.autoFocus = Config.autoFocus.Value;

            if (Config.adaptiveFocus.Value && this.chargeDuration <= 0.1f)
                this.autoFocus = true;

            this.shotCooldown -= Time.fixedDeltaTime;
            this.characterBody.outOfCombatStopwatch = 0f;
            this.characterBody.isSprinting = false;
            this.attackSpeedStat = this.characterBody.attackSpeed;
            this.damageStat = this.characterBody.damage;
            this.critStat = this.characterBody.crit;
        }

        private void UpdateLightEffects()
        {
            Ray ray = this.GetAimRay();
            if (Physics.Raycast(ray.origin, ray.direction, out var raycastHit, this.bulletRange, LayerIndex.CommonMasks.bullet))
            {
                this.lightEffectInstance.SetActive(true);
                this.lightEffectInstance.transform.position = raycastHit.point + (ray.direction * -0.3f);
            }
            else
            {
                this.lightEffectInstance.SetActive(false);
            }
        }

        private void UpdateCharge()
        {
            if (this.skillLocator.secondary.stock < 1)
            {
                this.isCharged = false;
                this.chargeTimer = 0f;
            }
            else if (this.shotCooldown <= 0f)
            {
                this.chargeTimer += Time.fixedDeltaTime;
            }

            if (!this.isCharged && this.chargeTimer >= this.chargeDuration)
            {
                this.isCharged = true;
                Util.PlaySound("sfx_driver_pistol_ready", this.gameObject);
            }

            this.iDrive.chargeValue = Util.Remap(this.chargeTimer, 0f, this.chargeDuration, 0f, 1f);
        }

        protected override void FireBullet()
        {
            base.PlayAnimation("Gesture, Override", this.animationString, this.playbackRateString, this.animationDuration);

            this.shotCooldown = SteadyAim.baseShotDuration / this.attackSpeedStat;
            this.wasCharged = this.isCharged;
            this.isCharged = false;
            this.chargeTimer = 0f;

            if (this.wasCharged && NetworkServer.active)
            {
                var itemCount = this.characterBody.inventory ? this.characterBody.inventory.GetItemCount(DLC2Content.Items.IncreasePrimaryDamage) : 0;
                if (itemCount > 0)
                    this.characterBody.AddIncreasePrimaryDamageStack();
            }

            this.isCrit = this.RollCrit();
            if (this.isCrit)
            {
                this.cachedShotTimer = base.fixedAge + base.fireDelayFraction;
                this.cachedShots++;
            }

            if (base.fixedAge <= 0.25f)
                this.jamFlag = true;

            base.FireBullet();
        }

        protected void FireCachedBullet()
        {
            this.shotCooldown = SteadyAim.baseShotDuration / this.attackSpeedStat;

            this.DropShells();
            this.iDrive.ConsumeAmmo(this.ammoConsumption, this.useAttackSpeed);
            Util.PlaySound("sfx_driver_pistol_shoot_charged", base.gameObject);
            EffectManager.SimpleMuzzleFlash(this.muzzleFlashPrefab, this.gameObject, this.muzzleString, false);

            base.isCrit = RollCrit();
            base.bulletCount = this.cachedShots;

            // don't call the override here, sneaky hack
            if (base.isAuthority)
                base.FireBulletAuthority();

            base.bulletCount = 1;
            this.cachedShots = 0;
        }

        protected override void FireBulletAuthority()
        {
            base.FireBulletAuthority();

            if (this.iDrive.shurikenComponent)
                this.iDrive.shurikenComponent.OnSkillActivated(base.skillLocator.primary);

            if (this.wasCharged)
                this.skillLocator.secondary.DeductStock(1);
        }

        protected override void AuthorityModifyBulletAttack(ref BulletAttack bulletAttack) 
        {
            base.AuthorityModifyBulletAttack(ref bulletAttack);

            if (this.wasCharged)
            {
                bulletAttack.force = 600f;
                bulletAttack.falloffModel = BulletAttack.FalloffModel.None;
                bulletAttack.damageType.damageSource = DamageSource.Secondary;
            }

            bulletAttack.modifyOutgoingDamageCallback = delegate (BulletAttack _bulletAttack, ref BulletAttack.BulletHit hitInfo, DamageInfo damageInfo)
            {
                if (BulletAttack.IsSniperTargetHit(hitInfo))
                {
                    damageInfo.damage *= this.iDrive.passive.isPistolOnly ? 2f : 1.5f;
                    damageInfo.damageColorIndex = DamageColorIndex.Sniper;

                    if (this.wasCharged)
                    {
                        EffectData effectData = new EffectData
                        {
                            origin = hitInfo.point,
                            rotation = Quaternion.LookRotation(-hitInfo.direction)
                        };

                        effectData.SetHurtBoxReference(hitInfo.hitHurtBox);
                        EffectManager.SpawnEffect(weakPoint, effectData, true);
                        Util.PlaySound("sfx_driver_headshot", base.gameObject);
                        hitInfo.hitHurtBox.healthComponent.gameObject.AddComponent<DriverHeadshotTracker>();
                    }
                }
            };
        }

        public override void ModifyNextState(EntityState nextState)
        {
            base.ModifyNextState(nextState);

            if (nextState is Reload reloadState)
            {
                reloadState.camParamsOverrideHandle = this.camParamsOverrideHandle;
                reloadState.steadyAimType = this.GetType();
                reloadState.animString = "SteadyAimReload";
                reloadState.aiming = true;
            }
            else if (this.camParamsOverrideHandle.isValid)
            {
                this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.lightEffectInstance)
                Destroy(this.lightEffectInstance);

            if (NetworkServer.active)
                this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);

            if (this.overlayController != null)
            {
                HudOverlayManager.RemoveOverlay(this.overlayController);
                this.overlayController = null;
            }

            if (this.outer.destroying && this.camParamsOverrideHandle.isValid)
                this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

            if (!this.cancelling)
                base.PlayAnimation("Gesture, Override", this.exitAnimation, this.playbackRateString, 0.2f);

            base.PlayAnimation("AimPitch", "AimPitch");

            this.crosshairOverrideRequest?.Dispose();
            this.iDrive.chargeValue = 0f;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}