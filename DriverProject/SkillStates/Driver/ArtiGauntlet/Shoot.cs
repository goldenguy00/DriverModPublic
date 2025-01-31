using RoR2;
using UnityEngine;
using EntityStates;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver.ArtiGauntlet
{
    public class Shoot : BaseDriverSkillState
    {
        public static float damageCoefficient = 6f;
        public static float procCoefficient = 1f;
        public float baseDuration = 0.9f; // the base skill duration. i.e. attack speed
        public static float recoil = 16f;

        private float earlyExitTime;
        protected float duration;
        protected float fireDuration;
        protected bool hasFired;
        private bool isCrit;
        protected string muzzleString;

        protected virtual float _damageCoefficient => Shoot.damageCoefficient;
        protected virtual GameObject projectilePrefab => Modules.Projectiles.artiGauntletPrefab;

        public override void OnEnter()
        {
            base.OnEnter();
            this.characterBody.SetAimTimer(5f);
            this.muzzleString = "PistolMuzzle";
            this.hasFired = false;
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.isCrit = base.RollCrit();
            this.earlyExitTime = 0.4f * this.duration;

            if (this.isCrit) Util.PlaySound("sfx_driver_fire_preon", base.gameObject);
            else Util.PlaySound("sfx_driver_fire_preon", base.gameObject);

            base.PlayAnimation("Gesture, Override", "Shoot", "Shoot.playbackRate", this.duration);

            this.fireDuration = 0;

            if (this.iDrive) this.iDrive.ConsumeAmmo();
        }

        public virtual void Fire()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;

                float recoilAmplitude = Shoot.recoil / this.attackSpeedStat;

                base.AddRecoil2(-0.4f * recoilAmplitude, -0.8f * recoilAmplitude, -0.3f * recoilAmplitude, 0.3f * recoilAmplitude);
                this.characterBody.AddSpreadBloom(4f);
                EffectManager.SimpleMuzzleFlash(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/MuzzleflashSmokeRing.prefab").WaitForCompletion(), this.gameObject, this.muzzleString, false);

                if (base.isAuthority)
                {
                    Ray aimRay = this.GetAimRay();
                    ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                    {
                        projectilePrefab = this.projectilePrefab,
                        position = aimRay.origin,
                        rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                        owner = this.gameObject,
                        damage = this.damageStat * this._damageCoefficient,
                        force = 1200f,
                        crit = this.isCrit,
                        damageColorIndex = DamageColorIndex.Default,
                        target = null,
                        speedOverride = 120f,
                        useSpeedOverride = true,
                        damageTypeOverride = iDrive.DamageType
                    });
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.fireDuration)
            {
                this.Fire();
            }

            if (this.iDrive && this.iDrive.weaponDef != this.cachedWeaponDef)
            {
                base.PlayAnimation("Gesture, Override", this.iDrive.weaponDef.equipAnimationString);
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextState(new WaitForReload());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= this.earlyExitTime) return InterruptPriority.Any;
            return InterruptPriority.Skill;
        }
    }
}