using RoR2;
using UnityEngine;

namespace RobDriver.SkillStates.Driver
{
    public class UseSyringeScepter : UseSyringe
    {
        protected override void ApplyBuff()
        {
            this.characterBody.AddTimedBuff(Modules.Buffs.syringeScepterBuff, 6f);
            this.characterBody.AddTimedBuff(DLC1Content.Buffs.KillMoveSpeed, 6f);

            EffectManager.SpawnEffect(Modules.Assets.scepterSyringeBuffEffectPrefab, new EffectData
            {
                origin = this.FindModelChild("PistolMuzzle").position,
                rotation = Quaternion.identity
            }, true);

            EffectManager.SpawnEffect(Modules.Assets.scepterSyringeBuffEffectPrefab2, new EffectData
            {
                origin = this.transform.position + new Vector3(0f, 0.5f, 0f),
                rotation = Quaternion.identity,
                rootObject = this.gameObject
            }, true);

            if (this.modelTransform)
            {
                var temporaryOverlay = TemporaryOverlayManager.AddOverlay(modelTransform.gameObject);
                temporaryOverlay.duration = 12f;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = Modules.Assets.syringeScepterOverlayMat;
                temporaryOverlay.AddToCharacterModel(this.modelTransform.GetComponent<CharacterModel>());
            }
        }
    }
}