using RoR2;
using UnityEngine;
using EntityStates;
using RoR2.Projectile;
using UnityEngine.AddressableAssets;
using static UnityEngine.ParticleSystem.PlaybackState;
using static UnityEngine.UI.GridLayoutGroup;
using UnityEngine.UIElements;

namespace RobDriver.SkillStates.Driver.SMG
{
    public class PhaseRound : BaseDriverSkillState
    {
        public static float damageCoefficient = 6f;
        public static float procCoefficient = 1f;
        public float baseDuration = 0.9f; // the base skill duration. i.e. attack speed
        public static float recoil = 12f;

        private float earlyExitTime;
        protected float duration;
        protected float fireDuration;
        protected bool hasFired;
        private bool isCrit;
        protected string muzzleString;
        protected virtual float _damageCoefficient => Shoot.damageCoefficient;

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
                this.characterBody.AddSpreadBloom(12f);
                EffectManager.SimpleMuzzleFlash(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/MuzzleflashFMJ.prefab").WaitForCompletion(), this.gameObject, this.muzzleString, false);

                if (base.isAuthority)
                {
                    Ray aimRay = this.GetAimRay();

                    var fireProjectileInfo = new FireProjectileInfo
                    {
                        projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/FMJRamping.prefab").WaitForCompletion(),
                        position = aimRay.origin,
                        rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                        owner = this.gameObject,
                        damage = this.damageStat * this._damageCoefficient,
                        force = 1200f,
                        crit = this.isCrit,
                        damageColorIndex = DamageColorIndex.Default,
                        target = null,
                        speedOverride = 120f,
                        fuseOverride = -1f
                    };
                    var damageType = fireProjectileInfo.projectilePrefab.GetComponent<ProjectileDamage>().damageType;
                    damageType.damageSource = DamageSource.Secondary;
                    fireProjectileInfo.damageTypeOverride = damageType;

                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
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
                this.outer.SetNextStateToMain();
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