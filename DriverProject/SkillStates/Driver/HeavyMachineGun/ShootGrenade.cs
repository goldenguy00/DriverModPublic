using RoR2;
using UnityEngine;
using RoR2.Projectile;
using RobDriver.SkillStates.BaseStates;
using EntityStates;

namespace RobDriver.SkillStates.Driver.HeavyMachineGun
{
    public class ShootGrenade : BaseDriverProjectileAttack
    {
        public static float _damageCoefficient = 8f;

        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override string animationString => "FireTwohand";
        protected override string shootSound => "sfx_driver_grenade_launcher_shoot";
        protected override DamageTypeCombo? damageType => this.iDrive.DamageType;
        protected override GameObject projectilePrefab => Modules.Projectiles.hmgGrenadeProjectilePrefab;
        protected override GameObject muzzleFlashPrefab => BaseDriverProjectileAttack._muzzleFlashPrefab;

        public override void OnEnter()
        {
            base.damageCoefficient = _damageCoefficient;
            base.ammoConsumption = 1f;
            base.useAttackSpeed = true;
            base.useICBM = true;

            base.damageColorIndex = DamageColorIndex.Default;
            base.target = null;
            base.maxDistance = -1f;
            base.fuseOverride = -1f;
            base.speedOverride = 80f;
            base.force = 1200f;
            base.selfForce = 10f;

            base.baseDuration = 0.6f;
            base.earlyExitFraction = 0.4f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 16f;
            base.arcPitch = 0f;
            base.spreadBloom = 4f;
            base.aimTimer = 5f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "ShotgunMuzzle";

            base.OnEnter();
        }

        protected override void AuthorityModifyProjectileInfo(ref FireProjectileInfo fireProjectileInfo)
        {
            base.AuthorityModifyProjectileInfo(ref fireProjectileInfo);

            var damageType = fireProjectileInfo.damageTypeOverride.Value;
            damageType.damageSource = DamageSource.Secondary;

            fireProjectileInfo.damageTypeOverride = damageType;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= this.earlyExitTime)
                return InterruptPriority.Any;

            return InterruptPriority.PrioritySkill;
        }
    }
}