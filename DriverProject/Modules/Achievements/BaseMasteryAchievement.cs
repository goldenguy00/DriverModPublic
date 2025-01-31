using RoR2;
using RoR2.Achievements;

namespace RobDriver.Modules.Achievements
{
    public abstract class BaseMasteryAchievement : BaseAchievement
    {
        public abstract float RequiredDifficultyCoefficient { get; }
        public abstract DifficultyIndex RequiredEclipseLevel { get; }

        public override BodyIndex LookUpRequiredBodyIndex() => Survivors.Driver.bodyIndex;

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();
            Run.onClientGameOverGlobal += this.OnClientGameOverGlobal;
        }

        public override void OnBodyRequirementBroken()
        {
            Run.onClientGameOverGlobal -= this.OnClientGameOverGlobal;
            base.OnBodyRequirementBroken();
        }

        private void OnClientGameOverGlobal(Run run, RunReport runReport)
        {
            if (runReport?.gameEnding && runReport.gameEnding.isWin)
            {
                var difficultyIndex = runReport.ruleBook.FindDifficulty();
                var difficultyDef = DifficultyCatalog.GetDifficultyDef(difficultyIndex);
                if (difficultyDef != null)
                {
                    var isDifficulty = difficultyDef.countsAsHardMode && difficultyDef.scalingValue >= RequiredDifficultyCoefficient;
                    var isInferno = difficultyDef.nameToken == "INFERNO_NAME";
                    var isEclipse = difficultyIndex <= DifficultyIndex.Eclipse8 && difficultyIndex >= RequiredEclipseLevel;

                    if (isDifficulty || isInferno || isEclipse)
                        Grant();
                }
            }
        }
    }
}
