using RoR2;
using UnityEngine;
using RobDriver.SkillStates.BaseStates;
using RoR2.Projectile;

namespace RobDriver.SkillStates.Driver.ArtiGauntlet
{
    public class Shoot : BaseDriverProjectileAttack
    {
        public static float _damageCoefficient = 4f;

        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override string animationString => "Shoot";
        protected override string shootSound => "sfx_driver_fire_preon";
        protected override DamageTypeCombo? damageType => this.iDrive.DamageType;
        protected override GameObject projectilePrefab => Modules.Projectiles.artiGauntletPrefab;
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
            speedOverride = -1f;
            force = 120f;
            selfForce = 25f;

            baseDuration = 0.9f;
            earlyExitFraction = 0.4f;
            fireDelayFraction = 0f;

            arcPitch = 0f;
            spreadBloom = 4f;
            aimTimer = 5f;

            playbackRateString = "Shoot.playbackRate";
            muzzleString = "PistolMuzzle";

            base.OnEnter();
        }

        protected override void AuthorityModifyProjectileInfo(ref FireProjectileInfo fireProjectileInfo)
        {
            base.AuthorityModifyProjectileInfo(ref fireProjectileInfo);

            var damageType = fireProjectileInfo.damageTypeOverride.Value;
            damageType.damageType |= DamageType.IgniteOnHit;

            fireProjectileInfo.damageTypeOverride = damageType;
        }
    }
}