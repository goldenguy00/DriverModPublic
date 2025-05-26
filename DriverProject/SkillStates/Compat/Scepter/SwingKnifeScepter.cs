using EntityStates;
using R2API;
using RobDriver.Modules;
using RobDriver.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace RobDriver.SkillStates.Driver.Scepter
{
    public class SwingKnifeScepter : BaseDriverMeleeAttack
    {
        public static float _damageCoefficient = 4f;

        private const float fixedDuration = 1.2f;
        private const float sheateStartTime = 1.1f;
        private const float durationAdd = 1.3f;

        private bool sheathe;

        protected override bool cancelOnPickup => false;
        protected override string showProp => "KnifeModel";
        protected override bool holsterGun => true;
        protected override string animationString => "HammerSwing" + this.swingIndex;
        protected override string muzzleString => "SwingPointMuzzle" + this.swingIndex;


        public override void OnEnter()
        {
            RefreshState();

            this.swingComboCount = 2;
            this.hitboxName = "Hammer";

            this.interruptPriority = InterruptPriority.Pain;
            this.damageCoefficient = _damageCoefficient + this.characterBody.attackSpeed;
            this.pushForce = 0f;
            this.baseDuration = fixedDuration;
            this.useAttackSpeed = false;

            this.earlyExitFraction = 0.5f;
            this.attackStartFraction = 0.2f;
            this.attackEndFraction = 0.4f;

            this.hitStopDuration = Time.fixedDeltaTime * 2f;
            this.smoothHitstop = true;

            this.swingSoundString = DriverPlugin.StarstormInstalled ? "NemmandoSwing" : "Play_merc_sword_swing";
            this.swingEffectPrefab = Modules.Assets.redKatanaSwing;
            this.hitEffectPrefab = Modules.Assets.redSlashImpactEffect;
            this.impactSound = Modules.Assets.knifeImpactSoundDef.index;

            this.damageType.damageType = DamageType.Stun1s;
            this.damageType.damageSource = DamageSource.Special;
            this.damageType.AddModdedDamageType(DriverDamageTypes.KnifeWound);

            this.childLocator.FindChildGameObject("AltWeaponModel").SetActive(false);

            base.OnEnter();
        }

        protected override void OnHitEnemyAuthority(int amount)
        {
            base.OnHitEnemyAuthority(amount);

            if (this.skillLocator && this.skillLocator.special)
                this.skillLocator.special.Reset();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.cancelling)
                return;

            if (!this.sheathe && base.fixedAge >= sheateStartTime)
            {
                this.sheathe = true;
                base.duration += durationAdd;

                this.PlayCrossfade("Gesture, Override", "Sheathe", 0.05f);
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            this.childLocator.FindChildGameObject("AltWeaponModel").SetActive(true);
        }
    }
}