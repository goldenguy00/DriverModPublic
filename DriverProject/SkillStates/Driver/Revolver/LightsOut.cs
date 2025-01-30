using RoR2;
using UnityEngine;
using EntityStates;
using RoR2.UI;
using UnityEngine.AddressableAssets;
using R2API;

namespace RobDriver.SkillStates.Driver.Revolver
{
    public class LightsOut : BaseDriverSkillState
    {
        public static float damageCoefficient = 15f;
        public static float procCoefficient = 1f;
        public float baseDuration = 0.8f;

        protected virtual string shootSoundString => "Play_bandit2_R_fire";

        private float duration;
        private bool kill;
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;

        public override void OnEnter()
        {
            base.OnEnter();
            this.characterBody.SetAimTimer(2f);
            this.duration = this.baseDuration / this.attackSpeedStat;

            base.PlayAnimation("Gesture, Override", "ShootLightsOut", "Action.playbackRate", this.duration);

            if (this.iDrive && iDrive.defaultWeaponDef != iDrive.weaponDef) this.iDrive.weaponTimer = 0.1f;

            this.Fire();

            this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(this.characterBody,
                Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2CrosshairPrepRevolverFire.prefab").WaitForCompletion(), 
                CrosshairUtils.OverridePriority.Skill);
        }

        private void Fire()
        {
            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, this.gameObject, "PistolMuzzle", false);

            Util.PlaySound(this.shootSoundString, this.gameObject);

            if (base.isAuthority)
            {
                float recoil = 24f;
                base.AddRecoil2(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);

                this.FireBullet();
            }

            base.characterBody.AddSpreadBloom(1.25f);
        }

        protected virtual void FireBullet()
        {
            Ray aimRay = base.GetAimRay();

            BulletAttack bulletAttack = new BulletAttack
            {
                bulletCount = 1,
                aimVector = aimRay.direction,
                origin = aimRay.origin,
                damage = LightsOut.damageCoefficient * this.damageStat,
                damageColorIndex = DamageColorIndex.Default,
                damageType = iDrive.DamageType,
                falloffModel = BulletAttack.FalloffModel.None,
                maxDistance = 9999f,
                force = 9999f,
                hitMask = LayerIndex.CommonMasks.bullet,
                minSpread = 0f,
                maxSpread = 0f,
                isCrit = this.RollCrit(),
                owner = this.gameObject,
                muzzleName = "PistolMuzzle",
                smartCollision = true,
                procChainMask = default(ProcChainMask),
                procCoefficient = LightsOut.procCoefficient,
                radius = 1f,
                sniper = false,
                stopperMask = LayerIndex.CommonMasks.bullet,
                weapon = null,
                tracerEffectPrefab = Shoot.critTracerEffectPrefab,
                spreadPitchScale = 1f,
                spreadYawScale = 1f,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
            };

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
                    hitInfo.hitHurtBox.healthComponent.gameObject.AddComponent<Modules.Components.DriverHeadshotTracker>();
                }
            };
            bulletAttack.Fire();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= (0.5f * this.duration))
            {
                if (!this.kill)
                {
                    this.kill = true;
                    if (this.iDrive)
                    {
                        if (this.iDrive.weaponTimer == 0.1f)
                        {
                            this.iDrive.weaponTimer = 0f;
                            base.PlayAnimation("Gesture, Override", this.iDrive.weaponDef.equipAnimationString);
                        }
                        else base.PlayAnimation("Gesture, Override", "RefreshPistol");
                    }
                    
                }
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextState(new WaitForReload());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            this.crosshairOverrideRequest?.Dispose();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}