using RoR2;
using System.Collections.Generic;
using System.Linq;

namespace RobDriver.Modules
{
    internal static class DriverWeaponSkinCatalog
    {
        internal static Dictionary<SkinIndex, Dictionary<ushort, DriverWeaponSkinDef>> driverSkinDefs { get; private set; } = new Dictionary<SkinIndex, Dictionary<ushort, DriverWeaponSkinDef>>();

        internal static void AddSkin(SkinIndex index, Dictionary<ushort, DriverWeaponSkinDef> skinDef)
        {
            driverSkinDefs.Add(index, skinDef);
        }

        internal static Dictionary<ushort, DriverWeaponSkinDef> GetWeaponSkinCatalog(ModelSkinController skinController)
        {
            if (skinController?.skins == null || skinController.skins.Length == 0) return null;

            var skinDef = skinController.skins.ElementAtOrDefault(skinController.currentSkinIndex);
            if (skinDef != null && driverSkinDefs.TryGetValue(skinDef.skinIndex, out var weaponSkinCatalog))
            {
                return weaponSkinCatalog;
            }
            return null;
        }
    }
}