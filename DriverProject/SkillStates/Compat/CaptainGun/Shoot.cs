using RobDriver.SkillStates.BaseStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver.CaptainGun
{
    public class Shoot : BaseDriverShootState
    {
        public static float _damageCoefficient = 1.2f;
        public static uint _bulletCount = 8;

        private static GameObject _hitPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/HitsparkCaptainShotgun.prefab").WaitForCompletion();

        protected override float damageCoefficient => _damageCoefficient;
        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override float maxBulletSpread => Util.Remap(base.characterBody.spreadBloomAngle, 0f, 2f, 8f, 0f);
        protected override string shootSoundString => base.characterBody.spreadBloomAngle <= 2f 
            ? "Play_captain_m1_shotgun_shootTight"
            : "Play_captain_m1_shootWide";
        protected override string animationString => "Shoot";
        protected override DamageTypeCombo damageType => this.iDrive.DamageType;
        protected override GameObject tracerPrefab => this.isCrit ? Modules.Assets.shotgunTracerCrit : Modules.Assets.shotgunTracer;
        protected override GameObject muzzleFlashPrefab => EntityStates.Commando.CommandoWeapon.FireBarrage.effectPrefab;
        protected override GameObject hitEffectPrefab => _hitPrefab;

        public override void OnEnter()
        {
            base.procCoefficient = 0.75f;
            base.bulletCount = _bulletCount;
            base.dropShells = 1;
            base.ammoConsumption = 1f;
            base.useAttackSpeed = true;

            base.bulletFalloff = BulletAttack.FalloffModel.None;
            base.hitMask = LayerIndex.CommonMasks.bullet;
            base.stopperMask = LayerIndex.CommonMasks.bullet;
            base.damageColorIndex = DamageColorIndex.Default;

            base.bulletRange = 2000f;
            base.bulletThiccness = 0.3f;
            base.bulletForce = 500f;
            base.selfForce = 100f;

            base.baseDuration = 0.9f;
            base.earlyExitFraction = 0.8f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 6f;
            base.spreadBloom = -0.2f;
            base.aimTimer = 4f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "ShotgunMuzzle";

            base.OnEnter();
        }
    }
}
