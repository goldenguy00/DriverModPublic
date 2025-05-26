using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.RavSword
{
    public class WallJumpSmall : BaseDriverState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            base.PlayAnimation("Body", "Jump");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.outer.SetNextStateToMain();
        }
    }
}