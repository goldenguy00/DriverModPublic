using UnityEngine;

namespace RobDriver.Modules.Weapons
{
    public class FalsePistol : BundledWeapon<FalsePistol>
    {
        public override void LoadWeaponFromBundle()
        {
            this.weaponDef = Assets.mainAssetBundle.LoadAsset<DriverWeaponDef>("FalsePistol");

            this.weaponDef.primarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.FalsePistol.Shoot)),
                "Weapon",
                "ROB_DRIVER_BODY_PRIMARY_FALSE_PISTOL_NAME",
                "ROB_DRIVER_BODY_PRIMARY_FALSE_PISTOL_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolIcon"), false);

            this.weaponDef.secondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = "ROB_DRIVER_BODY_SECONDARY_FALSE_PISTOL_NAME",
                skillNameToken = "ROB_DRIVER_BODY_SECONDARY_FALSE_PISTOL_NAME",
                skillDescriptionToken = "ROB_DRIVER_BODY_SECONDARY_FALSE_PISTOL_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.FalsePistol.SteadyAim)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 3,
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
                requiredStock = 0,
                stockToConsume = 0,
                autoHandleLuminousShot = false
            });
        }
    }
}
