using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace RobDriver.Modules.Achievements
{
    //string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, uint lunarCoinReward, Type serverTrackerType = null
    //automatically creates language tokens "ACHIEVEMENT_{identifier.ToUpper()}_NAME" and "ACHIEVEMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(IDENTIFIER, UNLOCKABLE_IDENTIFIER, null, 10, null)]
    internal class DriverUnlockAchievement : BaseAchievement
    {
        public const string IDENTIFIER = "ROB_DRIVER_BODY_UNLOCKABLE_ACHIEVEMENT_ID";
        public const string UNLOCKABLE_IDENTIFIER = "ROB_DRIVER_BODY_UNLOCKABLE_REWARD_ID"; 
        public static Sprite Sprite => Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texDriverAchievement");

        public override void OnInstall()
        {
            base.OnInstall();

            CharacterBody.onBodyStartGlobal += OnBodyStartGlobal;
        }

        public override void OnUninstall()
        {
            base.OnUninstall();

            CharacterBody.onBodyStartGlobal -= OnBodyStartGlobal;
        }

        private void OnBodyStartGlobal(CharacterBody characterBody)
        {
            if (Run.instance && Run.instance.stageClearCount >= 2 && Run.instance.time <= 900f)
            {
                base.Grant();
            }
        }

    }
}