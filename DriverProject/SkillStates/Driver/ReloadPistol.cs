using EntityStates;
using RoR2;
using static RoR2.CameraTargetParams;
using UnityEngine.Networking;

namespace RobDriver.SkillStates.Driver
{
    public class ReloadPistol : BaseDriverSkillState
    {
        public float baseDuration = 1.2f;
        public string animString = "ReloadPistol";
        public CameraParamsOverrideHandle camParamsOverrideHandle;
        public bool aiming;

        private bool wasAiming;
        private float duration;
        private bool heheheha;

        public override void OnEnter()
        {
            base.OnEnter();
            if (this.iDrive.passive.isPistolOnly) this.baseDuration *= 0.5f;
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.GetModelAnimator().SetFloat("aimBlend", 1f);
            base.PlayCrossfade("Gesture, Override", this.animString, "Action.playbackRate", this.duration * 1.1f, 0.1f);
            Util.PlaySound("sfx_driver_reload_01", this.gameObject);
            this.wasAiming = this.aiming;

            if (NetworkServer.active && this.aiming) this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (base.fixedAge <= this.duration && this.camParamsOverrideHandle.isValid) this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);
            if (this.camParamsOverrideHandle.isValid && !this.aiming) this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);
            if (NetworkServer.active && this.aiming) this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);

            if (this.inputBank.skill3.down && this.skillLocator.utility.skillDef == Modules.Survivors.Driver.skateboardSkillDef)
                base.PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (this.aiming)
            {
                this.characterBody.isSprinting = false;
                this.characterBody.SetAimTimer(1f);
            }

            if (base.isAuthority && this.aiming && !this.inputBank.skill2.down)
            {
                this.aiming = false;
                if (this.camParamsOverrideHandle.isValid) this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);
                this.GetModelAnimator().SetFloat("aimBlend", 0f);
                if (NetworkServer.active) this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            }

            if (!this.aiming && this.wasAiming && base.fixedAge >= (0.8f * this.duration) && !this.heheheha)
            {
                this.heheheha = true;
                base.PlayCrossfade("Gesture, Override", "BufferEmpty", 0.25f);
            }

            // early exit to sync with sound queues
            if (base.fixedAge >= 0.75f * this.duration && this.iDrive.weaponTimer != this.iDrive.maxWeaponTimer)
            {
                this.iDrive.FinishReload();
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                if (this.aiming)
                {
                    this.outer.SetNextState(new SteadyAim
                    {
                        skipAnim = true,
                        camParamsOverrideHandle = this.camParamsOverrideHandle
                    });
                }
                else this.outer.SetNextStateToMain();

                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (iDrive.weaponTimer > 0) return InterruptPriority.Any;
            return InterruptPriority.PrioritySkill;
        }
    }
}