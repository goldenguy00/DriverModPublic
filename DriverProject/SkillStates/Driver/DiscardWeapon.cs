using EntityStates;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver
{
    public class DiscardWeapon : BaseDriverSkillState
    {
        private float duration = 0.5f;

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.fixedAge >= this.duration)
                this.outer.SetNextStateToMain();
        }
        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
