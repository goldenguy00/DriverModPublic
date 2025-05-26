using RoR2;
using RoR2.Achievements;

namespace RobDriver.Modules.Achievements
{
    public abstract class BaseDifficultyAchievement : BaseAchievement
    {
        public abstract float RequiredDifficultyCoefficient { get; }
        public abstract DifficultyIndex MinimumDifficultyIndex { get; }

        public override BodyIndex LookUpRequiredBodyIndex() => Survivors.Driver.bodyIndex;

        public override void OnBodyRequirementMet()
        {
            base.OnBodyRequirementMet();

            Run.onClientGameOverGlobal += this.Run_OnClientGameOverGlobal;
        }

        public override void OnBodyRequirementBroken()
        {
            base.OnBodyRequirementBroken();

            Run.onClientGameOverGlobal -= this.Run_OnClientGameOverGlobal;
        }

        protected virtual void Run_OnClientGameOverGlobal(Run run, RunReport runReport)
        {
            if (base.meetsBodyRequirement && runReport?.gameEnding && runReport.gameEnding.isWin)
            {
                var difficultyIndex = runReport.ruleBook.FindDifficulty();
                var difficultyDef = DifficultyCatalog.GetDifficultyDef(difficultyIndex);
                if (difficultyDef != null)
                {
                    var isDifficulty = difficultyDef.countsAsHardMode && difficultyDef.scalingValue >= RequiredDifficultyCoefficient;
                    var isInferno = difficultyDef.nameToken == "INFERNO_NAME";
                    var isEclipse = difficultyIndex <= DifficultyIndex.Eclipse8 && difficultyIndex >= MinimumDifficultyIndex;

                    if (isDifficulty || isInferno || isEclipse)
                        Grant();
                }
            }
        }
    }
}
