using RobDriver.SkillStates.Driver.NemmandoSword;
using RoR2.Skills;
using UnityEngine;

namespace RobDriver.Modules.Weapons
{
    public class NemKatana : BaseWeapon<NemKatana>
    {
        public override string weaponName => "Murasama";
        public override string weaponNameToken => "ROB_DRIVER_WEAPON_NEM_KATANA_NAME";
        public override string weaponDesc => "The only thing I know.";
        public override string weaponDescToken => "ROB_DRIVER_WEAPON_NEM_KATANA_DESC";

        public override Sprite icon => Assets.mainAssetBundle.LoadAsset<Sprite>("texNemCommandoPrimary");
        public override DriverWeaponTier dropTier => DriverWeaponTier.Void;
        public override DriverWeaponDef.AnimationSet animationSet => DriverWeaponDef.AnimationSet.BigMelee;
        public override DriverWeaponDef.BuffType buffType => DriverWeaponDef.BuffType.Damage;
        public override int shotCount => 64;

        public override Mesh mesh => Assets.LoadMesh("meshKatana");
        public override Material material => Assets.nemKatanaMat;
        public override GameObject crosshairPrefab => Assets.needlerCrosshairPrefab;
        public override GameObject pickupPrefabOverride => Assets.voidPickupModel;
        public override Color? colorOverride => Helpers.voidItemColor;

        public override float dropChance => 100f;
        public override string uniqueDropBodyName => "NemCommando";

        public override SkillDef primarySkillDef => Skills.CreateAndAddPrimarySkillDef(
            new EntityStates.SerializableEntityStateType(typeof(SwingSword)),
            "Weapon",
            "ROB_DRIVER_BODY_PRIMARY_NEMMANDO_SWORD_NAME",
            "ROB_DRIVER_BODY_PRIMARY_NEMMANDO_SWORD_DESCRIPTION",
            Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNemCommandoPrimary"),
            true);

        public override SkillDef secondarySkillDef => Skills.nemmandoGunSecondarySkillDef;
    }
}