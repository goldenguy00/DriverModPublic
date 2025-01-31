using RoR2;
using UnityEngine;

namespace RobDriver.Modules.Achievements
{
    //string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, uint lunarCoinReward, Type serverTrackerType = null
    //automatically creates language tokens "ACHIEVEMENT_{identifier.ToUpper()}_NAME" and "ACHIEVEMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    internal class MasteryAchievement : BaseMasteryAchievement
    {
        public const string identifier = "ROB_DRIVER_MONSOON";
        public const string nameToken = "ACHIEVEMENT_ROB_DRIVER_MONSOON_NAME";
        public const string unlockableIdentifier = "ROB_DRIVER_MONSOON_UNLOCKABLE";

        public static Sprite Sprite => Assets.mainAssetBundle.LoadAsset<Sprite>("texMonsoonSkin");

        //difficulty coeff 3 is monsoon. 3.5 is typhoon for grandmastery skins
        public override float RequiredDifficultyCoefficient => 3;
        public override DifficultyIndex RequiredEclipseLevel => DifficultyIndex.Eclipse1;
    }
}