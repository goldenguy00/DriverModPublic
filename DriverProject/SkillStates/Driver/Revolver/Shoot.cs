namespace RobDriver.SkillStates.Driver.Revolver
{
    public class Shoot : SkillStates.Driver.Shoot
    {
        protected override float _damageCoefficient => 3.2f;
        public override string shootSoundString => "Play_bandit2_R_fire";
    }
}