using UnityEngine;
using RoR2;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.RavSword
{
    public class ThrowSlash : BaseDriverMeleeAttack
    {
        private float charge;

        protected override string muzzleString => "SwingPointMuzzle2";

        public override void OnEnter()
        {
            this.RefreshState();

            this.swingComboCount = 1;
            this.hitboxName = "Hammer";

            this.charge = Mathf.Clamp01(Util.Remap(this.characterMotor.velocity.magnitude, 0f, 60f, 0f, 1f));

            this.damageCoefficient = Util.Remap(this.charge, 0f, 1f, 2.3f, 2.3f * 2.5f);
            this.pushForce = 200f;
            this.baseDuration = 0.8f;
            this.earlyExitFraction = 0.5f;

            this.attackStartFraction = 0f;
            this.attackEndFraction = 0.3f;

            this.hitStopDuration = 0.08f;
            this.smoothHitstop = true;

            this.swingSoundString = DriverPlugin.RavagerInstalled ? "sfx_ravager_swing" : "sfx_driver_swing_knife";
            this.swingEffectPrefab = Modules.Assets.ravagerSlashEffect;
            this.hitEffectPrefab = Modules.Assets.redSlashImpactEffect;
            this.impactSound = Modules.Assets.knifeImpactSoundDef.index;

            this.damageType = this.iDrive.DamageType;

            if (this.charge >= 0.45f)
            {
                this.hitStopDuration *= 2.5f;
                this.attackEndFraction = 0.7f;
                this.swingSoundString = DriverPlugin.RavagerInstalled ? "sfx_ravager_bigswing" : "sfx_driver_swing_hammer";
                this.impactSound = Modules.Assets.hammerImpactSoundDef.index;
                this.swingEffectPrefab = Modules.Assets.ravagerBigSlashEffect;
                this.damageType |= DamageType.Stun1s;
            }

            base.OnEnter();
        }

        protected override void OnHitEnemyAuthority(int amount)
        {
            base.OnHitEnemyAuthority(amount);

            if (this.characterBody.characterMotor.jumpCount > 0)
                this.characterBody.characterMotor.jumpCount--;
        }

        protected override void ClearHitStop()
        {
            base.ClearHitStop();

            if (this.characterMotor)
                this.characterMotor.velocity = this.storedVelocity * 0.5f;
        }

        protected override void PlayAttackAnimation()
        {
            if (this.charge >= 0.45f)
            {
                PlayAnimation("FullBody, Override", "ThrowSlashMax", "Slash.playbackRate", this.duration * 2f);
                PlayAnimation("Gesture, Override", "ThrowSlashMax", "Slash.playbackRate", this.duration * 2f);
            }
            else 
                PlayAnimation("Gesture, Override", "ThrowSlash", "Slash.playbackRate", this.duration * 2f);
        }
    }
}