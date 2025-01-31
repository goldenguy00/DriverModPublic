using R2API;
using RobDriver.Modules;
using RoR2;

namespace RobDriver
{
    public static class DriverDamageTypes
    {
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

        public static DotController.DotIndex GougeDotIndex;

        internal static void Init()
        {
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

            GougeDotIndex = DotAPI.RegisterDotDef(0.25f, 0.25f, DamageColorIndex.SuperBleed, Buffs.gougeDebuff);
        }
    }
}