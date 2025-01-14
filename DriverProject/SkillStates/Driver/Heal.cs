using UnityEngine;
using RoR2;
using UnityEngine.Networking;

namespace RobDriver.SkillStates.Driver
{
    public class Heal : BaseDriverSkillState
    {
        public float startupDelay = 0.65f;
        public float healPercentPerTick = 0.01f;
        public float tickStopwatch = 0.25f;

        protected override string prop => "MedkitModel";
        protected override bool hideGun => true;

        private float stopwatch;
        private bool cancelling;

        public override void OnEnter()
        {
            base.OnEnter();
            base.PlayCrossfade("FullBody, Override", "Heal", "Action.playbackRate", this.startupDelay, 0.1f);
        }

        public override void OnExit()
        {
            base.OnExit();
            base.PlayAnimation("FullBody, Override", "BufferEmpty");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.attackSpeedStat = this.characterBody.attackSpeed;

            if (base.fixedAge >= (this.startupDelay / this.attackSpeedStat))
            {
                this.HandleHeal();
            }

            if (base.isAuthority)
            {
                if (!this.isGrounded) this.cancelling = true;

                if (this.healthComponent.health >= this.healthComponent.fullHealth || (this.inputBank.skill4.justPressed && base.fixedAge >= 0.1f))
                {
                    this.cancelling = true;
                }

                if (this.inputBank.moveVector != Vector3.zero && base.fixedAge >= 0.1f) this.cancelling = true;

                if (this.cancelling)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        protected virtual void HandleHeal()
        {
            this.stopwatch += Time.fixedDeltaTime;

            if (this.stopwatch >= (this.tickStopwatch / this.attackSpeedStat))
            {
                this.TryHeal();
            }
        }

        protected virtual void TryHeal()
        {
            this.stopwatch = 0f;

            if (NetworkServer.active)
            {
                this.healthComponent.Heal(this.healthComponent.fullHealth * this.healPercentPerTick, default(ProcChainMask));
            }
        }
    }
}