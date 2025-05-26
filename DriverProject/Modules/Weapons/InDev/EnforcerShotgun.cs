using UnityEngine;

namespace RobDriver.Modules.Weapons
{
    public class EnforcerShotgun : BundledWeapon<EnforcerShotgun>
    {
        public override void LoadWeaponFromBundle()
        {
            this.weaponDef = Assets.mainAssetBundle.LoadAsset<DriverWeaponDef>("EnforcerShotgun");

            this.weaponDef.primarySkillDef = Skills.riotShotgunPrimarySkillDef;
            this.weaponDef.secondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = "ROB_DRIVER_BODY_SECONDARY_TEAR_GAS_NAME",
                skillNameToken = "ROB_DRIVER_BODY_SECONDARY_TEAR_GAS_NAME",
                skillDescriptionToken = "ROB_DRIVER_BODY_SECONDARY_TEAR_GAS_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texHeavyMachineGunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RiotShotgun.TearGas)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
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
                requiredStock = 0,
                stockToConsume = 0,
                autoHandleLuminousShot = false
            });
        }
    }
}
