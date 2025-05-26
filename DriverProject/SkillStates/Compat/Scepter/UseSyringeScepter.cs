using RobDriver.SkillStates.Driver;
using RoR2;

namespace RobDriver.SkillStates.Driver.Scepter
{
    public class UseSyringeScepter : UseSyringe
    {
        protected override float buffDuration => 8f;

        protected override void SelectBuffServer()
        {
            this.characterBody.AddTimedBuff(DLC1Content.Buffs.KillMoveSpeed, buffDuration);
            base.ApplyBuffServer(Modules.Buffs.syringeScepterBuff, Modules.Assets.scepterSyringeBuffEffectPrefab, Modules.Assets.scepterSyringeBuffEffectPrefab2, Modules.Assets.syringeScepterOverlayMat);
        }
    }
}