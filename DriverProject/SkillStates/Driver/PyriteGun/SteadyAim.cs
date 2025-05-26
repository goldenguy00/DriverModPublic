namespace RobDriver.SkillStates.Driver.PyriteGun
{
    public class SteadyAim : Driver.SteadyAim
    {
        internal static new float _damageCoefficient = 6f;
        protected override float damageCoefficient => 6f;
        protected override string shootSoundString => "sfx_driver_pistol_shoot_charged";
    }
}