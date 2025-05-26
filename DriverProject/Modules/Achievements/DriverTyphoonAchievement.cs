using RoR2;
using UnityEngine;

namespace RobDriver.Modules.Achievements
{
    //string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, uint lunarCoinReward, Type serverTrackerType = null
    //automatically creates language tokens "ACHIEVEMENT_{identifier.ToUpper()}_NAME" and "ACHIEVEMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(IDENTIFIER, UNLOCKABLE_IDENTIFIER, DriverUnlockAchievement.IDENTIFIER, 10, null)]
    internal class DriverTyphoonAchievement : BaseDifficultyAchievement
    {
        public const string IDENTIFIER = "ROB_DRIVER_BODY_TYPHOON_UNLOCKABLE_ACHIEVEMENT_ID";
        public const string UNLOCKABLE_IDENTIFIER = "ROB_DRIVER_BODY_TYPHOON_UNLOCKABLE_REWARD_ID";

        public static Sprite Sprite => Assets.mainAssetBundle.LoadAsset<Sprite>("texTyphoonSkin");

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3.5f;
        public override DifficultyIndex MinimumDifficultyIndex => DifficultyIndex.Eclipse8;
    }
}