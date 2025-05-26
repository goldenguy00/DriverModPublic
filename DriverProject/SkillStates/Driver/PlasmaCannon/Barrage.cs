using UnityEngine;

namespace RobDriver.SkillStates.Driver.PlasmaCannon
{
    public class Barrage : RocketLauncher.Barrage
    {
        internal static new float _damageCoefficient = 10f;
        protected override float damageCoefficient => 10f;
        protected override GameObject projectilePrefab => Modules.Projectiles.plasmaCannonProjectilePrefab;
        protected override float ammoMod => 2f;
    }
}
