using UnityEngine;
using RoR2;
using EntityStates;
using UnityEngine.AddressableAssets;
using static RoR2.CameraTargetParams;
using RoR2.UI;
using RoR2.HudOverlay;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.Revolver
{
    public class AimLightsOut : BaseDriverSkillState
    {
        public static float baseDuration = 0.6f;

        private float duration;
        private GameObject effectInstance;
        private uint spinPlayID;
        private CameraParamsOverrideHandle camParamsOverrideHandle;
        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private OverlayController overlayController;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = AimLightsOut.baseDuration / this.attackSpeedStat;
            this.camParamsOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, DriverCameraParams.AIM_PISTOL, 0.9f * this.duration);
            this.animator = this.GetModelAnimator();

            this.effectInstance = GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoReloadFX.prefab").WaitForCompletion());
            this.effectInstance.transform.parent = this.FindModelChild("Pistol");
            this.effectInstance.transform.localRotation = Quaternion.Euler(new Vector3(0f, 80f, 0f));
            this.effectInstance.transform.localPosition = Vector3.zero;

            this.spinPlayID = Util.PlaySound("sfx_driver_pistol_spin", this.gameObject);

            base.PlayAnimation("Gesture, Override", "AimLightsOut", "Action.playbackRate", this.duration);

            this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(this.characterBody, 
                Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2CrosshairPrepRevolver.prefab").WaitForCompletion(), 
                CrosshairUtils.OverridePriority.Skill);

            this.overlayController = HudOverlayManager.AddOverlay(this.gameObject, new OverlayCreationParams
            {
                prefab = Modules.Assets.headshotOverlay,
                childLocatorEntry = "ScopeContainer"
            });
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.StartAimMode(0.5f);
            this.animator.SetFloat("aimY", this.inputBank.aimDirection.y);

            if (this.cancelling)
                return;

            if (base.fixedAge >= (0.9f * this.duration))
            {
                if (this.effectInstance)
                {
                    if (this.spinPlayID != 0u) AkSoundEngine.StopPlayingID(this.spinPlayID);
                    EntityState.Destroy(this.effectInstance);

                    Util.PlaySound("sfx_driver_pistol_ready", this.gameObject);
                }
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                if (!this.inputBank.skill2.down)
                {
                    this.SetNextState();
                }
            }
        }

        protected virtual void SetNextState()
        {
            this.outer.SetNextState(new LightsOut());
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.effectInstance)
            {
                if (this.spinPlayID != 0u) AkSoundEngine.StopPlayingID(this.spinPlayID);
                EntityState.Destroy(this.effectInstance);
            }

            if (this.camParamsOverrideHandle.isValid)
                this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

            this.crosshairOverrideRequest?.Dispose();

            if (this.overlayController != null)
            {
                HudOverlayManager.RemoveOverlay(this.overlayController);
                this.overlayController = null;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}