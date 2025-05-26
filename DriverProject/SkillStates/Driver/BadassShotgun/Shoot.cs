using RoR2;
using UnityEngine;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.BadassShotgun
{
    public class Shoot : BaseDriverShootState
    {
        public static float _damageCoefficient = 1.4f;
        public static uint _bulletCount = 24;

        protected override float damageCoefficient => _damageCoefficient;
        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override float maxBulletSpread => 8f;
        protected override string shootSoundString => this.isCrit ? "sfx_driver_shotgun_shoot_critical" : "sfx_driver_shotgun_shoot";
        protected override string animationString => "Shoot";
        protected override DamageTypeCombo damageType => this.iDrive.DamageType;
        protected override GameObject tracerPrefab => this.isCrit ? Modules.Assets.shotgunTracerCrit : Modules.Assets.shotgunTracer;
        protected override GameObject muzzleFlashPrefab => EntityStates.Commando.CommandoWeapon.FireBarrage.effectPrefab;
        protected override GameObject hitEffectPrefab => EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab;

        public override void OnEnter()
        {
            base.procCoefficient = 1f;
            base.bulletCount = _bulletCount;
            base.dropShells = 4;
            base.ammoConsumption = 2f;
            base.useAttackSpeed = true;

            base.bulletFalloff = BulletAttack.FalloffModel.None;
            base.hitMask = LayerIndex.CommonMasks.bullet;
            base.stopperMask = LayerIndex.CommonMasks.bullet;
            base.damageColorIndex = DamageColorIndex.Default;

            base.bulletRange = 250f;
            base.bulletThiccness = 1f;
            base.bulletForce = 3000f;
            base.selfForce = 3000f;

            base.baseDuration = 1.6f;
            base.earlyExitFraction = 0.5f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 40f;
            base.spreadBloom = 4f;
            base.aimTimer = 5f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "ShotgunMuzzle";

            base.OnEnter();

            if (this.isGrounded)
                this.selfForce *= 0.25f;

            this.selfForce /= this.attackSpeedStat;
        }
    }
}