using R2API.Networking;
using RobDriver.Modules.Components;
using RoR2;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;
using MaterialHud;
using System.Linq;
using TMPro;
using UnityEngine.AddressableAssets;
using R2API;
using R2API.Networking.Interfaces;
using RoR2.UI;
using RoR2.CharacterAI;
using RoR2.Projectile;
using System.Collections.Generic;
using System;
using HG;
using RoR2.Orbs;
using RobDriver.Modules.Components.UI;
using RoR2.Skills;
using RobDriver.Modules.Survivors;

namespace RobDriver.Modules.Misc
{
    public static class DriverHooks
    {
        internal static void Init()
        {
            if (Config.dynamicCrosshairUniversal.Value)
                On.RoR2.UI.CrosshairController.Awake += CrosshairController_Awake;

            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;

            On.RoR2.HealthComponent.TakeDamageProcess += HealthComponent_TakeDamageProcess;
            On.RoR2.GlobalEventManager.ProcessHitEnemy += GlobalEventManager_ProcessHitEnemy;
            On.RoR2.GlobalEventManager.OnHitAllProcess += GlobalEventManager_OnHitAllProcess;
            GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;

            HUD.onHudTargetChangedGlobal += HUDSetup;
            On.RoR2.UI.HGButton.Start += HGButton_Start;

            On.RoR2.SkillLocator.ApplyAmmoPack += SkillLocator_ApplyAmmoPack;
            On.RoR2.SkillLocator.ResetSkills += SkillLocator_ResetSkills;

            // dazed debuff
            On.EntityStates.AI.BaseAIState.AimAt += BaseAIState_AimAt;
            On.EntityStates.AI.BaseAIState.AimInDirection += BaseAIState_AimInDirection;

            On.RoR2.UI.GameEndReportPanelController.AssignUnlockToStrip += GameEndReportPanelController_AssignUnlockToStrip;

            // heresy anims
            On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.OnEnter += PlayVisionsAnimation;
            On.EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary.PlayChargeAnimation += PlayChargeLunarAnimation;
            On.EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary.PlayThrowAnimation += PlayThrowLunarAnimation;
            On.EntityStates.GlobalSkills.LunarDetonator.Detonate.OnEnter += PlayRuinAnimation;

            // fix heresy stocks when assigned without the item
            On.RoR2.Skills.LunarPrimaryReplacementSkill.GetMaxStock += LunarPrimaryReplacementSkill_GetMaxStock;
            On.RoR2.Skills.LunarPrimaryReplacementSkill.GetRechargeInterval += LunarPrimaryReplacementSkill_GetRechargeInterval;
            On.RoR2.Skills.LunarSecondaryReplacementSkill.GetRechargeInterval += LunarSecondaryReplacementSkill_GetRechargeInterval;

            On.RoR2.MapZone.TryZoneStart += MapZone_TryZoneStart;

            if (Config.enableArsenal.Value)
            {
                On.RoR2.UI.LoadoutPanelController.Row.UpdateHighlightedChoice += Row_UpdateHighlightedChoice;
                On.RoR2.UI.CharacterSelectController.BuildSkillStripDisplayData += CharacterSelectController_BuildSkillStripDisplayData;
            }
        }

        #region Damage Handling
        private static void RecalculateStatsAPI_GetStatCoefficients(CharacterBody self, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (!self)
                return;

            if (self.HasBuff(Buffs.woundDebuff))
                args.armorAdd -= 40f;

            if (self.HasBuff(Buffs.syringeDamageBuff))
                args.levelDamageAdd += 2f;

            if (self.HasBuff(Buffs.syringeAttackSpeedBuff))
                args.baseAttackSpeedAdd += 0.5f;

            if (self.HasBuff(Buffs.syringeCritBuff))
                args.critAdd += 30f;

            if (self.HasBuff(Buffs.syringeNewBuff))
            {
                args.baseAttackSpeedAdd += 0.5f;
                args.baseRegenAdd += 5f;
            }

            if (self.HasBuff(Buffs.syringeScepterBuff))
            {
                args.levelDamageAdd += 2.5f;
                args.baseAttackSpeedAdd += 0.75f;
                args.critAdd += 30f;
                args.critDamageMultAdd += 0.2f;
                args.levelRegenAdd += 2f;
            }
        }

        private static void HealthComponent_TakeDamageProcess(On.RoR2.HealthComponent.orig_TakeDamageProcess orig, HealthComponent self, DamageInfo damageInfo)
        {
            var applyGouge = false;

            if (NetworkServer.active && self.body && self.alive)
            {
                if (damageInfo.HasModdedDamageType(DriverDamageTypes.StunGrenadeDazed))
                    self.body.AddTimedBuff(Buffs.dazedDebuff, 10f);

                if ((damageInfo.damageType.IsDamageSourceSkillBased && self.body.HasBuff(Buffs.woundDebuff)) || damageInfo.HasModdedDamageType(DriverDamageTypes.KnifeWound))
                {
                    self.body.AddTimedBuff(Buffs.woundDebuff, 4f);

                    if (self.TryGetComponent<NetworkIdentity>(out var identity))
                        new SyncOverlay(identity.netId, self.gameObject).Send(NetworkDestination.Clients);
                }

                if (damageInfo.dotIndex == DriverDamageTypes.GougeDotIndex && damageInfo.procCoefficient == 0f)
                {
                    applyGouge = true;
                    damageInfo.procCoefficient = 0.2f;

                    if (damageInfo.attacker && damageInfo.attacker.TryGetComponent<CharacterBody>(out var attackerBody))
                        damageInfo.crit = Util.CheckRoll(attackerBody.crit, attackerBody.master);
                }

                if (damageInfo.damageType.damageTypeExtended.HasFlag(DamageTypeExtended.DamagePercentOfMaxHealth))
                {
                    var attackerBody = damageInfo.attacker ? damageInfo.attacker.GetComponent<CharacterBody>() : null;
                    if (attackerBody && attackerBody.bodyIndex == Driver.bodyIndex)
                    {
                        damageInfo.damageType.damageTypeExtended &= ~DamageTypeExtended.DamagePercentOfMaxHealth;
                        damageInfo.damage += 0.05f * self.combinedHealth;
                    }
                }
            }

            orig(self, damageInfo);

            if (applyGouge && self.alive && !damageInfo.rejected)
                GlobalEventManager.instance.OnHitEnemy(damageInfo, self.gameObject);
        }
        #endregion

        #region Global Event Manager
        private static void GlobalEventManager_ProcessHitEnemy(On.RoR2.GlobalEventManager.orig_ProcessHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            CharacterBody attackerBody = null;
            CharacterBody victimBody = null;

            var isDriver = NetworkServer.active && damageInfo.procCoefficient > 0f && damageInfo.damageType.IsDamageSourceSkillBased;
            if (isDriver)
            {
                attackerBody = damageInfo.attacker ? damageInfo.attacker.GetComponent<CharacterBody>() : null;
                isDriver = attackerBody && attackerBody.bodyIndex == Driver.bodyIndex && attackerBody.healthComponent && attackerBody.inventory && attackerBody.master;
            }

            if (isDriver)
            {
                damageInfo.damageType.RemoveModdedDamageType(DriverDamageTypes.Generic);
                isDriver = damageInfo.damageType.HasAnyModdedDamageType();
            }

            if (isDriver)
            {
                victimBody = victim ? victim.GetComponent<CharacterBody>() : null;
                isDriver = victimBody && victimBody.healthComponent;
            }

            if (!isDriver)
            {
                orig(self, damageInfo, victim);
                return;
            }

            var procChance = 100f * damageInfo.procCoefficient;

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.MysteryShot))
            {
                damageInfo.RemoveModdedDamageType(DriverDamageTypes.MysteryShot);

                var bulletInfo = DriverBulletCatalog.GetWeightedRandomBullet(DriverWeaponTier.Unique);
                if (bulletInfo != DriverBulletCatalog.Mystery)
                    damageInfo.damageType |= bulletInfo.damageType;
            }

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.VoidMissileShot))
            {
                damageInfo.procChainMask.AddProc(ProcType.MicroMissile);

                var icbmCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                var missileCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.MissileVoid) + attackerBody.inventory.GetItemCount(RoR2Content.Items.Missile);
                var damageValue = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 0.4f + 0.4f * missileCount) * DriverPlugin.GetICBMDamageMult(attackerBody);

                for (var i = 0; i < (icbmCount == 0 ? 1 : 3); i++)
                {
                    OrbManager.instance.AddOrb(new MissileVoidOrb
                    {
                        origin = attackerBody.aimOrigin,
                        damageValue = damageValue,
                        isCrit = damageInfo.crit,
                        teamIndex = attackerBody.teamComponent.teamIndex,
                        attacker = damageInfo.attacker,
                        procChainMask = damageInfo.procChainMask,
                        procCoefficient = 0.2f,
                        damageColorIndex = DamageColorIndex.Void,
                        target = victimBody.mainHurtBox
                    });
                }
            } // end plimp

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.CoinShot))
            {
                OrbManager.instance.AddOrb(new GoldOrb
                {
                    origin = damageInfo.position,
                    target = attackerBody.mainHurtBox,
                    goldAmount = (uint)(2f * Run.instance.difficultyCoefficient)
                });

                EffectManager.SimpleImpactEffect(
                    LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/CoinImpact"),
                    damageInfo.position,
                    Vector3.up,
                    transmit: true);
            } // end moneyshot

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.IceBlastShot))
            {
                var itemCount = attackerBody.inventory.GetItemCount(RoR2Content.Items.IceRing);
                var damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 1.25f + 1.25f * itemCount);

                EffectManager.SimpleImpactEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/IceRingExplosion"), damageInfo.position, Vector3.up, transmit: true);

                victimBody.AddTimedBuff(RoR2Content.Buffs.Slow80, 3f + 3f * itemCount);
                victimBody.healthComponent.TakeDamage(new DamageInfo
                {
                    damage = damage,
                    damageColorIndex = DamageColorIndex.Item,
                    damageType = DamageType.Generic,
                    attacker = damageInfo.attacker,
                    crit = damageInfo.crit,
                    force = Vector3.zero,
                    inflictor = damageInfo.inflictor,
                    position = damageInfo.position,
                    procChainMask = damageInfo.procChainMask,
                    procCoefficient = 0f
                });
            } // end runald

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.HookShot) && Util.CheckRoll(procChance, attackerBody.master))
            {
                damageInfo.procChainMask.AddProc(ProcType.BounceNearby);

                var itemCount = attackerBody.inventory.GetItemCount(RoR2Content.Items.BounceNearby);
                var damageValue = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 0.25f + itemCount);

                var targets = CollectionPool<HurtBox, List<HurtBox>>.RentCollection();
                var exclusions = CollectionPool<HealthComponent, List<HealthComponent>>.RentCollection();
                var bouncedObjects = new List<HealthComponent>();

                exclusions.Add(attackerBody.healthComponent);
                exclusions.Add(victimBody.healthComponent);
                bouncedObjects.Add(victimBody.healthComponent);

                BounceOrb.SearchForTargets(new BullseyeSearch(), attackerBody.teamComponent.teamIndex, damageInfo.position, 30f /*range*/, 10 /*maxTargets*/, targets, exclusions);

                foreach (var hurtBox in targets)
                {
                    OrbManager.instance.AddOrb(new BounceOrb
                    {
                        origin = damageInfo.position,
                        damageValue = damageValue,
                        isCrit = damageInfo.crit,
                        teamIndex = attackerBody.teamComponent.teamIndex,
                        attacker = damageInfo.attacker,
                        procChainMask = damageInfo.procChainMask,
                        procCoefficient = 0.33f,
                        damageColorIndex = DamageColorIndex.Default,
                        bouncedObjects = bouncedObjects,
                        target = hurtBox
                    });
                }

                CollectionPool<HurtBox, List<HurtBox>>.ReturnCollection(targets);
                CollectionPool<HealthComponent, List<HealthComponent>>.ReturnCollection(exclusions);

            } // end hookshot

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.FlameTornadoShot) && Util.CheckRoll(procChance, attackerBody.master))
            {
                var itemCount = attackerBody.inventory.GetItemCount(RoR2Content.Items.FireRing);
                var damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 1.5f + 1.5f * itemCount) / 3.3f * 0.3f;

                var vector = damageInfo.position - attackerBody.aimOrigin;
                vector.y = 0f;

                var rotation = Quaternion.identity;
                var speedOverride = 0f;
                if (vector != Vector3.zero)
                {
                    speedOverride = -1f;
                    rotation = Util.QuaternionSafeLookRotation(vector, Vector3.up);
                }

                ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                {
                    damage = damage,
                    crit = damageInfo.crit,
                    damageColorIndex = DamageColorIndex.Item,
                    position = damageInfo.position,
                    procChainMask = damageInfo.procChainMask,
                    force = 0f,
                    owner = damageInfo.attacker,
                    projectilePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FireTornado"),
                    rotation = rotation,
                    speedOverride = speedOverride,
                    target = null
                });
            } // end kjaro

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.DaggerShot) && Util.CheckRoll(procChance, attackerBody.master))
            {
                var position = Vector3.Lerp(victim.transform.position, attackerBody.transform.position, 0.75f) + Vector3.up * 1.8f + UnityEngine.Random.insideUnitSphere * 0.5f;
                var rotation = Util.QuaternionSafeLookRotation(Vector3.up + UnityEngine.Random.insideUnitSphere * 0.1f);

                var itemCount = attackerBody.inventory.GetItemCount(RoR2Content.Items.Dagger);
                var damageValue = Util.OnKillProcDamage(attackerBody.damage, 3f + 1.5f * itemCount);
                var force = 200f;

                ProjectileManager.instance.FireProjectile(
                    Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Dagger/DaggerProjectile.prefab").WaitForCompletion(),
                    position,
                    rotation,
                    attackerBody.gameObject,
                    damageValue,
                    force,
                    Util.CheckRoll(attackerBody.crit, attackerBody.master),
                    DamageColorIndex.Item);
            } // end daggers

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.MissileShot) && Util.CheckRoll(procChance, attackerBody.master))
            {
                damageInfo.procChainMask.AddProc(ProcType.Missile);

                var icbmCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                var missileCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.MissileVoid) + attackerBody.inventory.GetItemCount(RoR2Content.Items.Missile);

                var missileDamage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 1.5f + 1.5f * missileCount) * DriverPlugin.GetICBMDamageMult(attackerBody);

                var initialDirection = Vector3.up + UnityEngine.Random.insideUnitSphere * 0.1f;

                var fireProjectileInfo = new FireProjectileInfo
                {
                    projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/MissileProjectile.prefab").WaitForCompletion(),
                    position = attackerBody.corePosition,
                    rotation = Util.QuaternionSafeLookRotation(initialDirection),
                    procChainMask = damageInfo.procChainMask,
                    target = victim,
                    owner = attackerBody.gameObject,
                    damage = missileDamage,
                    crit = damageInfo.crit,
                    force = 200f,
                    damageColorIndex = DamageColorIndex.Item
                };
                ProjectileManager.instance.FireProjectile(fireProjectileInfo);

                if (icbmCount > 0)
                {
                    var axis = attackerBody.transform.position;

                    var fireProjectileInfo2 = fireProjectileInfo;
                    fireProjectileInfo2.rotation = Util.QuaternionSafeLookRotation(Quaternion.AngleAxis(45f, axis) * initialDirection);
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo2);

                    var fireProjectileInfo3 = fireProjectileInfo;
                    fireProjectileInfo3.rotation = Util.QuaternionSafeLookRotation(Quaternion.AngleAxis(-45f, axis) * initialDirection);
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo3);
                }
            } // end atg

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.LightningStrikeRounds) && Util.CheckRoll(procChance, attackerBody.master))
            {
                damageInfo.procChainMask.AddProc(ProcType.LightningStrikeOnHit);

                var itemCount = attackerBody.inventory.GetItemCount(RoR2Content.Items.LightningStrikeOnHit);
                var damageValue = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 2.5f + 2.5f * itemCount);

                var target = victimBody.mainHurtBox;
                if (victimBody.hurtBoxGroup)
                    target = victimBody.hurtBoxGroup.hurtBoxes[UnityEngine.Random.Range(0, victimBody.hurtBoxGroup.hurtBoxes.Length)];

                OrbManager.instance.AddOrb(new SimpleLightningStrikeOrb
                {
                    attacker = attackerBody.gameObject,
                    damageColorIndex = DamageColorIndex.Item,
                    damageValue = damageValue,
                    isCrit = damageInfo.crit,
                    procChainMask = damageInfo.procChainMask,
                    procCoefficient = 1f,
                    target = target
                });
            } // end cherf

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.FireballRounds) && Util.CheckRoll(procChance, attackerBody.master))
            {
                damageInfo.procChainMask.AddProc(ProcType.Meatball);

                var height = victimBody.characterMotor ? victimBody.characterMotor.capsuleHeight * 0.5f : 0f;
                var origin = victim.transform.position + Vector3.up * (height + 2f);
                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashFireMeatBall"), new EffectData
                {
                    scale = 1f,
                    origin = origin
                }, transmit: true);

                var itemCount = attackerBody.inventory.GetItemCount(RoR2Content.Items.FireballsOnHit);
                var damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 1.5f + 1.5f * itemCount);

                var rotation = Vector3.up;
                for (var i = 0; i < 3; i++)
                {
                    var offset = i * (float)Math.PI * 2f / 3;
                    ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                    {
                        projectilePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FireMeatBall"),
                        position = origin + new Vector3(Mathf.Sin(offset), 0f, Mathf.Cos(offset)),
                        rotation = Util.QuaternionSafeLookRotation(rotation),
                        procChainMask = damageInfo.procChainMask,
                        target = victim,
                        owner = attackerBody.gameObject,
                        damage = damage,
                        crit = damageInfo.crit,
                        force = 200f,
                        damageColorIndex = DamageColorIndex.Item,
                        speedOverride = UnityEngine.Random.Range(15f, 30f),
                        useSpeedOverride = true
                    });
                    rotation.x += Mathf.Sin(offset + UnityEngine.Random.Range(-20f, 20f));
                    rotation.z += Mathf.Cos(offset + UnityEngine.Random.Range(-20f, 20f));
                }
            } // end merf

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.StickyShot) && Util.CheckRoll(procChance, attackerBody.master))
            {
                var forward = victimBody.corePosition - damageInfo.position;
                var rotation = forward.magnitude != 0f ? Util.QuaternionSafeLookRotation(forward) : UnityEngine.Random.rotationUniform;

                var itemCount = attackerBody.inventory.GetItemCount(RoR2Content.Items.StickyBomb);
                var damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 1.8f + 0.2f * itemCount);

                ProjectileManager.instance.FireProjectile(
                    LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/StickyBomb"),
                    damageInfo.position,
                    rotation,
                    damageInfo.attacker,
                    damage,
                    100f,
                    damageInfo.crit,
                    DamageColorIndex.Item,
                    null /*target*/,
                    attackerBody.healthComponent.alive ? forward.magnitude * 5f : -1f);
            } // end sticky

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.VoidLightning) && Util.CheckRoll(procChance, attackerBody.master))
            {
                damageInfo.procChainMask.AddProc(ProcType.ChainLightning);

                var itemCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.ChainLightningVoid) + attackerBody.inventory.GetItemCount(RoR2Content.Items.ChainLightning);
                var damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 0.4f);

                OrbManager.instance.AddOrb(new VoidLightningOrb
                {
                    origin = damageInfo.position,
                    damageValue = damage,
                    isCrit = damageInfo.crit,
                    totalStrikes = 3 + 2 * itemCount,
                    teamIndex = attackerBody.teamComponent.teamIndex,
                    attacker = damageInfo.attacker,
                    procChainMask = damageInfo.procChainMask,
                    procCoefficient = 0.2f,
                    damageColorIndex = DamageColorIndex.Void,
                    secondsPerStrike = 0.1f,
                    target = victimBody.mainHurtBox
                });
            } // end polylute

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.Hemorrhage) && Util.CheckRoll(procChance, attackerBody.master))
            {
                DotController.InflictDot(
                    victim,
                    damageInfo.attacker,
                    DotController.DotIndex.SuperBleed,
                    15f * damageInfo.procCoefficient);
            } // end superbleed

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.Gouge) && Util.CheckRoll(procChance, attackerBody.master))
            {
                DotController.InflictDot(
                    victim,
                    damageInfo.attacker,
                    DriverDamageTypes.GougeDotIndex,
                    4f,
                    1.5f);
            } // end gouge

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.BetterBurn) && Util.CheckRoll(procChance, attackerBody.master))
            {
                var dotInfo = new InflictDotInfo
                {
                    victimObject = victim,
                    attackerObject = damageInfo.attacker,
                    dotIndex = DotController.DotIndex.StrongerBurn,
                    duration = 6f,
                    damageMultiplier = 4f,
                    maxStacksFromAttacker = null,
                    preUpgradeDotIndex = DotController.DotIndex.Burn,
                    totalDamage = 0.5f * damageInfo.damage
                };
                DotController.InflictDot(ref dotInfo);
            } // end super burn

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.Helfire) && Util.CheckRoll(procChance, attackerBody.master))
            {
                var num = 0.05f * 6f * victimBody.healthComponent.fullCombinedHealth;
                var inflictDotInfo = new InflictDotInfo
                {
                    attackerObject = damageInfo.attacker,
                    victimObject = victim,
                    totalDamage = 24f * num,
                    damageMultiplier = 24f,
                    dotIndex = DotController.DotIndex.Helfire,
                    maxStacksFromAttacker = 1u
                };
                StrengthenBurnUtils.CheckDotForUpgrade(attackerBody.inventory, ref inflictDotInfo);
                DotController.InflictDot(ref inflictDotInfo);
            } // end helfire

            if (damageInfo.HasModdedDamageType(DriverDamageTypes.Collapse) && Util.CheckRoll(procChance, attackerBody.master))
            {
                damageInfo.procChainMask.AddProc(ProcType.FractureOnHit);

                DotController.InflictDot(
                    duration: DotController.GetDotDef(DotController.DotIndex.Fracture).interval,
                    victimObject: victim,
                    attackerObject: damageInfo.attacker,
                    dotIndex: DotController.DotIndex.Fracture,
                    damageMultiplier: 1f,
                    maxStacksFromAttacker: null);

            } // end void bleed

            orig(self, damageInfo, victim);
        }

        private static void GlobalEventManager_OnHitAllProcess(On.RoR2.GlobalEventManager.orig_OnHitAllProcess orig, GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
        {
            if (damageInfo.procCoefficient > 0 && !damageInfo.rejected && damageInfo.HasModdedDamageType(DriverDamageTypes.ExplosiveRounds) && NetworkServer.active)
            {
                var attackerBody = damageInfo.attacker ? damageInfo.attacker.GetComponent<CharacterBody>() : null;
                if (attackerBody)
                {
                    var itemCount = attackerBody.inventory ? attackerBody.inventory.GetItemCount(RoR2Content.Items.Behemoth) : 0;
                    var radius = (1.5f + 2.5f * itemCount) * damageInfo.procCoefficient;
                    var baseDamage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 0.6f);

                    EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
                    {
                        origin = damageInfo.position,
                        scale = radius,
                        rotation = Util.QuaternionSafeLookRotation(damageInfo.force)
                    }, transmit: true);

                    var mask = damageInfo.procChainMask;
                    mask.AddProc(ProcType.Behemoth);
                    new BlastAttack
                    {
                        position = damageInfo.position,
                        baseDamage = baseDamage,
                        baseForce = 0f,
                        radius = radius,
                        attacker = damageInfo.attacker,
                        inflictor = null,
                        teamIndex = TeamComponent.GetObjectTeam(damageInfo.attacker),
                        crit = damageInfo.crit,
                        procChainMask = mask,
                        procCoefficient = 0f,
                        damageColorIndex = DamageColorIndex.Item,
                        falloffModel = BlastAttack.FalloffModel.None,
                        damageType = damageInfo.damageType
                    }.Fire();
                }
            }

            orig(self, damageInfo, hitObject);
        }

        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            // hehehahaha
            if (!NetworkServer.active || !damageReport.victimBody)
                return;

            var hasDriver = InstanceTracker.Any<DriverController>();

            if (!hasDriver)
            {
                foreach (var pcmc in PlayerCharacterMasterController.instances)
                {
                    if (pcmc.master && pcmc.master.backupBodyIndex == Driver.bodyIndex)
                    {
                        hasDriver = true;
                        break;
                    }
                }
            }

            if (!hasDriver)
                return;

            #region Headshots
            if (damageReport.damageInfo.HasModdedDamageType(DriverDamageTypes.BloodExplosionIdentifier) || damageReport.victimBody.GetComponent<DriverHeadshotTracker>())
            {
                if (damageReport.victimBody.TryGetComponent<NetworkIdentity>(out var identity))
                    new SyncDecapitation(identity.netId, damageReport.victimBody.gameObject).Send(NetworkDestination.Clients);

                // rav orb yep
                var iDrive = damageReport.attacker ? damageReport.attacker.GetComponent<DriverController>() : null;
                if (iDrive && iDrive.weaponDef == Weapons.RavSword.instance?.weaponDef)
                {
                    OrbManager.instance.AddOrb(new ConsumeOrb
                    {
                        origin = damageReport.victimBody.corePosition,
                        target = Util.FindBodyMainHurtBox(damageReport.attackerBody)
                    });
                }
            }
            #endregion

            #region Weapon Drops
            var chance = Config.baseDropRate.Value;
            if (chance <= 0)
                return; // drop nothing

            var fuckMyAss = chance >= 100f;

            // higher chance if it's a big guy
            if (damageReport.victimBody.hullClassification >= HullClassification.Golem)
                chance = Mathf.Clamp(2f * chance, 0f, 100f);

            // minimum 25% chance if the slain enemy is an elite
            if (damageReport.victimBody.isElite)
                chance = Mathf.Clamp(chance, 20f, 100f);

            // halved on swarms, fuck You
            if (RunArtifactManager.instance && RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.Swarms))
                chance *= 0.5f;

            chance *= Driver.instance.pityMultiplier;

            var droppedWeapon = Util.CheckRoll(chance, damageReport.attackerMaster);

            // guaranteed if the slain enemy is a boss
            var isBoss = damageReport.victimBody.isChampion || damageReport.victimIsChampion;

            // simulacrum boss wave fix
            if ((damageReport.victimBody.isBoss || damageReport.victimIsBoss) && !Run.instance)
                isBoss = true;

            // terminal enemies from starstorm's relic of termination
            if (DriverPlugin.StarstormInstalled)
                droppedWeapon |= DriverPlugin.CheckIfBodyIsTerminal(damageReport.victimBody);

            if (isBoss || fuckMyAss)
                droppedWeapon = true;

            // all the above checks were originally checking the ATTACKER body
            // not the fucking victim
            // how

            // stop dropping weapons when void monsters kill each other plz this is an annoying bug
            if (damageReport.attackerTeamIndex != TeamIndex.Player && damageReport.victimTeamIndex != TeamIndex.Player)
                droppedWeapon = false;

            DriverWeaponDef weaponDef = null;
            DriverBulletDef bulletDef = null;
            if (DriverWeaponCatalog.weaponDrops.TryGetValue(damageReport.victimBodyIndex, out var weaponDrop))
            {
                if (Util.CheckRoll(weaponDrop.dropChance, damageReport.attackerMaster))
                {
                    droppedWeapon = true;
                    weaponDef = DriverWeaponCatalog.GetWeaponFromIndex(weaponDrop.weaponIndex);
                }
            }

            if (damageReport.victimBodyIndex == Driver.bodyIndex && damageReport.victimBody.TryGetComponent<DriverController>(out var iDie))
            {
                weaponDef = iDie.weaponDef;
                bulletDef = iDie.currentBulletDef;
                droppedWeapon = true;
            }

            if (droppedWeapon)
            {
                Driver.instance.pityMultiplier = 0.8f;
                var isNewAmmoType = false;
                var cutAmmo = false;

                if (!weaponDef || weaponDef == DriverWeaponCatalog.Pistol)
                {
                    isNewAmmoType = Util.CheckRoll(Config.godslingDropRateSplit.Value);
                    if (isBoss)
                        weaponDef = DriverWeaponCatalog.GetRandomWeaponFromTier(DriverWeaponTier.Legendary);
                    else
                    {
                        weaponDef = DriverWeaponCatalog.GetWeightedRandomWeapon(DriverWeaponTier.Unique);
                        if (weaponDef.tier >= DriverWeaponTier.Legendary)
                            cutAmmo = true;
                    }
                }

                if (!bulletDef || bulletDef == DriverBulletCatalog.Default)
                {
                    var tier = weaponDef.tier;
                    if (tier > DriverWeaponTier.Unique)
                        tier = DriverWeaponTier.Unique;

                    bulletDef = DriverBulletCatalog.GetRandomBulletFromTier(tier);
                }

                var weaponPickup = UnityEngine.Object.Instantiate(Assets.weaponPickup, damageReport.victimBody.corePosition, UnityEngine.Random.rotation);
                weaponPickup.GetComponent<SyncPickup>().SpawnWeapon(weaponDef, bulletDef, cutAmmo, isNewAmmoType);
            }
            else
            {
                // add pity
                Driver.instance.pityMultiplier += 0.05f;
            }
            #endregion
        }

        private static void MapZone_TryZoneStart(On.RoR2.MapZone.orig_TryZoneStart orig, MapZone self, Collider other)
        {
            orig(self, other);

            if (self.zoneType == MapZone.ZoneType.OutOfBounds && other.GetComponent<DestroyWeaponOnTimer>())
            {
                var destination = WeaponPickup.FindSafeTeleportDestination(other.bounds.min);

                if (destination.HasValue)
                {
                    Log.Debug("Teleported weapon pickup");
                    var rigidBody = other.GetComponent<Rigidbody>();
                    if (rigidBody)
                        rigidBody.velocity = Vector3.zero;

                    TeleportHelper.TeleportGameObject(other.gameObject, destination.Value + new Vector3(0f, 5f, 0f));
                }
                else
                {
                    Log.Warning("Unable to teleport weapon pickup");
                    GameObject.Destroy(other.gameObject);
                }
            }
        }
        #endregion

        #region UI
        private static void CrosshairController_Awake(On.RoR2.UI.CrosshairController.orig_Awake orig, CrosshairController self)
        {
            orig(self);

            if (self && !self.name.Contains("SprintCrosshair"))
            {
                if (!self.GetComponent<DynamicCrosshair>())
                    self.gameObject.AddComponent<DynamicCrosshair>();
            }
        }

        private static void GameEndReportPanelController_AssignUnlockToStrip(On.RoR2.UI.GameEndReportPanelController.orig_AssignUnlockToStrip orig,
            GameEndReportPanelController self, UnlockableDef unlockableDef, GameObject destUnlockableStrip)
        {
            orig(self, unlockableDef, destUnlockableStrip);

            if (DriverWeaponCatalog.weaponDefs.Any(def => def.nameToken == unlockableDef.nameToken))
            {
                if (unlockableDef.achievementIcon?.texture is Texture icon)
                    destUnlockableStrip.transform.Find("IconImage").GetComponent<RawImage>().texture = icon;
                destUnlockableStrip.GetComponent<TooltipProvider>().overrideTitleText = Language.GetString("UNLOCKABLE_ROB_DRIVER_WEAPON_NAME");
                destUnlockableStrip.GetComponent<TooltipProvider>().overrideBodyText = Language.GetString("UNLOCKABLE_ROB_DRIVER_WEAPON_DESC"); ;
            }
        }

        private static void CharacterSelectController_BuildSkillStripDisplayData(On.RoR2.UI.CharacterSelectController.orig_BuildSkillStripDisplayData orig,
            CharacterSelectController self, Loadout loadout, ref CharacterSelectController.BodyInfo bodyInfo, List<CharacterSelectController.StripDisplayData> dest)
        {
            orig(self, loadout, ref bodyInfo, dest);

            if (bodyInfo.bodyIndex != Driver.bodyIndex)
                return;

            var weaponDef = DriverWeaponCatalog.GetWeaponFromIndex((int)loadout.bodyLoadoutManager.GetSkillVariant(bodyInfo.bodyIndex, 1));
            for (int i = 0; i < dest.Count; i++)
            {
                var strip = dest[i];

                if (strip.actionName is "PrimarySkill")
                    SetDisplayData(ref strip, weaponDef.primarySkillDef ?? Skills.pistolPrimarySkillDef);
                else if (strip.actionName is "SecondarySkill")
                    SetDisplayData(ref strip, weaponDef.secondarySkillDef ?? Skills.pistolSecondarySkillDef);
                else
                    continue;

                dest[i] = strip;
            }
        }

        private static void SetDisplayData(ref CharacterSelectController.StripDisplayData displayData, SkillDef skillDef)
        {
            displayData.icon = skillDef.icon;
            displayData.titleString = Language.GetString(skillDef.skillNameToken);
            displayData.descriptionString = Language.GetString(skillDef.skillDescriptionToken);
            displayData.keywordString = string.Empty;

            if (skillDef.keywordTokens != null)
            {
                var stringBuilder = HG.StringBuilderPool.RentStringBuilder();
                for (var j = 0; j < skillDef.keywordTokens.Length; j++)
                {
                    stringBuilder.Append(Language.GetString(skillDef.keywordTokens[j])).Append("\n\n");
                }

                displayData.keywordString = stringBuilder.ToString();

                stringBuilder = HG.StringBuilderPool.ReturnStringBuilder(stringBuilder);
            }
        }

        private static void Row_UpdateHighlightedChoice(On.RoR2.UI.LoadoutPanelController.Row.orig_UpdateHighlightedChoice orig, LoadoutPanelController.Row self)
        {
            orig(self);

            if (!self.owner)
                return;

            ref var displayData = ref self.owner.currentDisplayData;
            if (displayData.bodyIndex != Driver.bodyIndex || displayData.userProfile == null)
                return;

            LoadoutPanelController.Row arsenalRow = null;
            LoadoutPanelController.Row primaryRow = null;
            LoadoutPanelController.Row secondaryRow = null;

            foreach (var row in self.owner.rows)
            {
                if (!row?.rowPanelTransform)
                    continue;

                var slot = row.rowPanelTransform.Find("LabelContainer/SlotLabel");
                var text = slot ? slot.GetComponent<LanguageTextMeshController>() : null;
                if (!text || string.IsNullOrEmpty(text.token))
                    continue;

                switch (text.token)
                {
                    case "ROB_DRIVER_ARSENAL_TOKEN":
                        arsenalRow = row;
                        break;
                    case "LOADOUT_SKILL_PRIMARY":
                        primaryRow = row;
                        break;
                    case "LOADOUT_SKILL_SECONDARY":
                        secondaryRow = row;
                        break;
                }
            }

            if (arsenalRow != null && primaryRow != null && secondaryRow != null)
            {
                var loadout = Loadout.RequestInstance();
                displayData.userProfile.CopyLoadout(loadout);

                var weaponDef = DriverWeaponCatalog.GetWeaponFromIndex(arsenalRow.findCurrentChoice(loadout));
                SetDisplayData(primaryRow, weaponDef.primarySkillDef ?? Skills.pistolPrimarySkillDef);
                SetDisplayData(secondaryRow, weaponDef.secondarySkillDef ?? Skills.pistolPrimarySkillDef);
                Loadout.ReturnInstance(loadout);
            }
        }

        private static void SetDisplayData(LoadoutPanelController.Row row, SkillDef skillDef)
        {
            var button = row.rowData.FirstOrDefault().button;
            if (button)
            {
                var nameText = Language.GetString(skillDef.skillNameToken);
                var bodyText = Language.GetString(skillDef.skillDescriptionToken);

                var toolTip = button.GetComponent<TooltipProvider>();
                toolTip.overrideTitleText = nameText;
                toolTip.overrideBodyText = bodyText;
                toolTip.titleColor = row.primaryColor;

                (button as HGButton).hoverToken = Language.GetStringFormatted("LOGBOOK_HOVER_DESCRIPTION_FORMAT", nameText, bodyText, ColorUtility.ToHtmlStringRGBA(row.primaryColor));
                (button.targetGraphic as Image).sprite = skillDef.icon;
            }
        }

        private static void HGButton_Start(On.RoR2.UI.HGButton.orig_Start orig, HGButton self)
        {
            orig(self);

            if (!Config.enableGodslingInMultiplayer.Value && !RoR2Application.isInSinglePlayer)
            {
                // this is literally the worst thing ever
                if (self && !string.IsNullOrEmpty(self.hoverToken) && self.hoverToken.Contains("Godsling"))
                    self.gameObject.SetActive(false);
            }
        }

        private static void HUDSetup(HUD hud)
        {
            if (hud.targetBodyObject && hud.targetMaster && hud.targetMaster.backupBodyIndex == Driver.bodyIndex)
            {
                if (!hud.targetMaster.hasAuthority) 
                    return;

                // weapon pickup notification
                var notificationPanel = hud.transform.Find("MainContainer").Find("NotificationArea").gameObject;
                var _new = notificationPanel.AddComponent<WeaponNotificationUIController>();

                _new.hud = hud;
                _new.notificationQueue = hud.targetMaster.gameObject.AddComponent<WeaponNotificationQueue>();

                if (DriverPlugin.HunkHudInstalled)
                {
                    HunkHudSetupNew(hud);
                    return;
                }

                if (DriverPlugin.RiskUIInstalled)
                    RiskUIHudSetup(hud);
                else 
                    NormalHudSetup(hud);
            }
        }

        private static void NormalHudSetup(HUD hud)
        {
            var skillsContainer = hud.equipmentIcons[0].gameObject.transform.parent;

            // remove existing
            if (skillsContainer.Find("WeaponSlot")) UnityEngine.Object.Destroy(skillsContainer.Find("WeaponSlot").gameObject);

            var oldUI = hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras").Find("AmmoTracker");
            if (oldUI) UnityEngine.Object.Destroy(oldUI.gameObject);

            // no one will notice these missing
            skillsContainer.Find("SprintCluster").gameObject.SetActive(false);
            skillsContainer.Find("InventoryCluster").gameObject.SetActive(false);

            var weaponSlot = UnityEngine.Object.Instantiate(skillsContainer.Find("EquipmentSlot").gameObject, skillsContainer);
            weaponSlot.name = "WeaponSlot";

            var equipmentIconComponent = weaponSlot.GetComponent<EquipmentIcon>();
            var weaponIconComponent = weaponSlot.AddComponent<WeaponIcon>();

            weaponIconComponent.iconImage = equipmentIconComponent.iconImage;
            weaponIconComponent.displayRoot = equipmentIconComponent.displayRoot;
            weaponIconComponent.flashPanelObject = equipmentIconComponent.stockFlashPanelObject;
            weaponIconComponent.reminderFlashPanelObject = equipmentIconComponent.reminderFlashPanelObject;
            weaponIconComponent.isReadyPanelObject = equipmentIconComponent.isReadyPanelObject;
            weaponIconComponent.tooltipProvider = equipmentIconComponent.tooltipProvider;
            weaponIconComponent.targetHUD = hud;
            weaponSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480f, -17.1797f);

            var keyText = weaponSlot.transform.Find("DisplayRoot").Find("EquipmentTextBackgroundPanel").Find("EquipmentKeyText").gameObject.GetComponent<HGTextMeshProUGUI>();
            keyText.gameObject.GetComponent<InputBindingDisplayController>().enabled = false;
            keyText.text = "Weapon";

            weaponSlot.transform.Find("DisplayRoot").Find("EquipmentStack").gameObject.SetActive(false);
            weaponSlot.transform.Find("DisplayRoot").Find("CooldownText").gameObject.SetActive(false);

            // duration bar
            var chargeBar = UnityEngine.Object.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("WeaponChargeBar"));
            chargeBar.transform.SetParent(weaponSlot.transform.Find("DisplayRoot"));

            var rect = chargeBar.GetComponent<RectTransform>();

            rect.localScale = new Vector3(0.75f, 0.1f, 1f);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(-10f, 13f);
            rect.localPosition = new Vector3(-33f, -10f, 0f);
            rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

            weaponIconComponent.durationDisplay = chargeBar;
            weaponIconComponent.durationBar = chargeBar.transform.GetChild(1).gameObject.GetComponent<Image>();
            weaponIconComponent.durationBarRed = chargeBar.transform.GetChild(0).gameObject.GetComponent<Image>();

            UnityEngine.Object.Destroy(equipmentIconComponent);

            // ammo display for alt passive
            var healthbarContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("BarRoots").Find("LevelDisplayCluster");

            var ammoTracker = UnityEngine.Object.Instantiate(healthbarContainer.gameObject, hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster"));
            ammoTracker.name = "AmmoTracker";
            ammoTracker.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

            UnityEngine.Object.DestroyImmediate(ammoTracker.transform.GetChild(0).gameObject);
            UnityEngine.Object.Destroy(ammoTracker.GetComponentInChildren<LevelText>());
            UnityEngine.Object.Destroy(ammoTracker.GetComponentInChildren<ExpBar>());

            ammoTracker.transform.Find("LevelDisplayRoot").Find("ValueText").gameObject.SetActive(false);
            UnityEngine.Object.DestroyImmediate(ammoTracker.transform.Find("ExpBarRoot").gameObject);

            ammoTracker.transform.Find("LevelDisplayRoot").GetComponent<RectTransform>().anchoredPosition = new Vector2(-12f, 0f);

            rect = ammoTracker.GetComponent<RectTransform>();
            rect.localScale = new Vector3(0.8f, 0.8f, 1f);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.offsetMin = new Vector2(120f, -40f);
            rect.offsetMax = new Vector2(120f, -40f);
            rect.pivot = new Vector2(0.5f, 0f);
            //positional data doesnt get sent to clients? Manually making offsets works..
            rect.anchoredPosition = new Vector2(50f, 0f);
            rect.localPosition = new Vector3(120f, -40f, 0f);

            var chargeBarAmmo = UnityEngine.Object.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("WeaponChargeBar"));
            chargeBarAmmo.name = "AmmoBar";
            chargeBarAmmo.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

            rect = chargeBarAmmo.GetComponent<RectTransform>();

            rect.localScale = new Vector3(0.75f, 0.1f, 1f);
            rect.anchorMin = new Vector2(100f, 2f);
            rect.anchorMax = new Vector2(100f, 2f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(100f, 2f);
            rect.localPosition = new Vector3(100f, 2f, 0f);
            rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

            var ammoTrackerComponent = ammoTracker.AddComponent<AmmoDisplay>();

            ammoTrackerComponent.targetHUD = hud;
            ammoTrackerComponent.targetText = ammoTracker.transform.Find("LevelDisplayRoot").Find("PrefixText").gameObject.GetComponent<LanguageTextMeshController>();
            ammoTrackerComponent.durationDisplay = chargeBarAmmo;
            ammoTrackerComponent.durationBar = chargeBarAmmo.transform.GetChild(1).gameObject.GetComponent<Image>();
            ammoTrackerComponent.durationBarRed = chargeBarAmmo.transform.GetChild(0).gameObject.GetComponent<Image>();
        }

        private static void RiskUIHudSetup(HUD hud)
        {
            // Get rid of old hud, im tired of fighting this
            var equipmentSlot = hud.equipmentIcons[0];

            var weaponSlot = equipmentSlot.transform.parent.Find("WeaponSlot")?.gameObject;
            if (weaponSlot) GameObject.DestroyImmediate(weaponSlot);

            weaponSlot = GameObject.Instantiate(equipmentSlot.gameObject, equipmentSlot.transform.parent);
            weaponSlot.name = "WeaponSlot";

            var equipmentIcon = weaponSlot.GetComponent<EquipmentIcon>();
            var weaponIcon = weaponSlot.AddComponent<WeaponIcon>();

            // whoever deleted the stock flash animations is a bad guy
            weaponIcon.iconImage = equipmentIcon.iconImage;
            weaponIcon.displayRoot = equipmentIcon.displayRoot;
            weaponIcon.flashPanelObject = equipmentIcon.stockFlashPanelObject;
            weaponIcon.reminderFlashPanelObject = equipmentIcon.reminderFlashPanelObject;
            weaponIcon.isReadyPanelObject = equipmentIcon.isReadyPanelObject;
            weaponIcon.tooltipProvider = equipmentIcon.tooltipProvider;
            weaponIcon.targetHUD = hud;

            var iconRect = weaponSlot.GetComponent<RectTransform>();
            iconRect.localScale = new Vector3(2f, 2f, 2f);
            iconRect.anchoredPosition = new Vector2(-128f, 60f);

            if (DriverPlugin.ExtendedLoadoutInstalled)
            {
                iconRect.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                iconRect.anchoredPosition = new Vector2(-110f, 60f);
            }

            var materialWeaponIcon = weaponSlot.AddComponent<MaterialWeaponIcon>();
            materialWeaponIcon.targetHUD = hud;
            materialWeaponIcon.mask = weaponSlot.transform.Find("DisplayRoot").Find("Mask").gameObject.GetComponent<Image>();
            materialWeaponIcon.cooldownRing = weaponSlot.transform.Find("DisplayRoot").Find("Mask").Find("CooldownRing").gameObject.GetComponent<Image>();
            materialWeaponIcon.cooldownRing.fillCenter = false;
            materialWeaponIcon.ammoBackground = weaponSlot.transform.Find("DisplayRoot").Find("BottomContainer").Find("StockTextContainer").gameObject;
            materialWeaponIcon.ammoBackground.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 2.5f);
            materialWeaponIcon.ammoBackground.GetComponent<RectTransform>().localScale = new Vector3(0.8f, -0.8f, 0.8f);
            materialWeaponIcon.ammoBackground.transform.SetAsFirstSibling();
            materialWeaponIcon.ammoText = materialWeaponIcon.ammoBackground.transform.Find("StockText").gameObject.GetComponent<TextMeshProUGUI>();

            weaponSlot.transform.Find("DisplayRoot").Find("BgImage").Find("IconPanel").Find("OnCooldown").gameObject.SetActive(false);

            GameObject.DestroyImmediate(weaponSlot.transform.Find("DisplayRoot").Find("BottomContainer").Find("SkillBackgroundPanel").gameObject);
            GameObject.DestroyImmediate(weaponSlot.transform.Find("DisplayRoot").Find("CooldownText").gameObject);
            MonoBehaviour.DestroyImmediate(materialWeaponIcon.cooldownRing.GetComponent<RedToColorRemapperIndividual>());
            MonoBehaviour.DestroyImmediate(weaponSlot.transform.Find("DisplayRoot").Find("BottomContainer").gameObject.GetComponent<HideFromBepinConfig>());
            MonoBehaviour.DestroyImmediate(weaponSlot.GetComponent<MaterialEquipmentIcon>());
            MonoBehaviour.DestroyImmediate(weaponSlot.GetComponent<BepinConfigParentManager>());
            MonoBehaviour.DestroyImmediate(weaponSlot.GetComponent<EquipmentIcon>());

            if (DriverPlugin.HunkInstalled)
            {
                if (DriverPlugin.IsHunkHudGlobal())
                {
                    HunkHudSetup(hud, weaponSlot, weaponIcon);
                    return;
                }
            }

            // duration bar
            var chargeBar = GameObject.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("WeaponChargeBar"));
            chargeBar.transform.SetParent(weaponSlot.transform.Find("DisplayRoot"));

            var rect = chargeBar.GetComponent<RectTransform>();
            rect.localScale = new Vector3(0.75f, 0.1f, 1f);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.localPosition = new Vector3(0f, 0f, 0f);
            rect.anchoredPosition = new Vector2(-8f, 36f);
            rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

            weaponIcon.durationDisplay = chargeBar;
            weaponIcon.durationBar = chargeBar.transform.GetChild(1).gameObject.GetComponent<Image>();
            weaponIcon.durationBarRed = chargeBar.transform.GetChild(0).gameObject.GetComponent<Image>();
        }

        private static void HunkHudSetup(HUD hud, GameObject weaponSlot, WeaponIcon weaponIconComponent)
        {
            var center = hud.transform.Find("MainContainer/MainUIArea/SpringCanvas/BottomRightCluster/CustomHealthBar/Center");
            center.GetComponent<HunkMod.Modules.Components.RectMover>().pos = new Vector3(-125f, 180f, 0f);
            center.Find("GunIcon").gameObject.SetActive(false);

            MonoBehaviour.DestroyImmediate(weaponSlot.GetComponent<HunkMod.Modules.Components.RectMover>());
            weaponSlot.transform.SetParent(center);
            weaponSlot.transform.SetAsFirstSibling();
            weaponSlot.transform.localPosition = Vector3.zero;
            weaponSlot.transform.Find("DisplayRoot/BottomContainer").localPosition = Vector3.zero;
            weaponSlot.transform.Find("DisplayRoot/BottomContainer/StockTextContainer").localScale = new Vector3(0.7f, -0.7f, 0.7f); // whoever did this needs to be shot asap

            var biomassBar = center.Find("BiomassHolder");
            biomassBar.Find("BiomassFillInstant").gameObject.SetActive(false);

            weaponIconComponent.maxFill = 0.7525f;
            weaponIconComponent.durationDisplay = biomassBar.gameObject;
            weaponIconComponent.durationBar = biomassBar.Find("BiomassFill").GetComponent<Image>();
            weaponIconComponent.durationBar.sprite = biomassBar.Find("BiomassBackground").GetComponent<Image>().sprite;
            weaponIconComponent.durationBarRed = biomassBar.Find("BiomassFillLag").GetComponent<Image>();
            weaponIconComponent.durationBarRed.sprite = biomassBar.Find("BiomassBackground").GetComponent<Image>().sprite;
        }

        private static void HunkHudSetupNew(HUD hud)
        {
            var hpBar = HunkHud.Components.UI.CustomHealthBar.instance;
            if (hpBar)
            {
                var weaponIcon = hpBar.gunIconHolder.GetComponent<WeaponIcon>();
                if (!weaponIcon)
                    weaponIcon = hpBar.gunIconHolder.AddComponent<WeaponIcon>();

                weaponIcon.targetHUD = hud;
                weaponIcon.iconImage = hpBar.gunIcon;
                weaponIcon.maxFill = 0.751f;
                weaponIcon.durationDisplay = hpBar.biomassBar;
                weaponIcon.durationBar = hpBar.biomassBar.transform.Find("BiomassFill").GetComponent<Image>();
                weaponIcon.durationBar.sprite = hpBar.biomassBar.transform.Find("BiomassBackground").GetComponent<Image>().sprite;
                weaponIcon.durationBarRed = hpBar.biomassBar.transform.Find("BiomassFillLag").GetComponent<Image>();
                weaponIcon.durationBarRed.sprite = hpBar.biomassBar.transform.Find("BiomassBackground").GetComponent<Image>().sprite;

                var materialWeaponIcon = hpBar.gunIconHolder.GetComponent<MaterialWeaponIcon>();
                if (!materialWeaponIcon)
                    materialWeaponIcon = hpBar.gunIconHolder.AddComponent<MaterialWeaponIcon>();

                materialWeaponIcon.targetHUD = hud;
                materialWeaponIcon.ammoBackground = hpBar.gunText.transform.parent.gameObject;
                materialWeaponIcon.ammoText = hpBar.gunText;

                hpBar.biomassBar.transform.Find("BiomassFillInstant").gameObject.SetActive(false);
                hpBar.gunIconHolder.SetActive(value: true);
                hpBar.characterIconHolder.SetActive(value: false);
            }
        }
        #endregion

        #region Skills
        private static void BaseAIState_AimInDirection(On.EntityStates.AI.BaseAIState.orig_AimInDirection orig, EntityStates.AI.BaseAIState self, ref BaseAI.BodyInputs dest, Vector3 aimDirection)
        {
            orig(self, ref dest, aimDirection);

            if (self.body)
            {
                if (self.body.HasBuff(Buffs.dazedDebuff))
                    dest.desiredAimDirection = UnityEngine.Random.onUnitSphere;

                if (self.body.HasBuff(DLC2Content.Buffs.DisableAllSkills))
                {
                    dest.pressSkill1 = false;
                    dest.pressSkill2 = false;
                    dest.pressSkill3 = false;
                    dest.pressSkill4 = false;
                    dest.pressActivateEquipment = false;
                }
            }
        }

        private static void BaseAIState_AimAt(On.EntityStates.AI.BaseAIState.orig_AimAt orig, EntityStates.AI.BaseAIState self, ref BaseAI.BodyInputs dest, BaseAI.Target aimTarget)
        {
            orig(self, ref dest, aimTarget);

            if (self.body)
            {
                if (self.body.HasBuff(Buffs.dazedDebuff))
                    dest.desiredAimDirection = UnityEngine.Random.onUnitSphere;

                if (self.body.HasBuff(DLC2Content.Buffs.DisableAllSkills))
                {
                    dest.pressSkill1 = false;
                    dest.pressSkill2 = false;
                    dest.pressSkill3 = false;
                    dest.pressSkill4 = false;
                    dest.pressActivateEquipment = false;
                }
            }
        }

        private static void SkillLocator_ApplyAmmoPack(On.RoR2.SkillLocator.orig_ApplyAmmoPack orig, SkillLocator self)
        {
            orig(self);

            if (self && NetworkServer.active && self.name.StartsWith(Driver.bodyName) && self.TryGetComponent<DriverController>(out var iDrive))
                iDrive.ServerResetTimer();
        }

        private static void SkillLocator_ResetSkills(On.RoR2.SkillLocator.orig_ResetSkills orig, SkillLocator self)
        {
            orig(self);

            if (self && NetworkServer.active && self.name.StartsWith(Driver.bodyName) && self.TryGetComponent<DriverController>(out var iDrive))
                iDrive.ServerResetTimer();
        }

        private static int LunarPrimaryReplacementSkill_GetMaxStock(On.RoR2.Skills.LunarPrimaryReplacementSkill.orig_GetMaxStock orig, LunarPrimaryReplacementSkill self, GenericSkill skillSlot)
        {
            return Mathf.Max(orig(self, skillSlot), self.baseMaxStock);
        }

        private static float LunarPrimaryReplacementSkill_GetRechargeInterval(On.RoR2.Skills.LunarPrimaryReplacementSkill.orig_GetRechargeInterval orig, LunarPrimaryReplacementSkill self, GenericSkill skillSlot)
        {
            return Mathf.Max(orig(self, skillSlot), self.baseRechargeInterval);
        }

        private static float LunarSecondaryReplacementSkill_GetRechargeInterval(On.RoR2.Skills.LunarSecondaryReplacementSkill.orig_GetRechargeInterval orig, LunarSecondaryReplacementSkill self, GenericSkill skillSlot)
        {
            return Mathf.Max(orig(self, skillSlot), self.baseRechargeInterval);
        }

        private static void PlayVisionsAnimation(On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.orig_OnEnter orig, EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle self)
        {
            orig(self);

            if (self.characterBody?.bodyIndex == Driver.bodyIndex)
            {
                self.PlayAnimation("Gesture, Override", "Shoot", "Shoot.playbackRate", self.duration * 12f);
                EffectManager.SimpleMuzzleFlash(EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.muzzleFlashEffectPrefab, self.gameObject, "PistolMuzzle", false);
            }
        }

        private static void PlayChargeLunarAnimation(On.EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary.orig_PlayChargeAnimation orig, EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary self)
        {
            orig(self);

            if (self.characterBody?.bodyIndex == Driver.bodyIndex)
                self.PlayAnimation("Gesture, Override", "ChargeHooks", "Hooks.playbackRate", self.duration * 0.5f);
        }

        private static void PlayThrowLunarAnimation(On.EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary.orig_PlayThrowAnimation orig, EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary self)
        {
            orig(self);

            if (self.characterBody?.bodyIndex == Driver.bodyIndex)
                self.PlayAnimation("Gesture, Override", "ThrowHooks", "Hooks.playbackRate", self.duration);
        }

        private static void PlayRuinAnimation(On.EntityStates.GlobalSkills.LunarDetonator.Detonate.orig_OnEnter orig, EntityStates.GlobalSkills.LunarDetonator.Detonate self)
        {
            orig(self);

            if (self.characterBody?.bodyIndex == Driver.bodyIndex)
            {
                //self.PlayAnimation("Gesture, Override", "CastRuin", "Ruin.playbackRate", self.duration * 0.5f);
                //Util.PlaySound("PaladinFingerSnap", self.gameObject);
                self.PlayAnimation("Gesture, Override", "PressVoidButton", "Action.playbackRate", 0.5f * self.duration);
                self.StartAimMode(self.duration + 0.5f);

                EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterExplosion.prefab").WaitForCompletion(),
                    new EffectData
                    {
                        origin = self.FindModelChild("HandL").position,
                        rotation = Quaternion.identity,
                        scale = 0.5f
                    }, false);
            }
        }
        #endregion
    }
}
