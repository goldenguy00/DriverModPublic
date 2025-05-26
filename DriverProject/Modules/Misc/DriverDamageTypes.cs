using R2API;
using RoR2;

namespace RobDriver.Modules
{
    public static class DriverDamageTypes
    {
        public static DamageAPI.ModdedDamageType Generic = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType HookShot = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType MissileShot = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType VoidMissileShot = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType ExplosiveRounds = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType FlameTornadoShot = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType IceBlastShot = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType DaggerShot = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType LightningStrikeRounds = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType FireballRounds = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType StickyShot = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType VoidLightning = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType CoinShot = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType MysteryShot = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType Hemorrhage = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType Gouge = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType BetterBurn = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType Collapse = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType Helfire = DamageAPI.ReserveDamageType();

        public static DamageAPI.ModdedDamageType BloodExplosionIdentifier = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType StunGrenadeDazed = DamageAPI.ReserveDamageType();
        public static DamageAPI.ModdedDamageType KnifeWound = DamageAPI.ReserveDamageType();

        public static DotController.DotIndex GougeDotIndex = DotAPI.RegisterDotDef(0.25f, 0.25f, DamageColorIndex.SuperBleed, Buffs.gougeDebuff);
    }
}