using RobDriver.SkillStates.Driver.SupplyDrop;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver.Scepter.SupplyDrop
{
    public class AimVoidDrop : AimSupplyDrop
    {
        protected override string showProp => "";

        private GameObject effectInstance;

        public override void OnEnter()
        {
            base.OnEnter();

            this.effectInstance = Object.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorChargeMegaBlaster.prefab").WaitForCompletion());
            this.effectInstance.transform.parent = this.FindModelChild("HandL");
            this.effectInstance.transform.localPosition = new Vector3(-0.5f, 0f, -0.2f);
            this.effectInstance.transform.localRotation = Quaternion.identity;
        }

        protected override void PlayAnimation()
        {
            PlayAnimation("Gesture, Override", "ReadyVoidButton", "Action.playbackRate", 0.8f);
            base.PlayAnimation("AimPitch", "SteadyAimPitch");
        }

        protected override CancelSupplyDrop GetCancelState() => new CancelVoidDrop();
        protected override FireSupplyDrop GetFireState() => new FireVoidDrop();

        public override void OnExit()
        {
            base.OnExit();

            base.PlayAnimation("AimPitch", "AimPitch");

            if (this.effectInstance)
                Destroy(this.effectInstance);
        }
    }
}