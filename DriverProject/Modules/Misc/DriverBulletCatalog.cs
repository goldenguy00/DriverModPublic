using R2API;
using RoR2.Orbs;
using RoR2;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace RobDriver.Modules
{
    public static class DriverBulletCatalog
    {
        internal static List<DriverBulletDef> bulletDefs { get; private set; } = new List<DriverBulletDef>();

        internal static DriverBulletDef Default { get; private set; }

        internal static DamageType[] allowedDamageTypes = new DamageType[]
        {
            DamageType.ResetCooldownsOnKill,
            DamageType.SlowOnHit,
            DamageType.Stun1s,
            DamageType.IgniteOnHit,
            DamageType.Freeze2s,
            DamageType.ClayGoo,
            DamageType.BleedOnHit,
            DamageType.PoisonOnHit,
            DamageType.WeakOnHit,
            DamageType.Nullify,
            DamageType.BonusToLowHealth,
            DamageType.BlightOnHit,
            DamageType.CrippleOnHit,
            DamageType.FruitOnHit
        };

        internal static void Init()
        {
            InitializeBullets();
        }

        public static void InitializeBullets()
        {
            Default = DriverBulletDef.CreateBulletDefFromInfo(new DriverBulletDefInfo()
            {
                nameToken = "Default",
                tier = DriverWeaponTier.Unique,
                moddedDriverBulletType = DamageTypes.Generic,
                driverBulletType = DamageType.Generic,
                trailColor = Color.white 
            });
            Default.index = 0;
            bulletDefs.Add(Default);

            foreach (DamageType i in allowedDamageTypes)
            {
                //Renaming
                switch (i)
                {
                    // common
                    case DamageType.SlowOnHit:
                        CreateBulletType("Slowing", i, DriverWeaponTier.Common, Color.yellow);
                        break;

                    case DamageType.Stun1s:
                        CreateBulletType("Stunning", i, DriverWeaponTier.Common, Color.gray);
                        break;

                    case DamageType.IgniteOnHit:
                        CreateBulletType("Incendiary", i, DriverWeaponTier.Common, new Color(255f / 255f, 127f / 255f, 80 / 255f));
                        break;

                    case DamageType.ClayGoo:
                        CreateBulletType("Goo", i, DriverWeaponTier.Common, Color.black);
                        break;

                    case DamageType.BleedOnHit:
                        CreateBulletType("Serrated", i, DriverWeaponTier.Common, DamageColorIndex.Bleed);
                        break;

                    case DamageType.PoisonOnHit:
                        CreateBulletType("Poison", i, DriverWeaponTier.Common, Color.green);
                        break;

                    case DamageType.WeakOnHit:
                        CreateBulletType("Weakening", i, DriverWeaponTier.Common, new Color(220f / 255f, 237f / 255f, 159f / 255f));
                        break;

                    case DamageType.BonusToLowHealth:
                        CreateBulletType("Executing", i, DriverWeaponTier.Common, DamageColorIndex.Fragile);
                        break;

                    case DamageType.BlightOnHit:
                        CreateBulletType("Blighting", i, DriverWeaponTier.Common, new Color(222f / 255f, 85f / 255f, 230f / 255f));
                        break;

                    // uncommon
                    case DamageType.ResetCooldownsOnKill:
                        CreateBulletType("Resetting", i, DriverWeaponTier.Uncommon, Color.red);
                        break;

                    case DamageType.CrippleOnHit:
                        CreateBulletType("Crippling", i, DriverWeaponTier.Uncommon, new Color(48f / 255f, 205f / 255f, 217f / 255f));
                        break;

                    case DamageType.FruitOnHit:
                        CreateBulletType("Fruitful", i, DriverWeaponTier.Uncommon, new Color(255f / 255f, 191f / 255f, 225f / 255f));
                        break;

                    case DamageType.Freeze2s:
                        CreateBulletType("Frostbite", i, DriverWeaponTier.Uncommon, Color.cyan);
                        break;

                    // void
                    case DamageType.Nullify:
                        CreateBulletType("Nullifying", i, DriverWeaponTier.Uncommon, DamageColorIndex.Void);
                        break;
                }
            }

            // common
            CreateBulletType("Coin", DamageTypes.CoinShot, DriverWeaponTier.Common, new Color(255 / 255f, 212 / 255f, 94 / 255f));

            // uncommon
            CreateBulletType("Explosive", DamageTypes.ExplosiveRounds, DriverWeaponTier.Uncommon, Color.yellow);
            CreateBulletType("Missle", DamageTypes.MissileShot, DriverWeaponTier.Uncommon, new Color(219 / 255f, 132 / 255f, 11 / 255f));
            CreateBulletType("Elemental Flame", DamageTypes.FlameTornadoShot, DriverWeaponTier.Uncommon, new Color(255f / 255f, 127f / 255f, 80 / 255f));
            CreateBulletType("Elemental Ice", DamageTypes.IceBlastShot, DriverWeaponTier.Uncommon, Color.cyan);
            CreateBulletType("Sticky", DamageTypes.StickyShot, DriverWeaponTier.Uncommon, new Color(255 / 255f, 117 / 255f, 48 / 255f));
            CreateBulletType("Mystery", DamageTypes.MysteryShot, DriverWeaponTier.Uncommon, new Color(30 / 255f, 51 / 255f, 45 / 255f));
            CreateBulletType("Hemorrhaging", DamageTypes.Hemorrhage, DriverWeaponTier.Uncommon, DamageColorIndex.SuperBleed);

            // legendary
            CreateBulletType("Dagger", DamageTypes.DaggerShot, DriverWeaponTier.Legendary, Color.black);
            CreateBulletType("Lightning", DamageTypes.LightningStrikeRounds, DriverWeaponTier.Legendary, Color.cyan);
            CreateBulletType("Fireball", DamageTypes.FireballRounds, DriverWeaponTier.Legendary, new Color(255f / 255f, 127f / 255f, 80 / 255f));
            CreateBulletType("Hook", DamageTypes.HookShot, DriverWeaponTier.Legendary, Color.grey);

            //void
            CreateBulletType("Void Missile", DamageTypes.VoidMissileShot, DriverWeaponTier.Uncommon, new Color(122 / 255f, 69 / 255f, 173 / 255f));
            CreateBulletType("Void Lightning", DamageTypes.VoidLightning, DriverWeaponTier.Legendary, new Color(194 / 255f, 115 / 255f, 255 / 255f));

        }

        public static void CreateBulletType(string nameToken, DamageType damageType, DriverWeaponTier tier, DamageColorIndex color)
        {
            CreateBulletType(nameToken, damageType, DamageTypes.Generic, tier, DamageColor.FindColor(color));
        }

        public static void CreateBulletType(string nameToken, DamageType damageType, DriverWeaponTier tier, Color color)
        {
            CreateBulletType(nameToken, damageType, DamageTypes.Generic, tier, color);
        }

        public static void CreateBulletType(string nameToken, DamageAPI.ModdedDamageType moddedBulletType, DriverWeaponTier tier, DamageColorIndex color)
        {
            CreateBulletType(nameToken, DamageType.Generic, moddedBulletType, tier, DamageColor.FindColor(color));
        }

        public static void CreateBulletType(string nameToken, DamageAPI.ModdedDamageType moddedBulletType, DriverWeaponTier tier, Color color)
        {
            CreateBulletType(nameToken, DamageType.Generic, moddedBulletType, tier, color);
        }

        public static void CreateBulletType(string nameToken, DamageType damageType, DamageAPI.ModdedDamageType moddedBulletType, DriverWeaponTier tier, Color color)
        {
            var bulletDef = DriverBulletDef.CreateBulletDefFromInfo(new DriverBulletDefInfo
            {
                nameToken = nameToken,
                driverBulletType = damageType,
                moddedDriverBulletType = moddedBulletType,
                tier = tier,
                trailColor = color
            });
            bulletDef.index = (ushort)bulletDefs.Count;
            bulletDefs.Add(bulletDef);
        }

        public static DriverBulletDef GetBulletDefFromIndex(int index)
        {
            return bulletDefs.ElementAtOrDefault(index) ?? Default;
        }

        public static DriverBulletDef GetWeightedRandomBullet(DriverWeaponTier maxTier)
        {
            int commonWeight = 5;
            int uncommonWeight = maxTier >= DriverWeaponTier.Uncommon ? 3 : 0;
            int legendaryWeight = maxTier >= DriverWeaponTier.Legendary ? 1 : 0;
            int rnd = UnityEngine.Random.Range(0, commonWeight + uncommonWeight + legendaryWeight);

            if (rnd < commonWeight) return GetRandomBulletFromTier(DriverWeaponTier.Common);
            if (rnd < commonWeight + uncommonWeight) return GetRandomBulletFromTier(DriverWeaponTier.Uncommon);
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

            if (validBullets.Count == 0) return Default;

            int rnd = UnityEngine.Random.Range(0, validBullets.Count());
            return validBullets[rnd];
        }
    }
}