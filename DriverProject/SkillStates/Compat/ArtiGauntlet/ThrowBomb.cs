using RoR2.Projectile;
using RoR2;
using UnityEngine;
using RobDriver.SkillStates.BaseStates;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver.ArtiGauntlet
{
    public class ThrowBomb : BaseDriverProjectileAttack
    {
        public static float minDamageCoefficient = 6f;
        public static float maxDamageCoefficient = 20f;

        public const float minSpeed = 20f;
        public const float maxSpeed = 160f;
        public const float minRecoil = 0.5f;
        public const float maxRecoil = 25f;

        private static GameObject _projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageLightningBombProjectile.prefab").WaitForCompletion();
        private static new GameObject _muzzleFlashPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MuzzleflashMageLightningLarge.prefab").WaitForCompletion();

        public float charge { get; set; }

        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration * 2f;
        protected override string animationString => "Shoot";
        protected override string shootSound => "Play_mage_m2_shoot";
        protected override DamageTypeCombo? damageType => this.iDrive.DamageType;
        protected override GameObject projectilePrefab => _projectilePrefab;
        protected override GameObject muzzleFlashPrefab => _muzzleFlashPrefab;

        public override void OnEnter()
        {
            this.damageCoefficient = Util.Remap(this.charge, 0f, 1f, minDamageCoefficient, maxDamageCoefficient);
            ammoConsumption = 1f;
            useAttackSpeed = true;
            useICBM = false;

            damageColorIndex = DamageColorIndex.Default;
            target = null;
            maxDistance = -1f;
            fuseOverride = -1f;
            speedOverride = -1f;
            force = 3000f * this.charge;
            selfForce = 1000f * this.charge;

            baseDuration = 0.4f;
            earlyExitFraction = 1f;
            fireDelayFraction = 0f;

            arcPitch = 0f;
            spreadBloom = 4f;
            aimTimer = 2f;

            playbackRateString = "Shoot.playbackRate";
            muzzleString = "PistolMuzzle";

            base.OnEnter();
        }

        protected override void PlayAnimation()
        {
            PlayCrossfade("Gesture, Override", this.animationString, this.playbackRateString, this.animationDuration, 0.1f);
        }

        protected override void AuthorityModifyProjectileInfo(ref FireProjectileInfo fireProjectileInfo)
        {
            base.AuthorityModifyProjectileInfo(ref fireProjectileInfo);
            var damageTypeOverride = fireProjectileInfo.damageTypeOverride.Value;
            damageTypeOverride.damageSource = DamageSource.Secondary;
            fireProjectileInfo.damageTypeOverride = damageTypeOverride;
        }
    }
}
