using RoR2.Skills;
using UnityEngine;

namespace RobDriver.Modules.Weapons
{
    public class RavSword : BaseWeapon<RavSword>
    {
        public override string weaponNameToken => "RAV_SWORD";
        public override string weaponName => "Fury";
        public override string weaponDesc => "Jump and Slash your way through enemies.";
        public override string iconName => "texSpinSlashIcon";
        public override GameObject crosshairPrefab => Modules.Assets.needlerCrosshairPrefab;
        public override DriverWeaponTier tier => DriverWeaponTier.Unique;
        public override int shotCount => 64;
        public override Mesh mesh => Config.enableRevengence.Value ? Assets.nemKatanaMesh : Modules.Assets.LoadMesh("meshRavagerSword");
        public override Material material => Config.enableRevengence.Value ? Assets.nemKatanaMat : Modules.Assets.CreateMaterial("matRavagerSword");
        public override DriverWeaponDef.AnimationSet animationSet => DriverWeaponDef.AnimationSet.BigMelee;
        public override DriverWeaponDef.BuffType buffType => DriverWeaponDef.BuffType.Damage;
        public override string calloutSoundString => "sfx_driver_callout_generic";
        public override string configIdentifier => "Ravagers Sword";
        public override float dropChance => 100f;
        public override bool addToPool => false;
        public override string uniqueDropBodyName => "RobRavagerBody";

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
            new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Compat.SlashCombo)),
            "Weapon",
            DriverPlugin.developerPrefix + "_DRIVER_BODY_PRIMARY_SLASHCOMBO_NAME",
            DriverPlugin.developerPrefix + "_DRIVER_BODY_PRIMARY_SLASHCOMBO_DESCRIPTION",
            Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSlashIcon"),
            true);

        public override SkillDef secondarySkillDef => Modules.Skills.CreateSkillDef(new SkillDefInfo
        {
            skillName = DriverPlugin.developerPrefix + "_RAVAGER_BODY_SPECIAL_PUNCH_NAME",
            skillNameToken = DriverPlugin.developerPrefix + "_RAVAGER_BODY_SPECIAL_PUNCH_NAME",
            skillDescriptionToken = DriverPlugin.developerPrefix + "_RAVAGER_BODY_SPECIAL_PUNCH_DESCRIPTION",
            skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texPunchIcon"),
            activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Compat.DashPunch)),
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

        public override void Init()
        {
            CreateLang();
            CreateWeapon();
        }
    }
}