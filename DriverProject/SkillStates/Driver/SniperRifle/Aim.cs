using UnityEngine;
using RoR2;
using EntityStates;
using static RoR2.CameraTargetParams;
using RoR2.HudOverlay;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver.SniperRifle
{
    public class Aim : BaseDriverSkillState
    {
        private CameraParamsOverrideHandle camParamsOverrideHandle;
        private bool cancelling;

        private OverlayController overlayController;

        public override void OnEnter()
        {
            base.OnEnter();
            this.camParamsOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, DriverCameraParams.AIM_SNIPER, 0.2f);

            base.PlayCrossfade("Gesture, Override", "AimTwohand", 0.2f);
            base.PlayCrossfade("AimPitch", "ShotgunAimPitch", 0.1f);

            this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            this.characterBody.hideCrosshair = true;

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

            if (this.iDrive && this.iDrive.weaponDef != this.cachedWeaponDef)
            {
                this.cancelling = true;
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.isAuthority)
            {
                if (this.inputBank.skill1.down)
                {
                    PrimarySkillShurikenBehavior shurikenComponent = this.GetComponent<PrimarySkillShurikenBehavior>();
                    if (shurikenComponent) shurikenComponent.OnSkillActivated(this.skillLocator.primary);

                    if (this.iDrive.weaponTimer <= 0)
                    {
                        cancelling = true;
                        this.outer.SetNextState(new ReloadPistol());
                    }
                    else
                    {
                        this.outer.SetNextState(new Shoot
                        {
                            aiming = true
                        });
                    }
                    return;
                }
            }

            if (!this.inputBank.skill2.down && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);
            base.PlayAnimation("Gesture, Override", "SteadyAimEnd", "Action.playbackRate", 0.2f);
            base.PlayAnimation("AimPitch", "AimPitch");
            this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

            this.characterBody.hideCrosshair = false;

            if (this.cancelling)
            {
                base.PlayAnimation("Gesture, Override", this.iDrive.weaponDef.equipAnimationString);
            }

            if (this.overlayController != null)
            {
                HudOverlayManager.RemoveOverlay(this.overlayController);
                this.overlayController = null;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}