using BepInEx;
using R2API.Utils;
using RoR2;
using UnityEngine;
using RobDriver.Modules.Survivors;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Security;

#pragma warning disable CS0618 // Type or member is obsolete
[module: UnverifiableCode]
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
[assembly: HG.Reflection.SearchableAttribute.OptIn]
#pragma warning restore CS0618 // Type or member is obsolete

namespace RobDriver
{
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TeamMoonstorm.Starstorm2", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rob.Hunk", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(R2API.ContentManagement.R2APIContentManager.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(R2API.DamageAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(R2API.DotAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(R2API.LanguageAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(R2API.Networking.NetworkingAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(R2API.PrefabAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(R2API.RecalculateStatsAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(R2API.SkillsAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(R2API.Skins.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [BepInDependency(R2API.SoundAPI.PluginGUID, BepInDependency.DependencyFlags.HardDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    public class DriverPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.rob.Driver";
        public const string MODNAME = "Driver";
        public const string MODVERSION = "2.1.8";

        public const string developerPrefix = "ROB";

        public static DriverPlugin instance;

        public static bool StarstormInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm");
        public static bool ScepterInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
        public static bool RooInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
        public static bool LitInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ContactLight.LostInTransit");
        public static bool ClassicItemsInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.RiskySleeps.ClassicItemsReturns");
        public static bool RiskUIInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("bubbet.riskui");
        public static bool ExtendedLoadoutInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.KingEnderBrine.ExtendedLoadout");
        public static bool GreenAlienHeadInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Borbo.GreenAlienHead");
        public static bool RavagerInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.Ravager");
        public static bool HunkInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.Hunk");
        public static bool HunkHudInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.public_ParticleSystem.HunkHud");

        private void Awake()
        {
            instance = this;
            Log.Init(Logger);

            Modules.Config.ReadConfig(Config);
            Modules.Assets.PopulateAssets();
            Modules.CameraParams.InitializeParams();
            Modules.States.RegisterStates();
            Modules.Projectiles.RegisterProjectiles();
            Modules.Tokens.AddTokens();
            Modules.ItemDisplays.PopulateDisplays();
            Modules.NetMessages.RegisterNetworkMessages();
            Modules.Unlockables.Init();

            new Driver().CreateCharacter();

            new Modules.ContentPacks().Initialize();
        }

        public static float GetICBMDamageMult(CharacterBody body)
        {
            float mult = 1f;
            if (body && body.inventory)
            {
                int itemcount = body.inventory.GetItemCount(DLC1Content.Items.MoreMissile);
                int stack = itemcount - 1;
                if (stack > 0) mult += stack * 0.5f;
            }
            return mult;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool CheckIfBodyIsTerminal(CharacterBody body)
        {
            return body.HasBuff(SS2.SS2Content.Buffs.BuffTerminationVFX);
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool IsHunkHudGlobal()
        {
            return HunkMod.Modules.Config.globalCustomHUD.Value;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void AddHunkFlashbang(GameObject prefab)
        {
            prefab.AddComponent<Modules.Components.FlashbangNearby>();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static GameObject GetHunkExplosion()
        {
            return HunkMod.Modules.HunkAssets.explosionEffect;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static GameObject GetHunkExplosionSmall()
        {
            return HunkMod.Modules.HunkAssets.smallExplosionEffect;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool IsItemGoldenGun(ItemIndex itemIndex)
        {
            var goldenGun = LostInTransit.LITContent.Items.GoldenGun;

            return goldenGun ? goldenGun.itemIndex == itemIndex : false;
        }

        // electric boogaloo
        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool IsItemGoldenGun2(ItemIndex itemIndex)
        {
            var goldenGun = ClassicItemsReturns.Items.Uncommon.GoldenGun.Instance;

            return goldenGun?.ItemDef ? goldenGun.ItemDef.itemIndex == itemIndex : false;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool IsItemScepter(ItemIndex itemIndex)
        {
            var scepter = AncientScepter.AncientScepterItem.instance;

            return scepter?.ItemDef ? scepter.ItemDef.itemIndex == itemIndex : false;
        }
    }
}