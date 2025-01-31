using RoR2;
using UnityEngine;

namespace RobDriver.SkillStates.Driver.LunarPistol
{
    public class Shoot : Driver.Shoot
    {
        protected override float _damageCoefficient => 3.5f;
        protected override GameObject tracerPrefab => Modules.Assets.lunarTracer;
        public override BulletAttack.FalloffModel falloff => BulletAttack.FalloffModel.None;
    }
}