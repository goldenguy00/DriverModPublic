using RoR2;
using EntityStates;

namespace RobDriver.SkillStates.Driver.Skateboard
{
    public class Stop : BaseDriverSkillState
    {
        public float baseDuration = 0.6f;
        protected override bool hideGun => true;
        protected override string prop => "SkateboardModel";

        private float duration;
        private float dismountTiming => this.duration * 0.7f;
        private float transitionDuration = 0.1f;
        private bool kill;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;;
            this.kill = false;
            this.skillLocator.utility.UnsetSkillOverride(this.skillLocator.utility, Modules.Survivors.Driver.skateCancelSkillDef, GenericSkill.SkillOverridePriority.Replacement);

            Util.PlaySound("sfx_driver_foley_syringe", this.gameObject);
            this.PlayCrossfade("FullBody, Override", "StopSkate", "Slide.playbackRate", this.duration, this.transitionDuration);
            this.SmallHop(this.characterMotor, 10f);
        }

        public override void OnExit()
        {
            base.OnExit();
            if (this.iDrive.weaponEffectInstance) Destroy(this.iDrive.weaponEffectInstance);
            this.GetModelChildLocator().FindChild("SkateboardBackModel").gameObject.SetActive(true);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (base.isAuthority && base.fixedAge >= this.dismountTiming && !this.kill)
            {
                if (this.iDrive.weaponEffectInstance) Destroy(this.iDrive.weaponEffectInstance);
                if (this.cachedWeaponDef.animationSet != DriverWeaponDef.AnimationSet.Default) this.GetModelChildLocator().FindChild("PistolModel").gameObject.SetActive(true);

                this.GetModelChildLocator().FindChild("SkateboardModel").gameObject.SetActive(false);
                this.GetModelChildLocator().FindChild("SkateboardBackModel").gameObject.SetActive(true);

                this.PlayCrossfade("Gesture, Override", this.cachedWeaponDef.equipAnimationString, this.transitionDuration);
                this.kill = true;
            }

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextState(new WaitForReload());
            }
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