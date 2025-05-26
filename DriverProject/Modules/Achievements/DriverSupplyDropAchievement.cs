using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace RobDriver.Modules.Achievements
{
    //string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, uint lunarCoinReward, Type serverTrackerType = null
    //automatically creates language tokens "ACHIEVEMENT_{identifier.ToUpper()}_NAME" and "ACHIEVEMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(IDENTIFIER, UNLOCKABLE_IDENTIFIER, DriverUnlockAchievement.IDENTIFIER, 10, null)]
    internal class DriverSupplyDropAchievement : BaseAchievement
    {
        public const string IDENTIFIER = "ROB_DRIVER_BODY_SUPPLY_DROP_UNLOCKABLE_ACHIEVEMENT_ID";
        public const string UNLOCKABLE_IDENTIFIER = "ROB_DRIVER_BODY_SUPPLY_DROP_UNLOCKABLE_REWARD_ID";

        public static Sprite Sprite => Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSupplyDropIcon");

        public static bool weaponHasDespawned;

        public override BodyIndex LookUpRequiredBodyIndex() => Survivors.Driver.bodyIndex;

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();

            weaponHasDespawned = false;

            TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterFinishGlobal += TeleporterInteraction_onTeleporterFinishGlobal;
        }

        public override void OnBodyRequirementBroken()
        {
            base.OnBodyRequirementBroken();

            weaponHasDespawned = false;

            TeleporterInteraction.onTeleporterBeginChargingGlobal -= TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterFinishGlobal -= TeleporterInteraction_onTeleporterFinishGlobal;
        }

        private void TeleporterInteraction_onTeleporterFinishGlobal(TeleporterInteraction obj)
        {
            if (base.meetsBodyRequirement && !weaponHasDespawned)
            {
                base.Grant();
            }
        }

        private void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction obj)
        {
            weaponHasDespawned = false;
        }
    }
}