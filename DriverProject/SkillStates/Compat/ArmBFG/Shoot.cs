using RoR2;
using UnityEngine;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.ArmBFG
{
    public class Shoot : BaseDriverProjectileAttack
    {
        public static float _damageCoefficient = 10f;

        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override string animationString => "Shoot";
        protected override string shootSound => "sfx_driver_fire_preon";
        protected override DamageTypeCombo? damageType => this.iDrive.DamageType;
        protected override GameObject projectilePrefab => Modules.Projectiles.plasmaCannonProjectilePrefab;
        protected override GameObject muzzleFlashPrefab => BaseDriverProjectileAttack._muzzleFlashPrefab;

        public override void OnEnter()
        {
            base.damageCoefficient = _damageCoefficient;
            base.ammoConsumption = 1f;
            base.useAttackSpeed = true;
            base.useICBM = false;

            base.damageColorIndex = DamageColorIndex.Default;
            base.target = null;
            base.maxDistance = -1f;
            base.fuseOverride = -1f;
            base.speedOverride = 200f;
            base.force = 1200f;
            base.selfForce = 25f;

            base.baseDuration = 1.8f;
            base.earlyExitFraction = 0.4f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 4f;
            base.arcPitch = 0f;
            base.spreadBloom = 4f;
            base.aimTimer = 5f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "ShotgunMuzzle";

            base.OnEnter();
        }
    }
}