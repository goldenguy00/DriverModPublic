using EntityStates;
using R2API;
using RoR2;
using UnityEngine;

namespace RobDriver.SkillStates.Driver.DualUzi
{
    public class Shoot : BaseDriverSkillState
    {
        public static float damageCoefficient = 1.1f;
        public static float procCoefficient = 1f;
        public static float baseDuration = 0.2f;
        public static float force = 20f;
        public static float recoil = 0.5f;
        public static float range = 256f;
        public static GameObject tracerEffectPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoDefault");

        private float duration;
        private float fireTime;
        private bool hasFired;
        private bool isCrit;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = Shoot.baseDuration / this.attackSpeedStat;
            this.characterBody.isSprinting = false;

            this.fireTime = 0.1f * this.duration;
            base.characterBody.SetAimTimer(2f);

            this.isCrit = base.RollCrit();

            if (base.isAuthority)
            {
                this.hasFired = true;
                this.Fire();
            }

            if (this.isCrit) base.PlayAnimation("Gesture, Override", "FireMachineGunCritical", "Shoot.playbackRate", this.duration * 2f);
            else base.PlayAnimation("Gesture, Override", "FireMachineGun", "Shoot.playbackRate", this.duration * 2f);

            if (this.iDrive) this.iDrive.ConsumeAmmo();
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        private void Fire()
        {
            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, this.gameObject, "PistolMuzzle", false);
            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab, this.gameObject, "PistolMuzzle2", false);

            if (this.isCrit) Util.PlaySound("sfx_driver_machinegun_shoot_critical", base.gameObject);
            else Util.PlaySound("sfx_driver_machinegun_shoot", base.gameObject);

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                base.AddRecoil2(-1f * Shoot.recoil, -2f * Shoot.recoil, -0.5f * Shoot.recoil, 0.5f * Shoot.recoil);

                BulletAttack bulletAttack = new BulletAttack
                {
                    bulletCount = 1,
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = Shoot.damageCoefficient * this.damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = iDrive.DamageType,
                    falloffModel = BulletAttack.FalloffModel.None,
                    maxDistance = Shoot.range,
                    force = Shoot.force,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = this.characterBody.spreadBloomAngle * 3.5f,
                    isCrit = this.isCrit,
                    owner = this.gameObject,
                    muzzleName = "PistolMuzzle",
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    radius = 0.5f,
                    sniper = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    weapon = null,
                    tracerEffectPrefab = this.tracerPrefab,
                    spreadPitchScale = 1f,
                    spreadYawScale = 1f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                };
                //bulletAttack.modifyOutgoingDamageCallback += Modules.Components.RicochetUtils.BulletAttackShootableDamageCallback;
                bulletAttack.Fire();

                new BulletAttack
                {
                    bulletCount = 1,
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = Shoot.damageCoefficient * this.damageStat,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = DamageType.Generic,
                    falloffModel = BulletAttack.FalloffModel.None,
                    maxDistance = Shoot.range,
                    force = Shoot.force,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    minSpread = 0f,
                    maxSpread = this.characterBody.spreadBloomAngle * 3.5f,
                    isCrit = this.isCrit,
                    owner = this.gameObject,
                    muzzleName = "PistolMuzzle2",
                    smartCollision = true,
                    procChainMask = default(ProcChainMask),
                    procCoefficient = procCoefficient,
                    radius = 0.5f,
                    sniper = false,
                    stopperMask = LayerIndex.CommonMasks.bullet,
                    weapon = null,
                    tracerEffectPrefab = this.tracerPrefab,
                    spreadPitchScale = 1f,
                    spreadYawScale = 1f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
                }.Fire();
            }

            base.characterBody.AddSpreadBloom(0.6f);
        }

        private GameObject tracerPrefab
        {
            get
            {
                if (this.isCrit) return Shoot.tracerEffectPrefab;
                else return Shoot.tracerEffectPrefab;
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireTime && base.isAuthority)
            {
                if (!this.hasFired)
                {
                    this.hasFired = true;
                    this.Fire();
                }
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            float kek = 0.5f;

            if (base.fixedAge >= kek * this.duration)
            {
                return InterruptPriority.Any;
            }

            return InterruptPriority.Skill;
        }
    }
}