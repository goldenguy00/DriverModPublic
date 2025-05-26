using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.Modules.Weapons
{
    public class ArmBFG : BaseWeapon<ArmBFG>
    {
        public override string weaponName => "Preon Accelerator";
        public override string weaponNameToken => "ROB_DRIVER_WEAPON_BFG_NAME";
        public override string weaponDesc => "Devastating blasts of condensed particles.";
        public override string weaponDescToken => "ROB_DRIVER_WEAPON_BFG_DESC";

        public override Sprite icon => Assets.mainAssetBundle.LoadAsset<Sprite>("texPreonWeaponIcon");
        public override DriverWeaponTier dropTier => DriverWeaponTier.Legendary; 
        public override DriverWeaponDef.AnimationSet animationSet => DriverWeaponDef.AnimationSet.Default;
        public override DriverWeaponDef.BuffType buffType => DriverWeaponDef.BuffType.AttackSpeed;
        public override int shotCount => 12;

        public override Mesh mesh => Modules.Assets.LoadMesh("meshArmBFG");
        public override Material material => Addressables.LoadAssetAsync<Material>("RoR2/Base/BFG/matBFG.mat").WaitForCompletion();
        public override GameObject crosshairPrefab => Modules.Assets.grenadeLauncherCrosshairPrefab;

        public override string equipAnimationString => "Recharge";
        public override string reloadAnimationString => "Recharge";
        public override bool disableHolster => true;

        public override SkillDef primarySkillDef => Modules.Skills.CreateAndAddPrimarySkillDef(
            new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.ArmBFG.Shoot)),
            "Weapon",
            "ROB_DRIVER_BODY_PRIMARY_BFG_NAME",
            "ROB_DRIVER_BODY_PRIMARY_BFG_DESCRIPTION",
            Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
            false);

        public override SkillDef secondarySkillDef => Skills.bashSkillDef;
    }
}