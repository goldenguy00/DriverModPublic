using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using EntityStates;

namespace RobDriver.SkillStates.Driver
{
    public class UseSyringe : BaseDriverSkillState
    {
        public float baseDuration = 1.2f;

        protected override string prop => "SyringeModel";
        private bool farded;
        private float duration;
        protected Transform modelTransform;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.modelTransform = this.GetModelTransform();

            base.PlayAnimation("Gesture, Override", "UseSyringe", "Action.playbackRate", this.duration);
            Util.PlaySound("sfx_driver_foley_syringe", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.farded)
            {
                if (base.fixedAge >= (0.5f * this.duration))
                {
                    this.farded = true;

                    Util.PlaySound("sfx_driver_injection", this.gameObject);
                    Util.PlaySound("sfx_driver_syringe_buff", this.gameObject);

                    if (NetworkServer.active)
                    {
                        this.ApplyBuff();
                    }
                }
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextState(new WaitForReload());
            }
        }

        protected virtual void ApplyBuff()
        {
            this.characterBody.AddTimedBuff(Modules.Buffs.syringeNewBuff, 6f);
            this.characterBody.AddTimedBuff(DLC1Content.Buffs.KillMoveSpeed, 6f);
            /*EffectManager.SpawnEffect(Modules.Assets.damageBuffEffectPrefab, new EffectData
            {
                origin = this.FindModelChild("PistolMuzzle").position,
                rotation = Quaternion.identity
            }, true);*/
            EffectManager.SpawnEffect(Modules.Assets.damageBuffEffectPrefab2, new EffectData
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
                temporaryOverlay.originalMaterial = Modules.Assets.syringeDamageOverlayMat;
                temporaryOverlay.AddToCharacterModel(this.modelTransform.GetComponent<CharacterModel>());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}