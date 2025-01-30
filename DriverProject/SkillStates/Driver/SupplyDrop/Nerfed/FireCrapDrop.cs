namespace RobDriver.SkillStates.Driver.SupplyDrop.Nerfed
{
    public class FireCrapDrop : FireSupplyDrop
    {
        protected override bool cutAmmo => true;

        protected override DriverWeaponDef weaponDef
        {
            get
            {
                return Modules.Config.randomSupplyDrop.Value
                    ? DriverWeaponCatalog.GetRandomWeapon()
                    : DriverWeaponCatalog.GetRandomWeaponFromTier(DriverWeaponTier.Uncommon);
            }
        }

        protected override DriverBulletDef bulletDef
        {
            get
            {
                return Modules.Config.randomSupplyDrop.Value
                    ? DriverBulletCatalog.GetWeightedRandomBullet(DriverWeaponTier.Legendary)
                    : DriverBulletCatalog.GetWeightedRandomBullet(DriverWeaponTier.Uncommon);
            }
        }
    }
}