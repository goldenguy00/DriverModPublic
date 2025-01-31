using UnityEngine;
using RoR2;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.Compat
{
    public class SlashCombo : BaseMeleeAttack
    {
        public static float _damageCoefficient = 1.8f;
        public static float finisherDamageCoefficient = 3.9f;
        private GameObject swingEffectInstance;

        public override void OnEnter()
        {
            this.RefreshState();
            this.hitboxName = "Hammer";

            this.damageCoefficient = _damageCoefficient;
            this.pushForce = 200f;
            this.baseDuration = 1.1f;
            this.baseEarlyExitTime = 0.5f;
            this.attackRecoil = 2f / this.attackSpeedStat;

            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.3f;

            this.hitStopDuration = 0.08f;
            this.smoothHitstop = true;

            if (DriverPlugin.RavagerInstalled) this.swingSoundString = "sfx_ravager_swing";
            else this.swingSoundString = "sfx_driver_swing_knife";
            this.swingEffectPrefab = Modules.Assets.redSwingEffect;
            this.hitSoundString = "";
            this.hitEffectPrefab = Modules.Assets.redSlashImpactEffect;
            this.impactSound = Modules.Assets.knifeImpactSoundDef.index;

            this.damageType = iDrive.DamageType;

            this.moddedDamageTypes = [this.iDrive.currentBulletDef.moddedDamageType];

            if (this.swingIndex == 0) this.muzzleString = "SwingMuzzle1";
            else this.muzzleString = this.muzzleString = "SwingMuzzle2";

            if (this.swingIndex == 2)
            {
                this.muzzleString = "SwingMuzzleLeap";

                this.duration *= 1.25f;
                this.baseEarlyExitTime = 0.75f;
                this.hitStopDuration *= 2.5f;
                this.attackStartTime = 0.22f;
                this.damageType |= DamageType.Stun1s;
                if (DriverPlugin.RavagerInstalled) this.swingSoundString = "sfx_ravager_bigswing";
                else this.swingSoundString = "sfx_driver_swing_hammer";
                this.impactSound = Modules.Assets.hammerImpactSoundDef.index;
                this.damageCoefficient = finisherDamageCoefficient;
            }

            base.OnEnter();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.iDrive && this.iDrive.weaponDef != this.cachedWeaponDef)
            {
                base.PlayAnimation("Gesture, Override", this.iDrive.weaponDef.equipAnimationString);
                this.outer.SetNextStateToMain();
                return;
            }
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
                    swingEffectInstance = Object.Instantiate<GameObject>(this.swingEffectPrefab, muzzleTransform);
                    ScaleParticleSystemDuration fuck = swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                    if (fuck) fuck.newDuration = fuck.initialDuration;
                }
            }
        }

        protected override void TriggerHitStop()
        {
            base.TriggerHitStop();

            if (swingEffectInstance)
            {
                ScaleParticleSystemDuration fuck = swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if (fuck) fuck.newDuration = 20f;
            }
        }

        protected override void ClearHitStop()
        {
            base.ClearHitStop();

            if (swingEffectInstance)
            {
                ScaleParticleSystemDuration fuck = swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if (fuck) fuck.newDuration = fuck.initialDuration;
            }
        }

        protected override void PlayAttackAnimation()
        {
            string animString = "SpinSlash";

            if (this.swingIndex == 0) animString = "Slash1";
            if (this.swingIndex == 1) animString = "Slash2";

            base.PlayAnimation("Gesture, Override", animString, "Slash.playbackRate", this.duration);
        }

        protected override void SetNextState()
        {
            this.FireShuriken();

            int index = (this.swingIndex + 1) % 3;

            this.outer.SetNextState(new SlashCombo
            {
                swingIndex = index
            });
        }
    }
}