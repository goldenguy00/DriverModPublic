using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.AddressableAssets;
using RobDriver.Modules.Components;
using RoR2.Projectile;

namespace RobDriver.SkillStates.Driver.MachineGun
{
    public class Zap : GenericProjectileBaseState
    {
        public static new float baseDuration = 0.8f;
        public static float baseDelayDuration = 0.5f * baseDuration;

        public static new float damageCoefficient = 3.8f;

        private uint playID;

        public override void OnEnter()
        {
            base.projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainTazer.prefab").WaitForCompletion();
            //base.effectPrefab = Modules.Assets.SomeMuzzleEffect;
            //targetmuzzle = "muzzleThrow"

            //base.attackSoundString = "sfx_driver_gun_throw";

            base.baseDuration = baseDuration;
            base.baseDelayBeforeFiringProjectile = baseDelayDuration;

            base.damageCoefficient = damageCoefficient;
            base.force = 120f;

            base.projectilePitchBonus = 0f;
            //base.minSpread = 0;
            //base.maxSpread = 0;

            base.recoilAmplitude = 0.1f;
            base.bloom = 10;

            base.OnEnter();

            this.playID = Util.PlaySound("sfx_driver_zap_prep", this.gameObject);

            DriverController iDrive = this.GetComponent<DriverController>();
            if (iDrive) iDrive.ConsumeAmmo();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override void FireProjectile()
        {
            base.FireProjectile();

            Util.PlaySound("sfx_driver_zap", this.gameObject);

            AkSoundEngine.StopPlayingID(this.playID);
        }

        public override void ModifyProjectileInfo(ref FireProjectileInfo fireProjectileInfo)
        {
            base.ModifyProjectileInfo(ref fireProjectileInfo);
            var damageType = base.projectilePrefab.GetComponent<ProjectileDamage>().damageType;
            damageType.damageSource = DamageSource.Secondary;
            fireProjectileInfo.damageTypeOverride = damageType;
        }

        public override void OnExit()
        {
            base.OnExit();

            this.GetModelAnimator().SetTrigger("endAim");
            AkSoundEngine.StopPlayingID(this.playID);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= (this.duration * this.delayBeforeFiringProjectile) + 0.1f && this.firedProjectile) return InterruptPriority.Any;
            return InterruptPriority.Pain;
        }

        public override void PlayAnimation(float duration)
        {
            if (base.GetModelAnimator())
            {
                base.PlayAnimation("Gesture, Override", "Zap", "Action.playbackRate", this.duration);
                base.PlayAnimation("AimPitch", "Shoot");
            }
        }
    }
}