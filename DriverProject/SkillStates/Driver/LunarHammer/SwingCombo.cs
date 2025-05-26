using UnityEngine;
using RobDriver.SkillStates.BaseStates;
using UnityEngine.AddressableAssets;
using RoR2;

namespace RobDriver.SkillStates.Driver.LunarHammer
{
    public class SwingCombo : BaseDriverMeleeAttack
    {
        public static float _damageCoefficient = 32f;

        private static GameObject _swingEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion();
        private static GameObject _hitEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/OmniImpactVFXLoaderLightning.prefab").WaitForCompletion();

        protected override string animationString => "HammerSwing" + this.swingIndex;
        protected override string muzzleString => "SwingPointMuzzle" + this.swingIndex;

        public override void OnEnter()
        {
            RefreshState();

            this.swingComboCount = 2;
            this.hitboxName = "Hammer";

            this.damageCoefficient = _damageCoefficient;
            this.pushForce = 1000f;
            this.baseDuration = 1.8f;
            this.earlyExitFraction = 0.5f;

            this.attackStartFraction = 0.2f;
            this.attackEndFraction = 0.3f;

            this.hitStopDuration = 0.2f;
            this.smoothHitstop = true;

            this.swingSoundString = "sfx_driver_swing_hammer";

            this.swingEffectPrefab = _swingEffectPrefab;
            this.hitEffectPrefab = _hitEffectPrefab;
            this.impactSound = Modules.Assets.hammerImpactSoundDef.index;

            this.damageType = this.iDrive.DamageType;
            this.damageType.damageType |= DamageType.Stun1s;

            base.OnEnter();
        }
    }
}