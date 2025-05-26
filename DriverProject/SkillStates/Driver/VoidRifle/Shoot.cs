using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.VoidRifle
{
    public class Shoot : BaseDriverShootState
    {
        public static float _damageCoefficient = 3.5f;

        private static GameObject _muzzleFlash = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidRaidCrab/MuzzleflashVoidRaidCrabMissiles.prefab").WaitForCompletion();

        protected override float damageCoefficient => _damageCoefficient;
        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration * 3f;
        protected override float maxBulletSpread => this.characterBody.spreadBloomAngle;
        protected override string shootSoundString => "sfx_driver_lunar_rifle_shoot";
        protected override string animationString => "FireTwohand";
        protected override DamageTypeCombo damageType => this.iDrive.DamageType;
        protected override GameObject tracerPrefab => VoidPistol.Shoot._tracerPrefab;
        protected override GameObject muzzleFlashPrefab => _muzzleFlash;
        protected override GameObject hitEffectPrefab => EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab;

        public override void OnEnter()
        {
            base.procCoefficient = 1f;
            base.bulletCount = 1;
            base.dropShells = 0;
            base.ammoConsumption = 1f;
            base.useAttackSpeed = true;

            base.hitMask = LayerIndex.CommonMasks.bullet;
            base.stopperMask = LayerIndex.world.mask;
            base.damageColorIndex = DamageColorIndex.Void;

            base.bulletRange = 500f;
            base.bulletThiccness = 1.5f;
            base.bulletForce = 25f;
            base.selfForce = 0f;

            base.baseDuration = 0.25f;
            base.earlyExitFraction = 0.5f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 4f;
            base.spreadBloom = 4f;
            base.aimTimer = 5f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "ShotgunMuzzle";

            base.OnEnter();
        }
    }
}