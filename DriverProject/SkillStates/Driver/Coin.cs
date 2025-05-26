using RobDriver.Modules;
using RoR2.Projectile;
using RoR2;
using UnityEngine;
using EntityStates;

namespace RobDriver.SkillStates.Driver
{
    public class Coin : BaseSkillState
    {
        private float baseDuration = 0.5f;
        private float duration;

        public virtual GameObject projectilePrefab => Projectiles.coinProjectile;

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = baseDuration / attackSpeedStat;

            Util.PlaySound("sfx_driver_coin", base.gameObject);

            base.PlayAnimation("LeftArm, Override", "FireShard");

            this.FireProjectile();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        private void FireProjectile()
        {
            if (base.isAuthority)
            {
                Ray aimRay = GetAimRay();
                aimRay.direction = Util.ApplySpread(aimRay.direction, 0, 0, 1f, 1f, 0f, -10);

                Vector3 flickDirection = aimRay.direction;
                flickDirection *= Mathf.Clamp(base.rigidbody.velocity.magnitude, 1f, 20f);
                flickDirection.y += Mathf.Max(base.rigidbody.velocity.y, 0);

                ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                { 
                    projectilePrefab = projectilePrefab,
                    position = aimRay.origin,
                    rotation = Util.QuaternionSafeLookRotation(flickDirection),
                    owner = this.gameObject
                });
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge < this.duration * 0.5)
                return InterruptPriority.Pain;

            return InterruptPriority.Skill;
        }
    }
}