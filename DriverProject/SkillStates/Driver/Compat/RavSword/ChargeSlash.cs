using RoR2;
using EntityStates;

namespace RobDriver.SkillStates.Driver.Compat
{
    public class ChargeSlash : BaseDriverSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            base.PlayCrossfade("Gesture, Override", "ChargeSlash", "Slash.playbackRate", 0.3f, 0.1f);
            if(DriverPlugin.RavagerInstalled) Util.PlaySound("sfx_ravager_foley_01", this.gameObject);
            else Util.PlaySound("sfx_driver_aim_foley", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                if (!this.inputBank.skill1.down && base.fixedAge >= 0.1f || this.isGrounded && base.fixedAge >= 1.25f)
                {
                    if (!this.iDrive.isWallClinging)
                    {
                        this.outer.SetNextState(new ThrowSlash());
                        return;
                    }
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}