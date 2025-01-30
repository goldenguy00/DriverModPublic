using UnityEngine;
using RoR2;
using RobDriver.SkillStates.BaseStates;
using R2API;

namespace RobDriver.SkillStates.Driver.Compat
{
    public class ThrowSlash : BaseMeleeAttack
    {
        private GameObject swingEffectInstance;
        private float charge;

        public override void OnEnter()
        {
            this.RefreshState();
            this.hitboxName = "Hammer";

            this.charge = Mathf.Clamp01(Util.Remap(this.characterMotor.velocity.magnitude, 0f, 60f, 0f, 1f));

            this.damageCoefficient = Util.Remap(this.charge, 0f, 1f, 2.3f, 2.3f * 2.5f);
            this.pushForce = 200f;
            this.baseDuration = 0.8f;
            this.baseEarlyExitTime = 0.5f;
            this.attackRecoil = 2f / this.attackSpeedStat;

            this.attackStartTime = 0f;
            this.attackEndTime = 0.3f;

            this.hitStopDuration = 0.08f;
            this.smoothHitstop = true;

            if (DriverPlugin.RavagerInstalled) this.swingSoundString = "sfx_ravager_swing";
            else this.swingSoundString = "sfx_driver_swing_knife";
            this.swingEffectPrefab = Modules.Assets.redSwingEffect;
            this.hitSoundString = "";
            this.hitEffectPrefab = Modules.Assets.redSlashImpactEffect;
            this.impactSound = Modules.Assets.knifeImpactSoundDef.index;

            this.damageType = this.iDrive.DamageType;

            this.muzzleString = "KnifeSwingMuzzle";

            if (this.charge >= 0.45f)
            {
                this.hitStopDuration *= 2.5f;
                this.attackEndTime = 0.7f;
                if (DriverPlugin.RavagerInstalled) this.swingSoundString = "sfx_ravager_bigswing";
                else this.swingSoundString = "sfx_driver_swing_hammer";
                this.impactSound = Modules.Assets.hammerImpactSoundDef.index;
                this.swingEffectPrefab = Modules.Assets.bigRedSwingEffect;
                this.damageType |= DamageType.Stun1s;
            }

            base.OnEnter();
        }

        protected override void OnHitEnemyAuthority(int amount)
        {
            base.OnHitEnemyAuthority(amount);

            this.iDrive.RefreshCling();
            if (this.iDrive.HasSpecialBullets && !ammoConsumed)
            {
                ammoConsumed = true;
                this.iDrive.ConsumeAmmo(1f, true);
            }
        }

        protected override void FireAttack()
        {
            if (base.isAuthority)
            {
                Vector3 direction = this.GetAimRay().direction;
                direction.y = Mathf.Max(direction.y, direction.y * 0.5f);
                this.FindModelChild("MeleePivot").rotation = Util.QuaternionSafeLookRotation(direction);
            }

            base.FireAttack();
        }

        protected override void PlaySwingEffect()
        {
            Util.PlaySound(this.swingSoundString, this.gameObject);
            if (this.swingEffectPrefab)
            {
                Transform muzzleTransform = this.FindModelChild(this.muzzleString);
                if (muzzleTransform)
                {
                    this.swingEffectInstance = UnityEngine.Object.Instantiate<GameObject>(this.swingEffectPrefab, muzzleTransform);
                    ScaleParticleSystemDuration fuck = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                    if (fuck) fuck.newDuration = fuck.initialDuration;
                }
            }
        }

        protected override void TriggerHitStop()
        {
            base.TriggerHitStop();

            if (this.swingEffectInstance)
            {
                ScaleParticleSystemDuration fuck = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if (fuck) fuck.newDuration = 20f;
            }
        }

        protected override void ClearHitStop()
        {
            base.characterMotor.velocity = this.storedVelocity * 0.5f;
            base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
            this.inHitPause = false;

            if (this.swingEffectInstance)
            {
                ScaleParticleSystemDuration fuck = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if (fuck) fuck.newDuration = fuck.initialDuration;
            }
        }

        protected override void PlayAttackAnimation()
        {
            if (this.charge >= 0.45f)
            {
                base.PlayAnimation("Gesture, Override", "BufferEmpty");
                base.PlayAnimation("FullBody, Override", "ThrowSlashMax", "Slash.playbackRate", this.duration * 2f);
                base.PlayAnimation("Gesture, Override", "ThrowSlashMax", "Slash.playbackRate", this.duration * 2f);
            }
            else base.PlayAnimation("Gesture, Override", "ThrowSlash", "Slash.playbackRate", this.duration);
        }

        protected override void SetNextState()
        {
            this.outer.SetNextState(new SlashCombo
            {
                swingIndex = 0
            });
        }
    }
}