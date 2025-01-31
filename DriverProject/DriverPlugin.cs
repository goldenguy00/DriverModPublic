using BepInEx;
using R2API.Utils;
using RoR2;
using System.Security;
using System.Security.Permissions;
using RobDriver.Modules.Survivors;
using System.Runtime.CompilerServices;

[module: UnverifiableCode]
#pragma warning disable CS0618 // Type or member is obsolete
[assembly: SecurityPermission(SecurityAction.RequestMinimum, SkipVerification = true)]
#pragma warning restore CS0618 // Type or member is obsolete

namespace RobDriver
{
    [BepInDependency("com.DestroyedClone.AncientScepter", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TeamMoonstorm.Starstorm2", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rune580.riskofoptions", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.ContactLight.LostInTransit", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.TeamMoonstorm.Starstorm2", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.RiskySleeps.ClassicItemsReturns", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.Borbo.GreenAlienHead", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("bubbet.riskui", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("com.rob.Ravager", BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    [BepInPlugin(MODUID, MODNAME, MODVERSION)]
    /*
     todo:
    rocket smoke puff persists
    text layering bug ask rob
    skateboard with afterburner
     
     */

    public class DriverPlugin : BaseUnityPlugin
    {
        public const string MODUID = "com.rob.Driver";
        public const string MODNAME = "Driver";
        public const string MODVERSION = "1.9.0";

        public const string developerPrefix = "ROB";

        public static DriverPlugin instance;

        public static bool StarstormInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.TeamMoonstorm.Starstorm2");
        public static bool ScepterInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.DestroyedClone.AncientScepter");
        public static bool RooInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions");
        public static bool LitInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.ContactLight.LostInTransit");
        public static bool ClassicItemsInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.RiskySleeps.ClassicItemsReturns");
        public static bool RiskUIInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("bubbet.riskui");
        public static bool ExtendedLoadoutInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.KingEnderBrine.ExtendedLoadout");
        public static bool GreenAlienHeadInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.Borbo.GreenAlienHead");
        public static bool RavagerInstalled => BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("com.rob.Ravager");

        private void Awake()
        {
            instance = this;

            Log.Init(Logger);

            Modules.Config.ReadConfig(Config);
            Modules.Assets.PopulateAssets();
            Modules.CameraParams.InitializeParams();
            Modules.States.RegisterStates();
            Modules.Buffs.RegisterBuffs();
            DriverDamageTypes.Init();
            DriverBulletCatalog.Init();
            Modules.Projectiles.RegisterProjectiles();
            Modules.Tokens.AddTokens();
            Modules.ItemDisplays.PopulateDisplays();
            Modules.NetMessages.RegisterNetworkMessages();
            Modules.Unlockables.Init();

            new Driver().CreateCharacter();

            new Modules.ContentPacks().Initialize();

            CreateWeapons();

            // uncomment this if network testing
            // just download nuxlar's MultiplayerModTesting ffs, youre just gonna forget to comment it out again
            //On.RoR2.Networking.NetworkManagerSystemSteam.OnClientConnect += (s, u, t) => { };
        }

        private void CreateWeapons()
        {
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
            new Modules.Weapons.NemSword().Init();
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
            if (DriverPlugin.StarstormInstalled) return _CheckIfBodyIsTerminal(body);
            return false;
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static bool _CheckIfBodyIsTerminal(CharacterBody body)
        {
            return body.HasBuff(Moonstorm.Starstorm2.SS2Content.Buffs.BuffTerminationReady);
        }
    }
}