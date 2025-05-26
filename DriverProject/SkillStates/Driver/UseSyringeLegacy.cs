using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using EntityStates;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver
{
    public class UseSyringeLegacy : BaseDriverSkillState
    {
        protected virtual float buffDuration => 6f;
        protected override bool cancelOnPickup => false;
        protected override string showProp => "SyringeModel";

        protected CharacterModel characterModel;

        private float baseDuration = 1.2f;
        private bool drugged;
        private float duration;

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = this.baseDuration / this.attackSpeedStat;
            this.characterModel = base.GetModelTransform()?.GetComponent<CharacterModel>();

            base.PlayAnimation("Gesture, Override", "UseSyringe", "Action.playbackRate", this.duration);
            Util.PlaySound("sfx_driver_foley_syringe", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.drugged)
            {
                if (base.fixedAge >= (0.5f * this.duration))
                {
                    this.drugged = true;

                    Util.PlaySound("sfx_driver_injection", this.gameObject);
                    Util.PlaySound("sfx_driver_syringe_buff", this.gameObject);

                    if (NetworkServer.active)
                    {
                        this.SelectBuffServer();
                    }
                }
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
            }
        }

        protected virtual void SelectBuffServer()
        {
            int i = Random.Range(0, 3);

            switch (i)
            {
                case 0:
                    ApplyBuffServer(Modules.Buffs.syringeDamageBuff, Modules.Assets.damageBuffEffectPrefab, Modules.Assets.damageBuffEffectPrefab2, Modules.Assets.syringeDamageOverlayMat);
                    break;
                case 1:
                    ApplyBuffServer(Modules.Buffs.syringeAttackSpeedBuff, Modules.Assets.attackSpeedBuffEffectPrefab, Modules.Assets.attackSpeedBuffEffectPrefab2, Modules.Assets.syringeAttackSpeedOverlayMat);
                    break;
                case 2:
                    ApplyBuffServer(Modules.Buffs.syringeCritBuff, Modules.Assets.critBuffEffectPrefab, Modules.Assets.critBuffEffectPrefab2, Modules.Assets.syringeCritOverlayMat);
                    break;
            }
        }

        protected virtual void ApplyBuffServer(BuffDef buff, GameObject muzzlePrefab, GameObject bodyPrefab, Material overlayMat)
        {
            this.characterBody.AddTimedBuff(buff, buffDuration);

            if (muzzlePrefab)
            {
                EffectManager.SpawnEffect(muzzlePrefab, new EffectData
                {
                    origin = this.FindModelChild("PistolMuzzle").position,
                    rotation = Quaternion.identity
                }, true);
            }

            if (bodyPrefab)
            {
                EffectManager.SpawnEffect(bodyPrefab, new EffectData
                {
                    origin = this.transform.position + new Vector3(0f, 0.5f, 0f),
                    rotation = Quaternion.identity,
                    rootObject = this.gameObject
                }, true);
            }

            if (this.characterModel && overlayMat)
            {
                var temporaryOverlay = TemporaryOverlayManager.AddOverlay(this.characterModel.gameObject);
                temporaryOverlay.duration = 12f;
                temporaryOverlay.animateShaderAlpha = true;
                temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                temporaryOverlay.destroyComponentOnEnd = true;
                temporaryOverlay.originalMaterial = overlayMat;
                temporaryOverlay.AddToCharacterModel(this.characterModel);
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }
    }
}