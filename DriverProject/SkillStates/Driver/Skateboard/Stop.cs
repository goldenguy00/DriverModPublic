using RoR2;
using EntityStates;
using RobDriver.SkillStates.BaseStates;
using RobDriver.Modules;

namespace RobDriver.SkillStates.Driver.Skateboard
{
    public class Stop : BaseDriverSkillState
    {
        public float baseDuration = 0.6f;

        protected override bool cancelOnPickup => false;
        protected override bool holsterGun => true;
        protected override string showProp => "SkateboardModel";

        private float duration;
        private float dismountTiming;
        private readonly float transitionDuration = 0.1f;
        private bool kill;

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = this.baseDuration / this.attackSpeedStat;
            this.dismountTiming = this.duration * 0.7f;

            Util.PlaySound("sfx_driver_foley_syringe", this.gameObject);
            this.PlayCrossfade("FullBody, Override", "StopSkate", "Slide.playbackRate", this.duration, this.transitionDuration);

            this.childLocator.FindChildGameObject("SkateboardBackModel").SetActive(false);

            this.SmallHop(this.characterMotor, 10f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.skillLocator.utility.stock > 0)
            {
                this.skillLocator.utility.stock = 0;
            }

            if (base.fixedAge >= this.dismountTiming && !this.kill)
            {
                if (this.cachedWeaponDef.animationSet != DriverWeaponDef.AnimationSet.Default)
                {
                    this.childLocator.FindChildGameObject("PistolModel").SetActive(true);
                }

                this.childLocator.FindChildGameObject("SkateboardModel").SetActive(false);
                this.childLocator.FindChildGameObject("SkateboardBackModel").SetActive(true);
                this.iDrive.DestroyHolsteredWeaponInstance();

                this.PlayCrossfade("Gesture, Override", this.cachedWeaponDef.equipAnimationString, this.transitionDuration);
                this.kill = true;
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            this.skillLocator.utility.UnsetWeaponSkill(Skills.skateCancelSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            this.childLocator.FindChildGameObject("SkateboardBackModel").SetActive(true);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            // this atrocity is because interrupting a crossfade will fuck everything up
            // what the hell man
            if (base.fixedAge <= this.transitionDuration || (this.kill &&
                base.fixedAge <= this.dismountTiming + this.transitionDuration)) return InterruptPriority.Death;

            return InterruptPriority.Skill;
        }
    }
}