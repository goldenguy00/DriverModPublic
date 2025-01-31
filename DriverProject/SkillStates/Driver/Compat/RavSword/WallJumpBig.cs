using UnityEngine;
using RoR2;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.Compat
{
    public class WallJumpBig : BaseDriverState
    {
        public float duration = 0.25f;

        public float jumpForce;
        public Vector3 jumpDir;

        private bool isSliding;

        public override void OnEnter()
        {
            base.OnEnter();
            this.GetModelAnimator().SetFloat("leapDir", this.inputBank.aimDirection.y);
            base.PlayAnimation("FullBody, Override Soft", "Leap");
            if(DriverPlugin.RavagerInstalled) Util.PlaySound("sfx_ravager_leap", this.gameObject); 
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority)
            {
                this.characterMotor.Motor.ForceUnground();
                this.characterMotor.velocity = jumpDir * jumpForce;

                if (this.isGrounded && !isSliding && base.fixedAge >= 0.1f)
                {
                    base.PlayAnimation("Body", "Sprint");
                    base.PlayAnimation("FullBody, Override Soft", "Slide");
                    isSliding = true;
                }

                this.characterDirection.moveVector = jumpDir;

                if (base.fixedAge >= duration)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }
    }
}