using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.Modules.Weapons
{
    public class CommandoSMG : BaseWeapon<CommandoSMG>
    {
        public override string weaponName => "Worn SMG";
        public override string weaponNameToken => "ROB_DRIVER_WEAPON_COMMANDOSMG_NAME";
        public override string weaponDesc => "An SMG that's seen better days.";
        public override string weaponDescToken => "ROB_DRIVER_WEAPON_COMMANDOSMG_DESC";

        public override Sprite icon => Assets.mainAssetBundle.LoadAsset<Sprite>("texCommandoSMGWeaponIcon");
        public override DriverWeaponTier dropTier => DriverWeaponTier.Unique;
        public override DriverWeaponDef.AnimationSet animationSet => DriverWeaponDef.AnimationSet.Default;
        public override DriverWeaponDef.BuffType buffType => DriverWeaponDef.BuffType.AttackSpeed;
        public override int shotCount => 64;

        public override Mesh mesh => Modules.Assets.LoadMesh("meshCommandoGun");
        public override Material material => Addressables.LoadAssetAsync<Material>("RoR2/Base/Commando/matCommandoDualies.mat").WaitForCompletion();
        public override GameObject crosshairPrefab => Modules.Assets.smgCrosshairPrefab;

        public override string equipAnimationString => "EquipPistol";
        public override float dropChance => 100f;
        public override string uniqueDropBodyName => "Commando";

        public override SkillDef primarySkillDef => Skills.CreateAndAddPrimarySkillDef(
            new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SMG.Shoot)),
            "Weapon",
            "ROB_DRIVER_BODY_PRIMARY_SMG_NAME",
            "ROB_DRIVER_BODY_PRIMARY_SMG_DESCRIPTION",
            Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texMachineGunIcon"),
            false);

        public override SkillDef secondarySkillDef => Skills.CreateAndAddSkillDef(new SkillDefInfo
        {
            skillName = "ROB_DRIVER_BODY_SECONDARY_MANDO_SMG_NAME",
            skillNameToken = "ROB_DRIVER_BODY_SECONDARY_MANDO_SMG_NAME",
            skillDescriptionToken = "ROB_DRIVER_BODY_SECONDARY_MANDO_SMG_DESCRIPTION",
            skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoSecondaryIcon"),
            activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SMG.SuppressiveFire)),
            activationStateMachineName = "Weapon",
            baseMaxStock = 1,
            baseRechargeInterval = 6f,
            beginSkillCooldownOnSkillEnd = false,
            canceledFromSprinting = false,
            forceSprintDuringState = false,
            fullRestockOnAssign = true,
            interruptPriority = EntityStates.InterruptPriority.Skill,
            resetCooldownTimerOnUse = false,
            isCombatSkill = true,
            mustKeyPress = false,
            cancelSprintingOnActivation = true,
            rechargeStock = 1,
            requiredStock = 1,
            stockToConsume = 1,
        });
    }
}