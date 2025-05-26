using RobDriver.SkillStates.BaseStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RobDriver.SkillStates.Driver.RiotShotgun
{
    public class TearGas : BaseDriverProjectileAttack
    {
        public static float _damageCoefficient = 5f;

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
            base.speedOverride = 75f;
            base.force = 120f;
            base.selfForce = 10f;

            base.baseDuration = 0.75f;
            base.earlyExitFraction = 0.4f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 6f;
            base.arcPitch = 5f;
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
    }
}
