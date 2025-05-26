using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.Modules.Weapons
{
    public class CrabGun : BaseWeapon<CrabGun>
    {
        public override string weaponName => "Nullifier";
        public override string weaponNameToken => "ROB_DRIVER_WEAPON_VOID_RIFLE_NAME";
        public override string weaponDesc => "Erase everything in sight.";
        public override string weaponDescToken => "ROB_DRIVER_WEAPON_VOID_RIFLE_DESC";

        public override Sprite icon => Assets.mainAssetBundle.LoadAsset<Sprite>("texCrabGunWeaponIcon");
        public override DriverWeaponTier dropTier => DriverWeaponTier.Void;
        public override DriverWeaponDef.AnimationSet animationSet => DriverWeaponDef.AnimationSet.TwoHanded;
        public override DriverWeaponDef.BuffType buffType => DriverWeaponDef.BuffType.AttackSpeed;
        public override int shotCount => 64;

        public override Mesh mesh => Modules.Assets.LoadMesh("meshCrabGun");
        public override Material material => Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidMegaCrab/matVoidMegaCrab.mat").WaitForCompletion();
        public override GameObject crosshairPrefab => Modules.Assets.circleCrosshairPrefab;
        public override GameObject pickupPrefabOverride => Assets.voidPickupModel;
        public override Color? colorOverride => Helpers.voidItemColor;

        public override float dropChance => 50f;
        public override string uniqueDropBodyName => "VoidMegaCrab";

        public override SkillDef primarySkillDef => Modules.Skills.CreateAndAddPrimarySkillDef(
            new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.VoidRifle.Shoot)),
            "Weapon",
            "ROB_DRIVER_BODY_PRIMARY_VOIDRIFLE_NAME",
            "ROB_DRIVER_BODY_PRIMARY_VOIDRIFLE_DESCRIPTION",
            Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
            false);

        public override SkillDef secondarySkillDef => Modules.Skills.bashSkillDef;
    }
}