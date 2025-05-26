using UnityEngine;
using RoR2;
using EntityStates;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver
{
    public class ThrowGrenade : BaseDriverProjectileAttack
    {
        public static float _damageCoefficient = 5f;

        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override string animationString => "ThrowGrenade";
        protected override string shootSound => "sfx_driver_gun_throw";
        protected override DamageTypeCombo? damageType => null;
        protected override GameObject projectilePrefab => Modules.Projectiles.stunGrenadeProjectilePrefab;
        protected override GameObject muzzleFlashPrefab => BaseDriverProjectileAttack._muzzleFlashPrefab;

        public override void OnEnter()
        {
            base.damageCoefficient = _damageCoefficient;
            base.ammoConsumption = 0f;
            base.useAttackSpeed = false;
            base.useICBM = true;

            base.damageColorIndex = DamageColorIndex.Default;
            base.target = null;
            base.maxDistance = -1f;
            base.fuseOverride = -1f;
            base.speedOverride = -1f;
            base.force = 120f;
            base.selfForce = 0f;

            base.baseDuration = 0.55f;
            base.earlyExitFraction = 1f;
            base.fireDelayFraction = 0.1f;

            base.visualRecoilAmplitude = 1f;
            base.visualRecoilVertical = 0.3f;
            base.visualRecoilHorizontal = 0.1f;
            base.arcPitch = -7.5f;
            base.spreadBloom = 4f;
            base.aimTimer = 2f;

            base.playbackRateString = "Grenade.playbackRate";
            base.muzzleString = "ShotgunMuzzle";

            base.OnEnter();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}