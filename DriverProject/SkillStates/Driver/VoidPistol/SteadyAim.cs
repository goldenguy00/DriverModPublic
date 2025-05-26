using UnityEngine;

namespace RobDriver.SkillStates.Driver.VoidPistol
{
    public class SteadyAim : Driver.SteadyAim
    {
        internal static new float _damageCoefficient = 9f;
        protected override float damageCoefficient => 9f;
        protected override GameObject tracerPrefab => Shoot._tracerPrefab;
        protected override bool isPiercing => true;
    }
}