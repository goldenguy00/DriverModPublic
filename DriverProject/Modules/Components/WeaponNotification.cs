using RoR2.UI;

namespace RobDriver.Modules.Components
{
    public class WeaponNotification : GenericNotification
    {
		public void SetWeapon(DriverWeaponDef weaponDef)
		{
			this.titleText.token = weaponDef.nameToken;
			this.descriptionText.token = weaponDef.descriptionToken;

			if (weaponDef.icon != null)
			{
				this.iconImage.texture = weaponDef.icon;
			}

			this.titleTMP.color = weaponDef.color;
		}
	}
}