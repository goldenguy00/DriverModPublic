using UnityEngine;
using RoR2;
using EntityStates;
using static RoR2.CameraTargetParams;
using UnityEngine.Networking;
using RobDriver.SkillStates.BaseStates;
using RobDriver.Modules;
using RoR2.UI;

namespace RobDriver.SkillStates.Driver.SupplyDrop
{
    public class AimSupplyDrop : BaseDriverSkillState
    {
        public static float radius = 6f;

        protected override bool cancelOnPickup => false;
        protected override bool holsterGun => true;
        protected override string showProp => "ButtonModel";
        protected GameObject areaIndicatorInstance { get; set; }

        private CrosshairUtils.OverrideRequest crosshairOverride;
        private CameraParamsOverrideHandle camParamsOverrideHandle;
        private int storedSecondaryStock;
        private float storedSecondaryRechargeStopwatch;

        public override void OnEnter()
        {
            base.OnEnter();

            this.camParamsOverrideHandle = CameraParams.OverrideCameraParams(base.cameraTargetParams, DriverCameraParams.AIM_PISTOL, 0.5f);
            this.crosshairOverride = CrosshairUtils.RequestOverrideForBody(this.characterBody, Modules.Assets.LoadCrosshair("SimpleDot"), CrosshairUtils.OverridePriority.PrioritySkill);

            if (NetworkServer.active)
                this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            this.PlayAnimation();

            if (EntityStates.Huntress.ArrowRain.areaIndicatorPrefab)
            {
                this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.Huntress.ArrowRain.areaIndicatorPrefab);
                this.areaIndicatorInstance.transform.localScale = Vector3.zero;
            }

            this.storedSecondaryStock = this.skillLocator.secondary.stock;
            this.storedSecondaryRechargeStopwatch = this.skillLocator.secondary.rechargeStopwatch;

            this.skillLocator.primary.SetWeaponSkill(Skills.confirmSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            this.skillLocator.secondary.SetWeaponSkill(Skills.cancelSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            Util.PlaySound("sfx_driver_foley", this.gameObject);
        }

        protected virtual void PlayAnimation()
        {
            base.PlayAnimation("Gesture, Override", "ReadyButton", "Action.playbackRate", 0.8f);
            base.PlayAnimation("AimPitch", "ShotgunAimPitch");
        }

        public override void Update()
        {
            base.Update();

            if (this.areaIndicatorInstance)
            {
                float maxDistance = 128f;

                Ray aimRay = base.GetAimRay();
                if (Physics.Raycast(aimRay, out var raycastHit, maxDistance, LayerIndex.CommonMasks.bullet))
                {
                    this.areaIndicatorInstance.transform.position = raycastHit.point;
                    this.areaIndicatorInstance.transform.up = raycastHit.normal;
                }
                else
                {
                    this.areaIndicatorInstance.transform.position = aimRay.GetPoint(maxDistance);
                    this.areaIndicatorInstance.transform.up = -aimRay.direction;
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.characterBody.outOfCombatStopwatch = 0f;
            this.characterBody.isSprinting = false;
            base.characterBody.SetAimTimer(0.2f);

            if (this.areaIndicatorInstance)
            {
                float value = Mathf.Clamp(base.fixedAge, 0f, 0.5f);
                float size = Util.Remap(value, 0f, 0.5f, 0f, AimSupplyDrop.radius);

                this.areaIndicatorInstance.transform.localScale = Vector3.one * size;
            }

            if (base.isAuthority)
            {
                if (this.inputBank.skill1.down)
                {
                    if (base.fixedAge >= 0.5f)
                    {
                        this.outer.SetNextState(this.GetFireState());
                    }
                }
                else if (this.inputBank.skill2.down || this.inputBank.skill4.down || 
                        (this.inputBank.skill3.down && this.skillLocator.utility.defaultSkillDef == Modules.Skills.skateboardSkillDef))
                {
                    if (base.fixedAge >= 0.1f)
                    {
                        this.GetCancelState();
                    }
                }
            }
        }

        protected virtual FireSupplyDrop GetFireState() => new();
        protected virtual CancelSupplyDrop GetCancelState() => new();

        public override void ModifyNextState(EntityState nextState)
        {
            base.ModifyNextState(nextState);

            if (nextState is FireSupplyDrop fireState)
            {
                var indicatorTransform = this.areaIndicatorInstance ? this.areaIndicatorInstance.transform : this.transform;
                fireState.dropPosition = indicatorTransform.position;
                fireState.dropRotation = indicatorTransform.rotation;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.areaIndicatorInstance)
                EntityState.Destroy(this.areaIndicatorInstance);

            if (NetworkServer.active)
                this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);

            base.PlayAnimation("AimPitch", "AimPitch");

            if (this.camParamsOverrideHandle.isValid)
                this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

            if (this.outer.destroying)
                base.PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);

            this.crosshairOverride?.Dispose();

            this.skillLocator.primary.UnsetWeaponSkill(Skills.confirmSkillDef, GenericSkill.SkillOverridePriority.Contextual);
            this.skillLocator.secondary.UnsetWeaponSkill(Skills.cancelSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            this.skillLocator.secondary.stock = this.storedSecondaryStock;
            this.skillLocator.secondary.rechargeStopwatch = this.storedSecondaryRechargeStopwatch;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}