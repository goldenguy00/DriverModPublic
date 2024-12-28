using UnityEngine;
using RobDriver.SkillStates.BaseStates;
using RoR2;
using R2API;
using RobDriver.Modules;

namespace RobDriver.SkillStates.Driver.Compat.NemmandoSword
{
    public class SwingSword : BaseMeleeAttack
    {
        public static float _damageCoefficient = 1.6f;

        private GameObject swingEffectInstance;

        public override void OnEnter()
        {
            RefreshState();

            this.hitboxName = "Hammer";

            this.damageCoefficient = _damageCoefficient;
            this.pushForce = 0f;
            this.baseDuration = 1.2f;
            this.baseEarlyExitTime = 0.5f;
            this.attackRecoil = 9f / this.attackSpeedStat;

            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.3f;

            this.hitStopDuration = 0.2f;
            this.smoothHitstop = true;

            this.swingSoundString = DriverPlugin.starstormInstalled ? "NemmandoSwing" : "Play_merc_sword_swing";

            this.swingEffectPrefab = Modules.Assets.redMercSwing;
            this.hitSoundString = "";
            this.hitEffectPrefab = Modules.Assets.redSlashImpactEffect;
            this.impactSound = Modules.Assets.knifeImpactSoundDef.index;

            this.damageType = iDrive.DamageType | DamageType.Stun1s;
            this.muzzleString = this.swingIndex == 0 ? "SwingMuzzle1" : "SwingMuzzle2";

            base.OnEnter();

            base.attack.AddModdedDamageType(iDrive.ModdedDamageType);
            base.attack.AddModdedDamageType(DamageTypes.Gouge);
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

        protected override void OnHitEnemyAuthority(int amount)
        {
            base.OnHitEnemyAuthority(amount);
            if (this.iDrive.HasSpecialBullets && !ammoConsumed)
            {
                ammoConsumed = true;
                this.iDrive.ConsumeAmmo(1f, true);
            }
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.iDrive && this.iDrive.weaponDef != this.cachedWeaponDef)
            {
                base.PlayAnimation("Gesture, Override", "BufferEmpty");
                this.outer.SetNextStateToMain();
                return;
            }
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

        protected override void PlayAttackAnimation()
        {
            if (this.swingIndex == 1) base.PlayCrossfade("Gesture, Override", "HammerSwing2", "Swing.playbackRate", this.duration, 0.05f);
            else base.PlayCrossfade("Gesture, Override", "HammerSwing1", "Swing.playbackRate", this.duration, 0.05f);
        }

        protected override void SetNextState()
        {
            int index = (this.swingIndex + 1) % 2;

            this.outer.SetNextState(new SwingSword
            {
                swingIndex = index
            });
        }
    }
}