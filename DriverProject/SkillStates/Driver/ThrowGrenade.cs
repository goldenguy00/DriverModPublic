using UnityEngine;
using RoR2;
using EntityStates;
using RoR2.Projectile;

namespace RobDriver.SkillStates.Driver
{
    public class ThrowGrenade : GenericProjectileBaseState
    {
        public static float baseDuration = 0.55f;
        public static float baseDelayDuration = 0.1f * baseDuration;

        public static float damageCoefficient = 5f;

        public override void OnEnter()
        {
            base.attackSoundString = "sfx_driver_gun_throw";

            base.baseDuration = baseDuration;
            base.baseDelayBeforeFiringProjectile = baseDelayDuration;

            base.damageCoefficient = damageCoefficient;
            base.force = 120f;

            base.projectilePitchBonus = -7.5f;

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

                // copied from moff's rocket
                // the fact that this item literally has to be hardcoded into character skillstates makes me so fucking angry you have no idea
                if (this.characterBody.inventory && this.characterBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile) > 0)
                {
                    float damageMult = DriverPlugin.GetICBMDamageMult(this.characterBody);

                    Vector3 rhs = Vector3.Cross(Vector3.up, aimRay.direction);
                    Vector3 axis = Vector3.Cross(aimRay.direction, rhs);

                    Vector3 direction = Quaternion.AngleAxis(-1.5f, axis) * aimRay.direction;
                    Quaternion rotation = Quaternion.AngleAxis(1.5f, axis);
                    Ray aimRay2 = new Ray(aimRay.origin, direction);
                    for (int i = 0; i < 3; i++)
                    {
                        ProjectileManager.instance.FireProjectile(Modules.Projectiles.stunGrenadeProjectilePrefab, 
                            aimRay2.origin, 
                            Util.QuaternionSafeLookRotation(aimRay2.direction),
                            this.gameObject, 
                            damageMult * this.damageStat * ThrowGrenade.damageCoefficient,
                            this.force,
                            this.RollCrit(), 
                            DamageColorIndex.Default, 
                            null, 
                            -1f);
                        aimRay2.direction = rotation * aimRay2.direction;
                    }
                }
                else
                {
                    ProjectileManager.instance.FireProjectile(Modules.Projectiles.stunGrenadeProjectilePrefab,
                        aimRay.origin,
                        Util.QuaternionSafeLookRotation(aimRay.direction),
                        this.gameObject,
                        this.damageStat * ThrowGrenade.damageCoefficient, 
                        this.force, 
                        this.RollCrit(), 
                        DamageColorIndex.Default, 
                        null, 
                        -1f);
                }
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