using BepInEx.Configuration;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;

namespace RobDriver.Modules
{
    internal static class Config
    {
        public static ConfigFile myConfig;

        public static ConfigEntry<bool> enableArsenal;
        public static ConfigEntry<bool> adaptiveFocus;
        public static ConfigEntry<bool> autoFocus;
        public static ConfigEntry<bool> sharedPickupVisuals;
        public static ConfigEntry<bool> oldPickupModel;
        public static ConfigEntry<float> baseDropRate;
        public static ConfigEntry<float> godslingDropRateSplit;
        public static ConfigEntry<bool> enableGodslingInMultiplayer;
        public static ConfigEntry<bool> weaponCallouts;
        public static ConfigEntry<bool> backupMagExtendDuration;
        public static ConfigEntry<bool> classicDodgeSound;
        public static ConfigEntry<bool> enablePistolUpgrade;
        public static ConfigEntry<bool> enablePickupNotifications;
        public static ConfigEntry<bool> predatoryOnHead;
        public static ConfigEntry<bool> enableCrosshairDot;
        public static ConfigEntry<bool> dynamicCrosshair;
        public static ConfigEntry<bool> dynamicCrosshairUniversal;
        public static ConfigEntry<bool> defaultPistolAnims;
        public static ConfigEntry<bool> enabledRedVfxForKnife;
        public static ConfigEntry<bool> enableRevengence;
        public static ConfigEntry<bool> randomSupplyDrop;
        public static ConfigEntry<bool> oldCritShot;
        public static ConfigEntry<bool> enableRecoil;
        public static ConfigEntry<bool> uniqueDropsAreLegendary; 
        public static ConfigEntry<bool> badass;
        public static ConfigEntry<bool> cursed;

        public static ConfigEntry<bool> enableMagneticPickups;
        public static ConfigEntry<bool> enableMagenticConditionalPickups;
        public static ConfigEntry<float> pickupRadius;

        public static ConfigEntry<float> baseHealth;
        public static ConfigEntry<float> healthGrowth;
        public static ConfigEntry<float> baseDamage;
        public static ConfigEntry<float> damageGrowth;
        public static ConfigEntry<float> baseArmor;
        public static ConfigEntry<float> armorGrowth;
        public static ConfigEntry<float> baseMovementSpeed;
        public static ConfigEntry<float> baseCrit;
        public static ConfigEntry<float> baseRegen;

        public static ConfigEntry<KeyboardShortcut> restKey;
        public static ConfigEntry<KeyboardShortcut> tauntKey;
        public static ConfigEntry<KeyboardShortcut> danceKey;

        public static List<WeaponConfigBinding> weaponConfigBinding = new List<WeaponConfigBinding>();

        public struct WeaponConfigBinding
        {
            public string identifier;
            public ConfigEntry<bool> enabled;
            public ConfigEntry<int> shotCount;
        }

        internal static void ReadConfig(ConfigFile config)
        {
            myConfig = config;

            #region General
            badass = Config.BindAndOptions("01 - General", "Badass Mode", false, "Makes the mod BadAss.", true);

            cursed = Config.BindAndOptions("01 - General", "Cursed", false, "Enables unfinished, stupid and old content.", true);

            enablePickupNotifications = Config.BindAndOptions("01 - General", "Enable Weapon Pickup Notifications", true, "If set to false, will disable the notifications from picking up weapons. (Client-side)");
            
            weaponCallouts = Config.BindAndOptions("01 - General", "Weapon Pickup Callouts", false, "If set to true, Driver will call out the weapons he picks up. (Client-side)");

            enableGodslingInMultiplayer = Config.BindAndOptions("01 - General", "Godsling Enabled In Multiplayer", true, "If set to true, the Godsling passive will be enabled for multiplayer.", true);

            #endregion

            #region Gameplay
            enableArsenal = Config.BindAndOptions("02 - Gameplay", "Enable Arsenal Passive", true, "If set to false, Driver will no longer be able to choose a default weapon and will only use the Pistol.", true);

            adaptiveFocus = Config.BindAndOptions("02 - Gameplay", "Adaptive Focus", true, "If set to true, Focus will always charge up before firing a shot once your attack speed reaches a certain amount. (Client-side)");
            
            autoFocus = Config.BindAndOptions("02 - Gameplay", "Focus Auto Charge", false, "If set to true, Focus will always charge up before firing a shot. Take control of your runs with the illusion of skill! (Client-side)");
            
            baseDropRate = Config.BindAndOptionsSlider("02 - Gameplay", "Base Drop Rate", 7f, "Base chance for weapons and ammo to drop on kill", 0f, 100f);

            godslingDropRateSplit = Config.BindAndOptionsSlider("02 - Gameplay", "Godsling Drop Rate Split", 50f, "Controls whether ammo or guns drop while using the Godsling passive, higher number means higher chance for ammo.", 0f, 100f);
            
            backupMagExtendDuration = Config.BindAndOptions("02 - Gameplay", "Backup Magazine Ammo Extension", false, "If set to true, Backup Magazines will increase the Ammo of weapon pickups by 0.5s.");
            
            enablePistolUpgrade = Config.BindAndOptions("02 - Gameplay", "Enable Pistol Upgrade", true, "If set to false, will stop Pistol from upgrading itself for run-ending boss fights.");
            
            randomSupplyDrop = Config.BindAndOptions("02 - Gameplay", "Random Supply Drop", false, "If set to true, Supply Drop will drop a random weapon from ANY tier. Completely unbalanced but fun! Use at your own risk.");

            oldCritShot = Config.BindAndOptions("02 - Gameplay", "Old Critical Shot", false, "If set to true, will use the old critical animation which spins the gun BEFORE shooting.");

            enableRecoil = Config.BindAndOptions("02 - Gameplay", "Enable Recoil", true, "Set to false to disable recoil from shooting guns.");

            uniqueDropsAreLegendary = Config.BindAndOptions("02 - Gameplay", "Unique Drops Are Legendary", false, "Unique weapon drops from survivors are now in the legendary pool.");
            #endregion

            #region Pickups

            enableMagneticPickups = Config.BindAndOptions("03 - Pickups", "Enable Magnetic Pickups", true, "Makes weapon and ammo drops move towards the player when they get close.", true);

            enableMagenticConditionalPickups = Config.BindAndOptions("03 - Pickups", "Only Magnetize Without Pickup Equipped", true, "Only magnetizes weapon and ammo drops when the player runs out of ammo.", true);

            pickupRadius = Config.BindAndOptionsSlider("03 - Pickups", "PickupRadius", 10f, "How close a pickup must be before it will begin to move towards a player.", 0f, 15f);

            #endregion

            #region Visuals and Effects
            sharedPickupVisuals = Config.BindAndOptions("04 - Visuals", "Shared Pickup Visuals", true, "If set to false, weapon pickups will only be visible while playing Driver. Setting this to true lets every character see them. (Client-side)");

            oldPickupModel = Config.BindAndOptions("04 - Visuals", "Old Weapon Pickup Model", false, "If set to true, uses the old goofy crate pickups instead of briefcases. (Client-side)");
            
            classicDodgeSound = Config.BindAndOptions("04 - Visuals", "Classic Dodge Sound", false, "If set to true, will use the old Combat Slide SFX. (Client-side)");
            
            predatoryOnHead = Config.BindAndOptions("04 - Visuals", "Predatory Instincts On Head", false, "If set to true, the item display for Predatory Instincts will be moved to the head like other survivors. (Client-side)", true);

            enableCrosshairDot = Config.BindAndOptions("04 - Visuals", "Enable Crosshair Dot", false, "If set to false, the dot in the center of the default crosshair will be hidden. (Client-side)", true);

            dynamicCrosshair = Config.BindAndOptions("04 - Visuals", "Dynamic Crosshair", true, "If set to false, will no longer highlight the crosshair when hovering over entities. (Client-side)", true);

            dynamicCrosshairUniversal = Config.BindAndOptions("04 - Visuals", "Dynamic Crosshair (Universal)", false, "If set to true, highlight the crosshair while hovering over entities, but for ALL characters. Overrides the other option. (Client-side)", true);

            defaultPistolAnims = Config.BindAndOptions("04 - Visuals", "Default Pistol Animations", true, "If set to true, return pistol passive scope animations back to the default.");
            
            enabledRedVfxForKnife = Config.BindAndOptions("04 - Visuals", "Red Slashing VFX", false, "If set to true, knife has red VFX.");

            enableRevengence = Config.BindAndOptions("04 - Visuals", "Revengence", false, "Some weapons are turned into Murasama.", true);
            #endregion

            #region Emotes
            restKey = Config.BindAndOptions("05 - Keybinds", "Rest Emote", new KeyboardShortcut(KeyCode.Alpha1), "Key used to Rest");
            tauntKey = Config.BindAndOptions("05 - Keybinds", "Salute Emote", new KeyboardShortcut(KeyCode.Alpha2), "Key used to Taunt");

            danceKey = Config.BindAndOptions("05 - Keybinds", "Dance Emote", new KeyboardShortcut(KeyCode.Alpha3), "Key used to Dance");
            #endregion

            #region Stats
            baseHealth = Config.BindAndOptionsSlider("06 - Character Stats", "Base Health", 110f, "", 1f, 500f, true);
            healthGrowth = Config.BindAndOptionsSlider("06 - Character Stats", "Health Growth", 33f, "", 0f, 100f, true);
            baseRegen = Config.BindAndOptionsSlider("06 - Character Stats", "Base Health Regen", 1.5f, "", 0f, 5f, true);
            baseArmor = Config.BindAndOptionsSlider("06 - Character Stats", "Base Armor", 0f, "", 0f, 20f, true);
            armorGrowth = Config.BindAndOptionsSlider("06 - Character Stats", "Armor Growth", 0f, "", 0f, 2f, true);
            baseDamage = Config.BindAndOptionsSlider("06 - Character Stats", "Base Damage", 12f, "", 1f, 24f, true);
            damageGrowth = Config.BindAndOptionsSlider("06 - Character Stats", "Damage Growth", 2.4f, "", 0f, 5f, true);
            baseMovementSpeed = Config.BindAndOptionsSlider("06 - Character Stats", "Base Movement Speed", 7f, "", 0f, 14f, true);
            baseCrit = Config.BindAndOptionsSlider("06 - Character Stats", "Base Crit", 1f, "", 0f, 100f, true);
            #endregion
        }

        public static void InitWeaponConfig(DriverWeaponDef weaponDef)
        {
            if (!weaponDef) return;
            if (string.IsNullOrWhiteSpace(weaponDef.configIdentifier)) return;

            var x = Config.BindAndOptionsSlider("07 - Weapons", weaponDef.configIdentifier + " - Base Ammo", weaponDef.shotCount, "How many shots this weapon can fire without any bonus attack speed.", 0, 200);

            var y = Config.BindAndOptions("07 - Weapons", weaponDef.configIdentifier + " - Enabled", true, "Set to false to remove this weapon from the drop pool.");

            foreach (WeaponConfigBinding i in weaponConfigBinding)
            {
                if (i.identifier == weaponDef.configIdentifier) return;
            }

            weaponConfigBinding.Add(new WeaponConfigBinding
            {
                identifier = weaponDef.configIdentifier,
                enabled = y,
                shotCount = x
            });
        }

        public static bool GetWeaponConfig(DriverWeaponDef weaponDef)
        {
            foreach (WeaponConfigBinding i in weaponConfigBinding)
            {
                if (i.identifier == weaponDef.configIdentifier)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool GetWeaponConfigEnabled(DriverWeaponDef weaponDef)
        {
            foreach (WeaponConfigBinding i in weaponConfigBinding)
            {
                if (i.identifier == weaponDef.configIdentifier)
                {
                    return i.enabled.Value;
                }
            }

            return true;
        }

        public static int GetWeaponConfigShotCount(DriverWeaponDef weaponDef)
        {
            foreach (WeaponConfigBinding i in weaponConfigBinding)
            {
                if (i.identifier == weaponDef.configIdentifier)
                {
                    return i.shotCount.Value;
                }
            }

            return 0;
        }

        public static void InitROO(Sprite modSprite, string modDescription)
        {
            if (DriverPlugin.RooInstalled) _InitROO(modSprite, modDescription);
        }

        public static void _InitROO(Sprite modSprite, string modDescription)
        {
            ModSettingsManager.SetModIcon(modSprite);
            ModSettingsManager.SetModDescription(modDescription);
        }

        public static ConfigEntry<T> BindAndOptions<T>(string section, string name, T defaultValue, string description = "", bool restartRequired = false)
        {
            if (string.IsNullOrEmpty(description))
            {
                description = name;
            }

            if (restartRequired)
            {
                description += " (restart required)";
            }

            ConfigEntry<T> configEntry = myConfig.Bind(section, name, defaultValue, description);

            if (DriverPlugin.RooInstalled)
            {
                TryRegisterOption(configEntry, restartRequired);
            }

            return configEntry;
        }

        public static ConfigEntry<float> BindAndOptionsSlider(string section, string name, float defaultValue, string description = "", float min = 0, float max = 20, bool restartRequired = false)
        {
            if (string.IsNullOrEmpty(description))
            {
                description = name;
            }

            description += " (Default: " + defaultValue + ")";

            if (restartRequired)
            {
                description += " (restart required)";
            }

            ConfigEntry<float> configEntry = myConfig.Bind(section, name, defaultValue, description);

            if (DriverPlugin.RooInstalled)
            {
                TryRegisterOptionSlider(configEntry, min, max, restartRequired);
            }

            return configEntry;
        }

        public static ConfigEntry<int> BindAndOptionsSlider(string section, string name, int defaultValue, string description = "", int min = 0, int max = 20, bool restartRequired = false)
        {
            if (string.IsNullOrEmpty(description))
            {
                description = name;
            }

            description += " (Default: " + defaultValue + ")";

            if (restartRequired)
            {
                description += " (restart required)";
            }

            ConfigEntry<int> configEntry = myConfig.Bind(section, name, defaultValue, description);

            if (DriverPlugin.RooInstalled)
            {
                TryRegisterOptionSlider(configEntry, min, max, restartRequired);
            }

            return configEntry;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void TryRegisterOption<T>(ConfigEntry<T> entry, bool restartRequired)
        {
            if (entry is ConfigEntry<string> stringEntry)
            {
                ModSettingsManager.AddOption(new StringInputFieldOption(stringEntry, restartRequired));
            }
            if (entry is ConfigEntry<float>)
            {
                ModSettingsManager.AddOption(new SliderOption(entry as ConfigEntry<float>, new SliderConfig()
                {
                    min = 0,
                    max = 20,
                    FormatString = "{0:0.00}",
                    restartRequired = restartRequired
                }));
            }
            if (entry is ConfigEntry<int>)
            {
                ModSettingsManager.AddOption(new IntSliderOption(entry as ConfigEntry<int>, restartRequired));
            }
            if (entry is ConfigEntry<bool>)
            {
                ModSettingsManager.AddOption(new CheckBoxOption(entry as ConfigEntry<bool>, restartRequired));
            }
            if (entry is ConfigEntry<KeyboardShortcut>)
            {
                ModSettingsManager.AddOption(new KeyBindOption(entry as ConfigEntry<KeyboardShortcut>, restartRequired));
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void TryRegisterOptionSlider(ConfigEntry<int> entry, int min, int max, bool restartRequired)
        {
            ModSettingsManager.AddOption(new IntSliderOption(entry as ConfigEntry<int>, new IntSliderConfig()
            {
                min = min,
                max = max,
                formatString = "{0:0.00}",
                restartRequired = restartRequired
            }));
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void TryRegisterOptionSlider(ConfigEntry<float> entry, float min, float max, bool restartRequired)
        {
            ModSettingsManager.AddOption(new SliderOption(entry as ConfigEntry<float>, new SliderConfig()
            {
                min = min,
                max = max,
                formatString = "{0:0.00}",
                restartRequired = restartRequired
            }));
        }

        internal static ConfigEntry<bool> CharacterEnableConfig(string characterName)
        {
            return Config.BindAndOptions("01 - General", "Enabled", true, "Set to false to disable this character", true);
        }

        internal static ConfigEntry<bool> ForceUnlockConfig(string characterName)
        {
            return Config.BindAndOptions("01 - General", "Force Unlock", false, "Makes this character unlocked by default", true);
        }

        public static bool GetKeyPressed(ConfigEntry<KeyboardShortcut> entry)
        {
            foreach (var item in entry.Value.Modifiers)
            {
                if (!Input.GetKey(item))
                {
                    return false;
                }
            }
            return Input.GetKeyDown(entry.Value.MainKey);
        }
    }

    public class StageSpawnInfo
    {
        private string stageName;
        private int minStages;

        public StageSpawnInfo(string stageName, int minStages)
        {
            this.stageName = stageName;
            this.minStages = minStages;
        }

        public string GetStageName()
        {
            return stageName;
        }
        public int GetMinStages()
        {
            return minStages;
        }
    }
}