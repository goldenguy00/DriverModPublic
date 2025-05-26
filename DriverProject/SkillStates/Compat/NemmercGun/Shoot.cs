using EntityStates;

namespace RobDriver.SkillStates.Driver.NemmercGun
{
    public class Shoot : Shoot2
    {
        protected override string animationString => "FireShotgun";

        public override void OnEnter()
        {
            base.OnEnter();

            // easy way to prevent the normal fixed update from running
            base.cancelling = true;
            base.earlyExitFraction = 0.1f;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.characterBody.SetAimTimer(2f);

            if (base.isAuthority && base.fixedAge >= this.earlyExitTime && !this.inputBank.skill1.down)
                this.outer.SetNextState(new Shoot2());
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Skill;
        }
    }
}