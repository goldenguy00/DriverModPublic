using RoR2;
using UnityEngine;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.ArmCannon
{
    public class Shoot : BaseDriverProjectileAttack
    {
        public static float _damageCoefficient = 10f;

        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override string animationString => "Shoot";
        protected override string shootSound => "sfx_driver_rocket_launcher_shoot";
        protected override DamageTypeCombo? damageType => this.iDrive.DamageType;
        protected override GameObject projectilePrefab => Modules.Projectiles.armCannonPrefab;
        protected override GameObject muzzleFlashPrefab => _muzzleFlashPrefab;

        public override void OnEnter()
        {
            damageCoefficient = _damageCoefficient;
            ammoConsumption = 1f;
            useAttackSpeed = true;
            useICBM = false;

            damageColorIndex = DamageColorIndex.Default;
            target = null;
            maxDistance = -1f;
            fuseOverride = -1f;
            speedOverride = 120f;
            force = 1200f;
            selfForce = 25f;

            baseDuration = 1.2f;
            earlyExitFraction = 0.4f;
            fireDelayFraction = 0f;

            visualRecoilAmplitude = 16f;
            arcPitch = 0f;
            spreadBloom = 4f;
            aimTimer = 5f;

            playbackRateString = "Shoot.playbackRate";
            muzzleString = "ShotgunMuzzle";

            base.OnEnter();
        }
    }
}