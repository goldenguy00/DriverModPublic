using RobDriver.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace RobDriver.SkillStates.Driver.MachineGun
{
    public class Shoot : BaseDriverShootState
    {
        public static float _damageCoefficient = 1.2f;

        private static GameObject _tracer = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCommandoDefault");

        protected override float damageCoefficient => _damageCoefficient;
        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration * 2f;
        protected override float maxBulletSpread => this.characterBody.spreadBloomAngle * 2.5f;
        protected override string shootSoundString => this.isCrit ? "sfx_driver_machinegun_shoot_critical" : "sfx_driver_machinegun_shoot";
        protected override string animationString => this.isCrit ? "FireMachineGunCritical" : "FireMachineGun";
        protected override DamageTypeCombo damageType => this.iDrive.DamageType | new DamageTypeCombo { damageType = DamageType.BypassArmor };
        protected override GameObject tracerPrefab => _tracer;
        protected override GameObject muzzleFlashPrefab => EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab;
        protected override GameObject hitEffectPrefab => EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab;

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

            base.bulletRange = 256f;
            base.bulletThiccness = 0.5f;
            base.bulletForce = 20f;
            base.selfForce = 0f;

            base.baseDuration = 0.21f;
            base.earlyExitFraction = 0.5f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 0.5f;
            base.visualRecoilVertical = 2f;
            base.visualRecoilHorizontal = 0.5f;
            base.spreadBloom = 0.225f;
            base.aimTimer = 2f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "PistolMuzzle";

            base.OnEnter();

            this.characterBody.isSprinting = false;
            this.iDrive.machineGunVFX.Play();
        }
    }
}