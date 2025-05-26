using EntityStates;
using RobDriver.SkillStates.BaseStates;
using RoR2;
using RoR2.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver.ArtiGauntlet
{
    public class ChargeBomb : BaseDriverSkillState
    {
        public static GameObject chargeEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/ChargeMageLightningBomb.prefab").WaitForCompletion();

        public static float baseChargeDuration = 1.5f;
        public static float minBloomRadius = 0.1f;
        public static float maxBloomRadius = 0.5f;
        public static float minChargeDuration = 0.5f;

        private float duration;
        private uint chargePlayID;
        private GameObject chargeEffectInstance;

        protected override bool cancelOnPickup => false;

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = baseChargeDuration / this.attackSpeedStat;

            this.chargePlayID = Util.PlayAttackSpeedSound("Play_mage_m2_charge", this.gameObject, this.attackSpeedStat);
            PlayAnimation("Gesture, Override", "ChargeHooks", "Hooks.playbackRate", this.duration);

            var muzzleTransform = childLocator.FindChild("PistolMuzzle");

            this.chargeEffectInstance = Object.Instantiate(chargeEffectPrefab, muzzleTransform.position, muzzleTransform.rotation);
            this.chargeEffectInstance.transform.parent = muzzleTransform;

            var scaleParticle = this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>();
            if (scaleParticle)
                scaleParticle.newDuration = this.duration;

            var objScale = this.chargeEffectInstance.GetComponent<ObjectScaleCurve>();
            if (objScale)
                objScale.timeMax = this.duration;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.characterBody.outOfCombatStopwatch = 0f;
            characterBody.SetAimTimer(0.2f);

            this.iDrive.chargeValue = Mathf.Clamp01(fixedAge / this.duration);

            if (this.cancelling)
                return;

            if (isAuthority)
            {
                if (fixedAge >= this.duration)
                    this.cancelling = true;

                if (fixedAge >= minChargeDuration && !IsKeyDownAuthority())
                    this.cancelling = true;

                if (this.cancelling)
                {
                    this.outer.SetNextState(new ThrowBomb()
                    {
                        charge = this.iDrive.chargeValue
                    });
                }
            }
        }

        protected override void OnWeaponChanged(DriverWeaponDef weaponDef)
        {
            base.OnWeaponChanged(weaponDef);

            this.cancelling = true;
            this.outer.SetNextState(new ThrowBomb()
            {
                charge = this.iDrive.chargeValue
            });
        }

        public override void OnExit()
        {
            base.OnExit();

            AkSoundEngine.StopPlayingID(this.chargePlayID);

            if (this.chargeEffectInstance)
                Destroy(this.chargeEffectInstance);

            if (this.iDrive)
                this.iDrive.chargeValue = 0f;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}
