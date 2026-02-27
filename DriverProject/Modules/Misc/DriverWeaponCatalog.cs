using RobDriver.Modules;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver
{
    public static class DriverWeaponCatalog
    {
        public struct WeaponDrop(ushort weaponIndex, float dropChance)
        {
            public ushort weaponIndex = weaponIndex;
            public float dropChance = dropChance;
        }

        public static Dictionary<BodyIndex, WeaponDrop> weaponDrops = [];
        public static DriverWeaponDef[] weaponDefs = [];

        internal static DriverWeaponDef Pistol;
        internal static DriverWeaponDef LunarPistol;
        internal static DriverWeaponDef VoidPistol;
        internal static DriverWeaponDef PyriteGun;
        internal static DriverWeaponDef BeetleShield;

        internal static DriverWeaponDef Needler;
        internal static DriverWeaponDef GoldenGun;

        internal static DriverWeaponDef Shotgun;
        internal static DriverWeaponDef RiotShotgun;
        internal static DriverWeaponDef SlugShotgun;

        internal static DriverWeaponDef MachineGun;
        internal static DriverWeaponDef HeavyMachineGun;
        internal static DriverWeaponDef Sniper;

        internal static DriverWeaponDef Bazooka;
        internal static DriverWeaponDef GrenadeLauncher;
        internal static DriverWeaponDef RocketLauncher;
        internal static DriverWeaponDef Behemoth;
        internal static DriverWeaponDef PrototypeRocketLauncher;
        internal static DriverWeaponDef ArmCannon;
        internal static DriverWeaponDef PlasmaCannon;

        internal static DriverWeaponDef BadassShotgun;
        internal static DriverWeaponDef LunarRifle;
        internal static DriverWeaponDef LunarHammer;

        internal static DriverWeaponDef NemmandoGun;
        internal static DriverWeaponDef NemmercGun;
        internal static DriverWeaponDef GolemRifle;


        internal static void InitWeaponDefs()
        {
            #region Weapons
            DriverWeaponCatalog.Pistol = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Pistol",
                nameToken = "ROB_DRIVER_PISTOL_NAME",
                description = "A reliable handgun that excels at nothing.",
                descriptionToken = "ROB_DRIVER_PISTOL_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolWeaponIcon"),
                tier = DriverWeaponTier.NoTier,
                animationSet = DriverWeaponDef.AnimationSet.Default,
                buffType = DriverWeaponDef.BuffType.Crit,
                shotCount = 26,

                primarySkillDef = Skills.pistolPrimarySkillDef,
                secondarySkillDef = Skills.pistolSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("PISTOL"),
                unlockableDef = null,

                mesh = Modules.Assets.LoadMesh("meshPistol"),
                material = Modules.Assets.pistolMat,
                crosshairPrefab = Modules.Assets.defaultCrosshairPrefab,
                pickupPrefabOverride = Modules.Assets.commonPickupModel,
                colorOveride = Helpers.uniqueItemColor,

                reloadAnimationString = "ReloadPistol",
                equipAnimationString = "EquipPistol",
                calloutSoundString = "",
                dropChance = 0f,
                disableHolster = false,
                dropBodyName = ""
            });

            DriverWeaponCatalog.LunarPistol = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Lunar Pistol",
                nameToken = "ROB_DRIVER_LUNAR_PISTOL_NAME",
                description = "A perfect weapon with no flaws. Speed is war.",
                descriptionToken = "ROB_DRIVER_LUNAR_PISTOL_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texLunarPistolWeaponIcon"),
                tier = DriverWeaponTier.NoTier,
                animationSet = DriverWeaponDef.AnimationSet.Default,
                buffType = DriverWeaponDef.BuffType.Damage,
                shotCount = 52,

                primarySkillDef = Skills.lunarPistolPrimarySkillDef,
                secondarySkillDef = Skills.lunarPistolSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("LUNAR_PISTOL"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("LUNAR_PISTOL"),

                mesh = Modules.Assets.LoadMesh("meshLunarPistol"),
                material = Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarGolem/matLunarGolem.mat").WaitForCompletion(),
                crosshairPrefab = Modules.Assets.defaultCrosshairPrefab,
                pickupPrefabOverride = Modules.Assets.lunarPickupModel,
                colorOveride = Helpers.lunarItemColor,

                reloadAnimationString = "ReloadPistol",
                equipAnimationString = "EquipPistol",
                calloutSoundString = "",
                dropChance = 0f,
                disableHolster = false,
                dropBodyName = ""
            });

            DriverWeaponCatalog.VoidPistol = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Void Pistol",
                nameToken = "ROB_DRIVER_VOID_PISTOL_NAME",
                description = "A weapon corrupted and powered up by the void.",
                descriptionToken = "ROB_DRIVER_VOID_PISTOL_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texVoidPistolWeaponIcon"),
                tier = DriverWeaponTier.NoTier,
                animationSet = DriverWeaponDef.AnimationSet.Default,
                buffType = DriverWeaponDef.BuffType.Damage,
                shotCount = 52,

                primarySkillDef = Skills.lunarPistolPrimarySkillDef,
                secondarySkillDef = Skills.lunarPistolSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("VOID_PISTOL"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("VOID_PISTOL"),

                mesh = Modules.Assets.LoadMesh("meshVoidPistol"),
                material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidJailer/matVoidJailer.mat").WaitForCompletion(),
                crosshairPrefab = Modules.Assets.defaultCrosshairPrefab,
                pickupPrefabOverride = Modules.Assets.voidPickupModel,
                colorOveride = Helpers.voidItemColor,

                reloadAnimationString = "ReloadPistol",
                equipAnimationString = "EquipPistol",
                calloutSoundString = "",
                dropChance = 0f,
                disableHolster = false,
            });

            new Modules.Weapons.FalsePistol();

            DriverWeaponCatalog.PyriteGun = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Pyrite Gun",
                nameToken = "ROB_DRIVER_PYRITEGUN_NAME",
                description = "A mockery of the real thing.",
                descriptionToken = "ROB_DRIVER_PYRITEGUN_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPyriteGunWeaponIcon"),
                tier = DriverWeaponTier.NoTier,
                animationSet = DriverWeaponDef.AnimationSet.Default,
                buffType = DriverWeaponDef.BuffType.Crit,
                shotCount = 18,

                primarySkillDef = Skills.pyriteGunPrimarySkillDef,
                secondarySkillDef = Skills.pyriteGunSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("PYRITEGUN"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("PYRITEGUN"),

                mesh = Modules.Assets.LoadMesh("meshGoldenGun"),
                material = Modules.Assets.LoadMaterial("matPyriteGun"),
                crosshairPrefab = Modules.Assets.defaultCrosshairPrefab,
                pickupPrefabOverride = Modules.Assets.uniquePickupModel,
                colorOveride = Helpers.uniqueItemColor,

                reloadAnimationString = "ReloadPistol",
                equipAnimationString = "EquipPistol",
                calloutSoundString = "",
                dropChance = 0f,
                disableHolster = false
            });

            DriverWeaponCatalog.BeetleShield = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Chitin Shield",
                nameToken = "ROB_DRIVER_BEETLESHIELD_NAME",
                description = "An offhand shield to protect you while you use your pistol.",
                descriptionToken = "ROB_DRIVER_BEETLESHIELD_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBeetleShieldWeaponIcon"),
                tier = DriverWeaponTier.Unique,
                animationSet = DriverWeaponDef.AnimationSet.Default,
                buffType = DriverWeaponDef.BuffType.Crit,
                shotCount = 32,

                primarySkillDef = Skills.beetleShieldPrimarySkillDef,
                secondarySkillDef = Skills.beetleShieldSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("BEETLESHIELD"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("BEETLESHIELD"),

                mesh = Modules.Assets.LoadMesh("meshBeetleShield"),
                material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Beetle/matBeetle.mat").WaitForCompletion(),
                crosshairPrefab = Modules.Assets.defaultCrosshairPrefab,
                pickupPrefabOverride = Modules.Assets.uniquePickupModel,
                colorOveride = Helpers.uniqueItemColor,

                reloadAnimationString = "ReloadPistol",
                equipAnimationString = "EquipPistol",
                calloutSoundString = "",
                dropChance = 0.1f,
                disableHolster = false
            });

            DriverWeaponCatalog.Needler = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Needler",
                nameToken = "ROB_DRIVER_NEEDLER_NAME",
                description = "Risk of Rain 2",
                descriptionToken = "ROB_DRIVER_NEEDLER_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNeedlerWeaponIcon"),
                tier = DriverWeaponTier.NoTier,
                animationSet = DriverWeaponDef.AnimationSet.Default,
                buffType = DriverWeaponDef.BuffType.Damage,
                shotCount = 28,

                primarySkillDef = Addressables.LoadAssetAsync<LunarPrimaryReplacementSkill>("RoR2/Base/LunarSkillReplacements/LunarPrimaryReplacement.asset").WaitForCompletion(),
                secondarySkillDef = Addressables.LoadAssetAsync<LunarSecondaryReplacementSkill>("RoR2/Base/LunarSkillReplacements/LunarSecondaryReplacement.asset").WaitForCompletion(),
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("NEEDLER"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("NEEDLER"),

                mesh = Modules.Assets.LoadMesh("meshNeedler"),
                material = Modules.Assets.LoadMaterial("matNeedler"),
                crosshairPrefab = Modules.Assets.needlerCrosshairPrefab,
                pickupPrefabOverride = Modules.Assets.lunarPickupModel,
                colorOveride = Helpers.lunarItemColor,

                reloadAnimationString = "ReloadPistol",
                equipAnimationString = "EquipPistol",
                calloutSoundString = "",
                dropChance = 100f,
                disableHolster = false
            });

            DriverWeaponCatalog.GoldenGun = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Golden Gun",
                nameToken = "ROB_DRIVER_GOLDENGUN_NAME",
                description = "Deals extraordinary damage, but only has a few shots.",
                descriptionToken = "ROB_DRIVER_GOLDENGUN_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGoldenGunWeaponIcon"),
                tier = DriverWeaponTier.NoTier,
                animationSet = DriverWeaponDef.AnimationSet.Default,
                buffType = DriverWeaponDef.BuffType.Crit,
                shotCount = 6,

                primarySkillDef = Skills.goldenGunPrimarySkillDef,
                secondarySkillDef = Skills.goldenGunSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("GOLDENGUN"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("GOLDENGUN"),

                mesh = Modules.Assets.LoadMesh("meshGoldenGun"),
                material = Modules.Assets.LoadMaterial("matGoldenGun"),
                crosshairPrefab = Modules.Assets.defaultCrosshairPrefab,
                pickupPrefabOverride = Modules.Assets.uniquePickupModel,
                colorOveride = Helpers.uniqueItemColor,

                reloadAnimationString = "ReloadPistol",
                equipAnimationString = "EquipPistol",
                calloutSoundString = "sfx_driver_callout_generic",
                dropChance = 100f,
                disableHolster = false
            });

            DriverWeaponCatalog.Shotgun = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Shotgun",
                nameToken = "ROB_DRIVER_SHOTGUN_NAME",
                description = "Close-range powerhouse with overwhelming damage.",
                descriptionToken = "ROB_DRIVER_SHOTGUN_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunWeaponIcon"),
                tier = DriverWeaponTier.Common,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.Damage,
                shotCount = 8,

                primarySkillDef = Skills.shotgunPrimarySkillDef,
                secondarySkillDef = Skills.bashSkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("SHOTGUN"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("SHOTGUN"),

                mesh = Modules.Assets.LoadMesh("meshSuperShotgun"),
                material = Modules.Assets.LoadMaterial("matShotgun"),
                crosshairPrefab = Modules.Assets.shotgunCrosshairPrefab,
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "ReloadShotgun",
                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_shotgun",
                dropChance = 0f,
                disableHolster = false
            });

            DriverWeaponCatalog.RiotShotgun = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Riot Shotgun",
                nameToken = "ROB_DRIVER_RIOT_SHOTGUN_NAME",
                description = "Piercing blasts great for crowd control.",
                descriptionToken = "ROB_DRIVER_RIOT_SHOTGUN_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRiotShotgunWeaponIcon"),
                tier = DriverWeaponTier.Common,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.AttackSpeed,
                shotCount = 8,

                primarySkillDef = Skills.riotShotgunPrimarySkillDef,
                secondarySkillDef = Skills.bashSkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("RIOT_SHOTGUN"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("RIOT_SHOTGUN"),

                mesh = Modules.Assets.LoadMesh("meshRiotShotgun"),
                material = Modules.Assets.LoadMaterial("matRiotShotgun"),
                crosshairPrefab = Modules.Assets.shotgunCrosshairPrefab,
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "ReloadPistol",
                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_shotgun",
                dropChance = 0f,
                disableHolster = false
            });

            DriverWeaponCatalog.SlugShotgun = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Slug Shotgun",
                nameToken = "ROB_DRIVER_SLUG_SHOTGUN_NAME",
                description = "Powerful single hits with heavy kickback.",
                descriptionToken = "ROB_DRIVER_SLUG_SHOTGUN_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlugShotgunWeaponIcon"),
                tier = DriverWeaponTier.Common,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.Damage,
                shotCount = 8,

                primarySkillDef = Skills.slugShotgunPrimarySkillDef,
                secondarySkillDef = Skills.bashSkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("SLUG_SHOTGUN"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("SLUG_SHOTGUN"),

                mesh = Modules.Assets.LoadMesh("meshSlugShotgun"),
                material = Modules.Assets.LoadMaterial("matSlugShotgun"),
                crosshairPrefab = Modules.Assets.shotgunCrosshairPrefab,
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "ReloadPistol",
                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_shotgun",
                dropChance = 0f,
                disableHolster = false
            });

            DriverWeaponCatalog.MachineGun = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Machine Gun",
                nameToken = "ROB_DRIVER_MACHINEGUN_NAME",
                description = "Shoots fast but has high spread.",
                descriptionToken = "ROB_DRIVER_MACHINEGUN_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMachineGunWeaponIcon"),
                tier = DriverWeaponTier.Common,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.AttackSpeed,
                shotCount = 48,

                primarySkillDef = Skills.machineGunPrimarySkillDef,
                secondarySkillDef = Skills.machineGunSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("MACHINEGUN"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("MACHINEGUN"),

                mesh = Modules.Assets.LoadMesh("meshMachineGun"),
                material = Modules.Assets.LoadMaterial("matMachineGun"),
                crosshairPrefab = Modules.Assets.defaultCrosshairPrefab,
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "ReloadPistol",
                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_machine_gun",
                dropChance = 0f,
                disableHolster = false
            });

            DriverWeaponCatalog.HeavyMachineGun = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Heavy Machine Gun",
                nameToken = "ROB_DRIVER_HEAVY_MACHINEGUN_NAME",
                description = "Accurate, armor piercing rounds.",
                descriptionToken = "ROB_DRIVER_HEAVY_MACHINEGUN_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texHeavyMachineGunWeaponIcon"),
                tier = DriverWeaponTier.Common,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.AttackSpeed,
                shotCount = 44,

                primarySkillDef = Skills.heavyMachineGunPrimarySkillDef,
                secondarySkillDef = Skills.heavyMachineGunSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("HEAVY_MACHINEGUN"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("HEAVY_MACHINEGUN"),

                mesh = Modules.Assets.LoadMesh("meshHeavyMachineGun"),
                material = Modules.Assets.LoadMaterial("matHeavyMachineGun"),
                crosshairPrefab = Modules.Assets.defaultCrosshairPrefab,
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "ReloadPistol",
                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_hmg",
                dropChance = 0f,
                disableHolster = false
            });

            DriverWeaponCatalog.Sniper = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Sniper Rifle",
                nameToken = "ROB_DRIVER_SNIPER_NAME",
                description = "Precise, fatal shots.",
                descriptionToken = "ROB_DRIVER_SNIPER_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSniperRifleWeaponIcon"),
                tier = DriverWeaponTier.Uncommon,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.Damage,
                shotCount = 6,

                primarySkillDef = Skills.sniperPrimarySkillDef,
                secondarySkillDef = Skills.sniperSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("SNIPER"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("SNIPER"),

                mesh = Modules.Assets.LoadMesh("meshSniperRifle"),
                material = Modules.Assets.LoadMaterial("matSniperRifle"),
                crosshairPrefab = Modules.Assets.defaultCrosshairPrefab,
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "ReloadPistol",
                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_sniper",
                dropChance = 0f,
                disableHolster = false
            });

            DriverWeaponCatalog.Bazooka = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Bazooka",
                nameToken = "ROB_DRIVER_BAZOOKA_NAME",
                description = "Chargeable arcing rockets.",
                descriptionToken = "ROB_DRIVER_BAZOOKA_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBazookaWeaponIcon"),
                tier = DriverWeaponTier.Uncommon,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.Damage,
                shotCount = 8,

                primarySkillDef = Skills.bazookaPrimarySkillDef,
                secondarySkillDef = Skills.bashSkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("BAZOOKA"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("BAZOOKA"),

                mesh = Modules.Assets.LoadMesh("meshBazooka"),
                material = Modules.Assets.LoadMaterial("matBazooka"),
                crosshairPrefab = Modules.Assets.bazookaCrosshairPrefab,
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "ReloadShotgun",
                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_rocket_launcher",
                dropChance = 0f,
                disableHolster = false
            });

            DriverWeaponCatalog.GrenadeLauncher = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Grenade Launcher",
                nameToken = "ROB_DRIVER_GRENADELAUNCHER_NAME",
                description = "Fast-firing grenades with high damage but low blast radius.",
                descriptionToken = "ROB_DRIVER_GRENADELAUNCHER_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGrenadeLauncherWeaponIcon"),
                tier = DriverWeaponTier.Uncommon,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.Damage,
                shotCount = 16,

                primarySkillDef = Skills.grenadeLauncherPrimarySkillDef,
                secondarySkillDef = Skills.bashSkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("GRENADELAUNCHER"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("GRENADELAUNCHER"),

                mesh = Modules.Assets.LoadMesh("meshGrenadeLauncher"),
                material = Modules.Assets.LoadMaterial("matGrenadeLauncher"),
                crosshairPrefab = Modules.Assets.grenadeLauncherCrosshairPrefab,
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "ReloadShotgun",
                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_grenade_launcher",
                dropChance = 0f,
                disableHolster = false
            });

            DriverWeaponCatalog.RocketLauncher = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Rocket Launcher",
                nameToken = "ROB_DRIVER_ROCKETLAUNCHER_NAME",
                description = "KABOOOM",
                descriptionToken = "ROB_DRIVER_ROCKETLAUNCHER_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherWeaponIcon"),
                tier = DriverWeaponTier.Legendary,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.AttackSpeed,
                shotCount = 20,

                primarySkillDef = Skills.rocketLauncherPrimarySkillDef,
                secondarySkillDef = Skills.rocketLauncherSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("ROCKETLAUNCHER"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("ROCKETLAUNCHER"),

                mesh = Modules.Assets.LoadMesh("meshRocketLauncher"),
                material = Modules.Assets.LoadMaterial("matRocketLauncher"),
                crosshairPrefab = Modules.Assets.rocketLauncherCrosshairPrefab,
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "ReloadShotgun",
                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_rocket_launcher",
                dropChance = 0f,
                disableHolster = false
            });

            DriverWeaponCatalog.Behemoth = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Brilliant Behemoth",
                nameToken = "ROB_DRIVER_BEHEMOTH_NAME",
                description = "huh?",
                descriptionToken = "ROB_DRIVER_BEHEMOTH_DESC",

                icon = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Behemoth/texBehemothIcon.png").WaitForCompletion(),
                tier = DriverWeaponTier.Unique,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.AttackSpeed,
                shotCount = 20,

                primarySkillDef = Skills.behemothPrimarySkillDef,
                secondarySkillDef = Skills.behemothSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("BEHEMOTH"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("BEHEMOTH"),

                mesh = Modules.Assets.LoadMesh("meshBehemoth"),
                material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Behemoth/matBehemoth.mat").WaitForCompletion(),
                crosshairPrefab = Modules.Assets.rocketLauncherCrosshairPrefab,
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "ReloadShotgun",
                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_rocket_launcher",
                dropChance = 0f,
                disableHolster = false
            });

            DriverWeaponCatalog.PrototypeRocketLauncher = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Prototype Rocket Launcher",
                nameToken = "ROB_DRIVER_ROCKETLAUNCHER_ALT_NAME",
                description = "A faulty prototype that can only fire a few shots.",
                descriptionToken = "ROB_DRIVER_ROCKETLAUNCHER_ALT_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherAltWeaponIcon"),
                tier = DriverWeaponTier.Unique,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.AttackSpeed,
                shotCount = 10,

                primarySkillDef = Skills.rocketLauncherAltPrimarySkillDef,
                secondarySkillDef = Skills.rocketLauncherAltSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("ROCKETLAUNCHER_ALT"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("ROCKETLAUNCHER_ALT"),

                mesh = Modules.Assets.LoadMesh("meshRocketLauncher"),
                material = Modules.Assets.LoadMaterial("matRocketLauncherAlt"),
                crosshairPrefab = Modules.Assets.rocketLauncherCrosshairPrefab,
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "ReloadShotgun",
                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_rocket_launcher",
                dropChance = 0f,
                disableHolster = false
            });

            DriverWeaponCatalog.ArmCannon = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Arm Cannon",
                nameToken = "ROB_DRIVER_ARMCANNON_NAME",
                description = "Arm Cannon scavenged from a Steel Mechorilla.",
                descriptionToken = "ROB_DRIVER_ARMCANNON_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texArmCannonWeaponIcon"),
                tier = DriverWeaponTier.Unique,
                animationSet = DriverWeaponDef.AnimationSet.Default,
                buffType = DriverWeaponDef.BuffType.AttackSpeed,
                shotCount = 20,

                primarySkillDef = Skills.armCannonPrimarySkillDef,
                secondarySkillDef = Skills.bashSkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("ARMCANNON"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("ARMCANNON"),

                mesh = Modules.Assets.LoadMesh("meshArmCannon"),
                material = Modules.Assets.LoadMaterial("matArmCannon"),
                crosshairPrefab = Modules.Assets.rocketLauncherCrosshairPrefab,
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "Recharge",
                equipAnimationString = "Recharge",
                calloutSoundString = "sfx_driver_callout_rocket_launcher",
                dropChance = 100f,
                disableHolster = true
            });

            DriverWeaponCatalog.PlasmaCannon = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Super Plasma Cannon",
                nameToken = "ROB_DRIVER_PLASMACANNON_NAME",
                description = "POWERRR!!!",
                descriptionToken = "ROB_DRIVER_PLASMACANNON_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPlasmaCannonWeaponIcon"),
                tier = DriverWeaponTier.Void,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.AttackSpeed,
                shotCount = 30,

                primarySkillDef = Skills.plasmaCannonPrimarySkillDef,
                secondarySkillDef = Skills.plasmaCannonSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("PLASMACANNON"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("PLASMACANNON"),

                mesh = Modules.Assets.LoadMesh("meshPlasmaCannon"),
                material = Modules.Assets.LoadMaterial("matPlasmaCannon"),
                crosshairPrefab = Modules.Assets.rocketLauncherCrosshairPrefab,
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "ReloadShotgun",
                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_laser",
                dropChance = 0f,
                disableHolster = false
            });

            DriverWeaponCatalog.BadassShotgun = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Badass Shotgun",
                nameToken = "ROB_DRIVER_BADASS_SHOTGUN_NAME",
                description = "A six-barreled shotgun...!?",
                descriptionToken = "ROB_DRIVER_BADASS_SHOTGUN_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texBadassShotgunWeaponIcon"),
                tier = DriverWeaponTier.Legendary,
                animationSet = DriverWeaponDef.AnimationSet.Default,
                buffType = DriverWeaponDef.BuffType.Damage,
                shotCount = 12,

                primarySkillDef = Skills.badassShotgunPrimarySkillDef,
                secondarySkillDef = Skills.bashSkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("BADASS_SHOTGUN"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("BADASS_SHOTGUN"),

                mesh = Modules.Assets.LoadMesh("meshSixBarrelShotgun"),
                material = Modules.Assets.LoadMaterial("matSawedOff"),
                crosshairPrefab = Modules.Assets.LoadCrosshair("SMG"),
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "ReloadShotgun",
                equipAnimationString = "EquipPistol",
                calloutSoundString = "sfx_driver_callout_shotgun",
                dropChance = 0f,
                disableHolster = false
            });

            DriverWeaponCatalog.LunarRifle = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Chimeric Cannon",
                nameToken = "ROB_DRIVER_LUNARRIFLE_NAME",
                description = "Blasts of condensed lunar energy.",
                descriptionToken = "ROB_DRIVER_LUNARRIFLE_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texLunarRifleWeaponIcon"),
                tier = DriverWeaponTier.Lunar,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.AttackSpeed,
                shotCount = 48,

                primarySkillDef = Skills.lunarRiflePrimarySkillDef,
                secondarySkillDef = Skills.bashSkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("LUNARRIFLE"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("LUNARRIFLE"),

                mesh = Modules.Assets.LoadMesh("meshLunarRifle"),
                material = Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarGolem/matLunarGolem.mat").WaitForCompletion(),
                crosshairPrefab = Modules.Assets.rocketLauncherCrosshairPrefab,
                pickupPrefabOverride = Modules.Assets.lunarPickupModel,
                colorOveride = Helpers.lunarItemColor,

                reloadAnimationString = "ReloadShotgun",
                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_generic",
                dropChance = 10f,
                disableHolster = false
            });

            DriverWeaponCatalog.LunarHammer = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Lunar Hammer",
                nameToken = "ROB_DRIVER_LUNARHAMMER_NAME",
                description = "Wield supreme power in the palm of your hand.",
                descriptionToken = "ROB_DRIVER_LUNARHAMMER_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texLunarHammerWeaponIcon"),
                tier = DriverWeaponTier.NoTier,
                animationSet = DriverWeaponDef.AnimationSet.BigMelee,
                buffType = DriverWeaponDef.BuffType.Damage,
                shotCount = 0,

                primarySkillDef = Skills.lunarHammerPrimarySkillDef,
                secondarySkillDef = Skills.lunarHammerSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("LUNARHAMMER"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("LUNARHAMMER"),

                mesh = Modules.Assets.LoadMesh("meshLunarHammer"),
                material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Brother/matBrotherHammer.mat").WaitForCompletion(),
                crosshairPrefab = Modules.Assets.needlerCrosshairPrefab,
                pickupPrefabOverride = Modules.Assets.lunarPickupModel,
                colorOveride = Helpers.lunarItemColor,

                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_generic",
                dropChance = 100f,
                disableHolster = false
            });

            DriverWeaponCatalog.NemmandoGun = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Nemesis Commando SMG",
                nameToken = "ROB_DRIVER_NEMMANDO_NAME",
                description = "Nemesis Commando's gun.",
                descriptionToken = "ROB_DRIVER_NEMMANDO_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoWeaponIcon"),
                tier = DriverWeaponTier.Void,
                animationSet = DriverWeaponDef.AnimationSet.Default,
                buffType = DriverWeaponDef.BuffType.Crit,
                shotCount = 64,

                primarySkillDef = Skills.nemmandoGunPrimarySkillDef,
                secondarySkillDef = Skills.nemmandoGunSecondarySkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("NEMMANDO"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("NEMMANDO"),

                mesh = Modules.Assets.LoadMesh("meshNemmandoGun"),
                material = Modules.Assets.LoadMaterial("matNemmandoGun"),
                crosshairPrefab = Modules.Assets.defaultCrosshairPrefab,
                pickupPrefabOverride = Modules.Assets.voidPickupModel,
                colorOveride = Helpers.voidItemColor,

                reloadAnimationString = "ReloadPistol",
                equipAnimationString = "EquipPistol",
                calloutSoundString = "sfx_driver_callout_generic",
                dropChance = 100f,
                disableHolster = false
            });

            DriverWeaponCatalog.NemmercGun = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Nemesis Mercenary Shotgun",
                nameToken = "ROB_DRIVER_NEMMERC_NAME",
                description = "Nemesis Mercenary's shotgun.",
                descriptionToken = "ROB_DRIVER_NEMMERC_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmercWeaponIcon"),
                tier = DriverWeaponTier.Void,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.Damage,
                shotCount = 48,

                primarySkillDef = Skills.nemmercGunPrimarySkillDef,
                secondarySkillDef = Skills.bashSkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("NEMMERC"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("NEMMERC"),

                mesh = Modules.Assets.LoadMesh("meshNemmercGun"),
                material = Modules.Assets.LoadMaterial("matNemmercGun"),
                crosshairPrefab = Modules.Assets.LoadCrosshair("SMG"),
                pickupPrefabOverride = Modules.Assets.voidPickupModel,
                colorOveride = Helpers.voidItemColor,

                reloadAnimationString = "ReloadShotgun",
                equipAnimationString = "EquipPistol",
                calloutSoundString = "sfx_driver_callout_shotgun",
                dropChance = 100f,
                disableHolster = false
            });

            DriverWeaponCatalog.GolemRifle = DriverWeaponCatalog.CreateAndAddWeapon(new DriverWeaponDefInfo
            {
                name = "Stone Cannon",
                nameToken = "ROB_DRIVER_GOLEMGUN_NAME",
                description = "Harness the intense beams of a Stone Golem.",
                descriptionToken = "ROB_DRIVER_GOLEMGUN_DESC",

                icon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGolemGunWeaponIcon"),
                tier = DriverWeaponTier.Unique,
                animationSet = DriverWeaponDef.AnimationSet.TwoHanded,
                buffType = DriverWeaponDef.BuffType.Damage,
                shotCount = 48,

                primarySkillDef = Skills.golemGunPrimarySkillDef,
                secondarySkillDef = Skills.bashSkillDef,
                arsenalSkillDef = Skills.CreateAndAddWeaponSkillDef("GOLEMGUN"),
                unlockableDef = Unlockables.CreateAndAddWeaponUnlockableDef("GOLEMGUN"),

                mesh = Modules.Assets.LoadMesh("meshGolemGun"),
                material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Golem/matGolem.mat").WaitForCompletion(),
                crosshairPrefab = Modules.Assets.circleCrosshairPrefab,
                pickupPrefabOverride = null,
                colorOveride = null,

                reloadAnimationString = "ReloadPistol",
                equipAnimationString = "BufferEmpty",
                calloutSoundString = "sfx_driver_callout_laser",
                dropChance = 1f,
                disableHolster = false
            });
            #endregion

            #region More Weapons
            new Modules.Weapons.ArmBFG().Init();
            new Modules.Weapons.CrabGun().Init();
            new Modules.Weapons.LunarGrenade().Init();
            new Modules.Weapons.ScavGun().Init();
            new Modules.Weapons.ArtiGauntlet().Init();
            new Modules.Weapons.BanditRevolver().Init();
            new Modules.Weapons.CommandoSMG().Init();
            new Modules.Weapons.Revolver().Init();
            new Modules.Weapons.SMG().Init();
            new Modules.Weapons.RavSword().Init();
            new Modules.Weapons.NemKatana().Init();

            new Modules.Weapons.VulkanShotgun();
            #endregion

            #region Drops
            DriverWeaponCatalog.AddWeaponDrop("Beetle", DriverWeaponCatalog.BeetleShield, 0.1f);
            DriverWeaponCatalog.AddWeaponDrop("Golem", DriverWeaponCatalog.GolemRifle, 1f);
            DriverWeaponCatalog.AddWeaponDrop("Titan", DriverWeaponCatalog.GolemRifle, 10f);
            DriverWeaponCatalog.AddWeaponDrop("LunarGolem", DriverWeaponCatalog.LunarRifle, 10f);
            DriverWeaponCatalog.AddWeaponDrop("TitanGold", DriverWeaponCatalog.GoldenGun, 100f);
            DriverWeaponCatalog.AddWeaponDrop("TimeCrystal", DriverWeaponCatalog.LunarRifle, 50f);
            DriverWeaponCatalog.AddWeaponDrop("BrotherHurt", DriverWeaponCatalog.LunarHammer, 100f);
            DriverWeaponCatalog.AddWeaponDrop("BrotherHurtBodyP3", DriverWeaponCatalog.LunarHammer, 100f, false);

            DriverWeaponCatalog.AddWeaponDrop("Mechorilla", DriverWeaponCatalog.ArmCannon, 100f);
            DriverWeaponCatalog.AddWeaponDrop("SS2UNemmando", DriverWeaponCatalog.NemmandoGun, 100f);
            DriverWeaponCatalog.AddWeaponDrop("NemMerc", DriverWeaponCatalog.NemmercGun, 100f);
            DriverWeaponCatalog.AddWeaponDrop("Heretic", DriverWeaponCatalog.Needler, 100f);
            #endregion
        }

        public static DriverWeaponDef CreateAndAddWeapon(DriverWeaponDefInfo weaponDefInfo)
        {
            if (!string.IsNullOrEmpty(weaponDefInfo.nameToken) && !string.IsNullOrEmpty(weaponDefInfo.name))
                R2API.LanguageAPI.Add(weaponDefInfo.nameToken, weaponDefInfo.name);

            if (!string.IsNullOrEmpty(weaponDefInfo.descriptionToken) && !string.IsNullOrEmpty(weaponDefInfo.description))
                R2API.LanguageAPI.Add(weaponDefInfo.descriptionToken, weaponDefInfo.description);

            return CreateAndAddWeapon(DriverWeaponDef.CreateWeaponDefFromInfo(weaponDefInfo));
        }

        public static DriverWeaponDef CreateAndAddWeapon(DriverWeaponDef weaponDef)
        {
            Array.Resize(ref weaponDefs, weaponDefs.Length + 1);

            int index = weaponDefs.Length - 1;
            weaponDef.index = (ushort)index;
            weaponDefs[index] = weaponDef;

            Config.InitWeaponConfig(weaponDef);

            weaponDef.crosshairPrefab ??= Modules.Assets.defaultCrosshairPrefab;
            if (weaponDef.icon == null)
            {
                Log.Warning($"Weapon {weaponDef.weaponName} is missing an icon! Assigning default based on tier.");
                weaponDef.icon = weaponDef.tier switch
                {
                    DriverWeaponTier.Common => Modules.Assets.commonWeaponIcon,
                    DriverWeaponTier.Uncommon => Modules.Assets.uncommonWeaponIcon,
                    DriverWeaponTier.Legendary => Modules.Assets.legendaryWeaponIcon,
                    DriverWeaponTier.Unique => Modules.Assets.uniqueWeaponIcon,
                    DriverWeaponTier.Void => Modules.Assets.voidWeaponIcon,
                    DriverWeaponTier.Lunar => Modules.Assets.lunarWeaponIcon,
                    _ => Modules.Assets.commonWeaponIcon
                };
            }

            if (Config.enableArsenal.Value && weaponDef.enabled)
            {
                weaponDef.arsenalSkillDef ??= Skills.CreateAndAddWeaponSkillDef(weaponDef.nameToken, weaponDef.descriptionToken);
                Skills.AddWeaponSkillToFamily(weaponDef.arsenalSkillDef, weaponDef.unlockableDef, weaponDef.icon);
            }

            Log.Debug("Added " + weaponDef.nameToken + " to catalog with rarity: " + Enum.GetName(typeof(DriverWeaponTier), weaponDef.tier));

            return weaponDef;
        }

        public static void AddWeaponDrop(string bodyName, DriverWeaponDef weaponDef, float dropChance = 0f, bool autoComplete = true)
        {
            if (string.IsNullOrWhiteSpace(bodyName)) 
                return;

            if (autoComplete)
            {
                bodyName = bodyName.Replace("(Clone)", "");

                if (!bodyName.EndsWith("Body"))
                    bodyName += "Body";
            }

            BodyCatalog.availability.CallWhenAvailable(() =>
            {
                var bodyIndex = BodyCatalog.FindBodyIndex(bodyName);

                if (bodyIndex != BodyIndex.None)
                {
                    weaponDrops[bodyIndex] = new WeaponDrop(weaponDef.index, Mathf.Max(dropChance, weaponDef.dropChance));
                    Log.Debug("Added " + weaponDef.nameToken + " to drop list for " + bodyName);
                }
                else
                {
                    Log.Warning("Failed to add " + weaponDef.nameToken + " to drop list for " + bodyName + " because the body index was not found.");
                }
            });
        }

        public static DriverWeaponDef GetWeaponFromIndex(int index) => HG.ArrayUtils.GetSafe(weaponDefs, index, Pistol);

        // These are all the pistol options that are forced upgrades with steadyaim
        public static bool IsWeaponPistol(DriverWeaponDef weaponDef)
        {
            return weaponDef == Pistol ||
                   weaponDef == VoidPistol ||
                   weaponDef == LunarPistol ||
                   weaponDef == PyriteGun ||
                   weaponDef == BeetleShield ||
                   weaponDef == Modules.Weapons.FalsePistol.instance.weaponDef;
        }

        public static DriverWeaponDef GetRandomWeapon()
        {
            List<DriverWeaponDef> validWeapons = [];

            for (int i = 0; i < weaponDefs.Length; i++)
            {
                var weaponDef = weaponDefs[i];
                if (weaponDef.enabled && weaponDef.shotCount > 0 && weaponDef.tier != DriverWeaponTier.NoTier)
                    validWeapons.Add(weaponDef);
            }

            if (validWeapons.Count == 0)
                return Pistol; // pistol failsafe

            return validWeapons[UnityEngine.Random.Range(0, validWeapons.Count)];
        }

        public static DriverWeaponDef GetWeightedRandomWeapon(DriverWeaponTier tier)
        {
            int commonWeight = 60;
            int uncommonWeight = tier >= DriverWeaponTier.Uncommon ? 35 : 0;
            int legendaryWeight = tier >= DriverWeaponTier.Legendary ? 2 : 0;
            int uniqueWeight = tier >= DriverWeaponTier.Unique ? 1 : 0;
            int rnd = UnityEngine.Random.Range(0, commonWeight + uncommonWeight + legendaryWeight + uniqueWeight);

            if (rnd < commonWeight) tier = DriverWeaponTier.Common;
            else if (rnd < commonWeight + uncommonWeight) tier = DriverWeaponTier.Uncommon;
            else if (rnd < commonWeight + uncommonWeight + legendaryWeight) tier = DriverWeaponTier.Legendary;

            return GetRandomWeaponFromTier(tier);
        }


        public static DriverWeaponDef GetRandomWeaponFromTier(DriverWeaponTier tier)
        {
            List<DriverWeaponDef> validWeapons = [];

            for (int i = 0; i < weaponDefs.Length; i++)
            {
                var weaponDef = weaponDefs[i];
                if (weaponDef.enabled && weaponDef.shotCount > 0 && weaponDef.tier != DriverWeaponTier.NoTier)
                {
                    if (weaponDef.tier == tier)
                        validWeapons.Add(weaponDef);
                    
                    if (Config.uniqueDropsAreLegendary.Value && tier == DriverWeaponTier.Legendary)
                    {
                        if (weaponDef.tier > DriverWeaponTier.Legendary)
                            validWeapons.Add(weaponDef);
                    }
                }
            }

            if (validWeapons.Count == 0)
                return Pistol; // pistol failsafe if you disabled rocket launcher like a fucking retard or something

            return validWeapons[UnityEngine.Random.Range(0, validWeapons.Count)];
        }
    }
}