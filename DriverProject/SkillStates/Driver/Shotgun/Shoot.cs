using RoR2;
using UnityEngine;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.Shotgun
{
    public class Shoot : BaseDriverShootState
    {
        public static float _damageCoefficient = 1.9f;
        public static uint _bulletCount = 8;

        protected override float damageCoefficient => _damageCoefficient;
        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override float maxBulletSpread => 7f;
        protected override string shootSoundString => this.isCrit ? "sfx_driver_shotgun_shoot_critical" : "sfx_driver_shotgun_shoot";
        protected override string animationString => "FireShotgun";
        protected override DamageTypeCombo damageType => this.iDrive.DamageType;
        protected override GameObject tracerPrefab => this.isCrit ? Modules.Assets.shotgunTracerCrit : Modules.Assets.shotgunTracer;
        protected override GameObject muzzleFlashPrefab => EntityStates.Commando.CommandoWeapon.FireBarrage.effectPrefab;
        protected override GameObject hitEffectPrefab => EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab;

        public override void OnEnter()
        {
            base.procCoefficient = 0.7f;
            base.bulletCount = _bulletCount;
            base.dropShells = 1;
            base.ammoConsumption = 1f;
            base.useAttackSpeed = true;

            base.bulletFalloff = BulletAttack.FalloffModel.DefaultBullet;
            base.hitMask = LayerIndex.CommonMasks.bullet;
            base.stopperMask = LayerIndex.CommonMasks.bullet;
            base.damageColorIndex = DamageColorIndex.Default;

            base.bulletRange = 100f;
            base.bulletThiccness = 1f;
            base.bulletForce = 500f;
            base.selfForce = 1000f;

            base.baseDuration = 1.1f;
            base.earlyExitFraction = 0.7f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 40f;
            base.spreadBloom = 4f;
            base.aimTimer = 4f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "ShotgunMuzzle";
            base.aimPitchString = "ShotgunAimPitch";

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!base.cancelling && base.fixedAge >= base.duration)
            {
                this.PlayAnimation("Gesture, Override", "ReloadShotgun", "Shoot.playbackRate", 1.75f);
            }
        }
    }
}