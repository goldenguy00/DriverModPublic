using RoR2;

namespace RobDriver.SkillStates.Driver.GoldenGun
{
    public class Shoot : Driver.Shoot
    {
        public static new float _damageCoefficient = 3.9f;
        protected override float damageCoefficient => _damageCoefficient;
        protected override string shootSoundString => "sfx_driver_pistol_shoot_charged";
        protected override BulletAttack.FalloffModel falloff => BulletAttack.FalloffModel.None;
    }
}