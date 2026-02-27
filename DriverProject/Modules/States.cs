using System.Collections.Generic;
using System;

namespace RobDriver.Modules
{
    public static class States
    {
        internal static List<Type> entityStates = [];

        public static void RegisterStates()
        {
            // main
            entityStates.Add(typeof(SkillStates.Driver.MainState));
            entityStates.Add(typeof(SkillStates.Driver.WeaponMainState));
            entityStates.Add(typeof(SkillStates.Driver.DiscardWeapon));
            entityStates.Add(typeof(SkillStates.Driver.Skateboard.StartGrind));

            entityStates.Add(typeof(SkillStates.Driver.Bash));
            entityStates.Add(typeof(SkillStates.Driver.Coin));
            entityStates.Add(typeof(SkillStates.Driver.Dash));
            entityStates.Add(typeof(SkillStates.Driver.Heal));
            entityStates.Add(typeof(SkillStates.Driver.JammedGun));
            entityStates.Add(typeof(SkillStates.Driver.Reload));
            entityStates.Add(typeof(SkillStates.Driver.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.Slide));
            entityStates.Add(typeof(SkillStates.Driver.SteadyAim));
            entityStates.Add(typeof(SkillStates.Driver.SwingKnife));
            entityStates.Add(typeof(SkillStates.Driver.ThrowGrenade));
            entityStates.Add(typeof(SkillStates.Driver.UseSyringe));
            entityStates.Add(typeof(SkillStates.Driver.UseSyringeLegacy));

            // compat
            entityStates.Add(typeof(SkillStates.Driver.Scepter.SwingKnifeScepter));
            entityStates.Add(typeof(SkillStates.Driver.Scepter.UseSyringeScepter));
            entityStates.Add(typeof(SkillStates.Driver.Scepter.ThrowMolotov));
            entityStates.Add(typeof(SkillStates.Driver.Scepter.CoinScepter));

            entityStates.Add(typeof(SkillStates.Driver.Scepter.SupplyDrop.AimVoidDrop));
            entityStates.Add(typeof(SkillStates.Driver.Scepter.SupplyDrop.FireVoidDrop));
            entityStates.Add(typeof(SkillStates.Driver.Scepter.SupplyDrop.CancelVoidDrop));

            entityStates.Add(typeof(SkillStates.Driver.NemmandoSword.SwingSword));

            entityStates.Add(typeof(SkillStates.Driver.RavSword.SlashCombo));
            entityStates.Add(typeof(SkillStates.Driver.RavSword.ChargeSlash));
            entityStates.Add(typeof(SkillStates.Driver.RavSword.ThrowSlash));

            entityStates.Add(typeof(SkillStates.Driver.RavSword.DashPunch));
            entityStates.Add(typeof(SkillStates.Driver.RavSword.PunchRecoil));

            entityStates.Add(typeof(SkillStates.Driver.RavSword.ChargeJump));
            entityStates.Add(typeof(SkillStates.Driver.RavSword.WallJump));
            entityStates.Add(typeof(SkillStates.Driver.RavSword.WallJumpBig));
            entityStates.Add(typeof(SkillStates.Driver.RavSword.WallJumpSmall));

            entityStates.Add(typeof(SkillStates.Driver.ArmCannon.Shoot));

            entityStates.Add(typeof(SkillStates.Driver.NemmandoGun.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.NemmandoGun.Submission));

            entityStates.Add(typeof(SkillStates.Driver.NemmercGun.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.NemmercGun.Shoot2));

            entityStates.Add(typeof(SkillStates.Driver.CaptainGun.AttemptAirstrike));
            entityStates.Add(typeof(SkillStates.Driver.CaptainGun.CallAirstrike));
            entityStates.Add(typeof(SkillStates.Driver.CaptainGun.ChargeShotgun));
            entityStates.Add(typeof(SkillStates.Driver.CaptainGun.Shoot));

            // weapons
            entityStates.Add(typeof(SkillStates.Driver.BeetleShield.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.BeetleShield.SteadyAim));

            entityStates.Add(typeof(SkillStates.Driver.GrenadeLauncher.Shoot));

            entityStates.Add(typeof(SkillStates.Driver.Bazooka.Charge));
            entityStates.Add(typeof(SkillStates.Driver.Bazooka.Fire));

            entityStates.Add(typeof(SkillStates.Driver.GoldenGun.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.GoldenGun.AimLightsOut));
            entityStates.Add(typeof(SkillStates.Driver.GoldenGun.LightsOut));

            entityStates.Add(typeof(SkillStates.Driver.Shotgun.Shoot));

            entityStates.Add(typeof(SkillStates.Driver.RiotShotgun.Shoot));

            entityStates.Add(typeof(SkillStates.Driver.SlugShotgun.Shoot));

            entityStates.Add(typeof(SkillStates.Driver.BadassShotgun.Shoot));

            entityStates.Add(typeof(SkillStates.Driver.HeavyMachineGun.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.HeavyMachineGun.ShootGrenade));

            entityStates.Add(typeof(SkillStates.Driver.LunarPistol.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.LunarPistol.SteadyAim));

            entityStates.Add(typeof(SkillStates.Driver.MachineGun.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.MachineGun.Zap));

            entityStates.Add(typeof(SkillStates.Driver.PlasmaCannon.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.PlasmaCannon.Barrage));

            entityStates.Add(typeof(SkillStates.Driver.RocketLauncher.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.RocketLauncher.Barrage));
            entityStates.Add(typeof(SkillStates.Driver.RocketLauncher.NerfedShoot));
            entityStates.Add(typeof(SkillStates.Driver.RocketLauncher.NerfedBarrage));

            entityStates.Add(typeof(SkillStates.Driver.PyriteGun.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.PyriteGun.SteadyAim));

            entityStates.Add(typeof(SkillStates.Driver.SniperRifle.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.SniperRifle.Aim));

            entityStates.Add(typeof(SkillStates.Driver.VoidPistol.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.VoidPistol.SteadyAim));

            entityStates.Add(typeof(SkillStates.Driver.FalsePistol.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.FalsePistol.SteadyAim));

            entityStates.Add(typeof(SkillStates.Driver.VoidRifle.Shoot));

            entityStates.Add(typeof(SkillStates.Driver.BadassShotgun.Shoot));

            entityStates.Add(typeof(SkillStates.Driver.LunarRifle.Shoot));

            entityStates.Add(typeof(SkillStates.Driver.LunarGrenade.Shoot));

            entityStates.Add(typeof(SkillStates.Driver.LunarHammer.SwingCombo));
            entityStates.Add(typeof(SkillStates.Driver.LunarHammer.FireShard));

            entityStates.Add(typeof(SkillStates.Driver.GolemGun.ChargeLaser));
            entityStates.Add(typeof(SkillStates.Driver.GolemGun.FireLaser));

            entityStates.Add(typeof(SkillStates.Driver.ArmBFG.Shoot));

            entityStates.Add(typeof(SkillStates.Driver.ArtiGauntlet.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.ArtiGauntlet.ChargeBomb));
            entityStates.Add(typeof(SkillStates.Driver.ArtiGauntlet.ThrowBomb));

            entityStates.Add(typeof(SkillStates.Driver.SMG.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.SMG.PhaseRound));
            entityStates.Add(typeof(SkillStates.Driver.SMG.SuppressiveFire));

            entityStates.Add(typeof(SkillStates.Driver.Revolver.Shoot));
            entityStates.Add(typeof(SkillStates.Driver.Revolver.AimLightsOut));
            entityStates.Add(typeof(SkillStates.Driver.Revolver.AimLightsOutReset));
            entityStates.Add(typeof(SkillStates.Driver.Revolver.LightsOut));
            entityStates.Add(typeof(SkillStates.Driver.Revolver.LightsOutReset));

            entityStates.Add(typeof(SkillStates.Driver.SupplyDrop.AimSupplyDrop));
            entityStates.Add(typeof(SkillStates.Driver.SupplyDrop.FireSupplyDrop));
            entityStates.Add(typeof(SkillStates.Driver.SupplyDrop.CancelSupplyDrop));

            entityStates.Add(typeof(SkillStates.Driver.SupplyDrop.Nerfed.AimCrapDrop));
            entityStates.Add(typeof(SkillStates.Driver.SupplyDrop.Nerfed.FireCrapDrop));
            entityStates.Add(typeof(SkillStates.Driver.SupplyDrop.Nerfed.CancelCrapDrop));

            entityStates.Add(typeof(SkillStates.Driver.Skateboard.Start));
            entityStates.Add(typeof(SkillStates.Driver.Skateboard.Idle));
            entityStates.Add(typeof(SkillStates.Driver.Skateboard.Stop));

            entityStates.Add(typeof(SkillStates.Emote.BaseEmote));
            entityStates.Add(typeof(SkillStates.Emote.Rest));
            entityStates.Add(typeof(SkillStates.Emote.Taunt));
            entityStates.Add(typeof(SkillStates.Emote.Dance));

            entityStates.Add(typeof(SkillStates.FuckMyAss));
        }
    }
}