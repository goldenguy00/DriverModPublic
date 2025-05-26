using UnityEngine;

namespace RobDriver.Modules
{
    internal static class Helpers
    {
        internal const string agilePrefix = "<style=cIsUtility>Agile.</style> ";

        internal const string whiteItemHex = "00FF66";
        internal const string greenItemHex = "00FF66";
        internal const string redItemHex = "FF0033";
        internal const string yellowItemHex = "FFFF00";
        internal const string lunarItemHex = "0066FF";
        internal const string voidItemHex = "C678B4";
        internal const string colorSuffix = "</color>";

        internal static Color noTierColor = new Color32(0, 0, 0, 255);
        internal static Color commonItemColor = new Color32(255, 255, 255, 255);
        internal static Color uncommonItemColor = new Color32(0, 255, 102, 255);
        internal static Color legendaryItemColor = new Color32(255, 0, 51, 255);
        internal static Color uniqueItemColor = new Color32(255, 255, 0, 255);
        internal static Color voidItemColor = new Color32(198, 120, 180, 255);
        internal static Color lunarItemColor = new Color32(0, 102, 255, 255);
        internal static Color badColor = new Color32(127, 0, 0, 255);

        internal static string ScepterDescription(string desc)
        {
            return "\n<color=#d299ff>SCEPTER: " + desc + "</color>";
        }

        internal static Color GetColorForTier(DriverWeaponTier tier) => tier switch
        {
            DriverWeaponTier.Common => Helpers.commonItemColor,
            DriverWeaponTier.Uncommon => Helpers.uncommonItemColor,
            DriverWeaponTier.Legendary => Helpers.legendaryItemColor,
            DriverWeaponTier.Unique => Helpers.uniqueItemColor,
            DriverWeaponTier.Void => Helpers.voidItemColor,
            DriverWeaponTier.Lunar => Helpers.lunarItemColor,
            _ => Helpers.noTierColor
        };

        internal static GameObject GetPickupPrefabForTier(DriverWeaponTier tier) => tier switch
        {
            DriverWeaponTier.Common => Assets.commonPickupModel,
            DriverWeaponTier.Uncommon => Assets.uncommonPickupModel,
            DriverWeaponTier.Legendary => Assets.legendaryPickupModel,
            DriverWeaponTier.Unique => Assets.uniquePickupModel,
            DriverWeaponTier.Void => Assets.voidPickupModel,
            DriverWeaponTier.Lunar => Assets.lunarPickupModel,
            _ => Assets.uniquePickupModel,
        };
    }
}