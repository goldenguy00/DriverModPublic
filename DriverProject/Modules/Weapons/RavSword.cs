using RobDriver.SkillStates.Driver.RavSword;
using RoR2.Skills;
using UnityEngine;

namespace RobDriver.Modules.Weapons
{
    public class RavSword : BaseWeapon<RavSword>
    {
        public override string weaponName => "Fury";
        public override string weaponNameToken => "ROB_DRIVER_WEAPON_RAV_SWORD_NAME";
        public override string weaponDesc => "Jump and Slash your way through enemies.";
        public override string weaponDescToken => "ROB_DRIVER_WEAPON_RAV_SWORD_DESC";

        public override Sprite icon => Assets.mainAssetBundle.LoadAsset<Sprite>("texSpinSlashIcon");
        public override DriverWeaponTier dropTier => DriverWeaponTier.Void;
        public override DriverWeaponDef.AnimationSet animationSet => DriverWeaponDef.AnimationSet.BigMelee;
        public override DriverWeaponDef.BuffType buffType => DriverWeaponDef.BuffType.Damage;
        public override int shotCount => 64;

        public override Mesh mesh => Config.enableRevengence.Value ? Assets.LoadMesh("meshKatana") : Modules.Assets.LoadMesh("meshRavagerSword");
        public override Material material => Config.enableRevengence.Value ? Assets.nemKatanaMat : Modules.Assets.LoadMaterial("matRavagerSword");
        public override GameObject crosshairPrefab => Modules.Assets.needlerCrosshairPrefab;
        public override GameObject pickupPrefabOverride => Assets.voidPickupModel;
        public override Color? colorOverride => Helpers.voidItemColor;

        public override float dropChance => 100f;
        public override string uniqueDropBodyName => "RobRavagerBody";

        public override SkillDef primarySkillDef => Modules.Skills.CreateAndAddPrimarySkillDef(
            new EntityStates.SerializableEntityStateType(typeof(SlashCombo)),
            "Weapon",
            "ROB_DRIVER_BODY_PRIMARY_RAV_SLASHCOMBO_NAME",
            "ROB_DRIVER_BODY_PRIMARY_RAV_SLASHCOMBO_DESCRIPTION",
            Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon"),
            true);

        public override SkillDef secondarySkillDef => Modules.Skills.CreateAndAddSkillDef(new SkillDefInfo
        {
            skillName = "ROB_DRIVER_BODY_SECONDARY_RAV_PUNCH_NAME",
            skillNameToken = "ROB_DRIVER_BODY_SECONDARY_RAV_PUNCH_NAME",
            skillDescriptionToken = "ROB_DRIVER_BODY_SECONDARY_RAV_PUNCH_DESCRIPTION",
            skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPunchIcon"),
            activationState = new EntityStates.SerializableEntityStateType(typeof(DashPunch)),
            activationStateMachineName = "Weapon",
            baseMaxStock = 1,
            baseRechargeInterval = 10f,
            beginSkillCooldownOnSkillEnd = false,
            canceledFromSprinting = false,
            forceSprintDuringState = false,
            fullRestockOnAssign = true,
            interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
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