using UnityEngine.Networking;

namespace RobDriver.SkillStates.Driver.BeetleShield
{
    public class SteadyAim : Driver.SteadyAim
    {
        internal static new float _damageCoefficient = 6f;
        protected override float damageCoefficient => 6f;

        protected override string animationString
        {
            get
            {
                string animString = this.baseShootAnimation;

                if (this.isCrit)
                    animString += "Critical";

                return animString;
            }
        }

        public override void OnEnter()
        {
            base.baseShootAnimation = "ShieldSteadyAimFire";
            base.enterAnimation = "ShieldSteadyAim";
            base.exitAnimation = "ShieldSteadyAimEnd";

            base.OnEnter();

            if (NetworkServer.active) 
                this.characterBody.AddBuff(RoR2.RoR2Content.Buffs.SmallArmorBoost);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (NetworkServer.active)
                this.characterBody.RemoveBuff(RoR2.RoR2Content.Buffs.SmallArmorBoost);
        }
    }
}