using EntityStates;
using EntityStates.Captain.Weapon;
using RobDriver.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace RobDriver.SkillStates.Driver.CaptainGun
{
    public class ChargeShotgun : BaseDriverSkillState
    {
        private static float maxChargeDuration = 2f;
        private float chargeDuration;
        private uint enterSoundID;

        private GameObject chargeupVfxGameObject;
        private GameObject holdChargeVfxGameObject;
        private Transform muzzleTransform;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();

            this.chargeDuration = maxChargeDuration / base.attackSpeedStat;

            animator = this.GetModelAnimator();
            animator.SetLayerWeight(animator.GetLayerIndex("AltPistol, Override"), 1f);
            base.PlayAnimation("AimPitch", "SteadyAimPitch");
            base.PlayCrossfade("Gesture, Override", "SteadyAim", "Action.playbackRate", 0.25f, 0.05f);

            this.muzzleTransform = base.childLocator.FindChild("PistolMuzzle");
            this.chargeupVfxGameObject = Object.Instantiate(ChargeCaptainShotgun.chargeupVfxPrefab, this.muzzleTransform);
            this.chargeupVfxGameObject.GetComponent<ScaleParticleSystemDuration>().newDuration = this.chargeDuration;

            this.enterSoundID = Util.PlayAttackSpeedSound(ChargeCaptainShotgun.enterSoundString, base.gameObject, base.attackSpeedStat);
            Util.PlaySound(ChargeCaptainShotgun.playChargeSoundString, base.gameObject);
        }

        public override void Update()
        {
            base.Update();
            base.characterBody.SetSpreadBloom(0.4f + (base.age / this.chargeDuration));
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            base.characterBody.SetAimTimer(1f);

            if (base.fixedAge >= this.chargeDuration)
            {
                if (this.chargeupVfxGameObject)
                {
                    EntityState.Destroy(this.chargeupVfxGameObject);
                    this.chargeupVfxGameObject = null;
                }

                if (!this.holdChargeVfxGameObject)
                {
                    this.holdChargeVfxGameObject = Object.Instantiate(ChargeCaptainShotgun.holdChargeVfxPrefab, this.muzzleTransform);
                }
            }

            if (base.isAuthority && !base.inputBank.skill1.down)
            {
                base.outer.SetNextState(new Shoot());
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.chargeupVfxGameObject)
            {
                EntityState.Destroy(this.chargeupVfxGameObject);
                this.chargeupVfxGameObject = null;
            }

            if (this.holdChargeVfxGameObject)
            {
                EntityState.Destroy(this.holdChargeVfxGameObject);
                this.holdChargeVfxGameObject = null;
            }

            AkSoundEngine.StopPlayingID(this.enterSoundID);
            Util.PlaySound(ChargeCaptainShotgun.stopChargeSoundString, base.gameObject);

            if (this.animator)
            {
                animator.SetLayerWeight(animator.GetLayerIndex("AltPistol, Override"), 0f);
                animator.SetTrigger("endAim");
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}
