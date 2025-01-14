using RoR2;
using RoR2.Achievements;
using System;
using UnityEngine;

namespace RobDriver.Modules.Achievements
{
    //string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, uint lunarCoinReward, Type serverTrackerType = null
    //automatically creates language tokens "ACHIEVEMENT_{identifier.ToUpper()}_NAME" and "ACHIEVEMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    internal class DriverUnlockAchievement : BaseAchievement
    {
        public const string identifier = "ROB_DRIVER_BODY_UNLOCK";
        public const string nameToken = "ACHIEVEMENT_ROB_DRIVER_BODY_UNLOCK_NAME";
        public const string unlockableIdentifier = "ROB_DRIVER_BODY_UNLOCK_UNLOCKABLE"; 
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