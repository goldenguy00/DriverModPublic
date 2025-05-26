using EntityStates;
using R2API;
using RobDriver.Modules.Components;
using RoR2;
using RoR2.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RobDriver.Modules
{
    internal static class Skills
    {
        public struct SkillDefPair(SkillDef skillDef, UnlockableDef unlockableDef = null)
        {
            public SkillDef skillDef = skillDef;
            public UnlockableDef unlockableDef = unlockableDef;
        }

        internal static List<SkillFamily> skillFamilies = [];
        internal static List<SkillDef> skillDefs = [];

        #region Skill Overrides
        internal static SkillDef pistolPrimarySkillDef;
        internal static SkillDef pistolSecondarySkillDef;

        internal static SkillDef lunarPistolPrimarySkillDef;
        internal static SkillDef lunarPistolSecondarySkillDef;

        internal static SkillDef voidPistolPrimarySkillDef;
        internal static SkillDef voidPistolSecondarySkillDef;

        internal static SkillDef goldenGunPrimarySkillDef;
        internal static SkillDef goldenGunSecondarySkillDef;

        internal static SkillDef pyriteGunPrimarySkillDef;
        internal static SkillDef pyriteGunSecondarySkillDef;

        internal static SkillDef revolverPrimarySkillDef;

        internal static SkillDef shotgunPrimarySkillDef;

        internal static SkillDef riotShotgunPrimarySkillDef;

        internal static SkillDef slugShotgunPrimarySkillDef;

        internal static SkillDef machineGunPrimarySkillDef;
        internal static SkillDef machineGunSecondarySkillDef;

        internal static SkillDef heavyMachineGunPrimarySkillDef;
        internal static SkillDef heavyMachineGunSecondarySkillDef;

        internal static SkillDef bazookaPrimarySkillDef;

        internal static SkillDef rocketLauncherPrimarySkillDef;
        internal static SkillDef rocketLauncherSecondarySkillDef;

        internal static SkillDef rocketLauncherAltPrimarySkillDef;
        internal static SkillDef rocketLauncherAltSecondarySkillDef;

        internal static SkillDef armCannonPrimarySkillDef;

        internal static SkillDef plasmaCannonPrimarySkillDef;
        internal static SkillDef plasmaCannonSecondarySkillDef;

        internal static SkillDef sniperPrimarySkillDef;
        internal static SkillDef sniperSecondarySkillDef;

        internal static SkillDef beetleShieldPrimarySkillDef;
        internal static SkillDef beetleShieldSecondarySkillDef;

        internal static SkillDef behemothPrimarySkillDef;
        internal static SkillDef behemothSecondarySkillDef;

        internal static SkillDef grenadeLauncherPrimarySkillDef;

        internal static SkillDef badassShotgunPrimarySkillDef;

        internal static SkillDef lunarRiflePrimarySkillDef;

        internal static SkillDef lunarHammerPrimarySkillDef;
        internal static SkillDef lunarHammerSecondarySkillDef;

        internal static SkillDef nemmandoGunPrimarySkillDef;
        internal static SkillDef nemmandoGunSecondarySkillDef;

        internal static SkillDef nemmercGunPrimarySkillDef;

        internal static SkillDef golemGunPrimarySkillDef;

        internal static SkillDef pistolReloadSkillDef;
        internal static SkillDef bashSkillDef;
        internal static SkillDef confirmSkillDef;
        internal static SkillDef cancelSkillDef;

        internal static SkillDef slideSkillDef;
        internal static SkillDef dashSkillDef;
        internal static SkillDef skateboardSkillDef;
        internal static SkillDef skateCancelSkillDef;

        internal static SkillDef stunGrenadeSkillDef;
        internal static SkillDef supplyDropSkillDef;
        internal static SkillDef supplyDropLegacySkillDef;
        internal static SkillDef knifeSkillDef;
        internal static SkillDef healSkillDef;
        internal static SkillDef syringeSkillDef;
        internal static SkillDef syringeLegacySkillDef;
        internal static SkillDef coinSkillDef;

        internal static SkillDef scepterGrenadeSkillDef;
        internal static SkillDef scepterSupplyDropSkillDef;
        internal static SkillDef scepterSupplyDropLegacySkillDef;
        internal static SkillDef scepterSyringeSkillDef;
        internal static SkillDef scepterSyringeLegacySkillDef;
        internal static SkillDef scepterKnifeSkillDef;
        internal static SkillDef scepterCoinSkillDef;

        public static void SetWeaponSkill(this GenericSkill skill, SkillDef skillDef, GenericSkill.SkillOverridePriority priority = GenericSkill.SkillOverridePriority.Replacement) =>
            skill.SetSkillOverride(skill, skillDef, priority);

        public static void UnsetWeaponSkill(this GenericSkill skill, SkillDef skillDef, GenericSkill.SkillOverridePriority priority = GenericSkill.SkillOverridePriority.Replacement) =>
            skill.UnsetSkillOverride(skill, skillDef, priority);

        public static void SetWeaponSkills(this SkillLocator skillLoc, DriverWeaponDef weaponDef, GenericSkill.SkillOverridePriority priority = GenericSkill.SkillOverridePriority.Replacement)
        {
            skillLoc.primary.SetWeaponSkill(weaponDef.primarySkillDef, priority);
            skillLoc.secondary.SetWeaponSkill(weaponDef.secondarySkillDef, priority);
        }

        public static void UnsetWeaponSkills(this SkillLocator skillLoc, DriverWeaponDef weaponDef, GenericSkill.SkillOverridePriority priority = GenericSkill.SkillOverridePriority.Replacement)
        {
            skillLoc.primary.UnsetWeaponSkill(weaponDef.primarySkillDef, priority);
            skillLoc.secondary.UnsetWeaponSkill(weaponDef.secondarySkillDef, priority);
        }
        #endregion

        #region genericskills
        public static void CreateSkillFamilies(GameObject targetPrefab)
        {
            foreach (GenericSkill obj in targetPrefab.GetComponentsInChildren<GenericSkill>())
            {
                UnityEngine.Object.DestroyImmediate(obj);
            }

            var skillLocator = targetPrefab.GetComponent<SkillLocator>();
            var passive = targetPrefab.AddComponent<DriverPassive>();
            var arsenal = targetPrefab.AddComponent<DriverArsenal>();

            passive.passiveSkillSlot = CreateGenericSkillWithSkillFamily(targetPrefab, "Passive", "ROB_DRIVER_PASSIVE_TOKEN");

            if (Config.enableArsenal.Value)
                arsenal.weaponSkillSlot = CreateGenericSkillWithSkillFamily(targetPrefab, "Arsenal", "ROB_DRIVER_ARSENAL_TOKEN");

            skillLocator.primary = CreateGenericSkillWithSkillFamily(targetPrefab, "Primary");
            skillLocator.secondary = CreateGenericSkillWithSkillFamily(targetPrefab, "Secondary");
            skillLocator.utility = CreateGenericSkillWithSkillFamily(targetPrefab, "Utility");
            skillLocator.special = CreateGenericSkillWithSkillFamily(targetPrefab, "Special");
        }

        public static GenericSkill CreateGenericSkillWithSkillFamily(GameObject targetPrefab, string familyName, string nameToken = "", bool hidden = false)
        {
            var skill = targetPrefab.AddComponent<GenericSkill>();
            skill.skillName = familyName;
            skill.hideInCharacterSelect = hidden;
            if (!string.IsNullOrWhiteSpace(nameToken))
                skill.SetLoadoutTitleTokenOverride(nameToken);

            var newFamily = ScriptableObject.CreateInstance<SkillFamily>();
            (newFamily as ScriptableObject).name = targetPrefab.name + familyName + "Family";
            newFamily.variants = [];
            skill._skillFamily = newFamily;

            skillFamilies.Add(newFamily);
            return skill;
        }
        #endregion

        #region skillfamilies

        //everything calls this
        public static void AddSkillToFamily(SkillFamily skillFamily, SkillDefPair skillDefPair)
        {
            Array.Resize(ref skillFamily.variants, skillFamily.variants.Length + 1);

            skillFamily.variants[^1] = new SkillFamily.Variant
            {
                skillDef = skillDefPair.skillDef,
                unlockableDef = skillDefPair.unlockableDef,
                viewableNode = new ViewablesCatalog.Node(skillDefPair.skillDef.skillNameToken, false, null)
            };
        }

        public static void AddSkillsToFamily(SkillFamily skillFamily, IEnumerable<SkillDefPair> skillDefPairs)
        {
            foreach (SkillDefPair skillDefPair in skillDefPairs)
            {
                AddSkillToFamily(skillFamily, skillDefPair);
            }
        }

        public static void AddPrimarySkills(SkillLocator skillLoc, params SkillDef[] skillDefs) => AddSkillsToFamily(skillLoc.primary.skillFamily, skillDefs.Select(skill => new SkillDefPair(skill)));
        public static void AddSecondarySkills(SkillLocator skillLoc, params SkillDef[] skillDefs) => AddSkillsToFamily(skillLoc.secondary.skillFamily, skillDefs.Select(skill => new SkillDefPair(skill)));
        public static void AddUtilitySkills(SkillLocator skillLoc, params SkillDef[] skillDefs) => AddSkillsToFamily(skillLoc.utility.skillFamily, skillDefs.Select(skill => new SkillDefPair(skill)));
        public static void AddSpecialSkills(SkillLocator skillLoc, params SkillDefPair[] skillDefs) => AddSkillsToFamily(skillLoc.special.skillFamily, skillDefs);
        public static void AddPassiveSkills(DriverPassive driverPassive, params SkillDefPair[] skillDefs) => AddSkillsToFamily(driverPassive.passiveSkillSlot.skillFamily, skillDefs);

        /// <summary>
        /// Adds a single weapon to the default weapon skill family
        /// </summary>
        /// <param name="locked">If true, weapon will need to be randomly encountered before they are selectable</param>
        public static void AddWeaponSkillToFamily(SkillDef arsenalSkillDef, UnlockableDef unlockableDef, Sprite icon)
        {
            var driverArsenal = Survivors.Driver.characterPrefab.GetComponent<DriverArsenal>();
            if (driverArsenal?.weaponSkillSlot?.skillFamily == null)
                return;

            if (!arsenalSkillDef)
            {
                Log.Error("!!!!!! Arsenal skill def is null! Weapon will not be selectable from the menu !!!!!!");
                return;
            }

            arsenalSkillDef.icon = icon;
            if (unlockableDef != null)
                unlockableDef.achievementIcon = icon;

            DriverArsenal.passiveSkills.Add(arsenalSkillDef);
            AddSkillToFamily(driverArsenal.weaponSkillSlot.skillFamily, new SkillDefPair
            {
                skillDef = arsenalSkillDef,
                unlockableDef = unlockableDef
            });
        }
        #endregion

        #region skilldefs
        public static SkillDef CreateAndAddSkillDef(SkillDefInfo skillDefInfo)
        {
            var skillDef = ScriptableObject.CreateInstance<SkillDef>();
            skillDef.skillName = skillDefInfo.skillName;
            (skillDef as ScriptableObject).name = skillDefInfo.skillName;
            skillDef.skillNameToken = skillDefInfo.skillNameToken;
            skillDef.skillDescriptionToken = skillDefInfo.skillDescriptionToken;
            skillDef.icon = skillDefInfo.skillIcon;

            skillDef.activationState = skillDefInfo.activationState;
            skillDef.activationStateMachineName = skillDefInfo.activationStateMachineName;
            skillDef.baseMaxStock = skillDefInfo.baseMaxStock;
            skillDef.baseRechargeInterval = skillDefInfo.baseRechargeInterval;
            skillDef.beginSkillCooldownOnSkillEnd = skillDefInfo.beginSkillCooldownOnSkillEnd;
            skillDef.canceledFromSprinting = skillDefInfo.canceledFromSprinting;
            skillDef.forceSprintDuringState = skillDefInfo.forceSprintDuringState;
            skillDef.fullRestockOnAssign = skillDefInfo.fullRestockOnAssign;
            skillDef.interruptPriority = skillDefInfo.interruptPriority;
            skillDef.resetCooldownTimerOnUse = skillDefInfo.resetCooldownTimerOnUse;
            skillDef.isCombatSkill = skillDefInfo.isCombatSkill;
            skillDef.mustKeyPress = skillDefInfo.mustKeyPress;
            skillDef.cancelSprintingOnActivation = skillDefInfo.cancelSprintingOnActivation;
            skillDef.rechargeStock = skillDefInfo.rechargeStock;
            skillDef.requiredStock = skillDefInfo.requiredStock;
            skillDef.stockToConsume = skillDefInfo.stockToConsume;
            skillDef.autoHandleLuminousShot = skillDefInfo.autoHandleLuminousShot;

            skillDef.keywordTokens = skillDefInfo.keywordTokens;

            skillDefs.Add(skillDef);

            return skillDef;
        }

        public static SkillDef CreateAndAddPrimarySkillDef(SerializableEntityStateType state, string stateMachine, string skillNameToken, string skillDescriptionToken, Sprite skillIcon, bool agile) => 
            CreateAndAddSkillDef(
                new SkillDefInfo(
                    skillName: skillNameToken,
                    skillNameToken: skillNameToken, 
                    skillDescriptionToken: skillDescriptionToken,
                    skillIcon: skillIcon, 
                    activationState: state,
                    activationStateMachineName: stateMachine,
                    agile: agile
                )
            );

        internal static SkillDef CreateAndAddWeaponSkillDef(string name) => CreateAndAddWeaponSkillDef($"ROB_DRIVER_{name.ToUpperInvariant()}_NAME", $"ROB_DRIVER_{name.ToUpperInvariant()}_DESC");

        public static SkillDef CreateAndAddWeaponSkillDef(string nameToken, string descriptionToken) =>
            CreateAndAddSkillDef(
                new SkillDefInfo(
                    skillName: nameToken,
                    skillNameToken: nameToken,
                    skillDescriptionToken: descriptionToken,
                    skillIcon: null,
                    activationState: new SerializableEntityStateType(typeof(EntityStates.Idle)),
                    activationStateMachineName: "",
                    interruptPriority: InterruptPriority.Any,
                    isCombatSkill: false,
                    baseRechargeInterval: 0
                )
            );

        #endregion skilldefs
    }
}

    /// <summary>
    /// class for easily creating skilldefs with default values, and with a field for UnlockableDef
    /// </summary>
internal class SkillDefInfo
{
    public string skillName;
    public string skillNameToken;
    public string skillDescriptionToken;
    public string[] keywordTokens = [];
    public Sprite skillIcon;

    public SerializableEntityStateType activationState;
    public InterruptPriority interruptPriority;
    public string activationStateMachineName;

    public float baseRechargeInterval;

    public int baseMaxStock = 1;
    public int rechargeStock = 1;
    public int requiredStock = 1;
    public int stockToConsume = 1;

    public bool isCombatSkill = true;
    public bool canceledFromSprinting;
    public bool forceSprintDuringState;
    public bool cancelSprintingOnActivation = true;

    public bool beginSkillCooldownOnSkillEnd;
    public bool fullRestockOnAssign = true;
    public bool resetCooldownTimerOnUse;
    public bool mustKeyPress;
    public bool autoHandleLuminousShot = true;

    #region building
    public SkillDefInfo() { }

    public SkillDefInfo(string skillName,
                        string skillNameToken,
                        string skillDescriptionToken,
                        Sprite skillIcon,

                        SerializableEntityStateType activationState,
                        string activationStateMachineName,
                        InterruptPriority interruptPriority,
                        bool isCombatSkill,
                        float baseRechargeInterval) 
    {
        this.skillName = skillName;
        this.skillNameToken = skillNameToken;
        this.skillDescriptionToken = skillDescriptionToken;
        this.skillIcon = skillIcon;
        this.activationState = activationState;
        this.activationStateMachineName = activationStateMachineName;
        this.interruptPriority = interruptPriority;
        this.isCombatSkill = isCombatSkill;
        this.baseRechargeInterval = baseRechargeInterval;
    }
    /// <summary>
    /// Creates a skilldef for a typical primary.
    /// <para>combat skill, cooldown: 0, required stock: 0, InterruptPriority: Any</para>
    /// </summary>
    public SkillDefInfo(string skillName,
                            string skillNameToken,
                            string skillDescriptionToken,
                            Sprite skillIcon,

                            SerializableEntityStateType activationState,
                            string activationStateMachineName = "Weapon",
                            bool agile = false) 
    {
        this.skillName = skillName;
        this.skillNameToken = skillNameToken;
        this.skillDescriptionToken = skillDescriptionToken;
        this.skillIcon = skillIcon;

        this.activationState = activationState;
        this.activationStateMachineName = activationStateMachineName;

        this.interruptPriority = InterruptPriority.Any;
        this.isCombatSkill = true;
        this.baseRechargeInterval = 0;

        this.requiredStock = 0;
        this.stockToConsume = 0;

        this.cancelSprintingOnActivation = !agile;

        if (agile) this.keywordTokens = ["KEYWORD_AGILE"];
    }
    #endregion construction complete
}