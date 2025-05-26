using RoR2;
using System.Collections.Generic;
using System.Linq;

namespace RobDriver
{
    public static class DriverWeaponSkinCatalog
    {
        internal static Dictionary<SkinIndex, DriverWeaponSkinDef[]> driverSkinDefs = [];

        public static void AddSkin(SkinIndex index, IEnumerable<DriverWeaponSkinDef> skinDefs)
        {
            if (skinDefs.Any() && index != SkinIndex.None)
            {
                driverSkinDefs.Add(index, [.. skinDefs]);
            }
        }

        public static DriverWeaponDef.ModelSwapInfo[] GetModelSwapInfoForWeapon(ModelSkinController skinController, DriverWeaponDef weaponDef)
        {
            if (skinController?.skins?.Length == 0)
                return weaponDef.modelSwapInfo;

            return GetModelSwapInfoForWeapon(skinController.skins, skinController.currentSkinIndex, weaponDef);
        }

        public static DriverWeaponDef.ModelSwapInfo[] GetModelSwapInfoForWeapon(SkinDef[] skins, int skinIndex, DriverWeaponDef weaponDef)
        {
            var mainSkin = HG.ArrayUtils.GetSafe(skins, skinIndex);
            if (mainSkin == null || !driverSkinDefs.ContainsKey(mainSkin.skinIndex))
                return weaponDef.modelSwapInfo;

            var allWeaponSkins = driverSkinDefs[mainSkin.skinIndex];
            for (int i = 0; i < allWeaponSkins.Length; i++)
            {
                if (allWeaponSkins[i].weaponDefIndex == weaponDef.index)
                    return allWeaponSkins[i].modelSwapInfo;
            }

            return weaponDef.modelSwapInfo;
        }
    }
}