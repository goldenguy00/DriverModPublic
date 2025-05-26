namespace RobDriver.SkillStates.Driver.PyriteGun
{
    public class Shoot : Driver.Shoot
    {
        internal static new float _damageCoefficient = 2.5f;
        protected override float damageCoefficient => 2.5f;
        protected override string shootSoundString => "sfx_driver_pistol_shoot_charged";
    }
}