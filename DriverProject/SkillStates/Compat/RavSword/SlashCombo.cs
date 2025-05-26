using UnityEngine;
using RoR2;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.RavSword
{
    public class SlashCombo : BaseDriverMeleeAttack
    {
        public static float _damageCoefficient = 1.8f;
        public static float finisherDamageCoefficient = 3.9f;

        protected override string animationString => "Slash" + this.swingIndex;
        protected override string muzzleString => "SwingPointMuzzle" + this.swingIndex;
        public override void OnEnter()
        {
            this.RefreshState();

            this.swingComboCount = 3;
            this.hitboxName = "Hammer";
            this.swingSoundString = DriverPlugin.RavagerInstalled ? "sfx_ravager_swing" : "sfx_driver_swing_knife";

            this.damageCoefficient = _damageCoefficient;
            this.pushForce = 200f;
            this.baseDuration = 1.1f;
            this.earlyExitFraction = 0.5f;

            this.attackStartFraction = 0.2f;
            this.attackEndFraction = 0.3f;

            this.hitStopDuration = 0.08f;
            this.smoothHitstop = true;

            this.swingEffectPrefab = Modules.Assets.ravagerSlashEffect;
            this.hitEffectPrefab = Modules.Assets.redSlashImpactEffect;
            this.impactSound = Modules.Assets.knifeImpactSoundDef.index;

            this.damageType = this.iDrive.DamageType;

            if (this.swingIndex == 2)
            {
                this.duration *= 1.25f;
                this.earlyExitFraction = 0.75f;
                this.hitStopDuration *= 2.5f;
                this.attackStartFraction = 0.22f;
                this.damageType.damageType |= DamageType.Stun1s;
                this.swingSoundString = DriverPlugin.RavagerInstalled ? "sfx_ravager_bigswing" : "sfx_driver_swing_hammer";
                this.impactSound = Modules.Assets.hammerImpactSoundDef.index;
                this.damageCoefficient = finisherDamageCoefficient;
            }

            base.OnEnter();
        }

        protected override void OnHitEnemyAuthority(int amount)
        {
            base.OnHitEnemyAuthority(amount);

            if (this.characterBody.characterMotor.jumpCount > 0)
                this.characterBody.characterMotor.jumpCount--;
        }
    }
}