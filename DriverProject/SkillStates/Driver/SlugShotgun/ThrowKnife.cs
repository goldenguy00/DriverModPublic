using UnityEngine;
using RoR2;
using EntityStates;
using RoR2.Projectile;

namespace RobDriver.SkillStates.Driver.SlugShotgun
{
    public class ThrowKnife : GenericProjectileBaseState
    {
        public static new float baseDuration = 0.55f;
        public static float baseDelayDuration = 0.1f * baseDuration;

        public static new float damageCoefficient = 6.5f;

        public override void OnEnter()
        {
            base.attackSoundString = "sfx_driver_gun_throw";

            base.baseDuration = baseDuration;
            base.baseDelayBeforeFiringProjectile = baseDelayDuration;

            base.damageCoefficient = damageCoefficient;
            base.force = 120f;

            base.projectilePitchBonus = -7.5f;
            //base.minSpread = 0;
            //base.maxSpread = 0;

            base.recoilAmplitude = 0.1f;
            base.bloom = 10;

            base.OnEnter();
        }

        public override void FireProjectile()
        {
            // if i just rewrite it surely it can't break right?
            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();
                aimRay = this.ModifyProjectileAimRay(aimRay);
                aimRay.direction = Util.ApplySpread(aimRay.direction, this.minSpread, this.maxSpread, 1f, 1f, 0f, this.projectilePitchBonus);
                ProjectileManager.instance.FireProjectile(Modules.Projectiles.stunGrenadeProjectilePrefab, aimRay.origin, Util.QuaternionSafeLookRotation(aimRay.direction), 
                    base.gameObject, this.damageStat * ThrowGrenade.damageCoefficient, this.force, this.RollCrit(), DamageColorIndex.Default, null, -1f);
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        public override void PlayAnimation(float duration)
        {
            if (base.GetModelAnimator())
            {
                base.PlayAnimation("Gesture, Override", "ThrowGrenade", "Grenade.playbackRate", this.duration);
            }
        }
    }
}