namespace RobDriver.SkillStates.Driver.PyriteGun
{
    public class Shoot : SkillStates.Driver.Shoot
    {
        protected override float _damageCoefficient => 2.5f;
        public override string shootSoundString => "sfx_driver_pistol_shoot_charged";
    }
}