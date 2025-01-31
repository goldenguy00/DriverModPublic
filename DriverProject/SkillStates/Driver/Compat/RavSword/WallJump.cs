using UnityEngine;
using RoR2;
using System.Linq;
using EntityStates;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.Compat
{
    public class WallJump : BaseDriverState
    {
        private float airTime;

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.isGrounded)
            {
                this.airTime = 0f;
                this.iDrive.clingReady = true;
            }
            else this.airTime += Time.fixedDeltaTime;

            if (this.inputBank.jump.justPressed && !this.isGrounded && base.isAuthority)
            {
                if (this.airTime >= 0.15f)
                {
                    if (this.iDrive.clingReady)
                    {
                        // hopoo feather interaction
                        if (this.iDrive.featherTimer > 0f)
                        {
                            EntityStateMachine.FindByCustomName(this.gameObject, "Body").SetInterruptState(new ChargeJump
                            {
                                hopoo = true
                            }, InterruptPriority.Any);
                            return;
                        }
                        this.iDrive.clingReady = false;
                        
                        EntityStateMachine.FindByCustomName(this.gameObject, "Body").SetInterruptState(new ChargeJump(), InterruptPriority.Any);
                        
                        return;
                    }
                    if (this.AttemptEnemyStep())
                    {
                        base.PlayAnimation("Body", "JumpEnemy");
                        Util.PlaySound("sfx_ravager_enemystep", this.gameObject);
                        GenericCharacterMain.ApplyJumpVelocity(base.characterMotor, base.characterBody, 1.5f, 1.5f, false);
                        this.iDrive.RefreshCling();
                        this.iDrive.clingReady = true;
                        this.airTime = 0f;
                        return;
                    }
                }
            }
        }

        private bool AttemptEnemyStep()
        {
            SphereSearch s = new SphereSearch()
            {
                origin = this.transform.position,
                radius = 3f,
                mask = LayerIndex.entityPrecise.mask
            }.RefreshCandidates().FilterCandidatesByHurtBoxTeam(TeamMask.GetEnemyTeams(base.GetTeam()));
            return s.GetHurtBoxes().Any();
        }
    }
}