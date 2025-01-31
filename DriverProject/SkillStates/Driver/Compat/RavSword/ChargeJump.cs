using UnityEngine;
using RoR2;
using EntityStates;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.Compat
{
    public class ChargeJump : BaseDriverState
    {
        public float duration = 0.65f;
        public bool hopoo = false;

        private Vector3 origin;
        protected float jumpTime;
        private bool hasJumped;
        protected Vector3 jumpDir;
        protected float jumpForce;
        private bool isSliding;
        private uint playID;
        private Animator animator;

        public override void OnEnter()
        {
            base.OnEnter();
            origin = this.transform.position;
            base.PlayAnimation("FullBody, Override Soft", "BufferEmpty");
            this.iDrive.isWallClinging = true;
            animator = this.GetModelAnimator();

            PlayAnim();

            if (DriverPlugin.RavagerInstalled) playID = Util.PlaySound("sfx_ravager_charge_jump", this.gameObject);
            else playID = Util.PlaySound("HenryBazookaCharge", this.gameObject);
        }

        protected virtual void PlayAnim()
        {
            if (hopoo) base.PlayCrossfade("Body", "JumpChargeHopoo", "Jump.playbackRate", duration, 0.1f);
            else base.PlayCrossfade("Body", "JumpCharge", "Jump.playbackRate", duration, 0.1f);
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("Body", "AscendDescend");
            this.iDrive.isWallClinging = false;
            AkSoundEngine.StopPlayingID(playID);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (animator)
            {
                if (this.isGrounded) animator.SetFloat("airBlend", 0f);
                else animator.SetFloat("airBlend", 1f);
            }

            if (hasJumped)
            {
                jumpTime -= Time.fixedDeltaTime;

                if (jumpTime <= 0f)
                {
                    this.outer.SetNextStateToMain();
                }
                else this.characterMotor.velocity = jumpDir * jumpForce;

                if (this.isGrounded && !isSliding)
                {
                    base.PlayAnimation("Body", "Sprint");
                    base.PlayAnimation("FullBody, Override Soft", "Slide");
                    isSliding = true;
                }

                this.characterDirection.moveVector = jumpDir;

                return;
            }

            if (base.isAuthority)
            {
                this.characterMotor.Motor.SetPosition(origin);
                this.characterMotor.velocity = Vector3.zero;

                if (this.inputBank.skill1.down)
                {
                    EntityStateMachine.FindByCustomName(this.gameObject, "Weapon").SetInterruptState(new ChargeSlash(), InterruptPriority.Skill);
                }

                if (base.fixedAge >= duration || !this.inputBank.jump.down)
                {

                    if (base.fixedAge <= 0.2f)
                    {
                        GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, 1.6f, 1.5f, false);
                        this.outer.SetNextState(new WallJumpSmall());
                    }
                    else
                    {
                        this.characterBody.isSprinting = true;

                        float recoil = 15f;
                        base.AddRecoil(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);

                        float charge = Mathf.Clamp01(Util.Remap(base.fixedAge, 0f, duration, 0f, 1f));

                        jumpDir = this.GetAimRay().direction;

                        float movespeed = Mathf.Clamp(this.characterBody.moveSpeed, 1f, 18f);

                        jumpForce = Util.Remap(charge, 0f, 1f, 0.17733990147f, 0.37334975369f) * this.characterBody.jumpPower * movespeed * 0.5f;

                        this.characterMotor.velocity = jumpDir * jumpForce;
                        hasJumped = true;
                        SetJumpTime();

                        if (hopoo)
                        {
                            EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/FeatherEffect"), new EffectData
                            {
                                origin = base.characterBody.footPosition
                            }, true);
                        }

                        NextState();
                    }
                }
            }
        }

        protected virtual void SetJumpTime()
        {
            jumpTime = 0.25f;

            EffectData effectData = new EffectData
            {
                origin = this.transform.position + Vector3.up * 0.75f,
                rotation = Util.QuaternionSafeLookRotation(this.GetAimRay().direction),
                scale = 1f
            };

            EffectManager.SpawnEffect(Modules.Assets.bloodSpurtEffect, effectData, true);
        }

        protected virtual void NextState()
        {
            this.outer.SetNextState(new WallJumpBig
            {
                jumpDir = jumpDir,
                jumpForce = jumpForce
            });
        }
    }
}