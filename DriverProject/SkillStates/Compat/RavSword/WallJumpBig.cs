using UnityEngine;
using RoR2;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.RavSword
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
            base.PlayAnimation("FullBody, Override", "Leap");
            if (DriverPlugin.RavagerInstalled) Util.PlaySound("sfx_ravager_leap", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (isAuthority)
            {
                this.characterMotor.Motor.ForceUnground();
                this.characterMotor.velocity = jumpDir * jumpForce;

                if (this.isGrounded && !isSliding && fixedAge >= 0.1f)
                {
                    PlayAnimation("Body", "Sprint");
                    PlayAnimation("Gesture, Override", "HoldSword");
                    PlayCrossfade("FullBody, Override", "Slide", 0.1f);
                    this.GetModelAnimator().SetBool("holding", true);
                    isSliding = true;
                }

                this.characterDirection.moveVector = jumpDir;

                if (fixedAge >= duration)
                    this.outer.SetNextStateToMain();
            }
        }
    }
}