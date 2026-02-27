using UnityEngine;
using RoR2;
using RoR2.Orbs;

namespace RobDriver.Modules.Components
{
    public class ConsumeOrb : Orb
    {
        public override void Begin()
        {
            base.duration = Random.Range(1f, 3f);

            EffectData effectData = new EffectData
            {
                origin = this.origin,
                genericFloat = base.duration
            };

            effectData.SetHurtBoxReference(this.target);

            GameObject effectPrefab = Modules.Assets.consumeOrb;

            EffectManager.SpawnEffect(effectPrefab, effectData, true);
        }

        public override void OnArrival()
        {
            if (this.target && this.target.healthComponent)
            {
                this.target.healthComponent.HealFraction(0.1f, default(ProcChainMask));
            }
        }
    }
}