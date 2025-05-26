using RoR2;
using UnityEngine;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.Bazooka
{
    public class Fire : BaseDriverProjectileAttack
    {
        public static float minDamageCoefficient = 6f;
        public static float maxDamageCoefficient = 12f;

        public const float minSpeed = 20f;
        public const float maxSpeed = 160f;
        public const float minRecoil = 0.5f;
        public const float maxRecoil = 25f;

        public float charge { get; set; }

        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration * 2.5f;
        protected override string animationString => this.charge >= 0.8f ? "FireBazooka" : "FireTwohand";
        protected override string shootSound => this.isCrit ? "sfx_driver_bazooka_shoot_critical" : "sfx_driver_bazooka_shoot";
        protected override DamageTypeCombo? damageType => this.iDrive.DamageType;
        protected override GameObject projectilePrefab => Modules.Projectiles.bazookaProjectilePrefab;
        protected override GameObject muzzleFlashPrefab => BaseDriverProjectileAttack._muzzleFlashPrefab;

        public override void OnEnter()
        {
            this.damageCoefficient = Util.Remap(this.charge, 0f, 1f, Fire.minDamageCoefficient, Fire.maxDamageCoefficient);
            base.ammoConsumption = 1f;
            base.useAttackSpeed = true;
            base.useICBM = true;

            base.damageColorIndex = DamageColorIndex.Default;
            base.target = null;
            base.maxDistance = -1f;
            base.fuseOverride = -1f;
            base.speedOverride = Util.Remap(this.charge, 0f, 1f, Fire.minSpeed, Fire.maxSpeed);
            base.force = 1200f;
            base.selfForce = 25f;

            base.baseDuration = 0.4f;
            base.earlyExitFraction = 1f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = Util.Remap(this.charge, 0f, 1f, Fire.minRecoil, Fire.maxRecoil);
            base.visualRecoilVertical = 2f;
            base.visualRecoilHorizontal = 0.5f;
            base.arcPitch = 0f;
            base.spreadBloom = 4f;
            base.aimTimer = 2f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "ShotgunMuzzle";

            base.OnEnter();
        }
    }
}