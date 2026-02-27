using R2API;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using RobDriver.Modules;
using System;

namespace RobDriver
{
    public static class DriverBulletCatalog
    {
        public static DriverBulletDef[] bulletDefs = [];

        public static DriverBulletDef Default;

        // Common
        internal static DriverBulletDef Stunning;
        internal static DriverBulletDef Incendiary;
        internal static DriverBulletDef Serrated;
        internal static DriverBulletDef Poison;
        internal static DriverBulletDef Weakening;
        internal static DriverBulletDef Executing;
        internal static DriverBulletDef Blighting;
        internal static DriverBulletDef Resetting;

        // Uncommon
        internal static DriverBulletDef Crippling;
        internal static DriverBulletDef Fruitful;
        internal static DriverBulletDef Frostbite;
        internal static DriverBulletDef Nullifying;
        internal static DriverBulletDef Coin;
        internal static DriverBulletDef Explosive;
        internal static DriverBulletDef Missile;
        internal static DriverBulletDef Flaming;
        internal static DriverBulletDef Icy;
        internal static DriverBulletDef Sticky;
        internal static DriverBulletDef Mystery;
        internal static DriverBulletDef Hemorrhaging;
        internal static DriverBulletDef Helfire;
        internal static DriverBulletDef Infernal;

        // Legendary
        internal static DriverBulletDef Dagger;
        internal static DriverBulletDef Lightning;
        internal static DriverBulletDef Fireball;
        internal static DriverBulletDef Hook;
        internal static DriverBulletDef Ruinous;
        internal static DriverBulletDef GiantSlayer;
        internal static DriverBulletDef Disabling;
        //internal static DriverBulletDef Amputating;
        internal static DriverBulletDef Gouging;

        // Void
        internal static DriverBulletDef VoidMissile;
        internal static DriverBulletDef VoidLightning;
        internal static DriverBulletDef Collapse;

        internal static void InitBulletDefs()
        {
            Default = CreateBulletType("Default", DriverWeaponTier.NoTier, Color.white, DamageType.Generic);

            // Common
            Stunning = CreateBulletType("Stunning", DriverWeaponTier.Common, Color.gray, DamageType.Stun1s);
            Incendiary = CreateBulletType("Incendiary", DriverWeaponTier.Common, new Color32(255, 127, 80, 255), DamageType.IgniteOnHit);
            Serrated = CreateBulletType("Serrated", DriverWeaponTier.Common, DamageColor.FindColor(DamageColorIndex.Bleed), DamageType.BleedOnHit);
            Poison = CreateBulletType("Poison", DriverWeaponTier.Common, Color.green, DamageType.PoisonOnHit);
            Weakening = CreateBulletType("Weakening", DriverWeaponTier.Common, new Color32(220, 237, 159, 255), DamageType.WeakOnHit);
            Executing = CreateBulletType("Executing", DriverWeaponTier.Uncommon, DamageColor.FindColor(DamageColorIndex.Fragile), DamageType.BonusToLowHealth);
            Blighting = CreateBulletType("Blighting", DriverWeaponTier.Common, new Color32(222, 85, 230, 255), DamageType.BlightOnHit);
            Coin = CreateBulletType("Coin", DriverWeaponTier.Common, new Color32(255, 212, 94, 255), DriverDamageTypes.CoinShot);
            Mystery = CreateBulletType("Mystery", DriverWeaponTier.Common, new Color32(30, 51, 45, 255), DriverDamageTypes.MysteryShot);
            Fruitful = CreateBulletType("Fruity", DriverWeaponTier.Common, new Color32(255, 191, 225, 255), DamageType.FruitOnHit);
            Crippling = CreateBulletType("Crippling", DriverWeaponTier.Common, new Color32(48, 205, 217, 255), DamageType.CrippleOnHit);
            Nullifying = CreateBulletType("Nullifying", DriverWeaponTier.Common, DamageColor.FindColor(DamageColorIndex.Void), DamageType.Nullify);

            // Uncommon
            Collapse = CreateBulletType("Collapse", DriverWeaponTier.Uncommon, DamageColor.FindColor(DamageColorIndex.Void), DriverDamageTypes.Collapse);
            Resetting = CreateBulletType("Resetting", DriverWeaponTier.Uncommon, Color.red, DamageType.ResetCooldownsOnKill);
            VoidMissile = CreateBulletType("Void Missile", DriverWeaponTier.Uncommon, new Color32(122, 69, 173, 255), DriverDamageTypes.VoidMissileShot);
            Explosive = CreateBulletType("Explosive", DriverWeaponTier.Uncommon, Color.yellow, DriverDamageTypes.ExplosiveRounds);
            Sticky = CreateBulletType("Sticky", DriverWeaponTier.Uncommon, new Color32(255, 117, 48, 255), DriverDamageTypes.StickyShot);
            Disabling = CreateBulletType("Disabling", DriverWeaponTier.Uncommon, DamageColor.FindColor(DamageColorIndex.DelayedDamage), DamageTypeExtended.DisableAllSkills);
            Flaming = CreateBulletType("Elemental Flame", DriverWeaponTier.Uncommon, new Color32(255, 127, 80, 255), DriverDamageTypes.FlameTornadoShot);
            Infernal = CreateBulletType("Infernal", DriverWeaponTier.Uncommon, new Color32(255, 127, 80, 255), DriverDamageTypes.BetterBurn);
            Ruinous = CreateBulletType("Ruinous", DriverWeaponTier.Uncommon, DamageColor.FindColor(DamageColorIndex.Luminous), DamageType.LunarRuin);
            Icy = CreateBulletType("Elemental Ice", DriverWeaponTier.Uncommon, Color.cyan, DriverDamageTypes.IceBlastShot);
            Hook = CreateBulletType("Hook", DriverWeaponTier.Uncommon, Color.grey, DriverDamageTypes.HookShot);

            // Legendary
            VoidLightning = CreateBulletType("Void Lightning", DriverWeaponTier.Legendary, new Color32(194, 115, 255, 255), DriverDamageTypes.VoidLightning);
            Missile = CreateBulletType("Missile", DriverWeaponTier.Legendary, new Color32(219, 132, 11, 255), DriverDamageTypes.MissileShot);
            Dagger = CreateBulletType("Dagger", DriverWeaponTier.Legendary, Color.black, DriverDamageTypes.DaggerShot);
            Lightning = CreateBulletType("Lightning", DriverWeaponTier.Legendary, Color.cyan, DriverDamageTypes.LightningStrikeRounds);
            Fireball = CreateBulletType("Fireball", DriverWeaponTier.Legendary, new Color32(255, 127, 80, 255), DriverDamageTypes.FireballRounds);
            Frostbite = CreateBulletType("Frostbite", DriverWeaponTier.Legendary, Color.cyan, DamageType.Freeze2s);

            Gouging = CreateBulletType("Gouging", DriverWeaponTier.Unique, DamageColor.FindColor(DamageColorIndex.SuperBleed), DriverDamageTypes.Gouge);
            Hemorrhaging = CreateBulletType("Hemorrhaging", DriverWeaponTier.Unique, DamageColor.FindColor(DamageColorIndex.SuperBleed), DriverDamageTypes.Hemorrhage);
            Helfire = CreateBulletType("Helfire", DriverWeaponTier.Unique, new Color32(255, 127, 127, 255), DriverDamageTypes.Helfire);
            GiantSlayer = CreateBulletType("Giant Slayer", DriverWeaponTier.Unique, DamageColor.FindColor(DamageColorIndex.DeathMark), DamageTypeExtended.DamagePercentOfMaxHealth);
            //Amputating = CreateBulletType("Amputating", DriverWeaponTier.Unique, DamageColor.FindColor(DamageColorIndex.DeathMark), DamageTypeExtended.);
        }

        public static DriverBulletDef CreateBulletType(string bulletName, DriverWeaponTier tier, Color color, DamageType damageType) => CreateBulletType(bulletName, tier, color, damageType, null, null);
        public static DriverBulletDef CreateBulletType(string bulletName, DriverWeaponTier tier, Color color, DamageTypeExtended damageType) => CreateBulletType(bulletName, tier, color, null, damageType, null);
        public static DriverBulletDef CreateBulletType(string bulletName, DriverWeaponTier tier, Color color, DamageAPI.ModdedDamageType damageType) => CreateBulletType(bulletName, tier, color, null, null, damageType);
        public static DriverBulletDef CreateBulletType(string bulletName, DriverWeaponTier tier, Color color, DamageType? damageType, DamageTypeExtended? damageTypeExtended, DamageAPI.ModdedDamageType? moddedDamageType)
        {
            return CreateAndAddBullet(new DriverBulletDefInfo
            {
                bulletName = bulletName,
                bulletNameToken = "ROB_DRIVER_BULLET_" + bulletName.ToUpper() + "_NAME",
                description = bulletName,
                descriptionToken = "ROB_DRIVER_BULLET_" + bulletName.ToUpper() + "_DESC",
                damageType = damageType,
                damageTypeExtended = damageTypeExtended,
                moddedDamageType = moddedDamageType,
                tier = tier,
                trailColor = color
            });
        }

        public static DriverBulletDef CreateAndAddBullet(DriverBulletDefInfo bulletDefInfo)
        {
            if (!string.IsNullOrEmpty(bulletDefInfo.bulletNameToken) && !string.IsNullOrEmpty(bulletDefInfo.bulletName))
                R2API.LanguageAPI.Add(bulletDefInfo.bulletNameToken, bulletDefInfo.bulletName);

            if (!string.IsNullOrEmpty(bulletDefInfo.descriptionToken) && !string.IsNullOrEmpty(bulletDefInfo.description))
                R2API.LanguageAPI.Add(bulletDefInfo.descriptionToken, bulletDefInfo.description);

            var bulletDef = DriverBulletDef.CreateBulletDefFromInfo(bulletDefInfo);

            // do it here so the editor doesnt fuckin implode
            bulletDef.damageType.AddModdedDamageType(bulletDefInfo.moddedDamageType ?? DriverDamageTypes.Generic);

            return CreateAndAddBullet(bulletDef);
        }

        public static DriverBulletDef CreateAndAddBullet(DriverBulletDef bulletDef)
        {
            Array.Resize(ref bulletDefs, bulletDefs.Length + 1);

            int index = bulletDefs.Length - 1;
            bulletDef.index = (ushort)index;
            bulletDefs[index] = bulletDef;

            Config.InitBulletConfig(bulletDef);

            Log.Debug("Added " + bulletDef.bulletName + " to catalog with tier: " + bulletDef.tier);

            return bulletDef;
        }

        public static DriverBulletDef GetBulletFromIndex(int index) => HG.ArrayUtils.GetSafe(bulletDefs, index, Default);

        public static DriverBulletDef GetWeightedRandomBullet(DriverWeaponTier tier)
        {
            int commonWeight = 60;
            int uncommonWeight = tier >= DriverWeaponTier.Uncommon ? 35 : 0;
            int legendaryWeight = tier >= DriverWeaponTier.Legendary ? 2 : 0;
            int uniqueWeight = tier >= DriverWeaponTier.Unique ? 1 : 0;
            int rnd = UnityEngine.Random.Range(0, commonWeight + uncommonWeight + legendaryWeight + uniqueWeight);

            if (rnd < commonWeight) tier = DriverWeaponTier.Common;
            else if (rnd < commonWeight + uncommonWeight) tier = DriverWeaponTier.Uncommon;
            else if (rnd < commonWeight + uncommonWeight + legendaryWeight) tier = DriverWeaponTier.Legendary;

            return GetRandomBulletFromTier(tier);
        }

        public static DriverBulletDef GetRandomBulletFromTier(DriverWeaponTier tier)
        {
            var validBullets = new List<DriverBulletDef>();

            foreach (var bulletDef in bulletDefs)
            {
                if (bulletDef.enabled && bulletDef.tier != DriverWeaponTier.NoTier)
                {
                    if (bulletDef.tier == tier)
                        validBullets.Add(bulletDef);

                    if (Config.uniqueDropsAreLegendary.Value && tier == DriverWeaponTier.Legendary)
                    {
                        if (bulletDef.tier > DriverWeaponTier.Legendary)
                            validBullets.Add(bulletDef);
                    }
                }
            }

            return validBullets.Count > 0
                ? validBullets[UnityEngine.Random.Range(0, validBullets.Count)]
                : Default;
        }
    }
}