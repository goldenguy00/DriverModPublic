using RoR2;
using UnityEngine;
using EntityStates;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.NemmercGun
{
    public class Shoot2 : BaseDriverShootState
    {
        public static float _damageCoefficient = 1.8f;
        public static uint _bulletCount = 8;

        protected override float damageCoefficient => _damageCoefficient;
        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override float maxBulletSpread => 8f;
        protected override string shootSoundString => DriverPlugin.StarstormInstalled ? "Play_nemmerc_primary_fire2" : this.isCrit ? "sfx_driver_shotgun_shoot_critical" : "sfx_driver_shotgun_shoot";
        protected override string animationString => "FireRiotShotgun";
        protected override DamageTypeCombo damageType => this.iDrive.DamageType;
        protected override GameObject tracerPrefab => this.isCrit ? Modules.Assets.shotgunTracerCrit : Modules.Assets.shotgunTracer;
        protected override GameObject muzzleFlashPrefab => EntityStates.Commando.CommandoWeapon.FireBarrage.effectPrefab;
        protected override GameObject hitEffectPrefab => EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab;

        public override void OnEnter()
        {
            base.procCoefficient = 0.5f;
            base.bulletCount = _bulletCount;
            base.dropShells = 1;
            base.ammoConsumption = 1f;
            base.useAttackSpeed = true;

            base.bulletFalloff = BulletAttack.FalloffModel.DefaultBullet;
            base.hitMask = LayerIndex.CommonMasks.bullet;
            base.stopperMask = LayerIndex.CommonMasks.bullet;
            base.damageColorIndex = DamageColorIndex.Default;

            base.bulletRange = 200f;
            base.bulletThiccness = 1f;
            base.bulletForce = 50f;
            base.selfForce = 400f;

            base.baseDuration = 1.5f;
            base.earlyExitFraction = 0.75f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 8f;
            base.spreadBloom = 4f;
            base.aimTimer = 4f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "ShotgunMuzzle";
            base.aimPitchString = "ShotgunAimPitch";

            base.OnEnter();
        }
    }
}