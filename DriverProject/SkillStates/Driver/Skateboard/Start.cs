using RoR2;
using EntityStates;
using RobDriver.SkillStates.BaseStates;
using RobDriver.Modules;

namespace RobDriver.SkillStates.Driver.Skateboard
{
    public class Start : BaseDriverSkillState
    {
        public float baseDuration = 0.5f;

        protected override bool cancelOnPickup => false;
        protected override bool holsterGun => true;
        protected override string showProp => "SkateboardModel";

        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;

            this.skillLocator.utility.SetWeaponSkill(Skills.skateCancelSkillDef, GenericSkill.SkillOverridePriority.Contextual);

            this.childLocator.FindChildGameObject("SkateboardBackModel").SetActive(false);

            // pistol has good animation blending, others dont
            if (this.iDrive.weaponDef.animationSet != DriverWeaponDef.AnimationSet.Default)
            {
                base.PlayAnimation("Gesture, Override", "BufferEmpty");
            }

            Util.PlaySound("sfx_driver_foley_syringe", this.gameObject);
            base.PlayCrossfade("FullBody, Override", "StartSkate", "Slide.playbackRate", this.duration, 0.05f);

            this.SmallHop(this.characterMotor, 10f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextState(new Idle());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}