using RoR2;
using System.Collections.Generic;
using System.Linq;

namespace RobDriver
{
    public static class DriverWeaponSkinCatalog
    {
        public static Dictionary<SkinIndex, Dictionary<ushort, DriverWeaponSkinDef>> driverSkinDefs { get; private set; } = new Dictionary<SkinIndex, Dictionary<ushort, DriverWeaponSkinDef>>();

        public static void AddSkin(SkinIndex index, Dictionary<ushort, DriverWeaponSkinDef> skinDef)
        {
            driverSkinDefs.Add(index, skinDef);
        }

        public static Dictionary<ushort, DriverWeaponSkinDef> GetWeaponSkinCatalog(ModelSkinController skinController)
        {
            if (skinController?.skins?.Any() != true)
                return null;

            var skinDef = skinController.skins.ElementAtOrDefault(skinController.currentSkinIndex);
            if (skinDef != null && driverSkinDefs.TryGetValue(skinDef.skinIndex, out var weaponSkinCatalog))
            {
                return weaponSkinCatalog;
            }
            return null;
        }

        public static bool GetWeaponSkin(ModelSkinController skinController, DriverWeaponDef weaponDef, out DriverWeaponSkinDef weaponSkinDef)
        {
            weaponSkinDef = null;
            var catalog = GetWeaponSkinCatalog(skinController);

            if (catalog != null && catalog.ContainsKey(weaponDef.index))
            {
                weaponSkinDef = catalog[weaponDef.index];
            }
            
            return weaponSkinDef != null;
        }
    }
}