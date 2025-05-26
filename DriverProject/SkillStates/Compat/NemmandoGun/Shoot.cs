using RoR2;

namespace RobDriver.SkillStates.Driver.NemmandoGun
{
    public class Shoot : Driver.Shoot
    {
        internal static new float _damageCoefficient = 2.4f;
        protected override float damageCoefficient => 2.4f;
        protected override BulletAttack.FalloffModel falloff => BulletAttack.FalloffModel.None;
        protected override string shootSoundString => DriverPlugin.StarstormInstalled ? "NemmandoShoot" : "sfx_driver_pistol_shoot_charged";
    }
}