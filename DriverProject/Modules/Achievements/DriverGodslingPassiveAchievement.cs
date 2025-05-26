using RoR2;
using UnityEngine;

namespace RobDriver.Modules.Achievements
{
    //string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, uint lunarCoinReward, Type serverTrackerType = null
    //automatically creates language tokens "ACHIEVEMENT_{identifier.ToUpper()}_NAME" and "ACHIEVEMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(IDENTIFIER, UNLOCKABLE_ITENTIFIER, DriverUnlockAchievement.IDENTIFIER, 10, null)]
    internal class DriverGodslingPassiveAchievement : BaseDifficultyAchievement
    {
        public const string IDENTIFIER = "ROB_DRIVER_BODY_GODSLING_UNLOCKABLE_ACHIEVEMENT_ID";
        public const string UNLOCKABLE_ITENTIFIER = "ROB_DRIVER_BODY_GODSLING_UNLOCKABLE_REWARD_ID";
        public static Sprite Sprite => Assets.mainAssetBundle.LoadAsset<Sprite>("texAltPassiveIcon");

        public override float RequiredDifficultyCoefficient => 3f;
        public override DifficultyIndex MinimumDifficultyIndex => DifficultyIndex.Hard;

        public static bool weaponPickedUpHard;

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();

            weaponPickedUpHard = false;
        }

        public override void OnBodyRequirementBroken()
        {
            base.OnBodyRequirementBroken();

            weaponPickedUpHard = false;
        }

        protected override void Run_OnClientGameOverGlobal(Run run, RunReport runReport)
        {
            if (!weaponPickedUpHard)
            {
                base.Run_OnClientGameOverGlobal(run, runReport);
            }
        }
    }
}