using EntityStates;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.SupplyDrop
{
    public class CancelSupplyDrop : BaseDriverSkillState
    {
        public static float baseDuration = 0.25f;

        protected override string showProp => "ButtonModel";
        protected override bool holsterGun => true;

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = baseDuration / this.attackSpeedStat;

            base.PlayCrossfade("Gesture, Override", "BufferEmpty", this.duration / 2f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}