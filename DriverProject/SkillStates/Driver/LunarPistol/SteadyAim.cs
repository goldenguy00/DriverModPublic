using UnityEngine;

namespace RobDriver.SkillStates.Driver.LunarPistol
{
    public class SteadyAim : Driver.SteadyAim
    {
        protected override float _damageCoefficient => 9f;
        protected override GameObject tracerPrefab => this.lastCharge ? Modules.Assets.chargedLunarTracer : Modules.Assets.lunarTracer;
        protected override bool isPiercing => true;
    }
}