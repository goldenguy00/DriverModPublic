namespace RobDriver.SkillStates.Driver.SupplyDrop.Nerfed
{
    public class FireCrapDrop : FireSupplyDrop
    {
        private DriverWeaponTier weaponTier;

        protected override bool cutAmmo => true;

        // please don't make me pay for my sins, call weaponDef first i beg of you
        protected override DriverBulletDef bulletDef => DriverBulletCatalog.GetRandomBulletFromTier(weaponTier);
        protected override DriverWeaponDef weaponDef
        {
            get
            {
                var weaponDef = Modules.Config.randomSupplyDrop.Value
                    ? DriverWeaponCatalog.GetRandomWeapon()
                    : DriverWeaponCatalog.GetWeightedRandomWeapon(DriverWeaponTier.Uncommon);

                this.weaponTier = weaponDef.tier;
                if (this.weaponTier == DriverWeaponTier.NoTier || this.weaponTier > DriverWeaponTier.Unique)
                    this.weaponTier = DriverWeaponTier.Unique;

                return weaponDef;
            }
        }
    }
}