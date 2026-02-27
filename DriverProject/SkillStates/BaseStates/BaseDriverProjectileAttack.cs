using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.BaseStates
{
    public abstract class BaseDriverProjectileAttack : BaseDriverSkillState
    {
        protected static readonly GameObject _muzzleFlashPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/MuzzleflashSmokeRing.prefab").WaitForCompletion();

        protected float damageCoefficient = 1f;
        protected float ammoConsumption = 1f;
        protected bool useAttackSpeed = true;
        protected bool useICBM = true;

        protected InterruptPriority interruptPriority = InterruptPriority.Skill;
        protected DamageColorIndex damageColorIndex = DamageColorIndex.Default;
        protected GameObject target;
        protected float maxDistance = -1f;
        protected float fuseOverride = -1f;
        protected float speedOverride = -1f;
        protected float force = 0f;
        protected float selfForce = 0f;

        protected float baseDuration = 1f;
        protected float earlyExitFraction = 1f;
        protected float fireDelayFraction = 0f;

        protected float visualRecoilAmplitude = 4f;
        protected float visualRecoilVertical = 0.8f;
        protected float visualRecoilHorizontal = 0.3f;
        protected float arcPitch = 4f;
        protected float spreadBloom = 4f;
        protected float aimTimer = 4f;

        protected string playbackRateString = "Shoot.playbackRate";
        protected string aimPitchString = "ShotgunAimPitch";
        protected string muzzleString = "ShotgunMuzzle";

        // these would normally be private members, but i was raised by apes
        protected float duration;
        protected bool hasFired;
        protected bool isCrit;

        // properties for things that might change at runtime (duration, crit, iDrive, etc)
        protected virtual float fireTimer => this.fireDelayFraction * this.duration;
        protected virtual float earlyExitTime => this.earlyExitFraction * this.duration;
        protected virtual float animationDuration => this.duration;
        protected virtual string animationString => "";
        protected virtual string shootSound => "";
        protected virtual DamageTypeCombo? damageType => this.iDrive.DamageType;
        protected virtual GameObject projectilePrefab => null;
        protected virtual GameObject muzzleFlashPrefab => _muzzleFlashPrefab;

        public override void OnEnter()
        {
            base.OnEnter();

            this.characterBody.SetAimTimer(this.aimTimer);

            this.isCrit = base.RollCrit();
            this.duration = this.baseDuration;

            if (this.useAttackSpeed)
            {
                this.duration /= this.attackSpeedStat;
                this.visualRecoilAmplitude /= this.attackSpeedStat;
            }

            this.PlayAnimation();

            if (this.fireTimer == 0f)
                FireProjectile();
        }

        protected virtual void PlayAnimation()
        {
            base.PlayAnimation("Gesture, Override", this.animationString, this.playbackRateString, this.animationDuration);
            base.PlayAnimation("AimPitch", this.aimPitchString);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.cancelling)
                return;

            if (!this.hasFired && base.fixedAge >= this.fireTimer)
                this.FireProjectile();

            if (base.isAuthority && base.fixedAge >= this.duration)
                this.outer.SetNextStateToMain();
        }

        public virtual void FireProjectile()
        {
            this.hasFired = true;

            this.iDrive.ConsumeAmmo(this.ammoConsumption, this.useAttackSpeed);

            if (!string.IsNullOrEmpty(this.shootSound))
                Util.PlaySound(this.shootSound, base.gameObject);

            if (this.visualRecoilAmplitude > 0f)
                base.AddRecoil(this.visualRecoilVertical * this.visualRecoilAmplitude, this.visualRecoilHorizontal * this.visualRecoilAmplitude);

            if (!string.IsNullOrEmpty(muzzleString) && this.muzzleFlashPrefab)
                EffectManager.SimpleMuzzleFlash(this.muzzleFlashPrefab, this.gameObject, this.muzzleString, false);

            if (base.isAuthority)
                FireProjectileAuthority();
        }

        public virtual void FireProjectileAuthority()
        {
            Ray aimRay = this.GetAimRay();

            if (this.arcPitch != 0f)
                aimRay.direction = Util.ApplySpread(aimRay.direction, 0f, 0f, 1f, 1f, 0f, -Mathf.Abs(this.arcPitch));

            int icbmCount = 0;
            if (this.useICBM)
                icbmCount = this.characterBody.inventory ? this.characterBody.inventory.GetItemCountEffective(DLC1Content.Items.MoreMissile) : 0;

            float damageMult = Mathf.Max(1f, 1f + (0.5f * (icbmCount - 1)));

            var projectileInfo = new FireProjectileInfo
            {
                projectilePrefab = this.projectilePrefab,
                position = aimRay.origin,
                rotation = Util.QuaternionSafeLookRotation(aimRay.direction),
                owner = this.gameObject,
                damage = this.damageStat * this.damageCoefficient * damageMult,
                force = this.force,
                crit = this.isCrit,
                damageColorIndex = this.damageColorIndex,
                target = this.target,
                speedOverride = this.speedOverride,
                fuseOverride = this.fuseOverride,
                damageTypeOverride = this.damageType,
                maxDistance = this.maxDistance,
                procChainMask = default
            };
            AuthorityModifyProjectileInfo(ref projectileInfo);

            ProjectileManager.instance.FireProjectile(projectileInfo);

            if (icbmCount > 0)
            {
                projectileInfo.rotation = Util.QuaternionSafeLookRotation(Quaternion.AngleAxis(15f, Vector3.up) * aimRay.direction);
                ProjectileManager.instance.FireProjectile(projectileInfo);

                projectileInfo.rotation = Util.QuaternionSafeLookRotation(Quaternion.AngleAxis(-15f, Vector3.up) * aimRay.direction);
                ProjectileManager.instance.FireProjectile(projectileInfo);
            }

            if (this.selfForce != 0f)
                this.characterMotor.ApplyForce(aimRay.direction * -Mathf.Abs(this.selfForce));

            if (this.spreadBloom > 0f)
                this.characterBody.AddSpreadBloom(this.spreadBloom);
        }

        protected virtual void AuthorityModifyProjectileInfo(ref FireProjectileInfo fireProjectileInfo) { }

        public override void OnExit()
        {
            base.OnExit();

            if (!this.hasFired)
                this.FireProjectile();

            this.GetModelAnimator().SetTrigger("endAim");
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= this.earlyExitTime)
                return InterruptPriority.Any;

            return this.interruptPriority;
        }
    }
}
