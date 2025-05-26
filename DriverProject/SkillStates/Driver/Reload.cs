using EntityStates;
using RoR2;
using static RoR2.CameraTargetParams;
using UnityEngine.Networking;
using RobDriver.SkillStates.BaseStates;
using System;
using UnityEngine;

namespace RobDriver.SkillStates.Driver
{
    public class Reload : BaseDriverState
    {
        public static float baseDuration = 1.2f;

        public string animString;
        public CameraParamsOverrideHandle camParamsOverrideHandle;
        public Type steadyAimType;
        public bool aiming;

        private bool wasAiming;
        private float duration;
        private bool heheheha;
        private bool hehehehaahaha;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = baseDuration / this.attackSpeedStat;
            if (this.iDrive.passive.isPistolOnly)
                this.duration *= 0.65f;

            this.wasAiming = this.aiming;

            if (NetworkServer.active && this.aiming)
                this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            Util.PlaySound("sfx_driver_reload_01", this.gameObject);

            this.animator = GetModelAnimator();
            this.animator.SetFloat("aimBlend", 1f);

            this.animString ??= this.cachedWeaponDef.reloadAnimationString;
            if (string.IsNullOrEmpty(this.animString))
                this.animString = "ReloadPistol";

            this.PlayCrossfade("Gesture, Override", this.animString, "Action.playbackRate", this.duration, 0.1f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.aiming)
            {
                this.characterBody.isSprinting = false;
                this.characterBody.SetAimTimer(1f);

                if (base.isAuthority && !this.inputBank.skill2.down)
                {
                    this.aiming = false;
                }
            }

            if (!this.aiming && this.wasAiming)
            {
                if (!this.hehehehaahaha)
                {
                    this.hehehehaahaha = true;
                    this.animator.SetFloat("aimBlend", 0f);

                    if (this.camParamsOverrideHandle.isValid)
                        this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

                    if (NetworkServer.active)
                        this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
                }

                if (base.fixedAge >= (0.8f * this.duration) && !this.heheheha)
                {
                    this.heheheha = true;
                    base.PlayCrossfade("Gesture, Override", "BufferEmpty", 0.25f);
                }
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                if (this.aiming && this.steadyAimType != null)
                {
                    this.outer.SetNextState(EntityStateCatalog.InstantiateState(steadyAimType));
                }
                else
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        public override void ModifyNextState(EntityState nextState)
        {
            base.ModifyNextState(nextState);

            if (nextState is SteadyAim aimState)
            {
                aimState.camParamsOverrideHandle = this.camParamsOverrideHandle;
                aimState.skipAnim = true;
            }
            else
            {
                if (this.camParamsOverrideHandle.isValid)
                    this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

                if (nextState is Skateboard.Start)
                    base.PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.camParamsOverrideHandle.isValid && this.outer.destroying)
                this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

            if (NetworkServer.active && this.aiming)
                this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.iDrive.AmmoPercent > 0) 
                return InterruptPriority.Any;

            return InterruptPriority.PrioritySkill;
        }
    }
}