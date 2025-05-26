using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.LunarRifle
{
    public class Shoot : BaseDriverShootState
    {
        public static float _damageCoefficient = 6f;

        private static GameObject _muzzleFlash = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/MuzzleflashLunarShard.prefab").WaitForCompletion();
        private static GameObject _hitEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/LunarGolem/LunarGolemTwinShotExplosion.prefab").WaitForCompletion();

        protected override float damageCoefficient => _damageCoefficient;
        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override float maxBulletSpread => this.characterBody.spreadBloomAngle;
        protected override string shootSoundString => "sfx_driver_lunar_rifle_shoot";
        protected override string animationString => "FireTwohand";
        protected override DamageTypeCombo damageType => this.iDrive.DamageType;
        protected override GameObject tracerPrefab => Modules.Assets.lunarRifleTracer;
        protected override GameObject muzzleFlashPrefab => _muzzleFlash;
        protected override GameObject hitEffectPrefab => _hitEffect;

        public override void OnEnter()
        {
            base.procCoefficient = 1f;
            base.bulletCount = 1;
            base.dropShells = 0;
            base.ammoConsumption = 1f;
            base.useAttackSpeed = true;

            base.bulletFalloff = BulletAttack.FalloffModel.None;
            base.hitMask = LayerIndex.CommonMasks.bullet;
            base.stopperMask = LayerIndex.CommonMasks.bullet;
            base.damageColorIndex = DamageColorIndex.Default;

            base.bulletRange = 500f;
            base.bulletThiccness = 1f;
            base.bulletForce = 2500f;
            base.selfForce = 500f;

            base.baseDuration = 0.65f;
            base.earlyExitFraction = 0.5f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 12f;
            base.spreadBloom = 4f;
            base.aimTimer = 5f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "ShotgunMuzzle";

            base.OnEnter();
        }
    }
}