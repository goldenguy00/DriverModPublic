using RoR2;
using UnityEngine;
using EntityStates;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.SMG
{
    public class PhaseRound : BaseDriverProjectileAttack
    {
        public static float _damageCoefficient = 6f;

        protected new static readonly GameObject _muzzleFlashPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion();
        protected static readonly GameObject _projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/FMJRamping.prefab").WaitForCompletion();

        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override string animationString => "Shoot";
        protected override string shootSound => "sfx_driver_fire_preon";
        protected override DamageTypeCombo? damageType => this.iDrive.DamageType;
        protected override GameObject projectilePrefab => PhaseRound._projectilePrefab;
        protected override GameObject muzzleFlashPrefab => PhaseRound._muzzleFlashPrefab;

        public override void OnEnter()
        {
            base.damageCoefficient = _damageCoefficient;
            base.ammoConsumption = 2f;
            base.useAttackSpeed = true;
            base.useICBM = false;

            base.interruptPriority = InterruptPriority.PrioritySkill;
            base.damageColorIndex = DamageColorIndex.Default;
            base.target = null;
            base.maxDistance = -1f;
            base.fuseOverride = -1f;
            base.speedOverride = 120f;
            base.force = 1200f;
            base.selfForce = 25f;

            base.baseDuration = 0.9f;
            base.earlyExitFraction = 0.4f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 1.5f;
            base.arcPitch = 0f;
            base.spreadBloom = 12f;
            base.aimTimer = 5f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "PistolMuzzle";

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