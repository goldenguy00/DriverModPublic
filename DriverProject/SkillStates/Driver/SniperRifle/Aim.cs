using UnityEngine;
using RoR2;
using EntityStates;
using static RoR2.CameraTargetParams;
using RoR2.HudOverlay;
using UnityEngine.AddressableAssets;
using RobDriver.SkillStates.BaseStates;
using UnityEngine.Networking;

namespace RobDriver.SkillStates.Driver.SniperRifle
{
    public class Aim : BaseDriverSkillState
    {
        private CameraParamsOverrideHandle camParamsOverrideHandle;
        private OverlayController overlayController;

        public override void OnEnter()
        {
            base.OnEnter();

            base.PlayCrossfade("Gesture, Override", "AimTwohand", 0.2f);
            base.PlayCrossfade("AimPitch", "ShotgunAimPitch", 0.1f);

            if (NetworkServer.active)
                this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            this.characterBody.hideCrosshair = true;

            this.camParamsOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, DriverCameraParams.AIM_SNIPER, 0.2f);

            this.overlayController = HudOverlayManager.AddOverlay(this.gameObject, new OverlayCreationParams
            {
                prefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerScopeLightOverlay.prefab").WaitForCompletion(),
                childLocatorEntry = "ScopeContainer"
            });
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.characterBody.outOfCombatStopwatch = 0f;
            this.characterBody.isSprinting = false;
            base.characterBody.SetAimTimer(0.2f);

            if (this.cancelling)
                return;

            if (base.isAuthority)
            {
                if (!this.inputBank.skill2.down)
                {
                    this.outer.SetNextStateToMain();
                }
                else if (this.inputBank.skill1.down)
                {
                    this.outer.SetNextState(this.iDrive.weaponTimer <= 0 ? new Reload() : new Shoot());
                }
            }
        }

        public override void ModifyNextState(EntityState nextState)
        {
            base.ModifyNextState(nextState);

            if (nextState is Shoot shootState)
            {
                shootState.aiming = true;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            this.characterBody.hideCrosshair = false;

            if (NetworkServer.active)
                this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);

            if (this.camParamsOverrideHandle.isValid)
                this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

            if (this.overlayController != null)
            {
                HudOverlayManager.RemoveOverlay(this.overlayController);
                this.overlayController = null;
            }

            if (!this.cancelling)
                base.PlayAnimation("Gesture, Override", "SteadyAimEnd", "Action.playbackRate", 0.2f);

            base.PlayAnimation("AimPitch", "AimPitch");
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}