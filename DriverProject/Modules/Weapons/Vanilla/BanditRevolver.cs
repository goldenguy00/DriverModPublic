using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.Modules.Weapons
{
    public class BanditRevolver : BaseWeapon<BanditRevolver>
    {
        public override string weaponName => "Persuader";
        public override string weaponNameToken => "ROB_DRIVER_WEAPON_BANDITREVOLVER_NAME";
        public override string weaponDesc => "A revolver that's been through hell and back.";
        public override string weaponDescToken => "ROB_DRIVER_WEAPON_BANDITREVOLVER_DESC";

        public override Sprite icon => Assets.mainAssetBundle.LoadAsset<Sprite>("texBanditRevolverWeaponIcon");
        public override DriverWeaponTier dropTier => DriverWeaponTier.Unique; 
        public override DriverWeaponDef.AnimationSet animationSet => DriverWeaponDef.AnimationSet.Default;
        public override DriverWeaponDef.BuffType buffType => DriverWeaponDef.BuffType.Damage;
        public override int shotCount => 6;

        public override Mesh mesh => Modules.Assets.LoadMesh("meshRevolver");
        public override Material material => Addressables.LoadAssetAsync<Material>("RoR2/Base/Bandit2/matBandit2Revolver.mat").WaitForCompletion();
        public override GameObject crosshairPrefab => Modules.Assets.revolverCrosshairPrefab;

        public override string equipAnimationString => "EquipPistol";
        public override float dropChance => 100f;
        public override string uniqueDropBodyName => "Bandit2";

        public override SkillDef primarySkillDef => Modules.Skills.revolverPrimarySkillDef;

        public override SkillDef secondarySkillDef => Modules.Skills.CreateAndAddSkillDef(new SkillDefInfo
        {
            skillName = "ROB_DRIVER_BODY_SECONDARY_BANDITREVOLVER_NAME",
            skillNameToken = "ROB_DRIVER_BODY_SECONDARY_BANDITREVOLVER_NAME",
            skillDescriptionToken = "ROB_DRIVER_BODY_SECONDARY_BANDITREVOLVER_DESCRIPTION",
            skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texGoldenGunSecondaryIcon"),
            activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Revolver.AimLightsOutReset)),
            activationStateMachineName = "Weapon",
            baseMaxStock = 1,
            baseRechargeInterval = 6f,
            beginSkillCooldownOnSkillEnd = true,
            canceledFromSprinting = false,
            forceSprintDuringState = false,
            fullRestockOnAssign = true,
            interruptPriority = EntityStates.InterruptPriority.Skill,
            resetCooldownTimerOnUse = true,
            isCombatSkill = true,
            mustKeyPress = true,
            cancelSprintingOnActivation = true,
            rechargeStock = 1,
            requiredStock = 1,
            stockToConsume = 1,
        });
    }
}