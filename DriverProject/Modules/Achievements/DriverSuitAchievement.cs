using RobDriver.Modules.Components;
using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace RobDriver.Modules.Achievements
{
    //string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, uint lunarCoinReward, Type serverTrackerType = null
    //automatically creates language tokens "ACHIEVEMENT_{identifier.ToUpper()}_NAME" and "ACHIEVEMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(IDENTIFIER, UNLOCKABLE_IDENTIFIER, DriverUnlockAchievement.IDENTIFIER, 10, null)]
    internal class DriverSuitAchievement : BaseAchievement
    {
        public const string IDENTIFIER = "ROB_DRIVER_BODY_SUIT_UNLOCKABLE_ACHIEVEMENT_ID";
        public const string UNLOCKABLE_IDENTIFIER = "ROB_DRIVER_BODY_SUIT_UNLOCKABLE_REWARD_ID";
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