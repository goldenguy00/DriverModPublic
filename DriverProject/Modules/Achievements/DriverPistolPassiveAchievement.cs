using RoR2;
using RoR2.Achievements;
using System;
using UnityEngine;

namespace RobDriver.Modules.Achievements
{
    //string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, uint lunarCoinReward, Type serverTrackerType = null
    //automatically creates language tokens "ACHIEVEMENT_{identifier.ToUpper()}_NAME" and "ACHIEVEMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    internal class DriverPistolPassiveAchievement : BaseAchievement
    {
        public const string identifier = "ROB_DRIVER_PISTOL_PASSIVE";
        public const string nameToken = "ACHIEVEMENT_ROB_DRIVER_PISTOL_PASSIVE_NAME";
        public const string unlockableIdentifier = "ROB_DRIVER_PISTOL_PASSIVE_UNLOCKABLE";
        public static Sprite Sprite => Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texAltPassiveIcon");

        public static bool weaponPickedUp;

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
            if (base.meetsBodyRequirement && !weaponPickedUp)
            {
                base.Grant();
            }
        }

        private void TeleporterInteraction_onTeleporterBeginChargingGlobal(TeleporterInteraction obj)
        {
            weaponPickedUp = false;
        }
    }
}