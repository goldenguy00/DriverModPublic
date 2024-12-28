using R2API;
using RoR2.Orbs;
using RoR2;
using UnityEngine.Networking;
using UnityEngine;
using RobDriver.Modules.Survivors;
using System.Collections.Generic;
using System;
using HG;
using UnityEngine.AddressableAssets;
using RoR2.Projectile;
using RobDriver.Modules.Components;
using System.Xml.Linq;
using R2API.Networking.Interfaces;
using R2API.Networking;

namespace RobDriver.Modules
{
    public static class DamageTypes
    {
        public static DamageAPI.ModdedDamageType Generic;
        public static DamageAPI.ModdedDamageType HookShot;
        public static DamageAPI.ModdedDamageType MissileShot;
        public static DamageAPI.ModdedDamageType VoidMissileShot;
        public static DamageAPI.ModdedDamageType ExplosiveRounds;
        public static DamageAPI.ModdedDamageType FlameTornadoShot;
        public static DamageAPI.ModdedDamageType IceBlastShot;
        public static DamageAPI.ModdedDamageType DaggerShot;
        public static DamageAPI.ModdedDamageType LightningStrikeRounds;
        public static DamageAPI.ModdedDamageType FireballRounds;
        public static DamageAPI.ModdedDamageType StickyShot;
        public static DamageAPI.ModdedDamageType VoidLightning;
        public static DamageAPI.ModdedDamageType CoinShot;
        public static DamageAPI.ModdedDamageType MysteryShot;
        public static DamageAPI.ModdedDamageType Hemorrhage;
        public static DamageAPI.ModdedDamageType Gouge;

        public static DamageAPI.ModdedDamageType BloodExplosionIdentifier;
        public static DamageAPI.ModdedDamageType StunGrenadeDazed;
        public static DamageAPI.ModdedDamageType KnifeWound;

        internal static void Init()
        {
            Generic = DamageAPI.ReserveDamageType();
            HookShot = DamageAPI.ReserveDamageType();
            MissileShot = DamageAPI.ReserveDamageType();
            VoidMissileShot = DamageAPI.ReserveDamageType();
            ExplosiveRounds = DamageAPI.ReserveDamageType();
            FlameTornadoShot = DamageAPI.ReserveDamageType();
            IceBlastShot = DamageAPI.ReserveDamageType();
            DaggerShot = DamageAPI.ReserveDamageType();
            LightningStrikeRounds = DamageAPI.ReserveDamageType();
            FireballRounds = DamageAPI.ReserveDamageType();
            StickyShot = DamageAPI.ReserveDamageType();
            VoidLightning = DamageAPI.ReserveDamageType();
            CoinShot = DamageAPI.ReserveDamageType();
            MysteryShot = DamageAPI.ReserveDamageType();
            Hemorrhage = DamageAPI.ReserveDamageType();
            Gouge = DamageAPI.ReserveDamageType();

            BloodExplosionIdentifier = DamageAPI.ReserveDamageType();
            StunGrenadeDazed = DamageAPI.ReserveDamageType();
            KnifeWound = DamageAPI.ReserveDamageType();

            Hook();
        }

        #region Hooks

        private static void Hook()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            On.RoR2.GlobalEventManager.ProcessHitEnemy += GlobalEventManager_ProcessHitEnemy;
            On.RoR2.GlobalEventManager.OnHitAll += GlobalEventManager_OnHitAll;
        }

        private static bool CheckRoll(float procChance, CharacterMaster characterMaster)
        {
            // small optimization but probably does fuck all for performance, oh well
            return procChance >= 100f || Util.CheckRoll(procChance, characterMaster);
        }

        private static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self.body)
            {
                if (damageInfo.HasModdedDamageType(StunGrenadeDazed))
                {
                    self.body.AddTimedBuff(Modules.Buffs.dazedDebuff, 10f);
                }

                if (damageInfo.HasModdedDamageType(KnifeWound))
                {
                    self.body.AddTimedBuff(Modules.Buffs.woundDebuff, 4f);

                    if (self.gameObject.TryGetComponent<NetworkIdentity>(out var identity))
                    {
                        new SyncOverlay(identity.netId, self.gameObject).Send(NetworkDestination.Clients);
                    }
                }

                if (damageInfo.dotIndex == Buffs.gougeIndex && self.alive)
                {
                    if (damageInfo.attacker && damageInfo.attacker.TryGetComponent<CharacterBody>(out var attackerBody))
                    {
                        damageInfo.crit = Util.CheckRoll(attackerBody.crit, attackerBody.master);
                    }
                }
            }

            orig(self, damageInfo);
        }

        private static void GlobalEventManager_ProcessHitEnemy(On.RoR2.GlobalEventManager.orig_ProcessHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            CharacterBody attackerBody = damageInfo.attacker ? damageInfo.attacker.GetComponent<CharacterBody>() : null;

            if (attackerBody && attackerBody.baseNameToken == Driver.bodyNameToken)
            {
                if (damageInfo.procCoefficient > 0f && !damageInfo.rejected && damageInfo.inflictor && !string.IsNullOrEmpty(damageInfo.inflictor.name))
                {
                    // this is terrible but actually networking it is even worse so idk
                    var name = damageInfo.inflictor.name;
                    if (name.StartsWith("Driver") && (name.EndsWith("Projectile(Clone)") || name.EndsWith("Grenade(Clone)")) && 
                        damageInfo.attacker.TryGetComponent<DriverController>(out var iDrive) && iDrive)
                    {
                        damageInfo.AddModdedDamageType(iDrive.ModdedDamageType);
                    }
                }

                if (damageInfo.HasModdedDamageType(MysteryShot))
                {
                    var bulletInfo = DriverBulletCatalog.GetWeightedRandomBullet(DriverWeaponTier.Legendary);

                    damageInfo.damageType |= bulletInfo.bulletType;
                    damageInfo.RemoveModdedDamageType(MysteryShot);
                    damageInfo.AddModdedDamageType(bulletInfo.moddedBulletType);
                } // end mysteryshot
            }

            orig.Invoke(self, damageInfo, victim);

            // double check that we are doing this for a player controlled driver
            if (NetworkServer.active && attackerBody && attackerBody.baseNameToken == Driver.bodyNameToken && damageInfo.procCoefficient != 0f)
            {
                CharacterBody victimBody = victim ? victim.GetComponent<CharacterBody>() : null;
                TeamComponent attackerTeam = attackerBody.GetComponent<TeamComponent>();
                TeamIndex attackerTeamIndex = attackerTeam ? attackerTeam.teamIndex : TeamIndex.Neutral;
                float procChance = 100f * damageInfo.procCoefficient;

                if (damageInfo.HasModdedDamageType(HookShot) && CheckRoll(procChance, attackerBody.master))
                {
                    List<HurtBox> targets = CollectionPool<HurtBox, List<HurtBox>>.RentCollection();
                    List<HealthComponent> exclusions = CollectionPool<HealthComponent, List<HealthComponent>>.RentCollection();
                    if (attackerBody.healthComponent)
                    {
                        exclusions.Add(attackerBody.healthComponent);
                    }
                    if (victimBody && victimBody.healthComponent)
                    {
                        exclusions.Add(victimBody.healthComponent);
                    }
                    BounceOrb.SearchForTargets(new BullseyeSearch(), attackerTeamIndex, damageInfo.position, 30f /*range*/, 10 /*maxTargets*/, targets, exclusions);
                    CollectionPool<HealthComponent, List<HealthComponent>>.ReturnCollection(exclusions);
                    List<HealthComponent> bouncedObjects = new List<HealthComponent> { victim.GetComponent<HealthComponent>() };

                    float damageValue = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 1f /*damageCoefficient*/);

                    ProcChainMask procChainMask = damageInfo.procChainMask;
                    procChainMask.AddProc(ProcType.BounceNearby);

                    for (int i = 0; i < targets.Count; i++)
                    {
                        HurtBox hurtBox = targets[i];
                        if (hurtBox)
                        {
                            OrbManager.instance.AddOrb(new BounceOrb
                            {
                                origin = damageInfo.position,
                                damageValue = damageValue,
                                isCrit = damageInfo.crit,
                                teamIndex = attackerTeamIndex,
                                attacker = damageInfo.attacker,
                                procChainMask = procChainMask,
                                procCoefficient = 0.33f,
                                damageColorIndex = DamageColorIndex.Default,
                                bouncedObjects = bouncedObjects,
                                target = hurtBox
                            });
                        }
                    }
                    CollectionPool<HurtBox, List<HurtBox>>.ReturnCollection(targets);
                } // end hookshot

                if (damageInfo.HasModdedDamageType(VoidMissileShot) && CheckRoll(procChance, attackerBody.master))
                {
                    int icbmCount = 0;
                    int missileCount = 0;

                    if (attackerBody.inventory)
                    {
                        icbmCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                        missileCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.MissileVoid);
                        missileCount += attackerBody.inventory.GetItemCount(RoR2Content.Items.Missile);
                    }

                    float damageCoefficient = 0.4f + 0.4f * missileCount;
                    float damageValue = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient) * DriverPlugin.GetICBMDamageMult(attackerBody);

                    ProcChainMask procChainMask = damageInfo.procChainMask;
                    procChainMask.AddProc(ProcType.MicroMissile);

                    for (int i = 0; i < (icbmCount == 0 ? 1 : 3); i++)
                    {
                        MissileVoidOrb missileVoidOrb = new MissileVoidOrb
                        {
                            origin = attackerBody.aimOrigin,
                            damageValue = damageValue,
                            isCrit = damageInfo.crit,
                            teamIndex = attackerTeamIndex,
                            attacker = damageInfo.attacker,
                            procChainMask = procChainMask,
                            procCoefficient = 0.2f,
                            damageColorIndex = DamageColorIndex.Void
                        };

                        if (victimBody)
                        {
                            missileVoidOrb.target = victimBody.mainHurtBox;
                            OrbManager.instance.AddOrb(missileVoidOrb);
                        }
                    }
                } // end plimp

                if (damageInfo.HasModdedDamageType(FlameTornadoShot) && CheckRoll(procChance, attackerBody.master))
                {
                    GameObject gameObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FireTornado");
                    float resetInterval = gameObject.GetComponent<ProjectileOverlapAttack>().resetInterval;
                    float lifetime = gameObject.GetComponent<ProjectileSimple>().lifetime;
                    int itemCount = attackerBody.inventory ? attackerBody.inventory.GetItemCount(RoR2Content.Items.FireRing) : 0;
                    float damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 1.5f + 1.5f * itemCount) / lifetime * resetInterval;

                    Vector3 vector = damageInfo.position - attackerBody.aimOrigin;
                    vector.y = 0f;
                    Quaternion rotation;
                    float speedOverride;
                    if (vector != Vector3.zero)
                    {
                        speedOverride = -1f;
                        rotation = Util.QuaternionSafeLookRotation(vector, Vector3.up);
                    }
                    else
                    {
                        rotation = Quaternion.identity;
                        speedOverride = 0f;
                    }

                    ProcChainMask procChainMask = damageInfo.procChainMask;
                    procChainMask.AddProc(ProcType.Rings);

                    ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                    {
                        damage = damage,
                        crit = damageInfo.crit,
                        damageColorIndex = DamageColorIndex.Item,
                        position = damageInfo.position,
                        procChainMask = procChainMask,
                        force = 0f,
                        owner = damageInfo.attacker,
                        projectilePrefab = gameObject,
                        rotation = rotation,
                        speedOverride = speedOverride,
                        target = null
                    });
                } // end kjaro

                if (damageInfo.HasModdedDamageType(IceBlastShot) && CheckRoll(procChance, attackerBody.master))
                {
                    int itemCount = attackerBody.inventory ? attackerBody.inventory.GetItemCount(RoR2Content.Items.IceRing) : 0;
                    float damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 1.25f + 1.25f * itemCount);

                    EffectManager.SimpleImpactEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/ImpactEffects/IceRingExplosion"), damageInfo.position, Vector3.up, transmit: true);
                    if (victimBody) victimBody.AddTimedBuff(RoR2Content.Buffs.Slow80, 3f + 3f * itemCount);

                    ProcChainMask procChainMask = damageInfo.procChainMask;
                    procChainMask.AddProc(ProcType.Rings);

                    if (victimBody && victimBody.healthComponent) 
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
                            procChainMask = procChainMask,
                            procCoefficient = 0f
                        });
                } // end runald

                if (damageInfo.HasModdedDamageType(DaggerShot) && CheckRoll(procChance, attackerBody.master))
                {
                    Vector3 position = Vector3.zero;
                    Transform victimTransform = victimBody.gameObject.transform;
                    Transform attackerTransform = attackerBody.gameObject.transform;
                    if (victimTransform && attackerTransform)
                    {
                        position = Vector3.Lerp(victimTransform.position, attackerTransform.position, 0.75f);
                    }
                    position += Vector3.up * 1.8f;
                    position += UnityEngine.Random.insideUnitSphere * 0.5f;

                    Quaternion rotation = Util.QuaternionSafeLookRotation(Vector3.up + UnityEngine.Random.insideUnitSphere * 0.1f);

                    int itemCount = attackerBody.inventory ? attackerBody.inventory.GetItemCount(RoR2Content.Items.Dagger) : 0;
                    float damageValue = Util.OnKillProcDamage(attackerBody.damage, 3f + 1.5f * itemCount);
                    float force = 200f;

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

                if (damageInfo.HasModdedDamageType(MissileShot) && CheckRoll(procChance, attackerBody.master))
                {
                    int icbmCount = 0;
                    int missileCount = 0;

                    if (attackerBody.inventory)
                    {
                        icbmCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                        missileCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.MissileVoid);
                        missileCount += attackerBody.inventory.GetItemCount(RoR2Content.Items.Missile);
                    }

                    float missileDamage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 1.5f + 1.5f * missileCount) * DriverPlugin.GetICBMDamageMult(attackerBody);

                    ProcChainMask procChainMask = damageInfo.procChainMask;
                    procChainMask.AddProc(ProcType.Missile);

                    var initialDirection = Vector3.up + UnityEngine.Random.insideUnitSphere * 0.1f;

                    FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                    {
                        projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/MissileProjectile.prefab").WaitForCompletion(),
                        position = attackerBody.corePosition,
                        rotation = Util.QuaternionSafeLookRotation(initialDirection),
                        procChainMask = procChainMask,
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

                        FireProjectileInfo fireProjectileInfo2 = fireProjectileInfo;
                        fireProjectileInfo2.rotation = Util.QuaternionSafeLookRotation(Quaternion.AngleAxis(45f, axis) * initialDirection);
                        ProjectileManager.instance.FireProjectile(fireProjectileInfo2);

                        FireProjectileInfo fireProjectileInfo3 = fireProjectileInfo;
                        fireProjectileInfo3.rotation = Util.QuaternionSafeLookRotation(Quaternion.AngleAxis(-45f, axis) * initialDirection);
                        ProjectileManager.instance.FireProjectile(fireProjectileInfo3);
                    }
                } // end atg

                if (damageInfo.HasModdedDamageType(LightningStrikeRounds) && CheckRoll(procChance, attackerBody.master))
                {
                    int itemCount = attackerBody.inventory ? attackerBody.inventory.GetItemCount(RoR2Content.Items.LightningStrikeOnHit) : 0;
                    float damageValue = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 2.5f + 2.5f * itemCount);

                    ProcChainMask procChainMask = damageInfo.procChainMask;
                    procChainMask.AddProc(ProcType.LightningStrikeOnHit);

                    HurtBox target = victimBody.mainHurtBox;
                    if (victimBody.hurtBoxGroup)
                    {
                        target = victimBody.hurtBoxGroup.hurtBoxes[UnityEngine.Random.Range(0, victimBody.hurtBoxGroup.hurtBoxes.Length)];
                    }

                    OrbManager.instance.AddOrb(new SimpleLightningStrikeOrb
                    {
                        attacker = attackerBody.gameObject,
                        damageColorIndex = DamageColorIndex.Item,
                        damageValue = damageValue,
                        isCrit = Util.CheckRoll(attackerBody.crit, attackerBody.master),
                        procChainMask = procChainMask,
                        procCoefficient = 1f,
                        target = target
                    });
                } // end cherf
                
                if (damageInfo.HasModdedDamageType(FireballRounds) && CheckRoll(procChance, attackerBody.master))
                {
                    Vector3 origin = (attackerBody.characterMotor ? (victim.transform.position + Vector3.up * (attackerBody.characterMotor.capsuleHeight * 0.5f + 2f)) : (victim.transform.position + Vector3.up * 2f));
                    EffectData effectData = new EffectData
                    {
                        scale = 1f,
                        origin = origin
                    };
                    EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/MuzzleFlashes/MuzzleflashFireMeatBall"), effectData, transmit: true);

                    int itemCount = attackerBody.inventory ? attackerBody.inventory.GetItemCount(RoR2Content.Items.FireballsOnHit) : 0;
                    float damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 1.5f + 1.5f * itemCount);

                    ProcChainMask procChainMask = damageInfo.procChainMask;
                    procChainMask.AddProc(ProcType.Meatball);

                    int fireballCount = 3;
                    Vector3 rotation = Vector3.up;
                    for (int i = 0; i < fireballCount; i++)
                    {
                        float offset = i * (float)Math.PI * 2f / fireballCount;
                        ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                        {
                            projectilePrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/FireMeatBall"),
                            position = origin + new Vector3(Mathf.Sin(offset), 0f, Mathf.Cos(offset)),
                            rotation = Util.QuaternionSafeLookRotation(rotation),
                            procChainMask = procChainMask,
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
                
                if (damageInfo.HasModdedDamageType(StickyShot) && CheckRoll(procChance, attackerBody.master))
                {
                    Vector3 forward = victimBody.corePosition - damageInfo.position;
                    Quaternion rotation = forward.magnitude != 0f ? Util.QuaternionSafeLookRotation(forward) : UnityEngine.Random.rotationUniform;

                    int itemCount = attackerBody.inventory ? attackerBody.inventory.GetItemCount(RoR2Content.Items.StickyBomb) : 0;
                    float damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 1.8f + 0.2f * itemCount);

                    ProjectileManager.instance.FireProjectile(LegacyResourcesAPI.Load<GameObject>("Prefabs/Projectiles/StickyBomb"),
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
                
                if (damageInfo.HasModdedDamageType(VoidLightning) && CheckRoll(procChance, attackerBody.master))
                {
                    int itemCount = 0;
                    if (attackerBody.inventory)
                    {
                        itemCount = attackerBody.inventory.GetItemCount(DLC1Content.Items.ChainLightningVoid);
                        itemCount += attackerBody.inventory.GetItemCount(RoR2Content.Items.ChainLightning);
                    }
                    float damage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, 0.6f);

                    ProcChainMask procChainMask = damageInfo.procChainMask;
                    procChainMask.AddProc(ProcType.ChainLightning);

                    VoidLightningOrb voidLightningOrb = new VoidLightningOrb
                    {
                        origin = damageInfo.position,
                        damageValue = damage,
                        isCrit = damageInfo.crit,
                        totalStrikes = 3 + 2 * itemCount,
                        teamIndex = attackerTeamIndex,
                        attacker = damageInfo.attacker,
                        procChainMask = procChainMask,
                        procCoefficient = 0.2f,
                        damageColorIndex = DamageColorIndex.Void,
                        secondsPerStrike = 0.1f
                    };

                    if (victimBody)
                    {
                        voidLightningOrb.target = victimBody.mainHurtBox;
                        OrbManager.instance.AddOrb(voidLightningOrb);
                    }
                } // end polylute
                
                if (damageInfo.HasModdedDamageType(CoinShot) && CheckRoll(procChance, attackerBody.master))
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

                if (damageInfo.HasModdedDamageType(Hemorrhage) && CheckRoll(procChance, attackerBody.master))
                {
                    if (victimBody && victimBody.healthComponent && victimBody.healthComponent.alive)
                    {
                        DotController.InflictDot(
                            victim,
                            damageInfo.attacker,
                            DotController.DotIndex.SuperBleed,
                            15f * damageInfo.procCoefficient);
                    }
                } // end superbleed

                if (damageInfo.HasModdedDamageType(Gouge) && CheckRoll(procChance, attackerBody.master))
                {
                    if (victimBody && victimBody.healthComponent && victimBody.healthComponent.alive)
                    {
                        DotController.InflictDot(
                            victim,
                            damageInfo.attacker,
                            Buffs.gougeIndex,
                            4f,
                            1.5f);
                    }
                } // end gouge
            }
        }

        private static void GlobalEventManager_OnHitAll(On.RoR2.GlobalEventManager.orig_OnHitAll orig, GlobalEventManager self, DamageInfo damageInfo, GameObject hitObject)
        {
            orig.Invoke(self, damageInfo, hitObject);

            if (damageInfo.procCoefficient == 0f || damageInfo.rejected || !damageInfo.attacker)
            {
                return;
            }
            CharacterBody attackerBody = damageInfo.attacker.GetComponent<CharacterBody>();
            if (!attackerBody)
            {
                return;
            }
            CharacterMaster master = attackerBody.master;
            if (!master)
            {
                return;
            }
            Inventory inventory = master.inventory;
            if (!inventory)
            {
                return;
            }

            if (attackerBody && attackerBody.baseNameToken == Driver.bodyNameToken && damageInfo.HasModdedDamageType(ExplosiveRounds))
            {
                float radius = (1.5f + 2.5f * inventory.GetItemCount(RoR2Content.Items.Behemoth)) * damageInfo.procCoefficient;
                float damageCoefficient = 0.6f;
                float baseDamage = Util.OnHitProcDamage(damageInfo.damage, attackerBody.damage, damageCoefficient);

                EffectManager.SpawnEffect(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/OmniEffect/OmniExplosionVFXQuick"), new EffectData
                {
                    origin = damageInfo.position,
                    scale = radius,
                    rotation = Util.QuaternionSafeLookRotation(damageInfo.force)
                }, transmit: true);

                BlastAttack obj = new BlastAttack
                {
                    position = damageInfo.position,
                    baseDamage = baseDamage,
                    baseForce = 0f,
                    radius = radius,
                    attacker = damageInfo.attacker,
                    inflictor = null,
                    teamIndex = TeamComponent.GetObjectTeam(damageInfo.attacker),
                    crit = damageInfo.crit,
                    procChainMask = damageInfo.procChainMask,
                    procCoefficient = 0f,
                    damageColorIndex = DamageColorIndex.Item,
                    falloffModel = BlastAttack.FalloffModel.None,
                    damageType = damageInfo.damageType
                };
                obj.Fire();
            }
            #endregion
        }
    }
}