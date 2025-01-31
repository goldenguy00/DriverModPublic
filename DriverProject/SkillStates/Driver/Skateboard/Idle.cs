using UnityEngine;
using RoR2;
using EntityStates;

namespace RobDriver.SkillStates.Driver.Skateboard
{
    public class Idle : BaseDriverSkillState
    {
        protected override bool hideGun => true;
        protected override string prop => "SkateboardModel";

        private float skateSpeedMultiplier = 0.8f;
        private Vector3 idealDirection;
        private uint skatePlayID = 0u;
        private bool isSprinting;
        private bool wasSprinting;
        private FootstepHandler footstep;

        public override void OnEnter()
        {
            base.OnEnter();
            this.footstep = this.modelLocator.modelTransform.GetComponent<FootstepHandler>();
            this.footstep.enabled = false;
            this.modelLocator.normalizeToFloor = true;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.footstep) this.footstep.enabled = false;

            if (base.isAuthority)
            {
                // this sucks but it works
                if (this.inputBank.skill1.down || this.inputBank.skill2.down || this.inputBank.skill4.down ||
                    (this.inputBank.skill3.justPressed && this.skillLocator.utility.stock > 0))
                {
                    outer.SetNextState(new Stop());
                    return;
                }

                this.isSprinting = this.inputBank.moveVector != Vector3.zero;

                this.UpdateSkateDirection();

                if (base.characterDirection)
                {
                    base.characterDirection.moveVector = this.idealDirection;
                    if (base.characterMotor && !(base.characterMotor.disableAirControlUntilCollision))
                    {
                        base.characterMotor.rootMotion += this.GetIdealVelocity() * Time.fixedDeltaTime;
                    }
                }
            }

            if (this.iDrive.weaponDef != this.cachedWeaponDef && this.iDrive.weaponEffectInstance)
                this.iDrive.weaponEffectInstance.GetComponent<Modules.Components.BackWeaponComponent>().Init(this.iDrive.weaponDef);

            this.characterBody.isSprinting = this.isSprinting;

            if (!this.wasSprinting && this.isSprinting && this.isGrounded) base.PlayCrossfade("FullBody, Override", "SkateAccelerate", 0.1f);
            this.wasSprinting = this.isSprinting;

            //sound
            if (base.isGrounded)
            {
                if (this.skatePlayID == 0u)
                {
                    Util.PlaySound("sfx_driver_skateboard_land", this.gameObject);
                    this.skatePlayID = Util.PlaySound("sfx_driver_skateboard_roll", this.gameObject);
                }

                AkSoundEngine.SetRTPCValue("Driver_SkateSpeed", Util.Remap(base.characterMotor.velocity.magnitude, 6f, 30f, 0f, 100f));
            }
            else
            {
                if (this.skatePlayID != 0u)
                {
                    base.PlayCrossfade("FullBody, Override", "SkateJump", 0.05f);
                    if (base.characterMotor.velocity.y >= 0.1f) Util.PlaySound("sfx_driver_skateboard_ollie", this.gameObject);
                    AkSoundEngine.StopPlayingID(this.skatePlayID);
                    this.skatePlayID = 0u;
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            this.footstep.enabled = true;
            this.modelLocator.normalizeToFloor = false;
            if (this.skatePlayID != 0u)
            {
                AkSoundEngine.StopPlayingID(this.skatePlayID);
                this.skatePlayID = 0u;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }

        private void UpdateSkateDirection()
        {
            if (base.inputBank)
            {
                Vector2 vector = Util.Vector3XZToVector2XY(base.inputBank.moveVector);
                if (vector != Vector2.zero)
                {
                    vector.Normalize();
                    this.idealDirection = new Vector3(vector.x, 0f, vector.y).normalized;
                }
            }
        }

        private Vector3 GetIdealVelocity()
        {
            return base.characterDirection.forward * base.characterBody.moveSpeed * this.skateSpeedMultiplier;
        }
    }
}