using UnityEngine.Networking;

namespace RobDriver.SkillStates.Driver.BeetleShield
{
    public class SteadyAim : Driver.SteadyAim
    {
        public override void OnEnter()
        {
            base.OnEnter();

            if (NetworkServer.active) this.characterBody.AddBuff(RoR2.RoR2Content.Buffs.SmallArmorBoost);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (NetworkServer.active) this.characterBody.RemoveBuff(RoR2.RoR2Content.Buffs.SmallArmorBoost);
        }

        protected override void PlayAnim()
        {
            base.PlayAnimation("Gesture, Override", "ShieldSteadyAim", "Action.playbackRate", 0.25f);
        }

        protected override void PlayExitAnim()
        {
            base.PlayAnimation("Gesture, Override", "ShieldSteadyAimEnd", "Action.playbackRate", 0.2f);
        }

        protected override void PlayShootAnim(bool wasCharged, bool wasCrit, float speed)
        {
            if (wasCrit) base.PlayAnimation("Gesture, Override", "ShieldSteadyAimFireCritical", "Action.playbackRate", speed);
            else base.PlayAnimation("Gesture, Override", "ShieldSteadyAimFire", "Action.playbackRate", speed);
        }
    }
}