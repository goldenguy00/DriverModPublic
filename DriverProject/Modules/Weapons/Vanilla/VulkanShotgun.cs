using R2API;
using RobDriver.Modules.Components.UI;
using RoR2.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.Modules.Weapons
{
    public class VulkanShotgun : BundledWeapon<VulkanShotgun>
    {
        public override void LoadWeaponFromBundle()
        {
            this.weaponDef = Assets.mainAssetBundle.LoadAsset<DriverWeaponDef>("VulkanShotgun");
            this.weaponDef.unlockableDef ??= Unlockables.CreateAndAddWeaponUnlockableDef("VULKAN_SHOTGUN");

            this.CreateCrosshair();
            this.CreateSkills();
        }

        private void CreateCrosshair()
        {
            this.weaponDef.crosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainCrosshair.prefab").WaitForCompletion().InstantiateClone("DriverCaptainCrosshair", false);
            var ctrl = this.weaponDef.crosshairPrefab.GetComponent<CrosshairController>();
            if (Config.dynamicCrosshair.Value)
                this.weaponDef.crosshairPrefab.AddComponent<DynamicCrosshair>();

            ctrl.maxSpreadAlpha = 0f;
            ctrl.minSpreadAlpha = 0.75f;
            ctrl.maxSpreadAngle = 3f;
            this.weaponDef.crosshairPrefab.transform.localScale = Vector3.one * 0.75f;
        }

        private void CreateSkills()
        {
            this.weaponDef.primarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.CaptainGun.ChargeShotgun)),
                "Weapon",
                "ROB_DRIVER_BODY_PRIMARY_VULKAN_SHOTGUN_NAME",
                "ROB_DRIVER_BODY_PRIMARY_VULKAN_SHOTGUN_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
                false);

            this.weaponDef.secondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = "ROB_DRIVER_BODY_SECONDARY_VULKAN_SHOTGUN_NAME",
                skillNameToken = "ROB_DRIVER_BODY_SECONDARY_VULKAN_SHOTGUN_NAME",
                skillDescriptionToken = "ROB_DRIVER_BODY_SECONDARY_VULKAN_SHOTGUN_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texGoldenGunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.CaptainGun.AttemptAirstrike)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 11f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            SkillStates.Driver.CaptainGun.AttemptAirstrike.primarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = "ROB_DRIVER_BODY_PRIMARY_VULKAN_AIRSTRIKE_NAME",
                skillNameToken = "ROB_DRIVER_BODY_PRIMARY_VULKAN_AIRSTRIKE_NAME",
                skillDescriptionToken = "ROB_DRIVER_BODY_PRIMARY_VULKAN_AIRSTRIKE_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texGoldenGunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.CaptainGun.CallAirstrike)),
                activationStateMachineName = "AltWeapon",
                baseMaxStock = 3,
                baseRechargeInterval = 11f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = true,
                cancelSprintingOnActivation = true,
                rechargeStock = 3,
                requiredStock = 1,
                stockToConsume = 1,
            });
        }

        protected override void CreateLang()
        {
            base.CreateLang();
            R2API.LanguageAPI.Add("ROB_DRIVER_BODY_PRIMARY_VULKAN_SHOTGUN_NAME", "Vulcan Shotgun");
            R2API.LanguageAPI.Add("ROB_DRIVER_BODY_PRIMARY_VULKAN_SHOTGUN_DESCRIPTION", $"Fire a blast of pellets that deal " +
                $"<style=cIsDamage>{SkillStates.Driver.CaptainGun.Shoot._bulletCount}x{100f * SkillStates.Driver.CaptainGun.Shoot._damageCoefficient}% damage</style>." +
                $" Charging the attack narrows the <style=cIsUtility>spread</style>.");

            R2API.LanguageAPI.Add("ROB_DRIVER_BODY_SECONDARY_VULKAN_SHOTGUN_NAME", "Orbital Strike");
            R2API.LanguageAPI.Add("ROB_DRIVER_BODY_SECONDARY_VULKAN_SHOTGUN_DESCRIPTION", $"Call 3 orbital strikes that each deal " +
                $"<style=cIsDamage>{100f * SkillStates.Driver.CaptainGun.CallAirstrike._damageCoefficient}% damage</style>.");

            R2API.LanguageAPI.Add("ROB_DRIVER_BODY_PRIMARY_VULKAN_AIRSTRIKE_NAME", "Fire Orbital Strike");
            R2API.LanguageAPI.Add("ROB_DRIVER_BODY_PRIMARY_VULKAN_AIRSTRIKE_DESCRIPTION", $"Call an orbital strike that deals " +
                $"<style=cIsDamage>{100f * SkillStates.Driver.CaptainGun.CallAirstrike._damageCoefficient}% damage</style>.");
        }
    }
}
