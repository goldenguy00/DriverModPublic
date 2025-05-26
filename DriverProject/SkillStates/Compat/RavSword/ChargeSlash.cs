using RoR2;
using EntityStates;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.RavSword
{
    public class ChargeSlash : BaseDriverSkillState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            PlayCrossfade("Gesture, Override", "ChargeSlash", "Slash.playbackRate", 0.3f, 0.1f);
            if (DriverPlugin.RavagerInstalled) Util.PlaySound("sfx_ravager_foley_01", this.gameObject);
            else Util.PlaySound("sfx_driver_aim_foley", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isAuthority)
            {
                if ((!this.inputBank.skill1.down && fixedAge >= 0.1f) || !this.iDrive.isWallClinging)
                {
                    this.outer.SetNextState(new ThrowSlash());
                }
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}