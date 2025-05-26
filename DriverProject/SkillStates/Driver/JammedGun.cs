using EntityStates;
using RobDriver.SkillStates.BaseStates;
using RoR2;
using UnityEngine;

namespace RobDriver.SkillStates.Driver
{
    public class JammedGun : BaseDriverSkillState
    {
        public float duration = 5f;

        public override void OnEnter()
        {
            base.OnEnter();

            base.PlayAnimation("Gesture, Override", "GunJammed", "Action.playbackRate", this.duration);

            EffectData effectData = new EffectData
            {
                origin = this.FindModelChild("PistolMuzzle").position,
                rotation = Quaternion.identity
            };
            EffectManager.SpawnEffect(Modules.Assets.jammedEffectPrefab, effectData, false);

            Util.PlaySound("sfx_driver_gun_jammed", this.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= this.duration)
                this.outer.SetNextStateToMain();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Death;
        }
    }
}