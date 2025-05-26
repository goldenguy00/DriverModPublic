using EntityStates;
using RoR2;
using UnityEngine;

namespace RobDriver.SkillStates.BaseStates
{
    public abstract class BaseDriverShootState : BaseDriverSkillState
    {
        protected float procCoefficient = 1f;
        protected uint bulletCount = 1;
        protected int dropShells = 0;
        protected float ammoConsumption = 1f;
        protected bool useAttackSpeed = true;

        protected InterruptPriority interruptPriority = InterruptPriority.Skill;
        protected BulletAttack.FalloffModel bulletFalloff = BulletAttack.FalloffModel.DefaultBullet;
        protected LayerMask hitMask = LayerIndex.CommonMasks.bullet;
        protected LayerMask stopperMask = LayerIndex.CommonMasks.bullet;
        protected DamageColorIndex damageColorIndex = DamageColorIndex.Default;

        protected float bulletRange = 2000f;
        protected float bulletThiccness = 1f;
        protected float bulletForce = 0f;
        protected float selfForce = 0f;

        protected float baseDuration = 1f;
        protected float earlyExitFraction = 1f;
        protected float fireDelayFraction = 0f;

        protected float visualRecoilAmplitude = 4f;
        protected float visualRecoilVertical = 0.8f;
        protected float visualRecoilHorizontal = 0.3f;
        protected float spreadBloom = 4f;
        protected float aimTimer = 4f;

        protected string playbackRateString = "Shoot.playbackRate";
        protected string aimPitchString = "AimPitch";
        protected string muzzleString = "PistolMuzzle";

        // these would normally be private members, but I was raised by apes
        protected float duration;
        protected bool hasFired;
        protected bool isCrit;

        // properties for things that might change at runtime (duration, crit, iDrive, etc)
        protected virtual float damageCoefficient => 1f;
        protected virtual float fireTime => this.fireDelayFraction * this.duration;
        protected virtual float earlyExitTime => this.earlyExitFraction * this.duration;
        protected virtual float animationDuration => this.duration;
        protected virtual float maxBulletSpread => this.characterBody.spreadBloomAngle;

        protected virtual string shootSoundString => "sfx_driver_pistol_shoot";
        protected virtual string animationString => "Shoot";

        protected virtual DamageTypeCombo damageType => this.iDrive.DamageType;
        protected virtual GameObject tracerPrefab => EntityStates.Commando.CommandoWeapon.FirePistol2.tracerEffectPrefab;
        protected virtual GameObject muzzleFlashPrefab => EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab;
        protected virtual GameObject hitEffectPrefab => EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab;

        public override void OnEnter()
        {
            base.OnEnter();
            
            this.characterBody.SetAimTimer(this.aimTimer);

            this.isCrit = base.RollCrit();
            this.duration = this.GetDuration();

            this.PlayEntryAnimation();

            if (this.fireTime == 0f)
                this.FireBullet();
        }

        protected virtual float GetDuration()
        {
            return this.useAttackSpeed
                ? this.baseDuration / this.attackSpeedStat
                : this.baseDuration;
        }

        protected virtual void PlayEntryAnimation()
        {
            base.PlayAnimation("Gesture, Override", this.animationString, this.playbackRateString, this.animationDuration);
            base.PlayAnimation("AimPitch", this.aimPitchString);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.cancelling)
                return;

            if (!this.hasFired && base.fixedAge >= this.fireTime)
                this.FireBullet();

            if (base.isAuthority && base.fixedAge >= this.duration)
                this.outer.SetNextStateToMain();
        }

        protected virtual void FireBullet()
        {
            this.hasFired = true;

            this.iDrive.ConsumeAmmo(this.ammoConsumption, this.useAttackSpeed);

            Util.PlaySound(this.shootSoundString, base.gameObject);

            if (this.dropShells > 0)
                this.DropShells();

            if (this.muzzleFlashPrefab && !string.IsNullOrEmpty(muzzleString))
                EffectManager.SimpleMuzzleFlash(this.muzzleFlashPrefab, this.gameObject, this.muzzleString, false);

            if (base.isAuthority)
                FireBulletAuthority();
        }

        protected virtual void DropShells()
        {
            var angle = -this.GetModelBaseTransform().right;
            for (int i = 0; i < this.dropShells; i++)
            {
                this.iDrive.DropShell(angle * -Random.Range(4, 12));
            }
        }

        protected virtual void FireBulletAuthority()
        {
            Ray aimRay = GetAimRay();
            var bulletAttack = new BulletAttack
            {
                aimVector = aimRay.direction,
                origin = aimRay.origin,
                damage = this.damageCoefficient * this.damageStat,
                damageColorIndex = this.damageColorIndex,
                damageType = this.damageType,
                falloffModel = this.bulletFalloff,
                maxDistance = this.bulletRange,
                force = this.bulletForce,
                hitMask = this.hitMask,
                isCrit = this.isCrit,
                owner = this.gameObject,
                muzzleName = this.muzzleString,
                smartCollision = true,
                procChainMask = default,
                procCoefficient = this.procCoefficient,
                radius = this.bulletThiccness,
                sniper = false,
                stopperMask = this.stopperMask,
                weapon = null,
                tracerEffectPrefab = this.tracerPrefab,
                spreadPitchScale = 1f,
                spreadYawScale = 1f,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                hitEffectPrefab = this.hitEffectPrefab,
                HitEffectNormal = false,
                minSpread = 0f,
                maxSpread = this.maxBulletSpread,
                bulletCount = this.bulletCount
            };
            AuthorityModifyBulletAttack(ref bulletAttack);

            if (bulletAttack.bulletCount <= 1)
            {
                bulletAttack.Fire();
            }
            else
            {
                var bulletSpread = bulletAttack.maxSpread;
                bulletAttack.minSpread = 0;
                bulletAttack.maxSpread = 0;
                bulletAttack.bulletCount = 1;
                bulletAttack.Fire();

                bulletAttack.minSpread = 0;
                bulletAttack.maxSpread = bulletSpread / 1.45f;
                bulletAttack.bulletCount = (uint)Mathf.CeilToInt(this.bulletCount / 2f) - 1;
                bulletAttack.Fire();

                bulletAttack.minSpread = bulletSpread / 1.45f;
                bulletAttack.maxSpread = bulletSpread;
                bulletAttack.bulletCount = (uint)Mathf.FloorToInt(this.bulletCount / 2f);
                bulletAttack.Fire();
            }

            if (this.selfForce != 0f)
                this.characterMotor.ApplyForce(aimRay.direction * -this.selfForce);

            if (this.spreadBloom != 0f)
                this.characterBody.AddSpreadBloom(this.spreadBloom);

            float recoilAmplitude = this.visualRecoilAmplitude / this.attackSpeedStat;
            if (recoilAmplitude != 0f)
                base.AddRecoil(this.visualRecoilVertical * recoilAmplitude, this.visualRecoilHorizontal * recoilAmplitude);
        }

        protected virtual void AuthorityModifyBulletAttack(ref BulletAttack bulletAttack) { }

        public override void OnExit()
        {
            base.OnExit();

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
