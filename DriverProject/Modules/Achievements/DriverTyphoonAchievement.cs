using RoR2;
using UnityEngine;

namespace RobDriver.Modules.Achievements
{
    //string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, uint lunarCoinReward, Type serverTrackerType = null
    //automatically creates language tokens "ACHIEVEMENT_{identifier.ToUpper()}_NAME" and "ACHIEVEMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    internal class GrandMasteryAchievement : BaseMasteryAchievement
    {
        public const string identifier = "ROB_DRIVER_TYPHOON";
        public const string nameToken = "ACHIEVEMENT_ROB_DRIVER_TYPHOON_NAME";
        public const string unlockableIdentifier = "ROB_DRIVER_TYPHOON_UNLOCKABLE";

        public static Sprite Sprite => Assets.mainAssetBundle.LoadAsset<Sprite>("texTyphoonSkin");

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3.5f;
        public override DifficultyIndex RequiredEclipseLevel => DifficultyIndex.Eclipse8;
    }
}