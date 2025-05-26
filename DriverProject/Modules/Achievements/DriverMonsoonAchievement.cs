using RoR2;
using UnityEngine;

namespace RobDriver.Modules.Achievements
{
    //string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, uint lunarCoinReward, Type serverTrackerType = null
    //automatically creates language tokens "ACHIEVEMENT_{identifier.ToUpper()}_NAME" and "ACHIEVEMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(IDENTIFIER, UNLOCKABLE_IDENTIFIER, DriverUnlockAchievement.IDENTIFIER, 10, null)]
    internal class DriverMonsoonAchievement : BaseDifficultyAchievement
    {
        public const string IDENTIFIER = "ROB_DRIVER_BODY_MONSOONUNLOCKABLE_ACHIEVEMENT_ID";
        public const string UNLOCKABLE_IDENTIFIER = "ROB_DRIVER_BODY_MONSOONUNLOCKABLE_REWARD_ID";

        public static Sprite Sprite => Assets.mainAssetBundle.LoadAsset<Sprite>("texMonsoonSkin");

        public override float RequiredDifficultyCoefficient => 3f;
        public override DifficultyIndex MinimumDifficultyIndex => DifficultyIndex.Hard;
    }
}