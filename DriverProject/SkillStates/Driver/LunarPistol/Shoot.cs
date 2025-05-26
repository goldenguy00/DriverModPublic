using RoR2;
using UnityEngine;

namespace RobDriver.SkillStates.Driver.LunarPistol
{
    public class Shoot : Driver.Shoot
    {
        internal static new float _damageCoefficient = 3.5f;
        protected override float damageCoefficient => 3.5f;
        protected override GameObject tracerPrefab => Modules.Assets.lunarTracer;
        protected override BulletAttack.FalloffModel falloff => BulletAttack.FalloffModel.None;
    }
}