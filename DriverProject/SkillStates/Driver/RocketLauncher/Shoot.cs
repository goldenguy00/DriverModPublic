using RoR2;
using UnityEngine;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.RocketLauncher
{
    public class Shoot : BaseDriverProjectileAttack
    {
        public static float _damageCoefficient = 5f;

        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override string animationString => "FireTwohand";
        protected override string shootSound => "sfx_driver_rocket_launcher_shoot";
        protected override DamageTypeCombo? damageType => this.iDrive.DamageType;
        protected override GameObject projectilePrefab => Modules.Projectiles.rocketProjectilePrefab;
        protected override GameObject muzzleFlashPrefab => BaseDriverProjectileAttack._muzzleFlashPrefab;

        public override void OnEnter()
        {
            base.damageCoefficient = _damageCoefficient;
            base.ammoConsumption = 1.5f;
            base.useAttackSpeed = true;
            base.useICBM = true;

            base.damageColorIndex = DamageColorIndex.Default;
            base.target = null;
            base.maxDistance = -1f;
            base.fuseOverride = -1f;
            base.speedOverride = 120f;
            base.force = 1200f;
            base.selfForce = 50f;

            base.baseDuration = 1.3f;
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
    }
}