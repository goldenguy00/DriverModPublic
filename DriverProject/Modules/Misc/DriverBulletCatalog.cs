using R2API;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RobDriver
{
    public static class DriverBulletCatalog
    {
        internal static List<DriverBulletDef> bulletDefs { get; private set; } = new List<DriverBulletDef>();

        internal static DriverBulletDef Default { get; private set; }

        // Common
        internal static DriverBulletDef Slowing;
        internal static DriverBulletDef Stunning;
        internal static DriverBulletDef Incendiary;
        internal static DriverBulletDef Goo;
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

        // Legendary
        internal static DriverBulletDef Dagger;
        internal static DriverBulletDef Lightning;
        internal static DriverBulletDef Fireball;
        internal static DriverBulletDef Hook;
        internal static DriverBulletDef Ruinous;
        internal static DriverBulletDef GiantSlayer;
        internal static DriverBulletDef Disabling;
        internal static DriverBulletDef Warping;
        internal static DriverBulletDef Amputating;

        // Void
        internal static DriverBulletDef VoidMissile;
        internal static DriverBulletDef VoidLightning;

        internal static void Init()
        {
            Default = CreateBulletType("Default", DriverWeaponTier.Unique, Color.white, DamageType.Generic);

            // Common
            Slowing = CreateBulletType("Slowing", DriverWeaponTier.Common, Color.yellow, DamageType.SlowOnHit);
            Stunning = CreateBulletType("Stunning", DriverWeaponTier.Common, Color.gray, DamageType.Stun1s);
            Incendiary = CreateBulletType("Incendiary", DriverWeaponTier.Common, new Color32(255, 127, 80, 255), DamageType.IgniteOnHit);
            Goo = CreateBulletType("Goo", DriverWeaponTier.Common, Color.black, DamageType.ClayGoo);
            Serrated = CreateBulletType("Serrated", DriverWeaponTier.Common, DamageColor.FindColor(DamageColorIndex.Bleed), DamageType.BleedOnHit);
            Poison = CreateBulletType("Poison", DriverWeaponTier.Common, Color.green, DamageType.PoisonOnHit);
            Weakening = CreateBulletType("Weakening", DriverWeaponTier.Common, new Color32(220, 237, 159, 255), DamageType.WeakOnHit);
            Executing = CreateBulletType("Executing", DriverWeaponTier.Common, DamageColor.FindColor(DamageColorIndex.Fragile), DamageType.BonusToLowHealth);
            Blighting = CreateBulletType("Blighting", DriverWeaponTier.Common, new Color32(222, 85, 230, 255), DamageType.BlightOnHit);

            Coin = CreateBulletType("Coin", DriverWeaponTier.Common, new Color32(255, 212, 94, 255), DriverDamageTypes.CoinShot);
            Mystery = CreateBulletType("Mystery", DriverWeaponTier.Common, new Color32(30, 51, 45, 255), DriverDamageTypes.MysteryShot);

            // Uncommon
            Resetting = CreateBulletType("Resetting", DriverWeaponTier.Uncommon, Color.red, DamageType.ResetCooldownsOnKill);
            Crippling = CreateBulletType("Crippling", DriverWeaponTier.Uncommon, new Color32(48, 205, 217, 255), DamageType.CrippleOnHit);
            Fruitful = CreateBulletType("Fruitful", DriverWeaponTier.Uncommon, new Color32(255, 191, 225, 255), DamageType.FruitOnHit);
            Frostbite = CreateBulletType("Frostbite", DriverWeaponTier.Uncommon, Color.cyan, DamageType.Freeze2s);

            Explosive = CreateBulletType("Explosive", DriverWeaponTier.Uncommon, Color.yellow, DriverDamageTypes.ExplosiveRounds);
            Missile = CreateBulletType("Missle", DriverWeaponTier.Uncommon, new Color32(219, 132, 11, 255), DriverDamageTypes.MissileShot);
            Flaming = CreateBulletType("Elemental Flame", DriverWeaponTier.Uncommon, new Color32(255, 127, 80, 255), DriverDamageTypes.FlameTornadoShot);
            Icy = CreateBulletType("Elemental Ice", DriverWeaponTier.Uncommon, Color.cyan, DriverDamageTypes.IceBlastShot);
            Sticky = CreateBulletType("Sticky", DriverWeaponTier.Uncommon, new Color32(255, 117, 48, 255), DriverDamageTypes.StickyShot);
            Hemorrhaging = CreateBulletType("Hemorrhaging", DriverWeaponTier.Uncommon, DamageColor.FindColor(DamageColorIndex.SuperBleed), DriverDamageTypes.Hemorrhage);

            // Legendary
            Dagger = CreateBulletType("Dagger", DriverWeaponTier.Legendary, Color.black, DriverDamageTypes.DaggerShot);
            Lightning = CreateBulletType("Lightning", DriverWeaponTier.Legendary, Color.cyan, DriverDamageTypes.LightningStrikeRounds);
            Fireball = CreateBulletType("Fireball", DriverWeaponTier.Legendary, new Color32(255, 127, 80, 255), DriverDamageTypes.FireballRounds);
            Hook = CreateBulletType("Hook", DriverWeaponTier.Legendary, Color.grey, DriverDamageTypes.HookShot);
            Ruinous = CreateBulletType("Ruinous", DriverWeaponTier.Legendary, DamageColor.FindColor(DamageColorIndex.DeathMark), DamageType.LunarRuin);
            GiantSlayer = CreateBulletType("Giant Slayer", DriverWeaponTier.Legendary, DamageColor.FindColor(DamageColorIndex.DeathMark), DamageTypeExtended.DamagePercentOfMaxHealth);
            Disabling = CreateBulletType("Disabling", DriverWeaponTier.Legendary, DamageColor.FindColor(DamageColorIndex.DeathMark), DamageTypeExtended.DisableAllSkills);
            Warping = CreateBulletType("Warping", DriverWeaponTier.Legendary, DamageColor.FindColor(DamageColorIndex.DeathMark), DamageTypeExtended.Warp);
            Amputating = CreateBulletType("Amputating", DriverWeaponTier.Legendary, DamageColor.FindColor(DamageColorIndex.DeathMark), DamageTypeExtended.Amputate);

            // Void
            Nullifying = CreateBulletType("Nullifying", DriverWeaponTier.Uncommon, DamageColor.FindColor(DamageColorIndex.Void), DamageType.Nullify);
            VoidMissile = CreateBulletType("Void Missile", DriverWeaponTier.Legendary, new Color32(122, 69, 173, 255), DriverDamageTypes.VoidMissileShot);
            VoidLightning = CreateBulletType("Void Lightning", DriverWeaponTier.Legendary, new Color32(194, 115, 255, 255), DriverDamageTypes.VoidLightning);
        }

        public static DriverBulletDef CreateBulletType(string nameToken, DriverWeaponTier tier, Color color, DamageType damageType) => CreateBulletType(nameToken, tier, color, damageType, null, null);
        public static DriverBulletDef CreateBulletType(string nameToken, DriverWeaponTier tier, Color color, DamageTypeExtended damageType) => CreateBulletType(nameToken, tier, color, null, damageType, null);
        public static DriverBulletDef CreateBulletType(string nameToken, DriverWeaponTier tier, Color color, DamageAPI.ModdedDamageType damageType) => CreateBulletType(nameToken, tier, color, null, null, damageType);
        public static DriverBulletDef CreateBulletType(string nameToken, DriverWeaponTier tier, Color color, DamageType? damageType, DamageTypeExtended? damageTypeExtended, DamageAPI.ModdedDamageType? moddedDamageType)
        {
            var bulletDef = DriverBulletDef.CreateBulletDefFromInfo(new DriverBulletDefInfo
            {
                nameToken = nameToken,
                damageType = damageType,
                damageTypeExtended = damageTypeExtended,
                moddedDamageType = moddedDamageType,
                tier = tier,
                trailColor = color
            });
            bulletDef.index = (ushort)bulletDefs.Count;
            bulletDefs.Add(bulletDef);

            return bulletDef;
        }

        public static DriverBulletDef CreateBulletType(DriverBulletDefInfo bulletDefInfo)
        {
            var bulletDef = DriverBulletDef.CreateBulletDefFromInfo(bulletDefInfo);
            bulletDef.index = (ushort)bulletDefs.Count;
            bulletDefs.Add(bulletDef);

            return bulletDef;
        }

        public static DriverBulletDef GetBulletDefFromIndex(int index)
        {
            var bullet = bulletDefs.ElementAtOrDefault(index);
            if (!bullet)
                Log.Error("Failed to get bullet at index " + index);

            return bullet ?? Default;
        }

        public static DriverBulletDef GetWeightedRandomBullet(DriverWeaponTier maxTier)
        {
            int commonWeight = 5;
            int uncommonWeight = maxTier >= DriverWeaponTier.Uncommon ? 3 : 0;
            int legendaryWeight = maxTier >= DriverWeaponTier.Legendary ? 1 : 0;
            int rnd = Random.Range(0, commonWeight + uncommonWeight + legendaryWeight);

            if (rnd < commonWeight)
                return GetRandomBulletFromTier(DriverWeaponTier.Common);

            if (rnd < commonWeight + uncommonWeight)
                return GetRandomBulletFromTier(DriverWeaponTier.Uncommon);

            return GetRandomBulletFromTier(DriverWeaponTier.Legendary);
        }

        public static DriverBulletDef GetRandomBulletFromTier(DriverWeaponTier tier)
        {
            var validBullets = new List<DriverBulletDef>();

            foreach (var bulletDef in bulletDefs)
            {
                if (bulletDef.tier == tier)
                {
                    validBullets.Add(bulletDef);
                }
            }

            return validBullets.Count > 0
                ? validBullets[Random.Range(0, validBullets.Count)]
                : Default;
        }
    }
}