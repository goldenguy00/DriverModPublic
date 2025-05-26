using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.Modules.Weapons
{
    public class ScavGun : BaseWeapon<ScavGun>
    {
        public override string weaponName => "Energy Cannon";
        public override string weaponNameToken => "ROB_DRIVER_WEAPON_SCAV_GUN_NAME";
        public override string weaponDesc => "A scavenged weapon that fires blasts of raw energy.";
        public override string weaponDescToken => "ROB_DRIVER_WEAPON_SCAV_GUN_DESC";

        public override Sprite icon => Assets.mainAssetBundle.LoadAsset<Sprite>("texScavGunWeaponIcon");
        public override DriverWeaponTier dropTier => DriverWeaponTier.Unique;
        public override DriverWeaponDef.AnimationSet animationSet => DriverWeaponDef.AnimationSet.TwoHanded;
        public override DriverWeaponDef.BuffType buffType => DriverWeaponDef.BuffType.AttackSpeed;
        public override int shotCount => 36;

        public override Mesh mesh => Modules.Assets.LoadMesh("meshScavGun");
        public override Material material => Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/TrimSheets/matTrimSheetConstructionBlueEmission.mat").WaitForCompletion();
        public override GameObject crosshairPrefab => Modules.Assets.grenadeLauncherCrosshairPrefab;
        public override GameObject pickupPrefabOverride => Assets.lunarPickupModel;
        public override Color? colorOverride => Helpers.lunarItemColor;

        public override float dropChance => 100f;
        public override string uniqueDropBodyName => "Scav";

        public override SkillDef primarySkillDef => Modules.Skills.lunarRiflePrimarySkillDef;

        public override SkillDef secondarySkillDef => Skills.bashSkillDef;
    }
}