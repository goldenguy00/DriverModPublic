using UnityEngine;

namespace RobDriver.SkillStates.Driver.FalsePistol
{
    public class SteadyAim : Driver.SteadyAim
    {
        public static new float _damageCoefficient = 9f;
        protected override float damageCoefficient => _damageCoefficient;
        protected override GameObject tracerPrefab => Shoot._tracerPrefab;
        protected override bool isPiercing => true;
    }
}