using RobDriver.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace RobDriver.SkillStates.Driver.RiotShotgun
{
    public class Shoot : BaseDriverShootState
    {
        public static float _damageCoefficient = 1.4f;
        public static uint _bulletCount = 8;

        protected override float damageCoefficient => _damageCoefficient;
        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override float maxBulletSpread => 4f;
        protected override string shootSoundString => this.isCrit ? "sfx_driver_riot_shotgun_shoot_critical" : "sfx_driver_riot_shotgun_shoot";
        protected override string animationString => "FireRiotShotgun";
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
            base.stopperMask = LayerIndex.world.mask;
            base.damageColorIndex = DamageColorIndex.Default;

            base.bulletRange = 100f;
            base.bulletThiccness = 0.7f;
            base.bulletForce = 25f;
            base.selfForce = 800f;

            base.baseDuration = 1.5f;
            base.earlyExitFraction = 0.75f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 20f;
            base.spreadBloom = 4f;
            base.aimTimer = 5f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "ShotgunMuzzle";
            base.aimPitchString = "ShotgunAimPitch";

            base.OnEnter();
        }
    }
}