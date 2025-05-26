using RoR2;
using RobDriver.SkillStates.BaseStates;
using UnityEngine;
using R2API;
using RobDriver.Modules;
using EntityStates;

namespace RobDriver.SkillStates.Driver
{
    public class SwingKnife : BaseDriverMeleeAttack
    {
        protected override bool cancelOnPickup => false;
        protected override string showProp => "KnifeModel";

        protected override string animationString => "SwingKnife";
        protected override string muzzleString => "SwingMuzzle0";

        public override void OnEnter()
        {
            base.RefreshState();

            this.interruptPriority = InterruptPriority.Pain;
            this.swingComboCount = 1;
            this.hitboxName = "Knife";

            this.damageCoefficient = 4.7f;
            this.pushForce = 200f;
            this.baseDuration = 1.2f;
            this.earlyExitFraction = 0.5f;

            this.attackStartFraction = 0.13f;
            this.attackEndFraction = 0.5f;

            this.hitStopDuration = 0.18f;
            this.smoothHitstop = true;
            this.ammoConsumed = true;

            this.swingSoundString = "sfx_driver_swing_knife";
            this.swingEffectPrefab = Config.enableRevengence.Value ? Modules.Assets.redKnifeSlashEffect : Modules.Assets.knifeSwingEffect;
            this.hitEffectPrefab = Config.enableRevengence.Value ? Modules.Assets.redSlashImpactEffect : Modules.Assets.knifeImpactEffect;
            this.impactSound = Modules.Assets.knifeImpactSoundDef.index;
             
            this.damageType.damageType = DamageType.Stun1s;
            this.damageType.damageSource = DamageSource.Special;
            this.damageType.AddModdedDamageType(DriverDamageTypes.KnifeWound);

            base.OnEnter();

            Util.PlaySound("sfx_driver_foley_knife", this.gameObject);
        }
    }
}