using UnityEngine;
using RoR2;
using EntityStates;
using static RoR2.CameraTargetParams;
using UnityEngine.Networking;

namespace RobDriver.SkillStates.Driver.SupplyDrop
{
    public class AimSupplyDrop : BaseDriverSkillState
    {
        public static float radius = 6f;

        private CameraParamsOverrideHandle camParamsOverrideHandle;
        protected GameObject areaIndicatorInstance { get; set; }
        private int storedSecondaryStock;
        private float storedSecondaryRechargeStopwatch;
        private bool isSkate;

        public override void OnEnter()
        {
            base.OnEnter();
            this.camParamsOverrideHandle = Modules.CameraParams.OverrideCameraParams(base.cameraTargetParams, DriverCameraParams.AIM_PISTOL, 0.5f);

            if (NetworkServer.active) this.characterBody.AddBuff(RoR2Content.Buffs.Slow50);

            //this.characterBody._defaultCrosshairPrefab = Modules.Assets.pistolAimCrosshairPrefab;
            this.characterBody._defaultCrosshairPrefab = Modules.Assets.LoadCrosshair("SimpleDot");

            this.ShowButton();

            if (EntityStates.Huntress.ArrowRain.areaIndicatorPrefab)
            {
                this.areaIndicatorInstance = UnityEngine.Object.Instantiate<GameObject>(EntityStates.Huntress.ArrowRain.areaIndicatorPrefab);
                this.areaIndicatorInstance.transform.localScale = Vector3.zero;
            }

            this.storedSecondaryStock = this.skillLocator.secondary.stock;
            this.storedSecondaryRechargeStopwatch = this.skillLocator.secondary.rechargeStopwatch;

            this.skillLocator.primary.SetSkillOverride(this.skillLocator.primary, Modules.Survivors.Driver.confirmSkillDef, GenericSkill.SkillOverridePriority.Network);
            this.skillLocator.secondary.SetSkillOverride(this.skillLocator.secondary, Modules.Survivors.Driver.cancelSkillDef, GenericSkill.SkillOverridePriority.Network);
            //this.skillLocator.special.SetSkillOverride(this.skillLocator.special, Modules.Survivors.Driver.cancelSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            Util.PlaySound("sfx_driver_foley", this.gameObject);

            this.isSkate = this.skillLocator.utility.defaultSkillDef == Modules.Survivors.Driver.skateboardSkillDef;
        }

        protected virtual void ShowButton()
        {
            base.PlayAnimation("Gesture, Override", "ReadyButton", "Action.playbackRate", 0.8f);
            base.PlayAnimation("AimPitch", "ShotgunAimPitch");
            this.FindModelChild("ButtonModel").gameObject.SetActive(true);
        }

        public override void Update()
        {
            base.Update();
            this.UpdateAreaIndicator();
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
                this.areaIndicatorInstance.transform.localScale = new Vector3(size, size, size);
            }

            if ((this.inputBank.skill1.down) && base.isAuthority)
            {
                if (base.fixedAge >= 0.5f)
                {
                    this.Fire();
                    return;
                }
            }

            if ((this.inputBank.skill2.down || this.inputBank.skill3.down || (this.isSkate && this.inputBank.skill4.down)) && base.isAuthority)
            {
                if (base.fixedAge >= 0.1f)
                {
                    this.Cancel();
                }
            }
        }

        protected virtual void Fire()
        {
            FireSupplyDrop nextState = new FireSupplyDrop();

            Transform indicatorTransform = this.areaIndicatorInstance ? this.areaIndicatorInstance.transform : transform;

            nextState.dropPosition = indicatorTransform.position;
            nextState.dropRotation = indicatorTransform.rotation;

            this.outer.SetNextState(nextState);
        }

        protected virtual void Cancel()
        {
            this.outer.SetNextState(new CancelSupplyDrop());
        }

        private void UpdateAreaIndicator()
        {
            if (this.areaIndicatorInstance)
            {
                float maxDistance = 128f;

                Ray aimRay = base.GetAimRay();
                RaycastHit raycastHit;
                if (Physics.Raycast(aimRay, out raycastHit, maxDistance, LayerIndex.CommonMasks.bullet))
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

        public override void OnExit()
        {
            base.OnExit();

            if (this.areaIndicatorInstance) EntityState.Destroy(this.areaIndicatorInstance.gameObject);

            if (NetworkServer.active) this.characterBody.RemoveBuff(RoR2Content.Buffs.Slow50);

            base.PlayAnimation("AimPitch", "AimPitch");
            this.cameraTargetParams.RemoveParamsOverride(this.camParamsOverrideHandle);

            if (this.outer.destroying)
            {
                base.PlayCrossfade("Gesture, Override", "BufferEmpty", 0.1f);
                this.HideButton();
            }

            this.characterBody._defaultCrosshairPrefab = this.iDrive.crosshairPrefab;

            this.skillLocator.primary.UnsetSkillOverride(this.skillLocator.primary, Modules.Survivors.Driver.confirmSkillDef, GenericSkill.SkillOverridePriority.Network);
            this.skillLocator.secondary.UnsetSkillOverride(this.skillLocator.secondary, Modules.Survivors.Driver.cancelSkillDef, GenericSkill.SkillOverridePriority.Network);
            //this.skillLocator.special.UnsetSkillOverride(this.skillLocator.special, Modules.Survivors.Driver.cancelSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            this.skillLocator.secondary.stock = this.storedSecondaryStock;
            this.skillLocator.secondary.rechargeStopwatch = this.storedSecondaryRechargeStopwatch;
        }

        protected virtual void HideButton()
        {
            this.FindModelChild("ButtonModel").gameObject.SetActive(false);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}