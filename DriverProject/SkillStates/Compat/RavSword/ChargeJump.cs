using UnityEngine;
using RoR2;
using EntityStates;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.RavSword
{
    public class ChargeJump : BaseDriverState
    {
        public float baseDuration = 0.65f;
        public bool hopoo = false;

        private Vector3 origin;
        private Vector3 jumpDir;
        private float jumpForce;
        private uint playID;
        private Animator animator;
        private EntityStateMachine weaponStateMachine;

        public override void OnEnter()
        {
            base.OnEnter();

            this.weaponStateMachine = EntityStateMachine.FindByCustomName(this.gameObject, "Weapon");
            this.animator = this.GetModelAnimator();
            this.origin = this.transform.position;
            this.iDrive.isWallClinging = true;

            base.PlayAnimation("FullBody, Override", "BufferEmpty");
            PlayCrossfade("Body", this.hopoo ? "JumpChargeHopoo" : "JumpCharge", "Jump.playbackRate", this.baseDuration, 0.1f);

            this.playID = Util.PlaySound(DriverPlugin.RavagerInstalled ? "sfx_ravager_charge_jump" : "HenryBazookaCharge", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.animator)
                this.animator.SetFloat("airBlend", this.isGrounded ? 0f : 1f);

            if (isAuthority)
            {
                this.characterMotor.Motor.SetPosition(this.origin);
                this.characterMotor.velocity = Vector3.zero;

                if (this.inputBank.skill1.down)
                    this.weaponStateMachine.SetInterruptState(new ChargeSlash(), InterruptPriority.PrioritySkill);

                if (fixedAge >= this.baseDuration || !this.inputBank.jump.down)
                {
                    if (fixedAge <= 0.2f)
                    {
                        GenericCharacterMain.ApplyJumpVelocity(characterMotor, characterBody, 1.6f, 1.5f, false);

                        this.outer.SetNextState(new WallJumpSmall());
                    }
                    else
                    {
                        HandleBigJump();

                        this.outer.SetNextState(new WallJumpBig
                        {
                            jumpDir = this.jumpDir,
                            jumpForce = this.jumpForce
                        });
                    }
                }
            }
        }

        private void HandleBigJump()
        {
            this.characterBody.isSprinting = true;

            var recoil = 15f;
            AddRecoil(-1f * recoil, -2f * recoil, -0.5f * recoil, 0.5f * recoil);

            var charge = Mathf.Clamp01(Util.Remap(fixedAge, 0f, baseDuration, 0f, 1f));
            var movespeed = Mathf.Clamp(this.characterBody.moveSpeed, 1f, 18f);

            this.jumpForce = Util.Remap(charge, 0f, 1f, 0.17733990147f, 0.37334975369f) * this.characterBody.jumpPower * movespeed * 0.5f;
            this.jumpDir = this.GetAimRay().direction;
            this.characterMotor.velocity = this.jumpDir * this.jumpForce;

            EffectManager.SpawnEffect(Modules.Assets.bloodSpurtEffect, new EffectData
            {
                origin = this.transform.position + Vector3.up * 0.75f,
                rotation = Util.QuaternionSafeLookRotation(this.GetAimRay().direction),
                scale = 1f
            }, true);

            if (this.hopoo)
            {
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/FeatherEffect"), new EffectData
                {
                    origin = characterBody.footPosition
                }, true);
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            AkSoundEngine.StopPlayingID(playID);
            base.PlayAnimation("Body", "AscendDescend");

            if (this.iDrive)
                this.iDrive.isWallClinging = false;
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}