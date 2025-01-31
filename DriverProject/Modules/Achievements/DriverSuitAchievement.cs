using RobDriver.Modules.Components;
using RoR2;
using RoR2.Achievements;
using System;
using UnityEngine;

namespace RobDriver.Modules.Achievements
{
    //string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, uint lunarCoinReward, Type serverTrackerType = null
    //automatically creates language tokens "ACHIEVEMENT_{identifier.ToUpper()}_NAME" and "ACHIEVEMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    internal class SuitAchievement : BaseAchievement
    {
        public const string identifier = "ROB_DRIVER_SUIT";
        public const string nameToken = "ACHIEVEMENT_ROB_DRIVER_SUIT_NAME";
        public const string unlockableIdentifier = "ROB_DRIVER_SUIT_UNLOCKABLE";
        public static Sprite Sprite => Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSuitSkin");

        public override BodyIndex LookUpRequiredBodyIndex() => Survivors.Driver.bodyIndex;

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();

            GlobalEventManager.onCharacterDeathGlobal += this.GlobalEventManager_onCharacterDeathGlobal;
        }

        public override void OnBodyRequirementBroken()
        {
            base.OnBodyRequirementBroken();

            GlobalEventManager.onCharacterDeathGlobal -= this.GlobalEventManager_onCharacterDeathGlobal;
        }

        private void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (damageReport.attackerBody && damageReport.attackerBody.bodyIndex == Survivors.Driver.bodyIndex)
            {
                if (damageReport.victimIsChampion && damageReport.attackerBody.TryGetComponent<DriverController>(out var iDrive))
                {
                    if (iDrive.weaponDef == DriverWeaponCatalog.Sniper)
                    {
                        if (base.meetsBodyRequirement)
                        {
                            base.Grant();
                        }
                    }
                }
            }
        }

    }
}