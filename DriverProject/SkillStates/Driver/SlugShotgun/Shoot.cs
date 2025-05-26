using RoR2;
using UnityEngine;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.SlugShotgun
{
    public class Shoot : BaseDriverShootState
    {
        public static float _damageCoefficient = 9f;

        protected override float damageCoefficient => _damageCoefficient;
        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override float maxBulletSpread => this.characterBody.spreadBloomAngle;
        protected override string shootSoundString => this.isCrit ? "sfx_driver_slug_shotgun_shoot_critical" : "sfx_driver_slug_shotgun_shoot";
        protected override string animationString => "FireRiotShotgun";
        protected override DamageTypeCombo damageType => this.iDrive.DamageType;
        protected override GameObject tracerPrefab => this.isCrit ? Modules.Assets.shotgunTracerCrit : Modules.Assets.shotgunTracer;
        protected override GameObject muzzleFlashPrefab => EntityStates.Commando.CommandoWeapon.FireBarrage.effectPrefab;
        protected override GameObject hitEffectPrefab => EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab;

        public override void OnEnter()
        {
            base.procCoefficient = 1f;
            base.bulletCount = 1;
            base.dropShells = 1;
            base.ammoConsumption = 1f;
            base.useAttackSpeed = true;

            base.bulletFalloff = BulletAttack.FalloffModel.None;
            base.hitMask = LayerIndex.CommonMasks.bullet;
            base.stopperMask = LayerIndex.CommonMasks.bullet;
            base.damageColorIndex = DamageColorIndex.Default;

            base.bulletRange = 200f;
            base.bulletThiccness = 1f;
            base.bulletForce = 2500f;
            base.selfForce = 500f;

            base.baseDuration = 1.6f;
            base.earlyExitFraction = 0.75f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 40f;
            base.spreadBloom = 4f;
            base.aimTimer = 5f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "ShotgunMuzzle";
            base.aimPitchString = "ShotgunAimPitch";

            base.OnEnter();
        }
    }
}