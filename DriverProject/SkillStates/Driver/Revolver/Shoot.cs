namespace RobDriver.SkillStates.Driver.Revolver
{
    public class Shoot : Driver.Shoot
    {
        public static new float _damageCoefficient = 3.2f;
        protected override float damageCoefficient => _damageCoefficient;
        protected override string shootSoundString => "Play_bandit2_R_fire";
    }
}