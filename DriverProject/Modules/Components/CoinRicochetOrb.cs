using R2API;
using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class CoinRicochetOrb : GenericDamageOrb
    {
        public static float redDamageCoefficient = 16f;

        public static float searchRadius = 50f;
        public BullseyeSearch search;
        public Vector3 coinPosition;
        public DriverController iDrive;
        public GameObject inflictor;
        public int bounceCount = 1;

        public float damageCoefficient = 1f;

        public override void Begin()
        {
            this.target = PickNextTarget(this.coinPosition);

            this.duration = this.distanceToTarget / this.speed;

            Color color = Color.Lerp(Color.yellow, Color.red, damageCoefficient / redDamageCoefficient);
            float scale = Mathf.Lerp(1, 2f, damageCoefficient / redDamageCoefficient);
            EffectData effectData = new EffectData
            {
                scale = this.scale * scale,
                origin = this.coinPosition,
                genericFloat = this.duration,
                color = color
            };
            effectData.SetHurtBoxReference(this.target);
            EffectManager.SpawnEffect(Assets.coinOrbEffect, effectData, true);
        }


        public override void OnArrival()
        {
            if (this.target)
            {
                HealthComponent healthComponent = target.healthComponent;
                if (healthComponent)
                {
                    if (!target.TryGetComponent<CoinController>(out var iCoin) && bounceCount > 1)
                    {
                        BlastAttack blastAttack = new BlastAttack
                        {
                            baseDamage = this.damageValue,
                            attacker = this.attacker,
                            inflictor = this.inflictor,
                            teamIndex = this.teamIndex,
                            baseForce = 1000f,
                            bonusForce = Vector3.zero,
                            crit = this.isCrit,
                            procChainMask = this.procChainMask,
                            procCoefficient = this.procCoefficient,
                            falloffModel = BlastAttack.FalloffModel.Linear,
                            position = target.transform.position,
                            radius = bounceCount * 2f,
                            damageColorIndex = this.damageColorIndex,
                            damageType = iDrive.DamageType
                        };
                        blastAttack.AddModdedDamageType(iDrive.ModdedDamageType);
                        blastAttack.AddModdedDamageType(DriverDamageTypes.BloodExplosionIdentifier);
                        blastAttack.Fire();

                        EffectData effectData = new EffectData
                        {
                            origin = target.transform.position,
                            scale = bounceCount
                        };
                        EffectManager.SpawnEffect(Assets.coinImpact, effectData, transmit: true);
                    }
                    else
                    {
                        DamageInfo damageInfo = new DamageInfo
                        {
                            damage = this.damageValue,
                            attacker = this.attacker,
                            inflictor = this.inflictor,
                            force = Vector3.zero,
                            crit = this.isCrit,
                            procChainMask = this.procChainMask,
                            procCoefficient = this.procCoefficient,
                            position = this.target.transform.position,
                            damageColorIndex = this.damageColorIndex
                        };

                        if (iCoin)
                        {
                            damageInfo.procCoefficient = 0f;
                            iCoin.bounceCountStored = bounceCount;
                            healthComponent.TakeDamage(damageInfo);
                        }
                        else
                        {
                            damageInfo.damageType = DamageType.Stun1s | iDrive.DamageType;
                            damageInfo.AddModdedDamageType(DriverDamageTypes.BloodExplosionIdentifier);
                            damageInfo.AddModdedDamageType(iDrive.ModdedDamageType);

                            healthComponent.TakeDamage(damageInfo);
                            GlobalEventManager.instance.OnHitEnemy(damageInfo, healthComponent.gameObject);
                            GlobalEventManager.instance.OnHitAll(damageInfo, healthComponent.gameObject);
                        }
                    }
                }
            }
        }

        public HurtBox PickNextTarget(Vector3 position)
        {
            HurtBox target = null;

            this.search = new BullseyeSearch
            {
                queryTriggerInteraction = QueryTriggerInteraction.Ignore,
                filterByDistinctEntity = true,
                filterByLoS = false,
                sortMode = BullseyeSearch.SortMode.Distance,
                teamMaskFilter = TeamMask.all,
                maxDistanceFilter = searchRadius,
                searchOrigin = position
            };
            search.RefreshCandidates();

            foreach (var hurtBox in search.GetResults())
            {
                if (hurtBox.healthComponent.TryGetComponent<CoinController>(out var coin) && coin.canRicochet)
                {
                    target = hurtBox;
                    break;
                }

                if (!target && hurtBox.healthComponent.body && this.teamIndex != hurtBox.healthComponent.body.teamComponent.teamIndex)
                {
                    target = hurtBox;
                }
            }
            return target;
        }
    }

}