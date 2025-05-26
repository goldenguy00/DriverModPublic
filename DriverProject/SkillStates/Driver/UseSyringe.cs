using RoR2;

namespace RobDriver.SkillStates.Driver
{
    public class UseSyringe : UseSyringeLegacy
    {
        protected override void SelectBuffServer()
        {
            this.characterBody.AddTimedBuff(DLC1Content.Buffs.KillMoveSpeed, buffDuration);
            base.ApplyBuffServer(Modules.Buffs.syringeNewBuff, Modules.Assets.scepterSyringeBuffEffectPrefab, Modules.Assets.damageBuffEffectPrefab2, Modules.Assets.syringeDamageOverlayMat);
        }
    }
}