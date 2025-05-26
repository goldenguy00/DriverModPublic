using RoR2;
using UnityEngine;
using EntityStates;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.RocketLauncher
{
    public class Barrage : BaseDriverSkillState
    {
        public static float _damageCoefficient = 3f;
        public static float procCoefficient = 1f;
        public float baseShotDuration = 0.05f;
        public static float recoil = 11f;

        protected virtual int baseRocketCount => 7;
        protected virtual float maxSpread => 0f;
        protected virtual GameObject projectilePrefab => Modules.Projectiles.missileProjectilePrefab;
        protected virtual float ammoMod => 1.5f;
        protected virtual float damageCoefficient => Barrage._damageCoefficient;

        private int remainingShots;
        private float shotTimer;
        private float shotDuration;
        protected string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();
            this.characterBody.SetAimTimer(5f);
            this.muzzleString = "ShotgunMuzzle";
            this.shotDuration = this.baseShotDuration / this.attackSpeedStat;
            this.remainingShots = Mathf.Clamp(Mathf.RoundToInt(this.baseRocketCount * this.attackSpeedStat), this.baseRocketCount, 40);

            this.shotTimer = this.shotDuration;
            this.remainingShots--;
            this.Fire();
        }

        public virtual void Fire()
        {
            if (this.iDrive) this.iDrive.ConsumeAmmo(this.ammoMod * (2f / this.baseRocketCount));

            base.PlayAnimation("Gesture, Override", "FireBazooka", "Shoot.playbackRate", 1.4f);
            base.PlayAnimation("AimPitch", "ShotgunAimPitch");

            Util.PlaySound("sfx_driver_rocket_launcher_shoot", base.gameObject);

            float recoilAmplitude = Barrage.recoil / this.attackSpeedStat;

            base.AddRecoil(0.8f * recoilAmplitude,0.3f * recoilAmplitude);
            this.characterBody.AddSpreadBloom(2f);
            EffectManager.SimpleMuzzleFlash(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/MuzzleflashSmokeRing.prefab").WaitForCompletion(), this.gameObject, this.muzzleString, false);

            if (base.isAuthority)
            {
                Ray aimRay = this.GetAimRay();
                aimRay.direction = Util.ApplySpread(aimRay.direction, 0f, this.maxSpread, 1f, 1f, 0f, 0f);

                var isCrit = base.RollCrit();

                var damageType = iDrive.DamageType;
                damageType.damageSource = DamageSource.Secondary;

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
                            speedOverride = 120f,
                            useSpeedOverride = true,
                            damageTypeOverride = damageType
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
                        speedOverride = 120f,
                        useSpeedOverride = true,
                        damageTypeOverride = damageType
                    });
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (cancelling)
                return;

            this.shotTimer -= Time.fixedDeltaTime;

            if (this.shotTimer <= 0f)
            {
                if (this.remainingShots > 0)
                {
                    this.shotTimer = this.shotDuration;
                    this.remainingShots--;
                    this.Fire();
                }
                else
                {
                    if (base.isAuthority)
                    {
                        this.outer.SetNextStateToMain();
                    }
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            this.GetModelAnimator().SetTrigger("endAim");
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}