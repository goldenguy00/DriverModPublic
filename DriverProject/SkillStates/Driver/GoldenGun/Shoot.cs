using RoR2;

namespace RobDriver.SkillStates.Driver.GoldenGun
{
    public class Shoot : SkillStates.Driver.Shoot
    {
        protected override float _damageCoefficient => 3.9f;
        protected override float baseCritDuration => 0.9f;
        public override string shootSoundString => "sfx_driver_pistol_shoot_charged";
        public override BulletAttack.FalloffModel falloff => BulletAttack.FalloffModel.None;
    }
}