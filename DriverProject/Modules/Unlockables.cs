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
            characterUnlockableDef = CreateAndAddUnlockableDef(DriverUnlockAchievement.identifier, DriverUnlockAchievement.nameToken, DriverUnlockAchievement.Sprite);

            masteryUnlockableDef = CreateAndAddUnlockableDef(MasteryAchievement.identifier, MasteryAchievement.nameToken, MasteryAchievement.Sprite);
            grandMasteryUnlockableDef = CreateAndAddUnlockableDef(GrandMasteryAchievement.identifier, GrandMasteryAchievement.nameToken, GrandMasteryAchievement.Sprite);
            suitUnlockableDef = CreateAndAddUnlockableDef(SuitAchievement.identifier, SuitAchievement.nameToken, SuitAchievement.Sprite);

            supplyDropUnlockableDef = CreateAndAddUnlockableDef(SupplyDropAchievement.identifier, SupplyDropAchievement.nameToken, SupplyDropAchievement.Sprite);
            pistolPassiveUnlockableDef = CreateAndAddUnlockableDef(DriverPistolPassiveAchievement.identifier, DriverPistolPassiveAchievement.nameToken, DriverPistolPassiveAchievement.Sprite);
            godslingPassiveUnlockableDef = CreateAndAddUnlockableDef(DriverGodslingPassiveAchievement.identifier, DriverGodslingPassiveAchievement.nameToken, DriverGodslingPassiveAchievement.Sprite);
        }

        internal static void AddUnlockableDef(UnlockableDef unlockableDef)
        {
            unlockableDefs.Add(unlockableDef);
        }

        internal static UnlockableDef CreateAndAddUnlockableDef(string identifier, string nameToken, Sprite achievementIcon)
        {
            var unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            unlockableDef.cachedName = identifier;
            unlockableDef.nameToken = nameToken;
            unlockableDef.achievementIcon = achievementIcon;

            AddUnlockableDef(unlockableDef);

            return unlockableDef;
        }

        public static UnlockableDef CreateAndAddWeaponUnlockableDef(DriverWeaponDef weaponDef)
        {
            var unlockableDef = ScriptableObject.CreateInstance<UnlockableDef>();
            unlockableDef.cachedName = weaponDef.nameToken;
            unlockableDef.nameToken = weaponDef.nameToken;
            unlockableDef.getHowToUnlockString = () => Language.GetString(weaponDef.descriptionToken);
            unlockableDef.getUnlockedString = () => Language.GetString(weaponDef.descriptionToken);
            unlockableDef.hidden = false;
            unlockableDef.achievementIcon = Sprite.Create(weaponDef.icon as Texture2D,
                new Rect(0, 0, weaponDef.icon.width, weaponDef.icon.height), new Vector2(0.5f, 0.5f));

            AddUnlockableDef(unlockableDef);

            return unlockableDef;
        }
    }
}
