using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace RobDriver.Modules.Achievements
{
    //string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, uint lunarCoinReward, Type serverTrackerType = null
    //automatically creates language tokens "ACHIEVEMENT_{identifier.ToUpper()}_NAME" and "ACHIEVEMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    internal class SupplyDropAchievement : BaseAchievement
    {
        public const string identifier = "ROB_DRIVER_SUPPLY_DROP";
        public const string nameToken = "ACHIEVEMENT_ROB_DRIVER_SUPPLY_DROP_NAME";
        public const string unlockableIdentifier = "ROB_DRIVER_SUPPLY_DROP_UNLOCKABLE";

        public static Sprite Sprite => Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSupplyDropIcon");

        public static bool weaponHasDespawned;

        public override BodyIndex LookUpRequiredBodyIndex() => Survivors.Driver.bodyIndex;

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();

            TeleporterInteraction.onTeleporterBeginChargingGlobal += TeleporterInteraction_onTeleporterBeginChargingGlobal;
            TeleporterInteraction.onTeleporterFinishGlobal += TeleporterInteraction_onTeleporterFinishGlobal;
        }

        public override void OnBodyRequirementBroken()
        {
            base.OnBodyRequirementBroken();

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