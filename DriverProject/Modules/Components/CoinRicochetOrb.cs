using System.Collections.Generic;
using System.Linq;
using R2API;
using RoR2;
using RoR2.Orbs;
using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class CoinRicochetOrb : GenericDamageOrb
    {
        public static float searchRadius = 50f;
        public static float redDamageCoefficient = 32f;

        public int bounceCount;
        public float ownerBaseDamage;
        public Color color;
        public bool scepter;
        public bool nerfed = true;

        public override void Begin()
        {
            this.target ??= this.scepter ? PickNextTargetScepter() : PickNextTarget();

            this.damageType.damageSource = DamageSource.Special;
            this.duration = Time.fixedDeltaTime + (this.distanceToTarget / this.speed);

            var damageCoefficient = this.damageValue / this.ownerBaseDamage;
            this.color = Color.Lerp(this.color, Color.red, damageCoefficient / redDamageCoefficient);
            this.scale = Mathf.Lerp(1f, 4f, damageCoefficient / redDamageCoefficient);

            EffectData effectData = new EffectData
            {
                scale = this.scale,
                origin = this.origin,
                genericFloat = this.duration,
                color = this.color
            };
            effectData.SetHurtBoxReference(this.target);
            EffectManager.SpawnEffect(Assets.coinOrbEffect, effectData, true);
        }

        public override void OnArrival()
        {
            var targetHc = this.target ? this.target.healthComponent : null;
            if (!targetHc)
                return;

            this.bounceCount++;
            this.damageValue += this.damageValue * (this.bounceCount / 4f);

            var targetCoinController = this.target.healthComponent.GetComponent<CoinController>();
            if (targetCoinController)
                targetCoinController.bounceCountStored = bounceCount;
            else
                this.damageType.AddModdedDamageType(DriverDamageTypes.BloodExplosionIdentifier);

            if (bounceCount > 2 && !targetCoinController)
            {
                new BlastAttack
                {
                    baseDamage = this.damageValue,
                    attacker = this.attacker,
                    teamIndex = this.teamIndex,
                    crit = this.isCrit,
                    procChainMask = this.procChainMask,
                    procCoefficient = this.procCoefficient,
                    falloffModel = BlastAttack.FalloffModel.Linear,
                    position = this.target.transform.position,
                    radius = this.scale * this.bounceCount,
                    damageColorIndex = this.damageColorIndex,
                    damageType = this.damageType
                }.Fire();
            }
            else
            {
                DamageInfo damageInfo = new DamageInfo
                {
                    damage = this.damageValue,
                    attacker = this.attacker,
                    crit = this.isCrit,
                    procChainMask = this.procChainMask,
                    procCoefficient = this.procCoefficient,
                    position = this.target.transform.position,
                    damageColorIndex = this.damageColorIndex,
                    damageType = this.damageType
                };

                targetHc.TakeDamage(damageInfo);
                if (!targetCoinController)
                {
                    GlobalEventManager.instance.OnHitEnemy(damageInfo, targetHc.gameObject);
                    GlobalEventManager.instance.OnHitAll(damageInfo, targetHc.gameObject);
                }
            }

            EffectManager.SpawnEffect(Assets.coinImpact, new EffectData
            {
                origin = this.target.transform.position,
                scale = this.scale
            }, transmit: true);
        }

        public HurtBox PickNextTarget()
        {
            var search = new BullseyeSearch
            {
                queryTriggerInteraction = QueryTriggerInteraction.Ignore,
                filterByDistinctEntity = true,
                filterByLoS = false,
                sortMode = BullseyeSearch.SortMode.Distance,
                teamMaskFilter = TeamMask.AllExcept(this.teamIndex),
                maxDistanceFilter = searchRadius,
                searchOrigin = this.origin
            };
            search.RefreshCandidates();

            HurtBox target = null;
            foreach (var hurtBox in search.GetResults())
            {
                if (hurtBox.healthComponent.GetComponent<CoinController>())
                {
                    return hurtBox;
                }
                
                if (!target)
                {
                    target = hurtBox;
                }
            }

            return target;
        }

        public HurtBox PickNextTargetScepter()
        {
            var search = new BullseyeSearch
            {
                queryTriggerInteraction = QueryTriggerInteraction.Ignore,
                filterByDistinctEntity = true,
                filterByLoS = false,
                sortMode = BullseyeSearch.SortMode.Distance,
                teamMaskFilter = TeamMask.AllExcept(this.teamIndex),
                maxDistanceFilter = searchRadius,
                searchOrigin = this.origin
            };
            search.RefreshCandidates();

            HurtBox target = null;
            List<HurtBox> enemyTargets = HG.ListPool<HurtBox>.RentCollection();
            foreach (var hurtBox in search.GetResults())
            {
                if (hurtBox.healthComponent.GetComponent<CoinController>())
                {
                    if (!target)
                        target = hurtBox;
                }
                else
                {
                    enemyTargets.Add(hurtBox);
                }
            }

            if (target && nerfed)
                return target;

            target ??= enemyTargets.FirstOrDefault();
            foreach (var hurtBox in enemyTargets)
            {
                if (hurtBox == target)
                    continue;

                OrbManager.instance.AddOrb(new CoinRicochetOrb()
                {
                    target = hurtBox,
                    color = this.color,
                    origin = this.origin,
                    speed = this.speed,
                    attacker = this.attacker,
                    damageValue = this.damageValue / enemyTargets.Count,
                    damageType = this.damageType,
                    teamIndex = this.teamIndex,
                    procCoefficient = this.procCoefficient,
                    isCrit = this.isCrit,
                    bounceCount = this.bounceCount,
                    ownerBaseDamage = this.ownerBaseDamage
                });
            }

            HG.ListPool<HurtBox>.ReturnCollection(enemyTargets);
            return target;
        }
    }

}