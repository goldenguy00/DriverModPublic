using System.Collections.Generic;
using RobDriver.Modules.Achievements;
using RoR2;
using UnityEngine;

namespace RobDriver.Modules
{
    internal static class Unlockables
    {
        internal static List<UnlockableDef> unlockableDefs = new List<UnlockableDef>();

        internal static UnlockableDef characterUnlockableDef;

        internal static UnlockableDef masteryUnlockableDef;
        internal static UnlockableDef grandMasteryUnlockableDef;
        internal static UnlockableDef suitUnlockableDef;

        internal static UnlockableDef supplyDropUnlockableDef;
        internal static UnlockableDef pistolPassiveUnlockableDef;
        internal static UnlockableDef godslingPassiveUnlockableDef;

        internal static void Init()
        {
            characterUnlockableDef = CreateAndAddUnlockableDef(DriverUnlockAchievement.IDENTIFIER, DriverUnlockAchievement.UNLOCKABLE_IDENTIFIER, DriverUnlockAchievement.Sprite);

            masteryUnlockableDef = CreateAndAddUnlockableDef(DriverMonsoonAchievement.IDENTIFIER, DriverMonsoonAchievement.UNLOCKABLE_IDENTIFIER, DriverMonsoonAchievement.Sprite);
            grandMasteryUnlockableDef = CreateAndAddUnlockableDef(DriverTyphoonAchievement.IDENTIFIER, DriverTyphoonAchievement.UNLOCKABLE_IDENTIFIER, DriverTyphoonAchievement.Sprite);
            suitUnlockableDef = CreateAndAddUnlockableDef(DriverSuitAchievement.IDENTIFIER, DriverSuitAchievement.UNLOCKABLE_IDENTIFIER, DriverSuitAchievement.Sprite);

            supplyDropUnlockableDef = CreateAndAddUnlockableDef(DriverSupplyDropAchievement.IDENTIFIER, DriverSupplyDropAchievement.UNLOCKABLE_IDENTIFIER, DriverSupplyDropAchievement.Sprite);
            pistolPassiveUnlockableDef = CreateAndAddUnlockableDef(DriverPistolPassiveAchievement.IDENTIFIER, DriverPistolPassiveAchievement.UNLOCKABLE_ITENTIFIER, DriverPistolPassiveAchievement.Sprite);
            godslingPassiveUnlockableDef = CreateAndAddUnlockableDef(DriverGodslingPassiveAchievement.IDENTIFIER, DriverGodslingPassiveAchievement.UNLOCKABLE_ITENTIFIER, DriverGodslingPassiveAchievement.Sprite);
        }

        internal static UnlockableDef CreateAndAddUnlockableDef(string identifier, string unlockableIdentifier, Sprite achievementIcon)
        {
            var unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            unlockableDef.cachedName = unlockableIdentifier.ToUpperInvariant();
            unlockableDef.nameToken = "ACHIEVEMENT_" + identifier.ToUpperInvariant() + "_NAME";
            unlockableDef.achievementIcon = achievementIcon;

            unlockableDefs.Add(unlockableDef);

            return unlockableDef;
        }

        internal static UnlockableDef CreateAndAddWeaponUnlockableDef(string name) => CreateAndAddWeaponUnlockableDef($"ROB_DRIVER_{name.ToUpperInvariant()}_NAME", $"ROB_DRIVER_{name.ToUpperInvariant()}_DESC");

        public static UnlockableDef CreateAndAddWeaponUnlockableDef(string nameToken, string descriptionToken)
        {
            var unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            unlockableDef.cachedName = nameToken;
            unlockableDef.nameToken = nameToken;
            unlockableDef.getHowToUnlockString = () => Language.GetString(descriptionToken);
            unlockableDef.getUnlockedString = () => Language.GetString(descriptionToken);
            unlockableDef.achievementIcon = null;
            unlockableDef.hidden = false;

            unlockableDefs.Add(unlockableDef);

            return unlockableDef;
        }
    }
}
