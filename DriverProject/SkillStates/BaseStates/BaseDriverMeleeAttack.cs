using EntityStates;
using RoR2;
using RoR2.Audio;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RobDriver.SkillStates.BaseStates
{
    public abstract class BaseDriverMeleeAttack : BaseDriverSkillState
    {
        public float duration;
        public int swingIndex;
        public int swingComboCount = 1;

        protected InterruptPriority interruptPriority = InterruptPriority.PrioritySkill;
        protected DamageTypeCombo damageType = DamageTypeCombo.Generic;
        protected Vector3 bonusForce = Vector3.zero;
        protected float damageCoefficient = 3.5f;
        protected float procCoefficient = 1f;
        protected float pushForce = 300f;
        protected float baseDuration = 1f;
        protected float attackStartFraction = 0.2f;
        protected float attackEndFraction = 0.4f;
        protected float earlyExitFraction = 0.5f;
        protected float hitStopDuration = 0.08f;
        protected float hitHopVelocity = 4f;

        protected string hitboxName = "Knife";
        protected string swingSoundString = "";
        protected string hitSoundString = "";
        protected GameObject swingEffectPrefab;
        protected GameObject hitEffectPrefab;
        protected GameObject swingEffectInstance;
        protected NetworkSoundEventIndex impactSound;

        protected OverlapAttack attack;
        protected List<HurtBox> hitResults = [];
        protected bool hasFired;
        protected bool hasHopped;
        protected bool smoothHitstop;
        protected bool useAttackSpeed = true;
        protected bool meleePivot = true;
        protected bool ammoConsumed;

        // hit pause
        protected float hitPauseTimer;
        protected bool inHitPause;
        protected float stopwatch;
        protected Animator animator;
        protected BaseState.HitStopCachedState hitStopCachedState;
        protected Vector3 storedVelocity;

        protected virtual string animationString => "Slash" + this.swingIndex;
        protected virtual string muzzleString => "SwingMuzzle" + this.swingIndex;

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = this.baseDuration;
            if (this.useAttackSpeed)
                this.duration /= this.attackSpeedStat;

            this.hasFired = false;
            this.animator = base.GetModelAnimator();
            base.characterBody.outOfCombatStopwatch = 0f;

            base.StartAimMode(0.5f + this.duration, false);

            this.PlayAttackAnimation();

            this.InitializeAttack();
        }

        protected virtual void PlayAttackAnimation()
        {
            base.PlayCrossfade("Gesture, Override", this.animationString, "Slash.playbackRate", this.duration, 0.05f);
        }

        protected virtual void InitializeAttack()
        {
            HitBoxGroup hitBoxGroup = null;
            Transform modelTransform = base.GetModelTransform();

            if (modelTransform)
            {
                hitBoxGroup = System.Array.Find(modelTransform.GetComponents<HitBoxGroup>(), (HitBoxGroup element) => element.groupName == this.hitboxName);
            }

            this.attack = new OverlapAttack
            {
                damageType = this.damageType,
                attacker = base.gameObject,
                inflictor = base.gameObject,
                teamIndex = base.GetTeam(),
                damage = this.damageCoefficient * this.damageStat,
                procCoefficient = this.procCoefficient,
                hitEffectPrefab = this.hitEffectPrefab,
                forceVector = this.bonusForce,
                pushAwayForce = this.pushForce,
                hitBoxGroup = hitBoxGroup,
                isCrit = base.RollCrit(),
                impactSound = this.impactSound
            };
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            this.hitPauseTimer -= Time.fixedDeltaTime;

            if (this.hitPauseTimer <= 0f && this.inHitPause)
                this.ClearHitStop();

            if (this.inHitPause)
            {
                if (base.characterMotor)
                    base.characterMotor.velocity = Vector3.zero;

                if (this.animator)
                    this.animator.SetFloat("Slash.playbackRate", 0f);
            }
            else
            {
                this.stopwatch += Time.fixedDeltaTime;
            }

            if (this.stopwatch >= (this.duration * this.attackStartFraction) && this.stopwatch <= (this.duration * this.attackEndFraction))
            {
                this.FireAttack();
            }

            if (base.isAuthority)
            {
                if (base.fixedAge >= (this.duration * this.earlyExitFraction))
                {
                    if (this.IsKeyDownAuthority())
                    {
                        if (!this.hasFired)
                            this.FireAttack();

                        this.activatorSkillSlot.ExecuteIfReady();
                    }
                }

                if (base.fixedAge >= this.duration)
                {
                    this.outer.SetNextStateToMain();
                }
            }
        }

        protected virtual void FireAttack()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;

                Util.PlayAttackSpeedSound(this.swingSoundString, base.gameObject, this.attackSpeedStat);

                this.PlaySwingEffect();
            }

            if (base.isAuthority)
            {
                if (this.meleePivot)
                {
                    Vector3 direction = this.GetAimRay().direction;
                    direction.y = Mathf.Max(direction.y, direction.y * 0.5f);
                    this.FindModelChild("MeleePivot").rotation = Util.QuaternionSafeLookRotation(direction);
                }

                this.hitResults.Clear();
                if (this.attack.Fire(this.hitResults))
                {
                    this.OnHitEnemyAuthority(this.hitResults.Count);
                }
            }
        }

        protected virtual void PlaySwingEffect()
        {
            if (!string.IsNullOrEmpty(this.swingSoundString))
                Util.PlaySound(this.swingSoundString, this.gameObject);

            if (this.swingEffectPrefab)
            {
                var muzzleTransform = this.FindModelChild(this.muzzleString);
                if (muzzleTransform)
                {
                    this.swingEffectInstance = Object.Instantiate(this.swingEffectPrefab, muzzleTransform);
                    var fuck = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                    if (fuck) fuck.newDuration = fuck.initialDuration;
                }
            }
        }

        protected virtual void OnHitEnemyAuthority(int amount)
        {
            Util.PlaySound(this.hitSoundString, base.gameObject);

            if (!this.hasHopped)
            {
                this.hasHopped = true;

                if (base.characterMotor && !base.characterMotor.isGrounded)
                    base.SmallHop(base.characterMotor, this.hitHopVelocity);
            }

            if (this.iDrive.HasSpecialBullets && !this.ammoConsumed)
            {
                this.ammoConsumed = true;
                this.iDrive.ConsumeAmmo(1f, true);
            }

            if (!this.inHitPause)
                this.TriggerHitStop();
        }

        protected virtual void TriggerHitStop()
        {
            this.storedVelocity = base.characterMotor.velocity;
            this.hitStopCachedState = base.CreateHitStopCachedState(base.characterMotor, this.animator, "Slash.playbackRate");
            this.hitPauseTimer = this.hitStopDuration / this.attackSpeedStat;
            this.inHitPause = true;

            if (this.swingEffectInstance)
            {
                var fuck = this.swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if (fuck) fuck.newDuration = 20f;
            }
        }

        protected virtual void ClearHitStop()
        {
            base.ConsumeHitStopCachedState(this.hitStopCachedState, base.characterMotor, this.animator);
            this.inHitPause = false;
            base.characterMotor.velocity = this.storedVelocity;

            if (swingEffectInstance)
            {
                var fuck = swingEffectInstance.GetComponent<ScaleParticleSystemDuration>();
                if (fuck) fuck.newDuration = fuck.initialDuration;
            }
        }

        public override void ModifyNextState(EntityState nextState)
        {
            base.ModifyNextState(nextState);

            if (nextState is BaseDriverMeleeAttack swingState && nextState.GetType() == this.GetType())
                swingState.swingIndex = (this.swingIndex + 1) % this.swingComboCount;
        }

        public override void OnExit()
        {
            if (!this.hasFired)
                this.FireAttack();

            if (this.inHitPause)
                this.ClearHitStop();

            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= this.duration * this.earlyExitFraction)
                return InterruptPriority.Any;

            return this.interruptPriority;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.swingIndex);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.swingIndex = reader.ReadInt32();
        }
    }
}