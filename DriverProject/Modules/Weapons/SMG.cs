using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.Modules.Weapons
{
    public class SMG : BaseWeapon<SMG>
    {
        public override string weaponName => "Submachine Gun";
        public override string weaponNameToken => "ROB_DRIVER_WEAPON_SMG_NAME";
        public override string weaponDesc => "Close-range gun with high damage and equally high spread.";
        public override string weaponDescToken => "ROB_DRIVER_WEAPON_SMG_DESC";

        public override Sprite icon => Assets.mainAssetBundle.LoadAsset<Sprite>("texSMGWeaponIcon");
        public override DriverWeaponTier dropTier => DriverWeaponTier.Common;
        public override DriverWeaponDef.AnimationSet animationSet => DriverWeaponDef.AnimationSet.Default;
        public override DriverWeaponDef.BuffType buffType => DriverWeaponDef.BuffType.AttackSpeed;
        public override int shotCount => 48;

        public override Mesh mesh => Modules.Assets.LoadMesh("meshCommandoGun");
        public override Material material => Addressables.LoadAssetAsync<Material>("RoR2/Base/Commando/matCommandoDualies.mat").WaitForCompletion();
        public override GameObject crosshairPrefab => Modules.Assets.smgCrosshairPrefab;
        public override string reloadAnimationString => "ReloadPistol";
        public override string equipAnimationString => "EquipPistol";

        public override SkillDef primarySkillDef => Modules.Skills.CreateAndAddPrimarySkillDef(
            new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SMG.Shoot)),
            "Weapon",
            "ROB_DRIVER_BODY_PRIMARY_SMG_NAME",
            "ROB_DRIVER_BODY_PRIMARY_SMG_DESCRIPTION",
            Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
            false);

        public override SkillDef secondarySkillDef => Modules.Skills.CreateAndAddSkillDef(new SkillDefInfo
        {
            skillName = "ROB_DRIVER_BODY_SECONDARY_SMG_NAME",
            skillNameToken = "ROB_DRIVER_BODY_SECONDARY_SMG_NAME",
            skillDescriptionToken = "ROB_DRIVER_BODY_SECONDARY_SMG_DESCRIPTION",
            skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
            activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SMG.PhaseRound)),
            activationStateMachineName = "Weapon",
            baseMaxStock = 1,
            baseRechargeInterval = 6f,
            beginSkillCooldownOnSkillEnd = false,
            canceledFromSprinting = false,
            forceSprintDuringState = false,
            fullRestockOnAssign = true,
            interruptPriority = EntityStates.InterruptPriority.Skill,
            resetCooldownTimerOnUse = true,
            isCombatSkill = true,
            mustKeyPress = false,
            cancelSprintingOnActivation = true,
            rechargeStock = 1,
            requiredStock = 1,
            stockToConsume = 1,
        });
    }
}