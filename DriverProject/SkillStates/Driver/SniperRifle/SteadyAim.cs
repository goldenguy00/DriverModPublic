using UnityEngine;
using RoR2;
using EntityStates;
using static RoR2.CameraTargetParams;
using RoR2.HudOverlay;
using UnityEngine.AddressableAssets;
using R2API;

namespace RobDriver.SkillStates.Driver.SniperRifle
{
    public class SteadyAim : BaseDriverSkillState
    {
        public float baseShotDuration = 1.4f;
        public float baseChargeDuration = 4f;

        public static float damageCoefficient = 50f;
        public static float recoil = 3f;

        private CameraParamsOverrideHandle camParamsOverrideHandle;
        private float shotCooldown;
        private float chargeTimer;
        private float chargeDuration;
        private bool isCharged;
        private bool isCrit;
        private PrimarySkillShurikenBehavior shurikenComponent;
        private bool autoFocus;
        private bool cancelling;

        private OverlayController overlayController;

        public override void OnEnter()
        {
            base.OnEnter();
            this.chargeDuration = this.baseChargeDuration / this.attackSpeedStat;
            this.camParamsOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, DriverCameraParams.AIM_SNIPER, 0.5f);

            base.PlayAnimation("Gesture, Override", "AimTwohand");
            base.PlayAnimation("AimPitch", "ShotgunAimPitch");

            this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            this.shurikenComponent = this.GetComponent<PrimarySkillShurikenBehavior>();

            if (this.characterBody.master && this.characterBody.master.inventory)
            {
                this.characterBody.master.inventory.onInventoryChanged += Inventory_onInventoryChanged;
            }

            this.characterBody._defaultCrosshairPrefab = Modules.Assets.pistolAimCrosshairPrefab;
            this.autoFocus = Modules.Config.autoFocus.Value;

            this.overlayController = HudOverlayManager.AddOverlay(this.gameObject, new OverlayCreationParams
            {
                prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerScopeLightOverlay.prefab").WaitForCompletion(),
                childLocatorEntry = "ScopeContainer"
            });
        }

        private void Inventory_onInventoryChanged()
        {
            this.shurikenComponent = this.GetComponent<PrimarySkillShurikenBehavior>();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.shotCooldown -= Time.fixedDeltaTime;
            this.characterBody.outOfCombatStopwatch = 0f;
            this.characterBody.isSprinting = false;
            base.characterBody.SetAimTimer(0.2f);
            this.attackSpeedStat = this.characterBody.attackSpeed;

            if (this.iDrive && this.iDrive.weaponDef != this.cachedWeaponDef)
            {
                this.cancelling = true;
                this.outer.SetNextStateToMain();
                return;
            }

            if (this.skillLocator.secondary.stock < 1)
            {
                this.isCharged = false;
                this.chargeTimer = 0f;
            }
            else
            {
                if (this.shotCooldown <= 0f) this.chargeTimer += Time.fixedDeltaTime;
            }

            if (!this.isCharged)
            {
                if (this.chargeTimer >= this.chargeDuration)
                {
                    this.isCharged = true;
                    Util.PlaySound("sfx_driver_sniper_click", this.gameObject);
                }
            }

            if (this.shotCooldown <= 0f && base.isAuthority)
            {
                if (this.autoFocus)
                {
                    if (this.inputBank.skill1.down)
                    {
                        if (this.skillLocator.secondary.stock > 0)
                        {
                            if (this.isCharged)
                            {
                                this.isCrit = this.RollCrit();
                                this.Fire();
                            }
                        }
                        else
                        {
                            this.isCrit = this.RollCrit();
                            this.Fire();
                        }
                    }
                }
                else
                {
                    if (this.inputBank.skill1.down)
                    {
                        this.isCrit = this.RollCrit();
                        this.Fire();
                    }
                }
            }

            if (!this.inputBank.skill2.down && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }

            if (this.iDrive)
            {
                if (this.skillLocator.secondary.stock < 1)
                {
                    this.iDrive.chargeValue = 0f;
                    return;
                }

                float value = Util.Remap(this.chargeTimer, 0f, this.chargeDuration, 0f, 1f);
                this.iDrive.chargeValue = value;
            }
        }

        public void Fire()
        {
            if (this.shurikenComponent) shurikenComponent.OnSkillActivated(base.skillLocator.primary);

            bool wasCharged = this.isCharged;

            this.shotCooldown = this.baseShotDuration / this.attackSpeedStat;
            this.isCharged = false;

            if (this.iDrive) this.iDrive.machineGunVFX.Play();

            base.characterBody.AddSpreadBloom(1f);
            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, base.gameObject, "ShotgunMuzzle", false);

            string soundString = "sfx_driver_sniper_shoot";
            if (wasCharged)
            {
                soundString = "sfx_driver_sniper_shoot_charged";
            }

            string animString = "SteadyAimFireCharged";

            Util.PlaySound(soundString, this.gameObject);
            base.PlayAnimation("Gesture, Override", animString, "Action.playbackRate", this.shotCooldown * 1.5f);

            float value = Util.Remap(this.chargeTimer, 0f, this.chargeDuration, 0f, 1f);

            this.chargeTimer = 0f;

            if (this.iDrive) this.iDrive.ConsumeAmmo(1f + value);

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                base.AddRecoil(-1f * SteadyAim.recoil, -2f * SteadyAim.recoil, -0.5f * SteadyAim.recoil, 0.5f * SteadyAim.recoil);

                float dmg = Shoot.damageCoefficient;
                if (this.skillLocator.secondary.stock > 0) dmg = Util.Remap(value, 0f, 1f, Shoot.damageCoefficient, SteadyAim.damageCoefficient);

                if (value >= 0.25f)
                {
                    this.skillLocator.secondary.DeductStock(1);
                }

                GameObject tracerPrefab = Modules.Assets.sniperTracer;
                //if (this.isCrit) tracerPrefab = Modules.Assets.sniperTracer;

                BulletAttack bulletAttack = new BulletAttack
                {
                    bulletCount = 1,
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = dmg * this.damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Stun1s | iDrive.DamageType,
                    falloffModel = BulletAttack.FalloffModel.None,
                    maxDistance = 2000f,
                    force = 1000f,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = 0f,
                    isCrit = isCrit,
                    owner = this.gameObject,
                    muzzleName = "ShotgunMuzzle",
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = Shoot.procCoefficient,
                    radius = 0.2f,
                    sniper = false,
                    stopperMask = LayerIndex.world.mask,
                    weapon = null,
                    tracerEffectPrefab = tracerPrefab,
                    spreadPitchScale = 0f,
                    spreadYawScale = 0f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                };
                bulletAttack.AddModdedDamageType(iDrive.ModdedDamageType);
                bulletAttack.modifyOutgoingDamageCallback = delegate (BulletAttack _bulletAttack, ref BulletAttack.BulletHit hitInfo, DamageInfo damageInfo)
                {
                    if (BulletAttack.IsSniperTargetHit(hitInfo))
                    {
                        damageInfo.damage *= 2f;
                        damageInfo.damageColorIndex = DamageColorIndex.Sniper;

                        EffectData effectData = new EffectData
                        {
                            origin = hitInfo.point,
                            rotation = Quaternion.LookRotation(-hitInfo.direction)
                        };

                        effectData.SetHurtBoxReference(hitInfo.hitHurtBox);
                        EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Common/VFX/WeakPointProcEffect.prefab").WaitForCompletion(), effectData, true);
                        Util.PlaySound("sfx_driver_headshot", base.gameObject);
                    }
                };
                bulletAttack.Fire();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            base.PlayAnimation("Gesture, Override", "SteadyAimEnd", "Action.playbackRate", 0.2f);
            base.PlayAnimation("AimPitch", "AimPitch");
            this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

            if (this.characterBody.master && this.characterBody.master.inventory)
            {
                this.characterBody.master.inventory.onInventoryChanged -= Inventory_onInventoryChanged;
            }

            if (!this.cancelling)
            {
                this.characterBody._defaultCrosshairPrefab = this.iDrive.crosshairPrefab;
            }
            else
            {
                base.PlayAnimation("Gesture, Override", this.iDrive.weaponDef.equipAnimationString);
            }

            if (this.overlayController != null)
            {
                HudOverlayManager.RemoveOverlay(this.overlayController);
                this.overlayController = null;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}