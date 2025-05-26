using EntityStates;
using RobDriver.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace RobDriver.SkillStates.Driver.Bazooka
{
    public class Charge : BaseDriverSkillState
    {
        public static float baseChargeDuration = 1f;
        public static float minChargeDuration = 0.08f;
        public static float minBloomRadius = 0.1f;
        public static float maxBloomRadius = 2f;

        private float duration;
        private uint chargePlayID;
        private GameObject chargeEffectInstance;

        protected override bool cancelOnPickup => false;

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = Charge.baseChargeDuration / this.attackSpeedStat;

            this.chargePlayID = Util.PlayAttackSpeedSound("HenryBazookaCharge", this.gameObject, this.attackSpeedStat);

            Transform muzzleTransform = base.FindModelChild("ShotgunMuzzle");
            if (muzzleTransform)
            {
                this.chargeEffectInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.LemurianBruiserMonster.ChargeMegaFireball.chargeEffectPrefab, muzzleTransform.position, muzzleTransform.rotation);
                this.chargeEffectInstance.transform.parent = muzzleTransform;
                this.chargeEffectInstance.transform.localScale *= 0.5f;
                this.chargeEffectInstance.GetComponent<ScaleParticleSystemDuration>().newDuration = this.duration;

                this.chargeEffectInstance.transform.Find("FlameBillboards, Local").gameObject.SetActive(false);
                this.chargeEffectInstance.transform.Find("SmokeBillboard").gameObject.SetActive(false);
            }

            base.PlayAnimation("AimPitch", "ShotgunAimPitch");
            base.PlayCrossfade("Gesture, Override", "AimTwohand", 0.6f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.characterBody.outOfCombatStopwatch = 0f;
            this.characterBody.isSprinting = false;
            base.characterBody.SetAimTimer(0.2f);

            this.iDrive.chargeValue = Mathf.Clamp01(base.fixedAge / this.duration);

            if (this.cancelling)
                return;

            if (base.isAuthority)
            {
                if (base.fixedAge >= this.duration)
                    this.cancelling = true;

                if (base.fixedAge >= Charge.minChargeDuration && !base.IsKeyDownAuthority())
                    this.cancelling = true;

                if (this.cancelling)
                {
                    this.outer.SetNextState(new Fire()
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
            this.outer.SetNextState(new Fire()
            {
                charge = this.iDrive.chargeValue
            });
        }

        public override void OnExit()
        {
            base.OnExit();

            AkSoundEngine.StopPlayingID(this.chargePlayID);

            if (this.chargeEffectInstance) 
                EntityState.Destroy(this.chargeEffectInstance);
            
            if (this.iDrive)
                this.iDrive.chargeValue = 0f;

            if (this.outer.destroying)
            {
                base.PlayAnimation("Gesture, Override", "BufferEmpty");
                base.PlayAnimation("AimPitch", "AimPitch");
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Frozen;
        }
    }
}