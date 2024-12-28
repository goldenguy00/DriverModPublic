using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver.Bazooka
{
    public class Fire : BaseDriverSkillState
    {
        public float charge;

        public static float baseDuration = 0.4f;
        public static float minSpeed = 20f;
        public static float maxSpeed = 160f;
        public static float minDamageCoefficient = 6f;
        public static float maxDamageCoefficient = 12f;
        public static float minRecoil = 0.5f;
        public static float maxRecoil = 25f;

        private float duration;
        private float speed;
        private float damageCoefficient;
        private float recoil;
        private bool hasFired;

        protected virtual GameObject projectilePrefab => Modules.Projectiles.bazookaProjectilePrefab;

        public override void OnEnter()
        {
            base.OnEnter();
            base.StartAimMode(2f);
            this.duration = Fire.baseDuration / this.attackSpeedStat;
            this.speed = Util.Remap(this.charge, 0f, 1f, Fire.minSpeed, Fire.maxSpeed);
            this.damageCoefficient = Util.Remap(this.charge, 0f, 1f, Fire.minDamageCoefficient, Fire.maxDamageCoefficient);
            this.recoil = Util.Remap(this.charge, 0f, 1f, Fire.minRecoil, Fire.maxRecoil);
            this.hasFired = false;

            if (this.iDrive) this.iDrive.ConsumeAmmo();

            if (this.charge >= 0.8f) base.PlayAnimation("Gesture, Override", "FireBazooka", "Shoot.playbackRate", 2.5f * this.duration);
            else base.PlayAnimation("Gesture, Override", "FireTwohand", "Shoot.playbackRate", 2.5f * this.duration);
            base.PlayAnimation("AimPitch", "Shoot");

            this.FireRocket();
        }

        private void FireRocket()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;

                this.characterBody.AddSpreadBloom(4f);
                EffectManager.SimpleMuzzleFlash(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/MuzzleflashSmokeRing.prefab").WaitForCompletion(), this.gameObject, "ShotgunMuzzle", false);

                bool isCrit = this.RollCrit();

                if (isCrit) Util.PlaySound("sfx_driver_bazooka_shoot_critical", this.gameObject);
                else Util.PlaySound("sfx_driver_bazooka_shoot", this.gameObject);

                if (base.isAuthority)
                {
                    base.AddRecoil2(-1f * this.recoil, -2f * this.recoil, -0.5f * this.recoil, 0.5f * this.recoil);

                    Ray aimRay = base.GetAimRay();

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
                            ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                            {
                                projectilePrefab = this.projectilePrefab,
                                position = aimRay2.origin,
                                rotation = Util.QuaternionSafeLookRotation(aimRay2.direction),
                                owner = this.gameObject,
                                damage = damageMult * this.damageStat * this.damageCoefficient,
                                force = 1200f,
                                crit = isCrit,
                                damageColorIndex = DamageColorIndex.Default,
                                target = null,
                                speedOverride = this.speed,
                                useSpeedOverride = true,
                                damageTypeOverride = iDrive.DamageType
                            });

                            aimRay2.direction = rotation * aimRay2.direction;
                        }
                    }
                    else
                    {
                        ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                        {
                            projectilePrefab = this.projectilePrefab,
                            position = aimRay.origin,
                            rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                            owner = this.gameObject,
                            damage = this.damageStat * this.damageCoefficient,
                            force = 1200f,
                            crit = isCrit,
                            damageColorIndex = DamageColorIndex.Default,
                            target = null,
                            speedOverride = this.speed,
                            useSpeedOverride = true,
                            damageTypeOverride = iDrive.DamageType
                        });
                    }
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            this.GetModelAnimator().SetTrigger("endAim");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= (0.5f * this.duration) && this.iDrive && this.iDrive.weaponDef != this.cachedWeaponDef)
            {
                base.PlayAnimation("Gesture, Override", this.iDrive.weaponDef.equipAnimationString);
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}