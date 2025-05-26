namespace RobDriver.SkillStates.Driver.RocketLauncher
{
    public class NerfedShoot : Shoot
    {
        internal static new float _damageCoefficient = 4.5f;
        public override void OnEnter()
        {
            base.damageCoefficient = 4.5f;
            base.ammoConsumption = 4f;

            base.OnEnter();
        }
    }
}