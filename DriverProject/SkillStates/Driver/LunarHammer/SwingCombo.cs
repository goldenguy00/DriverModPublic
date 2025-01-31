using UnityEngine;
using RobDriver.SkillStates.BaseStates;
using UnityEngine.AddressableAssets;
using RoR2;

namespace RobDriver.SkillStates.Driver.LunarHammer
{
    public class SwingCombo : BaseMeleeAttack
    {
        public static float _damageCoefficient = 32.1f;

        private GameObject swingEffectInstance;

        public override void OnEnter()
        {
            RefreshState();
            this.hitboxName = "Hammer";

            this.damageCoefficient = _damageCoefficient;
            this.pushForce = 1000f;
            this.baseDuration = 1.8f;
            this.baseEarlyExitTime = 0.5f;
            this.attackRecoil = 9f / this.attackSpeedStat;

            this.attackStartTime = 0.2f;
            this.attackEndTime = 0.3f;

            this.hitStopDuration = 0.2f;
            this.smoothHitstop = true;

            this.swingSoundString = "sfx_driver_swing_hammer";

            this.swingEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion();
            this.hitSoundString = "";
            this.hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/OmniImpactVFXLoaderLightning.prefab").WaitForCompletion();
            this.impactSound = Modules.Assets.hammerImpactSoundDef.index;

            this.damageType = DamageType.Stun1s | this.iDrive.DamageType;
            this.moddedDamageTypes = [this.iDrive.currentBulletDef.moddedDamageType];

            if (this.swingIndex == 0) this.muzzleString = "SwingCenter";
            else this.muzzleString = this.muzzleString ="SwingCenter2";

            base.OnEnter();
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
            if (this.swingIndex == 1) base.PlayCrossfade("Gesture, Override", "HammerSwing2", "Swing.playbackRate", this.duration, 0.1f);
            else base.PlayCrossfade("Gesture, Override", "HammerSwing1", "Swing.playbackRate", this.duration, 0.1f);
        }

        protected override void SetNextState()
        {
            int index = (this.swingIndex + 1) % 2;

            this.outer.SetNextState(new SwingCombo
            {
                swingIndex = index
            });
        }
    }
}