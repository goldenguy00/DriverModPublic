using RoR2;
using RoR2.Achievements;
using UnityEngine;

namespace RobDriver.Modules.Achievements
{
    //string identifier, string unlockableRewardIdentifier, string prerequisiteAchievementIdentifier, uint lunarCoinReward, Type serverTrackerType = null
    //automatically creates language tokens "ACHIEVEMENT_{identifier.ToUpper()}_NAME" and "ACHIEVEMENT_{identifier.ToUpper()}_DESCRIPTION" 
    [RegisterAchievement(identifier, unlockableIdentifier, null, 10, null)]
    internal class DriverGodslingPassiveAchievement : BaseAchievement
    {
        public const string identifier = "ROB_DRIVER_GODSLING";
        public const string nameToken = "ACHIEVEMENT_ROB_DRIVER_GODSLING_NAME";
        public const string unlockableIdentifier = "ROB_DRIVER_GODSLING_UNLOCKABLE";
        public static Sprite Sprite => Assets.mainAssetBundle.LoadAsset<Sprite>("texAltPassiveIcon");

        public static bool weaponPickedUpHard;

        public override BodyIndex LookUpRequiredBodyIndex() => Survivors.Driver.bodyIndex;

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();

            Run.onClientGameOverGlobal += this.OnClientGameOverGlobal;
            Run.onRunStartGlobal += this.OnRunStartGlobal;
        }

        public override void OnBodyRequirementBroken()
        {
            Run.onClientGameOverGlobal -= this.OnClientGameOverGlobal;
            Run.onRunStartGlobal -= this.OnRunStartGlobal;

            base.OnBodyRequirementBroken();
        }

        private void OnRunStartGlobal(Run run) => weaponPickedUpHard = false;

        public void OnClientGameOverGlobal(Run run, RunReport runReport)
        {
            if (runReport?.gameEnding && runReport.gameEnding.isWin && !weaponPickedUpHard)
            {
                var difficultyIndex = runReport.ruleBook.FindDifficulty();
                var difficultyDef = DifficultyCatalog.GetDifficultyDef(difficultyIndex);
                if (difficultyDef != null)
                {
                    var isDifficulty = difficultyDef.countsAsHardMode && difficultyDef.scalingValue >= 3f;
                    var isInferno = difficultyDef.nameToken == "INFERNO_NAME";
                    var isEclipse = difficultyIndex <= DifficultyIndex.Eclipse8 && difficultyIndex >= DifficultyIndex.Eclipse1;

                    if (isDifficulty || isInferno || isEclipse)
                        Grant();
                }
            }
        }
    }
}