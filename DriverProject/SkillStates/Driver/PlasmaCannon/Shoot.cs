using UnityEngine;

namespace RobDriver.SkillStates.Driver.PlasmaCannon
{
    public class Shoot : RocketLauncher.Shoot
    {
        internal static new float _damageCoefficient = 12f;
        protected override GameObject projectilePrefab => Modules.Projectiles.plasmaCannonProjectilePrefab;
        protected override string shootSound => "sfx_driver_plasma_cannon_shoot";
        public override void OnEnter()
        {
            base.damageCoefficient = 12f;
            base.ammoConsumption = 4f;

            base.OnEnter();
        }
    }
}
