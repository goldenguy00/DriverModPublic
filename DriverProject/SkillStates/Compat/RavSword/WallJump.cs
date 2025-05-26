using UnityEngine;
using RoR2;
using System.Linq;
using EntityStates;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.RavSword
{
    public class WallJump : BaseDriverState
    {
        private float airTime;
        private EntityStateMachine bodyStateMachine;

        public override void OnEnter()
        {
            base.OnEnter();

            this.bodyStateMachine = EntityStateMachine.FindByCustomName(this.gameObject, "Body");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.iDrive || this.iDrive.weaponDef != Modules.Weapons.RavSword.instance.weaponDef)
                return;


            if (this.characterBody.characterMotor.jumpCount < this.characterBody.maxJumpCount)
                this.iDrive.clingReady = true;

            if (this.isGrounded)
            {
                this.airTime = 0f;
                this.iDrive.clingReady = true;
            }
            else
            {
                this.airTime += Time.fixedDeltaTime;
                this.iDrive.featherTimer -= Time.fixedDeltaTime;
            }

            if (isAuthority && this.inputBank.jump.justPressed && !this.isGrounded && this.airTime >= 0.15f)
            {
                if (this.iDrive.clingReady)
                {
                    // hopoo feather interaction
                    if (this.iDrive.featherTimer > 0f)
                    {
                        bodyStateMachine.SetInterruptState(new ChargeJump
                        {
                            hopoo = true
                        }, InterruptPriority.Any);
                    }
                    else
                    {
                        this.iDrive.clingReady = false;

                        bodyStateMachine.SetInterruptState(new ChargeJump(), InterruptPriority.Any);
                    }
                }
                else if (this.AttemptEnemyStep())
                {
                    base.PlayAnimation("Body", "JumpEnemy");
                    Util.PlaySound("sfx_ravager_enemystep", this.gameObject);
                    GenericCharacterMain.ApplyJumpVelocity(characterMotor, characterBody, 1.5f, 1.5f, false);

                    if (this.characterBody.characterMotor.jumpCount > 0)
                        this.characterBody.characterMotor.jumpCount--;

                    this.iDrive.clingReady = true;
                    this.airTime = 0f;
                }
            }
        }

        private bool AttemptEnemyStep()
        {
            var s = new SphereSearch()
            {
                origin = this.transform.position,
                radius = 3f,
                mask = LayerIndex.entityPrecise.mask
            }.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(GetTeam()));
            return s.GetHurtBoxes().Any();
        }
    }
}