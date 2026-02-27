using RoR2;
using EntityStates;
using RobDriver.SkillStates.BaseStates;
using RobDriver.Modules;
using UnityEngine;

namespace RobDriver.SkillStates.Driver.Skateboard
{
    public class StartGrind : BaseDriverSkillState
    {
        protected override bool cancelOnPickup => false;
        protected override bool holsterGun => true;
        protected override string showProp => "SkateboardModel";

        private Vector3 prevPos;

        public override void OnEnter()
        {
            base.OnEnter();

            this.childLocator.FindChildGameObject("SkateboardBackModel").SetActive(false);

            base.PlayAnimation("Gesture, Override", "BufferEmpty");

            Util.PlaySound("sfx_driver_foley_syringe", this.gameObject);
            base.PlayCrossfade("FullBody, Override", "StartGrind", 0.1f);

            prevPos = transform.position;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            var currentPos = this.transform.position;
            if (currentPos != prevPos)
            {
                characterDirection.moveVector = (currentPos - prevPos).normalized;
                prevPos = currentPos;
            }
        }

        public override void ModifyNextState(EntityState nextState)
        {
            base.ModifyNextState(nextState);

            if (nextState is Idle)
                PlayCrossfade("FullBody, Override", "StartSkate", 0.25f);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}