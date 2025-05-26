using UnityEngine;
using RobDriver.SkillStates.BaseStates;
using RoR2;
using R2API;
using RobDriver.Modules;

namespace RobDriver.SkillStates.Driver.NemmandoSword
{
    public class SwingSword : BaseDriverMeleeAttack
    {
        public static float _damageCoefficient = 1.6f;

        protected override string animationString => "HammerSwing" + this.swingIndex;
        protected override string muzzleString => "SwingPointMuzzle" + this.swingIndex;

        public override void OnEnter()
        {
            RefreshState();

            this.swingComboCount = 2;
            this.hitboxName = "Hammer";
            this.swingSoundString = DriverPlugin.StarstormInstalled ? "NemmandoSwing" : "Play_merc_sword_swing";

            this.damageCoefficient = _damageCoefficient;
            this.pushForce = 0f;
            this.baseDuration = 1.2f;
            this.earlyExitFraction = 0.5f;

            this.attackStartFraction = 0.2f;
            this.attackEndFraction = 0.3f;

            this.hitStopDuration = 0.2f;
            this.smoothHitstop = true;

            this.swingEffectPrefab = Modules.Assets.redKatanaSwing;
            this.hitEffectPrefab = Modules.Assets.redSlashImpactEffect;
            this.impactSound = Modules.Assets.knifeImpactSoundDef.index;

            this.damageType = this.iDrive.DamageType;
            this.damageType.damageType |= DamageType.Stun1s;
            this.damageType.AddModdedDamageType(DriverDamageTypes.Gouge);

            base.OnEnter();
        }
    }
}