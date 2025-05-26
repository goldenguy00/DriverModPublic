using RobDriver.SkillStates.Driver.ArtiGauntlet;
using RoR2.Skills;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.Modules.Weapons
{
    public class ArtiGauntlet : BaseWeapon<ArtiGauntlet>
    {
        public override string weaponName => "Nano-Gauntlet";
        public override string weaponNameToken => "ROB_DRIVER_WEAPON_ARTI_GAUNTLET_NAME";
        public override string weaponDesc => "Shoot high damage fireballs that inflict burn.";
        public override string weaponDescToken => "ROB_DRIVER_WEAPON_ARTI_GAUNTLET_DESC";

        public override Sprite icon => Assets.mainAssetBundle.LoadAsset<Sprite>("texArtiGauntletWeaponIcon");
        public override DriverWeaponTier dropTier => DriverWeaponTier.Unique;
        public override DriverWeaponDef.AnimationSet animationSet => DriverWeaponDef.AnimationSet.Default;
        public override DriverWeaponDef.BuffType buffType => DriverWeaponDef.BuffType.Damage;
        public override int shotCount => 48;

        public override Mesh mesh => Modules.Assets.LoadMesh("meshArtiGauntlet");
        public override Material material => Addressables.LoadAssetAsync<Material>("RoR2/Base/Mage/matMage.mat").WaitForCompletion();
        public override GameObject crosshairPrefab => Modules.Assets.needlerCrosshairPrefab;

        public override string equipAnimationString => "Recharge";
        public override string reloadAnimationString => "Recharge";
        public override string uniqueDropBodyName => "Mage";
        public override float dropChance => 100f;
        public override bool disableHolster => true;

        public override SkillDef primarySkillDef => Modules.Skills.CreateAndAddPrimarySkillDef(
            new EntityStates.SerializableEntityStateType(typeof(Shoot)),
            "Weapon",
            "ROB_DRIVER_BODY_PRIMARY_ARTI_GAUNTLET_NAME",
            "ROB_DRIVER_BODY_PRIMARY_ARTI_GAUNTLET_DESCRIPTION",
            Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
            false);

        public override SkillDef secondarySkillDef => Modules.Skills.CreateAndAddSkillDef(new SkillDefInfo
        {
            skillName = "ROB_DRIVER_BODY_SECONDARY_ARTI_GAUNTLET_NAME",
            skillNameToken = "ROB_DRIVER_BODY_SECONDARY_ARTI_GAUNTLET_NAME",
            skillDescriptionToken = "ROB_DRIVER_BODY_SECONDARY_ARTI_GAUNTLET_DESCRIPTION",
            skillIcon = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
            activationState = new EntityStates.SerializableEntityStateType(typeof(ChargeBomb)),
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