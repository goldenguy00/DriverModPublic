using UnityEngine;

namespace RobDriver.SkillStates.Driver.LunarPistol
{
    public class SteadyAim : Driver.SteadyAim
    {
        internal static new float _damageCoefficient = 8f;
        protected override float damageCoefficient => 8f;
        protected override GameObject tracerPrefab => this.wasCharged ? Modules.Assets.chargedLunarTracer : Modules.Assets.lunarTracer;
        protected override bool isPiercing => true;
    }
}