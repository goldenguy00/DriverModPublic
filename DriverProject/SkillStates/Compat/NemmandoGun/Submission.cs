using RobDriver.Modules.Weapons;

namespace RobDriver.SkillStates.Driver.NemmandoGun
{
    public class Submission : SMG.SuppressiveFire
    {
        public static new float _damageCoefficient = 2.5f;
        public static uint _bulletCount = 6;

        protected override float damageCoefficient => _damageCoefficient;
        protected override float maxBulletSpread => 7f;

        public override void OnEnter()
        {
            procCoefficient = 0.2f;
            bulletCount = _bulletCount;
            dropShells = 0;
            visualRecoilAmplitude = 12f;

            base.OnEnter();

            if (this.cachedWeaponDef == NemKatana.instance?.weaponDef)
                this.iDrive.SetSkinnedWeaponModel(DriverWeaponCatalog.NemmandoGun);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.iDrive.weaponDef == NemKatana.instance?.weaponDef)
                this.iDrive.SetSkinnedWeaponModel(this.iDrive.weaponDef);
        }
    }
}