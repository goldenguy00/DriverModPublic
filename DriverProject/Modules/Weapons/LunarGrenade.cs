using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.Modules.Weapons
{
    public class LunarGrenade : BaseWeapon<LunarGrenade>
    {
        public override string weaponName => "Lunar Launcher";
        public override string weaponNameToken => "ROB_DRIVER_WEAPON_LUNAR_GRENADE_NAME";
        public override string weaponDesc => "Fire orbs of lunar energy.";
        public override string weaponDescToken => "ROB_DRIVER_WEAPON_LUNAR_GRENADE_DESC";

        public override Sprite icon => Assets.mainAssetBundle.LoadAsset<Sprite>("texLunarGrenadeWeaponIcon");
        public override DriverWeaponTier dropTier => DriverWeaponTier.Lunar;
        public override DriverWeaponDef.AnimationSet animationSet => DriverWeaponDef.AnimationSet.TwoHanded;
        public override DriverWeaponDef.BuffType buffType => DriverWeaponDef.BuffType.Damage;
        public override int shotCount => 48;

        public override Mesh mesh => Modules.Assets.LoadMesh("meshLunarGrenade");
        public override Material material => Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarGolem/matLunarGolem.mat").WaitForCompletion();
        public override GameObject crosshairPrefab => Modules.Assets.grenadeLauncherCrosshairPrefab;
        public override GameObject pickupPrefabOverride => Assets.lunarPickupModel;
        public override Color? colorOverride => Helpers.lunarItemColor;

        public override float dropChance => 10f;
        public override string uniqueDropBodyName => "LunarExploder";

        public override SkillDef primarySkillDef => Modules.Skills.CreateAndAddPrimarySkillDef(
            new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.LunarGrenade.Shoot)),
            "Weapon",
            "ROB_DRIVER_BODY_PRIMARY_LUNAR_GRENADELAUNCHER_NAME",
            "ROB_DRIVER_BODY_PRIMARY_LUNAR_GRENADELAUNCHER_DESCRIPTION",
            Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
            false);

        public override SkillDef secondarySkillDef => Modules.Skills.bashSkillDef;
    }
}