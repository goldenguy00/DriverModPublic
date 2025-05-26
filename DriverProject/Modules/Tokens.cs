using System;
using R2API;
using RobDriver.Modules.Achievements;
using RobDriver.SkillStates.Driver;

namespace RobDriver.Modules
{
    internal static class Tokens
    {
        internal static void AddTokens()
        {
            string desc = "The Driver is literally me.<color=#CCD3E0>" + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Each weapon has its own unique strengths and weaknesses so be sure to pick the right tool for the job." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Focus greatly increases your damage output, but be careful not to get flanked while aiming." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Combat Slide while shooting to make sure your damage has no downtime." + Environment.NewLine + Environment.NewLine;
            desc = desc + "< ! > Flashbang can be used to make a clean getaway in a pinch." + Environment.NewLine + Environment.NewLine;

            string outro = "..and so he left, still the same as he was when he began.";
            string outroFailure = "..and so he vanished, never to become a real human being.";

            string lore = "Back against the wall and odds" + Environment.NewLine;
            lore += "With the strength of a will and a cause" + Environment.NewLine;
            lore += "Your pursuits are called outstanding" + Environment.NewLine;
            lore += "You’re emotionally complex" + Environment.NewLine + Environment.NewLine;
            lore += "Against the grain of dystopic claims" + Environment.NewLine;
            lore += "Not the thoughts your actions entertain" + Environment.NewLine;
            lore += "And you have proved to be" + Environment.NewLine + Environment.NewLine + Environment.NewLine;
            lore += "A real human being and a real hero" + Environment.NewLine + Environment.NewLine;
            lore += "\"So, what do you do?\"" + Environment.NewLine + Environment.NewLine;
            lore += "\"I drive.\"";


            string prefix = "ROB_DRIVER_BODY_";

            LanguageAPI.Add(prefix + "NAME", "Driver");
            LanguageAPI.Add(prefix + "DESCRIPTION", desc);
            LanguageAPI.Add(prefix + "SUBTITLE", "Driver? I hardly know 'er!");
            LanguageAPI.Add(prefix + "LORE", lore);
            LanguageAPI.Add(prefix + "OUTRO_FLAVOR", outro);
            LanguageAPI.Add(prefix + "OUTRO_FAILURE", outroFailure);

            #region Skins
            LanguageAPI.Add(prefix + "DEFAULT_SKIN_NAME", "Default");
            LanguageAPI.Add(prefix + "MONSOON_SKIN_NAME", "Jacket");
            LanguageAPI.Add(prefix + "TYPHOON_SKIN_NAME", "Slugger");
            LanguageAPI.Add(prefix + "SUIT_SKIN_NAME", "Hitman");
            LanguageAPI.Add(prefix + "SUIT2_SKIN_NAME", "Hitman EX");
            LanguageAPI.Add(prefix + "SPECIALFORCES_SKIN_NAME", "Special Forces");
            LanguageAPI.Add(prefix + "GUERRILLA_SKIN_NAME", "Guerrilla");
            LanguageAPI.Add(prefix + "GREEN_SKIN_NAME", "Green");
            LanguageAPI.Add(prefix + "MINECRAFT_SKIN_NAME", "Minecraft");
            #endregion

            #region Passive
            LanguageAPI.Add(prefix + "PASSIVE_NAME", "Survivalist");
            LanguageAPI.Add(prefix + "PASSIVE_DESCRIPTION", $"Enemies have a chance to drop a new <style=cIsUtility>weapon</style>. These give you <style=cIsDamage>powerful attacks</style> for a limited time!");

            LanguageAPI.Add(prefix + "PASSIVE2_NAME", "Marksman (Legacy)");
            LanguageAPI.Add(prefix + "PASSIVE2_DESCRIPTION", $"Your trusty <style=cIsHealth>pistol</style> is all you need.");

            LanguageAPI.Add(prefix + "PASSIVE3_NAME", "Leadfoot");
            LanguageAPI.Add(prefix + "PASSIVE3_DESCRIPTION", $"Enemies have a chance to drop <style=cIsHealth>bullets</style>. These give your <style=cIsHealth>weapon</style> <style=cIsDamage>powerful attacks</style> for a limited time!");

            LanguageAPI.Add(prefix + "PASSIVE4_NAME", "Godsling");
            LanguageAPI.Add(prefix + "PASSIVE4_DESCRIPTION", $"I <style=cIsHealth>drive</style>.");

            LanguageAPI.Add(prefix + "CONFIRM_NAME", "Confirm");
            LanguageAPI.Add(prefix + "CONFIRM_DESCRIPTION", "Proceed with the current skill.");

            LanguageAPI.Add(prefix + "CANCEL_NAME", "Cancel");
            LanguageAPI.Add(prefix + "CANCEL_DESCRIPTION", "Cancel the current skill.");

            LanguageAPI.Add(prefix + "RELOAD_NAME", "Reload");
            LanguageAPI.Add(prefix + "RELOAD_DESCRIPTION", $"Reload your gun.");
            #endregion

            #region Primary
            LanguageAPI.Add(prefix + "PRIMARY_PISTOL_NAME", "Shoot");
            LanguageAPI.Add(prefix + "PRIMARY_PISTOL_DESCRIPTION", $"Fire your pistol for <style=cIsDamage>{100f * SkillStates.Driver.Shoot._damageCoefficient}% damage</style>.\n<style=cIsDamage>Critical hits shoot twice.</style>");

            LanguageAPI.Add(prefix + "PRIMARY_PYRITE_PISTOL_NAME", "Shoot");
            LanguageAPI.Add(prefix + "PRIMARY_PYRITE_PISTOL_DESCRIPTION", $"Fire your pistol for <style=cIsDamage>{100f * SkillStates.Driver.PyriteGun.Shoot._damageCoefficient}% damage</style>.\n<style=cIsDamage>Critical hits shoot twice.</style>");

            LanguageAPI.Add(prefix + "PRIMARY_BEETLESHIELD_NAME", "Shoot");
            LanguageAPI.Add(prefix + "PRIMARY_BEETLESHIELD_DESCRIPTION", $"Fire your pistol for <style=cIsDamage>{100f * SkillStates.Driver.BeetleShield.Shoot._damageCoefficient}% damage</style>.\n<style=cIsDamage>Critical hits shoot twice.</style>");

            LanguageAPI.Add(prefix + "PRIMARY_LUNAR_PISTOL_NAME", "Shoot");
            LanguageAPI.Add(prefix + "PRIMARY_LUNAR_PISTOL_DESCRIPTION", $"Fire your pistol for <style=cIsDamage>{100f * SkillStates.Driver.LunarPistol.Shoot._damageCoefficient}% damage</style>.\n<style=cIsDamage>Critical hits shoot twice.</style>");

            LanguageAPI.Add(prefix + "PRIMARY_VOID_PISTOL_NAME", "Shoot");
            LanguageAPI.Add(prefix + "PRIMARY_VOID_PISTOL_DESCRIPTION", $"Fire your pistol for <style=cIsDamage>{100f * SkillStates.Driver.VoidPistol.Shoot._damageCoefficient}% damage</style>.\n<style=cIsDamage>Critical hits shoot twice.</style>");

            LanguageAPI.Add(prefix + "PRIMARY_FALSE_PISTOL_NAME", "Shoot");
            LanguageAPI.Add(prefix + "PRIMARY_FALSE_PISTOL_DESCRIPTION", $"Fire your pistol for <style=cIsDamage>{100f * SkillStates.Driver.FalsePistol.Shoot._damageCoefficient}% damage</style>.\n<style=cIsDamage>Critical hits shoot twice.</style>");

            LanguageAPI.Add(prefix + "PRIMARY_GOLDENGUN_NAME", "Shoot");
            LanguageAPI.Add(prefix + "PRIMARY_GOLDENGUN_DESCRIPTION", $"Fire your pistol for <style=cIsDamage>{100f * SkillStates.Driver.GoldenGun.Shoot._damageCoefficient}% damage</style>.\n<style=cIsDamage>Critical hits shoot twice.</style>");

            LanguageAPI.Add(prefix + "PRIMARY_REVOLVER_NAME", "Shoot");
            LanguageAPI.Add(prefix + "PRIMARY_REVOLVER_DESCRIPTION", $"Fire your pistol for <style=cIsDamage>{100f * SkillStates.Driver.Revolver.Shoot._damageCoefficient}% damage</style>.\n<style=cIsDamage>Critical hits shoot twice.</style>");

            LanguageAPI.Add(prefix + "PRIMARY_SHOTGUN_NAME", "Blast");
            LanguageAPI.Add(prefix + "PRIMARY_SHOTGUN_DESCRIPTION", $"Fire a short-range blast for <style=cIsDamage>{SkillStates.Driver.Shotgun.Shoot._bulletCount}x{100f * SkillStates.Driver.Shotgun.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_RIOT_SHOTGUN_NAME", "Blast");
            LanguageAPI.Add(prefix + "PRIMARY_RIOT_SHOTGUN_DESCRIPTION", $"Fire a short-range <style=cIsUtility>piercing</style> blast for <style=cIsDamage>{SkillStates.Driver.RiotShotgun.Shoot._bulletCount}x{100f * SkillStates.Driver.RiotShotgun.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_SLUG_SHOTGUN_NAME", "Blast");
            LanguageAPI.Add(prefix + "PRIMARY_SLUG_SHOTGUN_DESCRIPTION", $"Fire a short-range slug for <style=cIsDamage>{100f * SkillStates.Driver.SlugShotgun.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_BADASS_SHOTGUN_NAME", "Blast");
            LanguageAPI.Add(prefix + "PRIMARY_BADASS_SHOTGUN_DESCRIPTION", $"Fire a short-range blast for <style=cIsDamage>{SkillStates.Driver.BadassShotgun.Shoot._bulletCount}x{100f * SkillStates.Driver.BadassShotgun.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_MACHINEGUN_NAME", "Spray");
            LanguageAPI.Add(prefix + "PRIMARY_MACHINEGUN_DESCRIPTION", $"Fire a rapid spray of shots for <style=cIsDamage>{100f * SkillStates.Driver.MachineGun.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_SMG_NAME", "Spray");
            LanguageAPI.Add(prefix + "PRIMARY_SMG_DESCRIPTION", $"Fire a rapid spray of shots for <style=cIsDamage>{100f * SkillStates.Driver.SMG.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_HEAVY_MACHINEGUN_NAME", "Spray");
            LanguageAPI.Add(prefix + "PRIMARY_HEAVY_MACHINEGUN_DESCRIPTION", $"Fire a spray of <style=cIsUtility>armor piercing</style> shots for <style=cIsDamage>{100f * SkillStates.Driver.HeavyMachineGun.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_BAZOOKA_NAME", "Fire");
            LanguageAPI.Add(prefix + "PRIMARY_BAZOOKA_DESCRIPTION", $"Charge and fire a rocket for <style=cIsDamage>{100f * SkillStates.Driver.Bazooka.Fire.minDamageCoefficient}-{100f * SkillStates.Driver.Bazooka.Fire.maxDamageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_ROCKETLAUNCHER_NAME", "Fire");
            LanguageAPI.Add(prefix + "PRIMARY_ROCKETLAUNCHER_DESCRIPTION", $"Fire a rocket for <style=cIsDamage>{100f * SkillStates.Driver.RocketLauncher.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_ROCKETLAUNCHER_ALT_NAME", "Fire");
            LanguageAPI.Add(prefix + "PRIMARY_ROCKETLAUNCHER_ALT_DESCRIPTION", $"Fire a rocket for <style=cIsDamage>{100f * SkillStates.Driver.RocketLauncher.NerfedShoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_GRENADELAUNCHER_NAME", "Fire");
            LanguageAPI.Add(prefix + "PRIMARY_GRENADELAUNCHER_DESCRIPTION", $"Launch a grenade for <style=cIsDamage>{100f * SkillStates.Driver.GrenadeLauncher.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_LUNAR_GRENADELAUNCHER_NAME", "Fire");
            LanguageAPI.Add(prefix + "PRIMARY_LUNAR_GRENADELAUNCHER_DESCRIPTION", $"Launch a lunar grenade for <style=cIsDamage>{100f * SkillStates.Driver.LunarGrenade.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_PLASMACANNON_NAME", "Fire");
            LanguageAPI.Add(prefix + "PRIMARY_PLASMACANNON_DESCRIPTION", $"Fire a burst of plasma for <style=cIsDamage>{100f * SkillStates.Driver.PlasmaCannon.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_BFG_NAME", "Fire");
            LanguageAPI.Add(prefix + "PRIMARY_BFG_DESCRIPTION", $"Fire a burst of plasma for <style=cIsDamage>{100f * SkillStates.Driver.ArmBFG.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_SNIPER_NAME", "Shoot");
            LanguageAPI.Add(prefix + "PRIMARY_SNIPER_DESCRIPTION", $"Fire your rifle for <style=cIsDamage>{100f * SkillStates.Driver.SniperRifle.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_LUNARRIFLE_NAME", "Shoot");
            LanguageAPI.Add(prefix + "PRIMARY_LUNARRIFLE_DESCRIPTION", $"Fire a blast for <style=cIsDamage>{100f * SkillStates.Driver.LunarRifle.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_VOIDRIFLE_NAME", "Shoot");
            LanguageAPI.Add(prefix + "PRIMARY_VOIDRIFLE_DESCRIPTION", $"Fire a blast for <style=cIsDamage>{100f * SkillStates.Driver.VoidRifle.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_LUNARHAMMER_NAME", "Crush");
            LanguageAPI.Add(prefix + "PRIMARY_LUNARHAMMER_DESCRIPTION", $"Swing your hammer for <style=cIsDamage>{100f * SkillStates.Driver.LunarHammer.SwingCombo._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_NEMMANDO_NAME", "Shoot");
            LanguageAPI.Add(prefix + "PRIMARY_NEMMANDO_DESCRIPTION", $"Fire your pistol for <style=cIsDamage>{100f * SkillStates.Driver.NemmandoGun.Shoot._damageCoefficient}% damage</style>.\n<style=cIsDamage>Critical hits shoot twice.</style>");

            LanguageAPI.Add(prefix + "PRIMARY_NEMMERC_NAME", "Splatter");
            LanguageAPI.Add(prefix + "PRIMARY_NEMMERC_DESCRIPTION", $"Fire a short-range blast for <style=cIsDamage>{SkillStates.Driver.NemmercGun.Shoot2._bulletCount}x{100f * SkillStates.Driver.NemmercGun.Shoot2._damageCoefficient}% damage</style>, and again when released.");

            LanguageAPI.Add(prefix + "PRIMARY_NEMMANDO_SWORD_NAME", "Blade of Cessation");
            LanguageAPI.Add(prefix + "PRIMARY_NEMMANDO_SWORD_DESCRIPTION", $"<style=cIsDamage>Gouging</style>. <style=cIsUtility>Agile</style>. Slice enemies in front for <style=cIsDamage>{100f * SkillStates.Driver.NemmandoSword.SwingSword._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_RAV_SLASHCOMBO_NAME", "Dismantle");
            LanguageAPI.Add(prefix + "PRIMARY_RAV_SLASHCOMBO_DESCRIPTION", $"Swing forward for <style=cIsDamage>{100f * SkillStates.Driver.RavSword.SlashCombo._damageCoefficient}% damage</style>. Every 3rd hit <style=cIsUtility>stuns</style> and deals <style=cIsDamage>{100f * SkillStates.Driver.RavSword.SlashCombo.finisherDamageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_ARMCANNON_NAME", "Fire");
            LanguageAPI.Add(prefix + "PRIMARY_ARMCANNON_DESCRIPTION", $"Fire a burst of plasma for <style=cIsDamage>{100f * SkillStates.Driver.ArmCannon.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_GOLEMGUN_NAME", "Shoot");
            LanguageAPI.Add(prefix + "PRIMARY_GOLEMGUN_DESCRIPTION", $"Fire a blast for <style=cIsDamage>{100f * SkillStates.Driver.VoidRifle.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "PRIMARY_ARTI_GAUNTLET_NAME", "Shoot");
            LanguageAPI.Add(prefix + "PRIMARY_ARTI_GAUNTLET_DESCRIPTION", $"Fire a blast for <style=cIsDamage>{100f * SkillStates.Driver.VoidRifle.Shoot._damageCoefficient}% damage</style>.");
            #endregion

            #region Secondary
            LanguageAPI.Add(prefix + "SECONDARY_PISTOL_NAME", "Focus");
            LanguageAPI.Add(prefix + "SECONDARY_PISTOL_DESCRIPTION", $"Take aim and charge a shot for up to <style=cIsDamage>{100f * SkillStates.Driver.SteadyAim._damageCoefficient}% damage</style>. <style=cIsUtility>Boosts rate of fire and accuracy.</style>");

            LanguageAPI.Add(prefix + "SECONDARY_PYRITE_PISTOL_NAME", "Focus");
            LanguageAPI.Add(prefix + "SECONDARY_PYRITE_PISTOL_DESCRIPTION", $"Take aim and charge a shot for up to <style=cIsDamage>{100f * SkillStates.Driver.PyriteGun.SteadyAim._damageCoefficient}% damage</style>. <style=cIsUtility>Boosts rate of fire and accuracy.</style>");

            LanguageAPI.Add(prefix + "SECONDARY_LUNAR_PISTOL_NAME", "Focus");
            LanguageAPI.Add(prefix + "SECONDARY_LUNAR_PISTOL_DESCRIPTION", $"Take aim and charge a shot for up to <style=cIsDamage>{100f * SkillStates.Driver.LunarPistol.SteadyAim._damageCoefficient}% damage</style>. <style=cIsUtility>Boosts rate of fire and accuracy.</style>");

            LanguageAPI.Add(prefix + "SECONDARY_VOID_PISTOL_NAME", "Focus");
            LanguageAPI.Add(prefix + "SECONDARY_VOID_PISTOL_DESCRIPTION", $"Take aim and charge a shot for up to <style=cIsDamage>{100f * SkillStates.Driver.VoidPistol.SteadyAim._damageCoefficient}% damage</style>. <style=cIsUtility>Boosts rate of fire and accuracy.</style>");

            LanguageAPI.Add(prefix + "SECONDARY_FALSE_PISTOL_NAME", "Focus");
            LanguageAPI.Add(prefix + "SECONDARY_FALSE_PISTOL_DESCRIPTION", $"Take aim and charge a shot for up to <style=cIsDamage>{100f * SkillStates.Driver.FalsePistol.SteadyAim._damageCoefficient}% damage</style>. <style=cIsUtility>Boosts rate of fire and accuracy.</style>");

            LanguageAPI.Add(prefix + "SECONDARY_BEETLESHIELD_NAME", "Focus");
            LanguageAPI.Add(prefix + "SECONDARY_BEETLESHIELD_DESCRIPTION", $"Take aim and charge a shot for up to <style=cIsDamage>{100f * SkillStates.Driver.BeetleShield.SteadyAim._damageCoefficient}% damage</style>. <style=cIsUtility>Boosts rate of fire, accuracy, and armor.</style>");

            LanguageAPI.Add(prefix + "SECONDARY_BASH_NAME", "Bash");
            LanguageAPI.Add(prefix + "SECONDARY_BASH_DESCRIPTION", $"<style=cIsDamage>Stun</style> and <style=cIsUtility>knock back</style> nearby enemies for <style=cIsDamage>{100f * Bash.damageCoefficient}% damage</style>.");

            //LanguageAPI.Add(prefix + "SECONDARY_SLUG_SHOTGUN_NAME", "Knife");
            //LanguageAPI.Add(prefix + "SECONDARY_SLUG_SHOTGUN_DESCRIPTION", $"Throw a knife that gets stuck in the first enemy hit for <style=cIsDamage>{100f * SkillStates.Driver.Shotgun.Bash.damageCoefficient}% damage</style>. Shoot this knife to deal an additional <style=cIsDamage>{100f * SkillStates.Driver.Shotgun.Bash.damageCoefficient}% damage</style> and inflict <style=cIsHealth>Bleed</style>.");

            LanguageAPI.Add(prefix + "SECONDARY_SMG_NAME", "Phase Round");
            LanguageAPI.Add(prefix + "SECONDARY_SMG_DESCRIPTION", $"Fire a piercing laser for <style=cIsDamage>{100f * SkillStates.Driver.SMG.PhaseRound._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "SECONDARY_MACHINEGUN_NAME", "Zap");
            LanguageAPI.Add(prefix + "SECONDARY_MACHINEGUN_DESCRIPTION", $"<style=cIsDamage>Shocking.</style> Fire a quick laser for <style=cIsDamage>{100f * SkillStates.Driver.MachineGun.Zap._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "SECONDARY_HEAVY_MACHINEGUN_NAME", "Grenade");
            LanguageAPI.Add(prefix + "SECONDARY_HEAVY_MACHINEGUN_DESCRIPTION", $"Launch a grenade that <style=cIsUtility>stuns</style> enemies for <style=cIsDamage>{100f * SkillStates.Driver.HeavyMachineGun.ShootGrenade._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "SECONDARY_ROCKETLAUNCHER_NAME", "Hailstorm");
            LanguageAPI.Add(prefix + "SECONDARY_ROCKETLAUNCHER_DESCRIPTION", $"Fire a rapid barrage of rockets for <style=cIsDamage>{100f * SkillStates.Driver.RocketLauncher.Barrage._damageCoefficient}% damage</style> each.");

            LanguageAPI.Add(prefix + "SECONDARY_ROCKETLAUNCHER_ALT_NAME", "Hailstorm");
            LanguageAPI.Add(prefix + "SECONDARY_ROCKETLAUNCHER_ALT_DESCRIPTION", $"Fire a rapid barrage of rockets for <style=cIsDamage>{100f * SkillStates.Driver.RocketLauncher.NerfedBarrage._damageCoefficient}% damage</style> each.");

            LanguageAPI.Add(prefix + "SECONDARY_PLASMACANNON_NAME", "Annihilation");
            LanguageAPI.Add(prefix + "SECONDARY_PLASMACANNON_DESCRIPTION", $"Fire a rapid barrage of plasma bursts for <style=cIsDamage>{100f * SkillStates.Driver.PlasmaCannon.Barrage._damageCoefficient}% damage</style> each.");

            LanguageAPI.Add(prefix + "SECONDARY_SNIPER_NAME", "Focus");
            LanguageAPI.Add(prefix + "SECONDARY_SNIPER_DESCRIPTION", $"Aim down your scope, <style=cIsDamage>exposing enemy weak points</style> and fire a devastating shot for <style=cIsDamage>{100f * SkillStates.Driver.SniperRifle.Shoot._damageCoefficient}% damage</style>.");

            LanguageAPI.Add(prefix + "SECONDARY_BANDITREVOLVER_NAME", "Lights Out");
            LanguageAPI.Add(prefix + "SECONDARY_BANDITREVOLVER_DESCRIPTION", $"Take aim and fire a devastating shot for <style=cIsDamage>{100f * SkillStates.Driver.Revolver.LightsOutReset.damageCoefficient}% damage</style>. <style=cIsHealth>Resets cooldowns and ammo on kill, but consumes the gun otherwise.</style>");

            LanguageAPI.Add(prefix + "SECONDARY_GOLDENGUN_NAME", "Desperado");
            LanguageAPI.Add(prefix + "SECONDARY_GOLDENGUN_DESCRIPTION", $"Take aim and fire a devastating shot for <style=cIsDamage>{100f * SkillStates.Driver.GoldenGun.LightsOut.damageCoefficient}% damage</style>. <style=cIsHealth>Consumes the gun on use.</style>");

            LanguageAPI.Add(prefix + "SECONDARY_LUNARHAMMER_NAME", "Shards");
            LanguageAPI.Add(prefix + "SECONDARY_LUNARHAMMER_DESCRIPTION", $"<style=cIsUtility>Agile.</style> Fire a volley of <style=cIsUtility>lunar shards</style>, dealing <style=cIsDamage>{100f * SkillStates.Driver.LunarHammer.FireShard.damageCoefficient}% damage</style> each.");

            LanguageAPI.Add(prefix + "SECONDARY_MANDO_SMG_NAME", "Suppressive Fire");
            LanguageAPI.Add(prefix + "SECONDARY_MANDO_SMG_DESCRIPTION", $"<style=cIsDamage>Stunning.</style> Fire repeatedly for <style=cIsDamage>{SkillStates.Driver.SMG.SuppressiveFire._baseShotCount}x{100f * SkillStates.Driver.SMG.SuppressiveFire._damageCoefficient}% damage</style>. The number of shots increases with attack speed.");

            LanguageAPI.Add(prefix + "SECONDARY_NEMMANDO_NAME", "Submission");
            LanguageAPI.Add(prefix + "SECONDARY_NEMMANDO_DESCRIPTION", $"<style=cIsDamage>Stunning.</style> Fire repeatedly for <style=cIsDamage>{SkillStates.Driver.NemmandoGun.Submission._bulletCount}x{100f * SkillStates.Driver.NemmandoGun.Submission._damageCoefficient}% damage</style> per shot. The number of shots increases with attack speed.");

            LanguageAPI.Add(prefix + "SECONDARY_NEMMERC_NAME", "Bash");
            LanguageAPI.Add(prefix + "SECONDARY_NEMMERC_DESCRIPTION", $"<style=cIsDamage>Stun</style> and <style=cIsUtility>knock back</style> nearby enemies for <style=cIsDamage>{100f * Bash.damageCoefficient}% damage</style>.");
            
            LanguageAPI.Add(prefix + "SECONDARY_RAV_PUNCH_NAME", "Pummel");
            LanguageAPI.Add(prefix + "SECONDARY_RAV_PUNCH_DESCRIPTION", $"Lunge and <style=cIsUtility>punch</style>, dealing <style=cIsDamage>{100f * SkillStates.Driver.RavSword.DashPunch.punchDamageCoefficient}% damage</style> with a <style=cIsUtility>shockwave</style> through them for the same damage.");

            LanguageAPI.Add(prefix + "SECONDARY_ARTI_GAUNTLET_NAME", "Nano-Bomb");
            LanguageAPI.Add(prefix + "SECONDARY_ARTI_GAUNTLET_DESCRIPTION", $"<style=cIsDamage>Stunning.</style> Charge up an <style=cIsDamage>exploding</style> nano-bomb that deals <style=cIsDamage>{100f * SkillStates.Driver.ArtiGauntlet.ThrowBomb.minDamageCoefficient}%-{100f * SkillStates.Driver.ArtiGauntlet.ThrowBomb.minDamageCoefficient}%</style> damage.");
            #endregion

            #region Utility
            LanguageAPI.Add(prefix + "UTILITY_SLIDE_NAME", "Combat Slide");
            LanguageAPI.Add(prefix + "UTILITY_SLIDE_DESCRIPTION", "<style=cIsUtility>Slide</style> on the ground for a short distance. You can <style=cIsDamage>fire while sliding.</style>");

            LanguageAPI.Add(prefix + "UTILITY_DASH_NAME", "Sidestep");
            LanguageAPI.Add(prefix + "UTILITY_DASH_DESCRIPTION", "<style=cIsUtility>Dash</style> a short distance. You can <style=cIsUtility>hold up to 2 charges.</style>");

            LanguageAPI.Add(prefix + "UTILITY_SKATEBOARD_NAME", "Skateboard");
            LanguageAPI.Add(prefix + "UTILITY_SKATEBOARD_DESCRIPTION", "Ride your <style=cIsUtility>skateboard</style>.");

            LanguageAPI.Add(prefix + "UTILITY_SKATEBOARD2_DESCRIPTION", "Get off your <style=cIsUtility>skateboard</style>.");
            #endregion

            #region Special
            LanguageAPI.Add(prefix + "SPECIAL_GRENADE_NAME", "Flashbang");
            LanguageAPI.Add(prefix + "SPECIAL_GRENADE_DESCRIPTION", $"Throw a grenade that <style=cIsUtility>dazes</style> enemies for <style=cIsDamage>{100f * SkillStates.Driver.ThrowGrenade._damageCoefficient}% damage</style>. <style=cIsUtility>Dazed enemies aim in random directions for 10 seconds.</style>");

            LanguageAPI.Add(prefix + "SPECIAL_GRENADE_SCEPTER_NAME", "Molotov");
            LanguageAPI.Add(prefix + "SPECIAL_GRENADE_SCEPTER_DESCRIPTION", $"Throw a grenade that <style=cIsUtility>dazes</style> enemies for <style=cIsDamage>{100f * SkillStates.Driver.ThrowGrenade._damageCoefficient}% damage</style>. <style=cIsUtility>Dazed enemies aim in random directions for 10 seconds.</style>" + Helpers.ScepterDescription("Throw a molotov that bursts into flames instead."));

            LanguageAPI.Add(prefix + "SPECIAL_SUPPLY_DROP_NAME", "Supply Drop");
            LanguageAPI.Add(prefix + "SPECIAL_SUPPLY_DROP_DESCRIPTION", $"Call down a briefcase containing a <color=#{Helpers.greenItemHex}>random weapon</color>. <style=cIsHealth>Weapon comes with only half its ammo.</style>");

            LanguageAPI.Add(prefix + "SPECIAL_SUPPLY_DROP_LEGACY_NAME", "Supply Drop (Legacy)");
            LanguageAPI.Add(prefix + "SPECIAL_SUPPLY_DROP_LEGACY_DESCRIPTION", $"Call down a briefcase containing a <color=#{Helpers.yellowItemHex}>Prototype Rocket Launcher</color>. <style=cIsUtility>You can only request one per stage.</style>");

            LanguageAPI.Add(prefix + "SPECIAL_SUPPLY_DROP_SCEPTER_NAME", "Call of the Void");
            LanguageAPI.Add(prefix + "SPECIAL_SUPPLY_DROP_SCEPTER_DESCRIPTION", $"Call down a briefcase containing a <color=#{Helpers.greenItemHex}>random weapon</color>. <style=cIsHealth>Weapon comes with only half its ammo.</style>" + Helpers.ScepterDescription("Summon a <color=#" + Helpers.voidItemHex + ">voidborn weapon</color> instead."));

            LanguageAPI.Add(prefix + "SPECIAL_SUPPLY_DROP_LEGACY_SCEPTER_NAME", "Call of the Void");
            LanguageAPI.Add(prefix + "SPECIAL_SUPPLY_DROP_LEGACY_SCEPTER_DESCRIPTION", $"Call down a briefcase containing a <color=#{Helpers.yellowItemHex}>Prototype Rocket Launcher</color>." + Helpers.ScepterDescription("Summon a <color=#" + Helpers.voidItemHex + ">voidborn weapon</color> instead."));

            LanguageAPI.Add(prefix + "SPECIAL_HEAL_NAME", "Self-Care");
            LanguageAPI.Add(prefix + "SPECIAL_HEAL_DESCRIPTION", $"Pull out a <style=cIsUtility>Medkit</style> and gradually <style=cIsUtility>heal</style> yourself back to full health.");

            LanguageAPI.Add(prefix + "SPECIAL_KNIFE_NAME", "Combat Knife");
            LanguageAPI.Add(prefix + "SPECIAL_KNIFE_DESCRIPTION", $"Slash nearby enemies with a serrated blade, dealing <style=cIsDamage>470% damage</style> and <style=cIsHealth>wounding</style> them, <style=cIsDamage>lowering their armor</style> for <style=cIsUtility>4 seconds</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_KNIFE_SCEPTER_NAME", "Combat Knife (Scepter)");
            LanguageAPI.Add(prefix + "SPECIAL_KNIFE_SCEPTER_DESCRIPTION", $"Slash nearby enemies with a serrated blade, dealing <style=cIsDamage>470% damage</style> and <style=cIsHealth>wounding</style> them, <style=cIsDamage>lowering their armor</style> for <style=cIsUtility>4 seconds</style>."
                + Helpers.ScepterDescription("Cooldown is halved and gain an extra stock."));

            LanguageAPI.Add(prefix + "SPECIAL_SYRINGE_NAME", "Experimental Syringe");
            LanguageAPI.Add(prefix + "SPECIAL_SYRINGE_DESCRIPTION", $"Inject yourself with a <style=cIsUtility>syringe</style>, giving you <style=cIsDamage>bonus attack speed</style>, <style=cIsUtility>movement speed</style> and <style=cIsHealing>health regen</style> for the next <style=cIsUtility>6 seconds</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_SYRINGE_SCEPTER_NAME", "Perfected Syringe");
            LanguageAPI.Add(prefix + "SPECIAL_SYRINGE_SCEPTER_DESCRIPTION", $"Inject yourself with a <style=cIsUtility>syringe</style>, giving you <style=cIsDamage>bonus attack speed</style> and <style=cIsHealing>health regen</style> for the next <style=cIsUtility>6 seconds</style>."
                + Helpers.ScepterDescription("Increases damage and critical chance and strengthens buffs."));

            LanguageAPI.Add(prefix + "SPECIAL_SYRINGELEGACY_NAME", "Suspicious Syringe (Legacy)");
            LanguageAPI.Add(prefix + "SPECIAL_SYRINGELEGACY_DESCRIPTION", $"Inject yourself with a <style=cIsUtility>syringe</style>, giving you a <style=cIsDamage>random offensive buff</style> for the next <style=cIsUtility>6 seconds</style>.");

            LanguageAPI.Add(prefix + "SPECIAL_SYRINGELEGACY_SCEPTER_NAME", "Perfected Syringe (Legacy)");
            LanguageAPI.Add(prefix + "SPECIAL_SYRINGELEGACY_SCEPTER_DESCRIPTION", $"Inject yourself with a <style=cIsUtility>syringe</style>, giving you a <style=cIsDamage>random offensive buff</style> for the next <style=cIsUtility>6 seconds</style>."
                + Helpers.ScepterDescription("Applies all three buffs at once."));

            LanguageAPI.Add(prefix + "SPECIAL_DRIVERCOIN_NAME", "Pay it Forward");
            LanguageAPI.Add(prefix + "SPECIAL_DRIVERCOIN_DESCRIPTION", $"Flick out a <style=cIsUtility>coin</style> which can be shot to multiply <style=cIsDamage>damage</style>.");
            #endregion

            #region Achievements
            // unlockables tied to achievements dont use tokens
            string nameFormat = "ACHIEVEMENT_{0}_NAME";
            string descFormat = "ACHIEVEMENT_{0}_DESCRIPTION";

            LanguageAPI.Add(string.Format(nameFormat, DriverUnlockAchievement.IDENTIFIER), "A Real Hero");
            LanguageAPI.Add(string.Format(descFormat, DriverUnlockAchievement.IDENTIFIER), "Reach stage 3 in less than 15 minutes.");

            LanguageAPI.Add(string.Format(nameFormat, DriverMonsoonAchievement.IDENTIFIER), "Driver: Mastery");
            LanguageAPI.Add(string.Format(descFormat, DriverMonsoonAchievement.IDENTIFIER), "As Driver, beat the game or obliterate on Monsoon.");

            LanguageAPI.Add(string.Format(nameFormat, DriverTyphoonAchievement.IDENTIFIER), "Driver: Grand Mastery");
            LanguageAPI.Add(string.Format(descFormat, DriverTyphoonAchievement.IDENTIFIER), "As Driver, beat the game or obliterate on Typhoon or win on Eclipse 8.\n<color=#8888>(Counts any difficulty Typhoon or higher)</color>");

            LanguageAPI.Add(string.Format(nameFormat, DriverSupplyDropAchievement.IDENTIFIER), "Driver: Locked and Loaded");
            LanguageAPI.Add(string.Format(descFormat, DriverSupplyDropAchievement.IDENTIFIER), "As Driver, complete a teleporter event without letting any briefcases despawn.");

            LanguageAPI.Add(string.Format(nameFormat, DriverPistolPassiveAchievement.IDENTIFIER), "Driver: Professional Killer");
            LanguageAPI.Add(string.Format(descFormat, DriverPistolPassiveAchievement.IDENTIFIER), "As Driver, complete a teleporter event without picking up any weapons.");

            LanguageAPI.Add(string.Format(nameFormat, DriverGodslingPassiveAchievement.IDENTIFIER), "Driver: Ryan Godsling");
            LanguageAPI.Add(string.Format(descFormat, DriverGodslingPassiveAchievement.IDENTIFIER), "As Driver, beat the game or obliterate on Monsoon or higher without picking up any weapons or bullets.");

            LanguageAPI.Add(string.Format(nameFormat, DriverSuitAchievement.IDENTIFIER), "Driver: Dressed to Kill");
            LanguageAPI.Add(string.Format(descFormat, DriverSuitAchievement.IDENTIFIER), "As Driver, land the killing blow on a boss with a Sniper Rifle.");
            #endregion

            #region Gun shit
            LanguageAPI.Add("UNLOCKABLE_ROB_DRIVER_WEAPON_NAME", "Weapon Unlocked");
            LanguageAPI.Add("UNLOCKABLE_ROB_DRIVER_WEAPON_DESC", "This weapon can now be selected at any time from Driver's Arsenal.");

            LanguageAPI.Add("ROB_DRIVER_PASSIVE_TOKEN", "Passive");
            LanguageAPI.Add("ROB_DRIVER_ARSENAL_TOKEN", "Arsenal");

            LanguageAPI.Add("ROB_DRIVER_JAMMED_POPUP", "JAMMED...");
            LanguageAPI.Add("ROB_DRIVER_UPGRADE_POPUP", "UPGRADE!");

            #endregion
        }
    }
}
