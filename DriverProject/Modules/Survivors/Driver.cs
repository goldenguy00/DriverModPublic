using BepInEx.Configuration;
using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using RoR2.CharacterAI;
using UnityEngine.AddressableAssets;
using System.Linq;
using RobDriver.Modules.Components;
using HG;
using RoR2.ContentManagement;
using RobDriver.Modules.Misc;
using System.Runtime.CompilerServices;
using RobDriver.SkillStates.Driver;

namespace RobDriver.Modules.Survivors
{
    internal class Driver
    {
        internal static Driver instance;

        internal static GameObject characterPrefab;
        internal static GameObject displayPrefab;
        internal static GameObject umbraMaster;

        internal static ConfigEntry<bool> forceUnlock;
        internal static ConfigEntry<bool> characterEnabled;

        internal float pityMultiplier = 1f;

        public static Color characterColor = new Color(145f / 255f, 0f, 1f);

        public const string bodyName = "RobDriverBody";
        public const string baseNameToken = "ROB_DRIVER_BODY_NAME";

        internal static BodyIndex bodyIndex => BodyCatalog.FindBodyIndex(bodyName);

        // item display stuffs
        internal static ItemDisplayRuleSet itemDisplayRuleSet;
        internal static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules;

        internal void CreateCharacter()
        {
            instance = this;

            characterEnabled = Config.CharacterEnableConfig("Driver");

            if (characterEnabled.Value)
            {
                forceUnlock = Config.ForceUnlockConfig("Driver");

                characterPrefab = CreateBodyPrefab();
                displayPrefab = Prefabs.CreateDisplayPrefab("DriverDisplay", characterPrefab);
                umbraMaster = CreateMaster(characterPrefab, "RobDriverMonsterMaster");
                Prefabs.RegisterNewSurvivor(characterPrefab, displayPrefab, "DRIVER", forceUnlock.Value ? null : Unlockables.characterUnlockableDef);

                DriverWeaponCatalog.InitWeaponDefs();
                DriverBulletCatalog.InitBulletDefs();
                DriverHooks.Init();

                ContentManager.onContentPacksAssigned += SetItemDisplays;
            }
        }

        private static GameObject CreateBodyPrefab()
        {
            #region Body
            GameObject newPrefab = Prefabs.CreatePrefab(bodyName, "mdlDriver", new BodyInfo
            {
                armor = Config.baseArmor.Value,
                armorGrowth = Config.armorGrowth.Value,
                bodyName = bodyName,
                bodyNameToken = baseNameToken,
                bodyColor = characterColor,
                characterPortrait = Assets.LoadCharacterIcon("Driver"),
                crosshair = Assets.LoadCrosshair("Standard"),
                damage = Config.baseDamage.Value,
                healthGrowth = Config.healthGrowth.Value,
                healthRegen = Config.baseRegen.Value,
                jumpCount = 1,
                maxHealth = Config.baseHealth.Value,
                subtitleNameToken = DriverPlugin.developerPrefix + "_DRIVER_BODY_SUBTITLE",
                podPrefab = RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/SurvivorPod"),
                moveSpeed = Config.baseMovementSpeed.Value,
                acceleration = 60f,
                jumpPower = 15f,
                attackSpeed = 1f,
                crit = Config.baseCrit.Value
            });

            ChildLocator childLocator = newPrefab.GetComponentInChildren<ChildLocator>();
            childLocator.gameObject.AddComponent<DriverAnimationEvents>();

            //CharacterBody body = newPrefab.GetComponent<CharacterBody>();
            //body.preferredInitialStateType = new EntityStates.SerializableEntityStateType(typeof(SpawnState));
            //body.bodyFlags = CharacterBody.BodyFlags.IgnoreFallDamage;
            //body.bodyFlags |= CharacterBody.BodyFlags.SprintAnyDirection;
            //body.sprintingSpeedMultiplier = 1.75f;

            //newPrefab.AddComponent<NinjaMod.Components.NinjaController>();

            //SfxLocator sfx = newPrefab.GetComponent<SfxLocator>();
            //sfx.barkSound = "";
            //sfx.landingSound = "";
            //sfx.deathSound = "";
            //sfx.fallDamageSound = "";

            //FootstepHandler footstep = newPrefab.GetComponentInChildren<FootstepHandler>();
            //footstep.footstepDustPrefab = Resources.Load<GameObject>("Prefabs/GenericHugeFootstepDust");
            //footstep.baseFootstepString = "Play_moonBrother_step";
            //footstep.sprintFootstepOverrideString = "Play_moonBrother_sprint";

            //KinematicCharacterMotor characterController = newPrefab.GetComponent<KinematicCharacterMotor>();
            //characterController.CapsuleRadius = 4f;
            //characterController.CapsuleHeight = 9f;

            //CharacterDirection direction = newPrefab.GetComponent<CharacterDirection>();
            //direction.turnSpeed = 135f;

            //Interactor interactor = newPrefab.GetComponent<Interactor>();
            //interactor.maxInteractionDistance = 8f;

            //newPrefab.GetComponent<CharacterDirection>().turnSpeed = 720f;
             

            newPrefab.GetComponent<CameraTargetParams>().cameraParams = CameraParams.CreateCameraParamsWithData(DriverCameraParams.DEFAULT);

            foreach (EntityStateMachine i in newPrefab.GetComponents<EntityStateMachine>())
            {
                if (i.customName == "Body")
                    i.mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.MainState));

                if (i.customName == "Weapon")
                    i.mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.WeaponMainState));
            }

            EntityStateMachine passiveController = newPrefab.AddComponent<EntityStateMachine>();
            passiveController.customName = "RavPassive";
            passiveController.initialStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RavSword.WallJump));
            passiveController.mainStateType = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RavSword.WallJump));

            EntityStateMachine stateMachine = newPrefab.AddComponent<EntityStateMachine>();
            stateMachine.customName = "AltWeapon";
            stateMachine.initialStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));
            stateMachine.mainStateType = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle));

            //var state = isPlayer ? typeof(EntityStates.SpawnTeleporterState) : typeof(SpawnState);
            //newPrefab.GetComponent<EntityStateMachine>().initialStateType = new EntityStates.SerializableEntityStateType(state);

            // schizophrenia
            newPrefab.GetComponent<CharacterDeathBehavior>().deathState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.FuckMyAss));

            newPrefab.AddComponent<DriverController>();
            #endregion

            #region Model
            var customRendererInfo = new CustomRendererInfo[]
            {
                new CustomRendererInfo
                {
                    childName = "Model",
                    material = Assets.mainMat
                },
                new CustomRendererInfo
                {
                    childName = "KnifeModel",
                    material = Config.enableRevengence.Value ? Assets.nemKatanaMat : Assets.knifeMat
                },
                new CustomRendererInfo
                {
                    childName = "ButtonModel",
                    material = Assets.buttonMat
                },
                new CustomRendererInfo
                {
                    childName = "SyringeModel",
                    material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Syringe/matSyringe.mat").WaitForCompletion()
                },
                new CustomRendererInfo
                {
                    childName = "AltWeaponModel",
                    material = Assets.nemKatanaMat
                },
                new CustomRendererInfo
                {
                    childName = "SluggerClothModelL",
                    material = Assets.clothMat
                },
                new CustomRendererInfo
                {
                    childName = "SluggerClothModelR",
                    material = Assets.clothMat
                },
                new CustomRendererInfo
                {
                    childName = "TieModel",
                    material = Assets.tieMat
                },
                new CustomRendererInfo
                {
                    childName = "SkateboardModel",
                    material = Assets.skateboardMat
                },
                new CustomRendererInfo
                {
                    childName = "SkateboardBackModel",
                    material = Assets.skateboardMat
                },
                new CustomRendererInfo
                {
                    childName = "PistolModel",
                    material = Assets.pistolMat
                },
                new CustomRendererInfo
                {
                    childName = "TimbsModelL",
                    material = Assets.timbsMat
                },
                new CustomRendererInfo
                {
                    childName = "TimbsModelR",
                    material = Assets.timbsMat
                }
            };

            CharacterModel characterModel = newPrefab.GetComponent<ModelLocator>().modelTransform.gameObject.AddComponent<CharacterModel>();
            Prefabs.SetupCharacterModel(newPrefab, characterModel, customRendererInfo);

            // hide the extra stuff
            childLocator.FindChildGameObject("PistolModel").SetActive(true);
            childLocator.FindChildGameObject("KnifeModel").SetActive(false);
            childLocator.FindChildGameObject("ButtonModel").SetActive(false);
            childLocator.FindChildGameObject("SyringeModel").SetActive(false);
            childLocator.FindChildGameObject("AltWeaponModel").SetActive(false);

            childLocator.FindChildGameObject("SluggerCloth").SetActive(false);
            childLocator.FindChildGameObject("SluggerClothModelL").SetActive(true);
            childLocator.FindChildGameObject("SluggerClothModelR").SetActive(true);

            childLocator.FindChildGameObject("Tie").SetActive(false);
            childLocator.FindChildGameObject("TieModel").SetActive(true);

            childLocator.FindChildGameObject("SkateboardModel").SetActive(false);
            childLocator.FindChildGameObject("SkateboardBackModel").SetActive(false);

            childLocator.FindChildGameObject("TimbsModelL").SetActive(Config.cursed.Value);
            childLocator.FindChildGameObject("TimbsModelR").SetActive(Config.cursed.Value);
            #endregion

            CreateHitboxes(newPrefab);
            SetupHurtboxes(newPrefab);
            CreateSkills(newPrefab);
            CreateSkins(newPrefab);
            InitializeItemDisplays(newPrefab);

            return newPrefab;
        }

        private static void SetupHurtboxes(GameObject bodyPrefab)
        {
            HurtBoxGroup hurtboxGroup = bodyPrefab.GetComponentInChildren<HurtBoxGroup>();
            List<HurtBox> hurtboxes = [bodyPrefab.GetComponentInChildren<ChildLocator>().FindChild("MainHurtbox").GetComponent<HurtBox>()];

            HealthComponent healthComponent = bodyPrefab.GetComponent<HealthComponent>();

            foreach (Collider i in bodyPrefab.GetComponent<ModelLocator>().modelTransform.GetComponentsInChildren<Collider>())
            {
                if (i.gameObject.name != "MainHurtbox")
                {
                    HurtBox hurtbox = i.gameObject.AddComponent<HurtBox>();
                    hurtbox.gameObject.layer = LayerIndex.entityPrecise.intVal;
                    hurtbox.healthComponent = healthComponent;
                    hurtbox.isBullseye = false;
                    hurtbox.damageModifier = HurtBox.DamageModifier.Normal;
                    hurtbox.hurtBoxGroup = hurtboxGroup;

                    hurtboxes.Add(hurtbox);
                }
            }

            hurtboxGroup.hurtBoxes = hurtboxes.ToArray();
        }

        private static GameObject CreateMaster(GameObject bodyPrefab, string masterName)
        {
            GameObject newMaster = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/CharacterMasters/LemurianMaster"), masterName, true);
            newMaster.GetComponent<CharacterMaster>().bodyPrefab = bodyPrefab;

            #region AI
            foreach (AISkillDriver ai in newMaster.GetComponentsInChildren<AISkillDriver>())
            {
                DriverPlugin.DestroyImmediate(ai);
            }

            newMaster.GetComponent<BaseAI>().fullVision = true;

            AISkillDriver shootClose = newMaster.AddComponent<AISkillDriver>();
            shootClose.customName = "ShootClose";
            shootClose.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            shootClose.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            shootClose.activationRequiresAimConfirmation = true;
            shootClose.activationRequiresTargetLoS = false;
            shootClose.selectionRequiresTargetLoS = true;
            shootClose.maxDistance = 24f;
            shootClose.minDistance = 0f;
            shootClose.requireSkillReady = true;
            shootClose.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            shootClose.ignoreNodeGraph = true;
            shootClose.moveInputScale = 1f;
            shootClose.driverUpdateTimerOverride = 1f;
            shootClose.buttonPressType = AISkillDriver.ButtonPressType.TapContinuous;
            shootClose.minTargetHealthFraction = Mathf.NegativeInfinity;
            shootClose.maxTargetHealthFraction = Mathf.Infinity;
            shootClose.minUserHealthFraction = 0.5f;
            shootClose.maxUserHealthFraction = 1f;
            shootClose.skillSlot = SkillSlot.Primary;

            AISkillDriver makeDistance = newMaster.AddComponent<AISkillDriver>();
            makeDistance.customName = "MakeDistance";
            makeDistance.movementType = AISkillDriver.MovementType.FleeMoveTarget;
            makeDistance.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            makeDistance.activationRequiresAimConfirmation = false;
            makeDistance.activationRequiresTargetLoS = false;
            makeDistance.selectionRequiresTargetLoS = false;
            makeDistance.maxDistance = 8f;
            makeDistance.minDistance = 0f;
            makeDistance.requireSkillReady = false;
            makeDistance.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            makeDistance.ignoreNodeGraph = true;
            makeDistance.moveInputScale = 1f;
            makeDistance.driverUpdateTimerOverride = -1f;
            makeDistance.buttonPressType = AISkillDriver.ButtonPressType.TapContinuous;
            makeDistance.minTargetHealthFraction = Mathf.NegativeInfinity;
            makeDistance.maxTargetHealthFraction = Mathf.Infinity;
            makeDistance.minUserHealthFraction = Mathf.NegativeInfinity;
            makeDistance.maxUserHealthFraction = 0.5f;
            makeDistance.skillSlot = SkillSlot.Primary;

            AISkillDriver steadyAim = newMaster.AddComponent<AISkillDriver>();
            steadyAim.customName = "SteadyAim";
            steadyAim.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            steadyAim.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            steadyAim.activationRequiresAimConfirmation = true;
            steadyAim.activationRequiresTargetLoS = false;
            steadyAim.selectionRequiresTargetLoS = true;
            steadyAim.maxDistance = 64f;
            steadyAim.minDistance = 8f;
            steadyAim.requireSkillReady = true;
            steadyAim.aimType = AISkillDriver.AimType.AtCurrentEnemy;
            steadyAim.ignoreNodeGraph = true;
            steadyAim.moveInputScale = 0.4f;
            steadyAim.driverUpdateTimerOverride = 0.5f;
            steadyAim.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            steadyAim.minTargetHealthFraction = Mathf.NegativeInfinity;
            steadyAim.maxTargetHealthFraction = Mathf.Infinity;
            steadyAim.minUserHealthFraction = Mathf.NegativeInfinity;
            steadyAim.maxUserHealthFraction = Mathf.Infinity;
            steadyAim.skillSlot = SkillSlot.Secondary;

            AISkillDriver steadyAimShoot = newMaster.AddComponent<AISkillDriver>();
            steadyAimShoot.customName = "SteadyAimShoot";
            steadyAimShoot.movementType = AISkillDriver.MovementType.StrafeMovetarget;
            steadyAimShoot.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            steadyAimShoot.activationRequiresAimConfirmation = true;
            steadyAimShoot.activationRequiresTargetLoS = true;
            steadyAimShoot.maxDistance = 64f;
            steadyAimShoot.minDistance = 8f;
            steadyAimShoot.aimType = AISkillDriver.AimType.AtMoveTarget;
            steadyAimShoot.ignoreNodeGraph = false;
            steadyAimShoot.moveInputScale = 1f;
            steadyAimShoot.driverUpdateTimerOverride = 1f;
            steadyAimShoot.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            steadyAimShoot.minTargetHealthFraction = Mathf.NegativeInfinity;
            steadyAimShoot.maxTargetHealthFraction = Mathf.Infinity;
            steadyAimShoot.minUserHealthFraction = Mathf.NegativeInfinity;
            steadyAimShoot.maxUserHealthFraction = Mathf.Infinity;
            steadyAimShoot.skillSlot = SkillSlot.Primary;
            steadyAim.nextHighPriorityOverride = steadyAimShoot;

            AISkillDriver followDriver = newMaster.AddComponent<AISkillDriver>();
            followDriver.customName = "Chase";
            followDriver.movementType = AISkillDriver.MovementType.ChaseMoveTarget;
            followDriver.moveTargetType = AISkillDriver.TargetType.CurrentEnemy;
            followDriver.activationRequiresAimConfirmation = false;
            followDriver.activationRequiresTargetLoS = false;
            followDriver.maxDistance = Mathf.Infinity;
            followDriver.minDistance = 0f;
            followDriver.aimType = AISkillDriver.AimType.AtMoveTarget;
            followDriver.ignoreNodeGraph = false;
            followDriver.moveInputScale = 1f;
            followDriver.driverUpdateTimerOverride = -1f;
            followDriver.buttonPressType = AISkillDriver.ButtonPressType.Hold;
            followDriver.minTargetHealthFraction = Mathf.NegativeInfinity;
            followDriver.maxTargetHealthFraction = Mathf.Infinity;
            followDriver.minUserHealthFraction = Mathf.NegativeInfinity;
            followDriver.maxUserHealthFraction = Mathf.Infinity;
            followDriver.skillSlot = SkillSlot.Utility;
            followDriver.shouldSprint = true;
            #endregion

            Prefabs.masterPrefabs.Add(newMaster);

            return newMaster;
        }

        private static void CreateHitboxes(GameObject prefab)
        {
            ChildLocator childLocator = prefab.GetComponentInChildren<ChildLocator>();
            GameObject model = childLocator.gameObject;

            Prefabs.SetupHitbox(model, "Hammer",
            [
                childLocator.FindChild("HammerHitbox")
            ]);

            Prefabs.SetupHitbox(model, "Sword",
            [
                childLocator.FindChild("SwordHitboxL"),
                childLocator.FindChild("SwordHitboxR")
            ]);

            Prefabs.SetupHitbox(model, "Knife",
            [
                childLocator.FindChild("KnifeHitbox")
            ]);
        }

        private static void CreateSkills(GameObject prefab)
        {
            Skills.CreateSkillFamilies(prefab);

            var passive = prefab.GetComponent<DriverPassive>();
            var skillLoc = prefab.GetComponent<SkillLocator>();
            skillLoc.passiveSkill.enabled = false;

            string prefix = "ROB_DRIVER_BODY_";

            #region Misc
            Skills.pistolReloadSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "RELOAD_NAME",
                skillNameToken = prefix + "RELOAD_NAME",
                skillDescriptionToken = prefix + "RELOAD_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texConfirmIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Reload)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Skills.confirmSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "CONFIRM_NAME",
                skillNameToken = prefix + "CONFIRM_NAME",
                skillDescriptionToken = prefix + "CONFIRM_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texConfirmIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "fuck",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
            });

            Skills.cancelSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "CANCEL_NAME",
                skillNameToken = prefix + "CANCEL_NAME",
                skillDescriptionToken = prefix + "CANCEL_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texCancelIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "fuck",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 0,
            });
            #endregion Misc

            #region Passive
            DriverPassive.defaultPassive = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "PASSIVE_NAME",
                skillNameToken = prefix + "PASSIVE_NAME",
                skillDescriptionToken = prefix + "PASSIVE_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texPassiveIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            DriverPassive.bulletsPassive = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "PASSIVE3_NAME",
                skillNameToken = prefix + "PASSIVE3_NAME",
                skillDescriptionToken = prefix + "PASSIVE3_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texLeadfootIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            DriverPassive.godslingPassive = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "PASSIVE4_NAME",
                skillNameToken = prefix + "PASSIVE4_NAME",
                skillDescriptionToken = prefix + "PASSIVE4_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texAltPassiveIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            DriverPassive.pistolOnlyPassive = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "PASSIVE2_NAME",
                skillNameToken = prefix + "PASSIVE2_NAME",
                skillDescriptionToken = prefix + "PASSIVE2_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texAltPassiveLegacyIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(EntityStates.Idle)),
                activationStateMachineName = "",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 2,
                stockToConsume = 1
            });

            Skills.AddPassiveSkills(passive,
                new Skills.SkillDefPair(DriverPassive.defaultPassive),
                new Skills.SkillDefPair(DriverPassive.bulletsPassive, Unlockables.pistolPassiveUnlockableDef),
                new Skills.SkillDefPair(DriverPassive.godslingPassive, Unlockables.godslingPassiveUnlockableDef));

            if (Config.cursed.Value)
                Skills.AddPassiveSkills(passive, new Skills.SkillDefPair(DriverPassive.pistolOnlyPassive, Unlockables.pistolPassiveUnlockableDef));
            #endregion

            #region Primary
            Skills.pistolPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shoot)),
                "Weapon",
                prefix + "PRIMARY_PISTOL_NAME",
                prefix + "PRIMARY_PISTOL_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolIcon"), false);

            Skills.beetleShieldPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.BeetleShield.Shoot)),
                "Weapon",
                prefix + "PRIMARY_BEETLESHIELD_NAME",
                prefix + "PRIMARY_BEETLESHIELD_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolIcon"), false);

            Skills.lunarPistolPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.LunarPistol.Shoot)),
                "Weapon",
                prefix + "PRIMARY_LUNAR_PISTOL_NAME",
                prefix + "PRIMARY_LUNAR_PISTOL_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolIcon"), false);

            Skills.voidPistolPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.VoidPistol.Shoot)),
                "Weapon",
                prefix + "PRIMARY_VOID_PISTOL_NAME",
                prefix + "PRIMARY_VOID_PISTOL_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolIcon"), false);

            Skills.pyriteGunPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.PyriteGun.Shoot)),
                "Weapon",
                prefix + "PRIMARY_PYRITE_PISTOL_NAME",
                prefix + "PRIMARY_PYRITE_PISTOL_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texGoldenGunIcon"), false);

            Skills.goldenGunPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.GoldenGun.Shoot)),
                "Weapon",
                prefix + "PRIMARY_GOLDENGUN_NAME",
                prefix + "PRIMARY_GOLDENGUN_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texGoldenGunIcon"), false);

            Skills.revolverPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Revolver.Shoot)),
                "Weapon",
                prefix + "PRIMARY_REVOLVER_NAME",
                prefix + "PRIMARY_REVOLVER_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texGoldenGunIcon"),
                false);

            Skills.shotgunPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Shotgun.Shoot)),
                "Weapon",
                prefix + "PRIMARY_SHOTGUN_NAME",
                prefix + "PRIMARY_SHOTGUN_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunIcon"),
                false);

            Skills.riotShotgunPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RiotShotgun.Shoot)),
                "Weapon",
                prefix + "PRIMARY_RIOT_SHOTGUN_NAME",
                prefix + "PRIMARY_RIOT_SHOTGUN_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texRiotShotgunIcon"),
                false);

            Skills.slugShotgunPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SlugShotgun.Shoot)),
                "Weapon",
                prefix + "PRIMARY_SLUG_SHOTGUN_NAME",
                prefix + "PRIMARY_SLUG_SHOTGUN_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texSlugShotgunIcon"),
                false);

            Skills.machineGunPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.MachineGun.Shoot)),
                "Weapon",
                prefix + "PRIMARY_MACHINEGUN_NAME",
                prefix + "PRIMARY_MACHINEGUN_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMachineGunIcon"),
                false);

            Skills.heavyMachineGunPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.HeavyMachineGun.Shoot)),
                "Weapon",
                prefix + "PRIMARY_HEAVY_MACHINEGUN_NAME",
                prefix + "PRIMARY_HEAVY_MACHINEGUN_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMachineGunIcon"),
                false);

            Skills.bazookaPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Bazooka.Charge)),
                "Weapon",
                prefix + "PRIMARY_BAZOOKA_NAME",
                prefix + "PRIMARY_BAZOOKA_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
                false);

            Skills.rocketLauncherPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RocketLauncher.Shoot)),
                "Weapon",
                prefix + "PRIMARY_ROCKETLAUNCHER_NAME",
                prefix + "PRIMARY_ROCKETLAUNCHER_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
                false);

            Skills.behemothPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RocketLauncher.Shoot)),
                "Weapon",
                prefix + "PRIMARY_ROCKETLAUNCHER_NAME",
                prefix + "PRIMARY_ROCKETLAUNCHER_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
                false);

            Skills.rocketLauncherAltPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RocketLauncher.NerfedShoot)),
                "Weapon",
                prefix + "PRIMARY_ROCKETLAUNCHER_ALT_NAME",
                prefix + "PRIMARY_ROCKETLAUNCHER_ALT_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
                false);

            Skills.grenadeLauncherPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.GrenadeLauncher.Shoot)),
                "Weapon",
                prefix + "PRIMARY_GRENADELAUNCHER_NAME",
                prefix + "PRIMARY_GRENADELAUNCHER_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherIcon"),
                false);

            Skills.armCannonPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.ArmCannon.Shoot)),
                "Weapon",
                prefix + "PRIMARY_ARMCANNON_NAME",
                prefix + "PRIMARY_ARMCANNON_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texArmCannonIcon"),
                false);

            Skills.plasmaCannonPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.PlasmaCannon.Shoot)),
                "Weapon",
                prefix + "PRIMARY_PLASMACANNON_NAME",
                prefix + "PRIMARY_PLASMACANNON_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texPlasmaCannonIcon"),
                false);

            Skills.sniperPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SniperRifle.Shoot)),
                "Weapon",
                prefix + "PRIMARY_SNIPER_NAME",
                prefix + "PRIMARY_SNIPER_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texSlugShotgunIcon"),
                false);

            Skills.badassShotgunPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.BadassShotgun.Shoot)),
                "Weapon",
                prefix + "PRIMARY_BADASS_SHOTGUN_NAME",
                prefix + "PRIMARY_BADASS_SHOTGUN_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunIcon"),
                false);

            Skills.lunarRiflePrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.LunarRifle.Shoot)),
                "Weapon",
                prefix + "PRIMARY_LUNARRIFLE_NAME",
                prefix + "PRIMARY_LUNARRIFLE_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texLunarRifleIcon"),
                false);

            Skills.golemGunPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.GolemGun.ChargeLaser)),
                "Weapon",
                prefix + "PRIMARY_GOLEMGUN_NAME",
                prefix + "PRIMARY_GOLEMGUN_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texGolemGunIcon"),
                false);

            Skills.lunarHammerPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.LunarHammer.SwingCombo)),
                "Weapon",
                prefix + "PRIMARY_LUNARHAMMER_NAME",
                prefix + "PRIMARY_LUNARHAMMER_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texLunarHammerIcon"),
                false);

            Skills.nemmandoGunPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.NemmandoGun.Shoot)),
                "Weapon",
                prefix + "PRIMARY_NEMMANDO_NAME",
                prefix + "PRIMARY_NEMMANDO_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoPrimaryIcon"),
                false);

            Skills.nemmercGunPrimarySkillDef = Skills.CreateAndAddPrimarySkillDef(
                new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.NemmercGun.Shoot)),
                "Weapon",
                prefix + "PRIMARY_NEMMERC_NAME",
                prefix + "PRIMARY_NEMMERC_DESCRIPTION",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmercPrimaryIcon"),
                false);

            Skills.AddPrimarySkills(skillLoc, Skills.pistolPrimarySkillDef);
            #endregion

            #region Secondary
            Skills.pistolSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_PISTOL_NAME",
                skillNameToken = prefix + "SECONDARY_PISTOL_NAME",
                skillDescriptionToken = prefix + "SECONDARY_PISTOL_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SteadyAim)),
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

            Skills.beetleShieldSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_BEETLESHIELD_NAME",
                skillNameToken = prefix + "SECONDARY_BEETLESHIELD_NAME",
                skillDescriptionToken = prefix + "SECONDARY_BEETLESHIELD_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.BeetleShield.SteadyAim)),
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

            Skills.pyriteGunSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_PYRITE_PISTOL_NAME",
                skillNameToken = prefix + "SECONDARY_PYRITE_PISTOL_NAME",
                skillDescriptionToken = prefix + "SECONDARY_PYRITE_PISTOL_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.PyriteGun.SteadyAim)),
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

            Skills.lunarPistolSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_LUNAR_PISTOL_NAME",
                skillNameToken = prefix + "SECONDARY_LUNAR_PISTOL_NAME",
                skillDescriptionToken = prefix + "SECONDARY_LUNAR_PISTOL_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.LunarPistol.SteadyAim)),
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

            Skills.voidPistolSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_VOID_PISTOL_NAME",
                skillNameToken = prefix + "SECONDARY_VOID_PISTOL_NAME",
                skillDescriptionToken = prefix + "SECONDARY_VOID_PISTOL_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.VoidPistol.SteadyAim)),
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

            Skills.goldenGunSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_GOLDENGUN_NAME",
                skillNameToken = prefix + "SECONDARY_GOLDENGUN_NAME",
                skillDescriptionToken = prefix + "SECONDARY_GOLDENGUN_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texGoldenGunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.GoldenGun.AimLightsOut)),
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
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1,
            });

            Skills.bashSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_BASH_NAME",
                skillNameToken = prefix + "SECONDARY_BASH_NAME",
                skillDescriptionToken = prefix + "SECONDARY_BASH_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(Bash)),
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

            Skills.machineGunSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_MACHINEGUN_NAME",
                skillNameToken = prefix + "SECONDARY_MACHINEGUN_NAME",
                skillDescriptionToken = prefix + "SECONDARY_MACHINEGUN_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texZapIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.MachineGun.Zap)),
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

            Skills.heavyMachineGunSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_HEAVY_MACHINEGUN_NAME",
                skillNameToken = prefix + "SECONDARY_HEAVY_MACHINEGUN_NAME",
                skillDescriptionToken = prefix + "SECONDARY_HEAVY_MACHINEGUN_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texHeavyMachineGunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.HeavyMachineGun.ShootGrenade)),
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

            Skills.rocketLauncherSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_ROCKETLAUNCHER_NAME",
                skillNameToken = prefix + "SECONDARY_ROCKETLAUNCHER_NAME",
                skillDescriptionToken = prefix + "SECONDARY_ROCKETLAUNCHER_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RocketLauncher.Barrage)),
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

            Skills.behemothSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_ROCKETLAUNCHER_NAME",
                skillNameToken = prefix + "SECONDARY_ROCKETLAUNCHER_NAME",
                skillDescriptionToken = prefix + "SECONDARY_ROCKETLAUNCHER_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RocketLauncher.Barrage)),
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

            Skills.rocketLauncherAltSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_ROCKETLAUNCHER_ALT_NAME",
                skillNameToken = prefix + "SECONDARY_ROCKETLAUNCHER_ALT_NAME",
                skillDescriptionToken = prefix + "SECONDARY_ROCKETLAUNCHER_ALT_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.RocketLauncher.NerfedBarrage)),
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

            Skills.sniperSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_SNIPER_NAME",
                skillNameToken = prefix + "SECONDARY_SNIPER_NAME",
                skillDescriptionToken = prefix + "SECONDARY_SNIPER_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texPistolSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SniperRifle.Aim)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 8f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
                autoHandleLuminousShot = false,
            });

            Skills.plasmaCannonSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_PLASMACANNON_NAME",
                skillNameToken = prefix + "SECONDARY_PLASMACANNON_NAME",
                skillDescriptionToken = prefix + "SECONDARY_PLASMACANNON_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texRocketLauncherSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.PlasmaCannon.Barrage)),
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

            Skills.lunarHammerSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_LUNARHAMMER_NAME",
                skillNameToken = prefix + "SECONDARY_LUNARHAMMER_NAME",
                skillDescriptionToken = prefix + "SECONDARY_LUNARHAMMER_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texLunarShardIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.LunarHammer.FireShard)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = true,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 0,
                stockToConsume = 0,
            });

            Skills.nemmandoGunSecondarySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SECONDARY_NEMMANDO_NAME",
                skillNameToken = prefix + "SECONDARY_NEMMANDO_NAME",
                skillDescriptionToken = prefix + "SECONDARY_NEMMANDO_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texNemmandoSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.NemmandoGun.Submission)),
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

            Skills.AddSecondarySkills(skillLoc, Skills.pistolSecondarySkillDef);
            #endregion

            #region Utility
            Skills.slideSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "UTILITY_SLIDE_NAME",
                skillNameToken = prefix + "UTILITY_SLIDE_NAME",
                skillDescriptionToken = prefix + "UTILITY_SLIDE_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSlideIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Slide)),
                activationStateMachineName = "Slide",
                baseMaxStock = 1,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.Skill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Skills.skateboardSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "UTILITY_SKATEBOARD_NAME",
                skillNameToken = prefix + "UTILITY_SKATEBOARD_NAME",
                skillDescriptionToken = prefix + "UTILITY_SKATEBOARD_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSkateboardIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Skateboard.Start)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = true,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Skills.skateCancelSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "UTILITY_SKATEBOARD_NAME",
                skillNameToken = prefix + "UTILITY_SKATEBOARD_NAME",
                skillDescriptionToken = prefix + "UTILITY_SKATEBOARD2_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texCancelIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Skateboard.Stop)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 1f,
                beginSkillCooldownOnSkillEnd = true,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.Any,
                resetCooldownTimerOnUse = true,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Skills.dashSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "UTILITY_DASH_NAME",
                skillNameToken = prefix + "UTILITY_DASH_NAME",
                skillDescriptionToken = prefix + "UTILITY_DASH_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texDashIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Dash)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 2,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = true,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Skills.AddUtilitySkills(skillLoc, Skills.slideSkillDef, Skills.dashSkillDef, Skills.skateboardSkillDef);
            #endregion

            #region Special
            Skills.stunGrenadeSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_GRENADE_NAME",
                skillNameToken = prefix + "SPECIAL_GRENADE_NAME",
                skillDescriptionToken = prefix + "SPECIAL_GRENADE_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texStunGrenadeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.ThrowGrenade)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
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
                stockToConsume = 1
            });

            Skills.scepterGrenadeSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_GRENADE_SCEPTER_NAME",
                skillNameToken = prefix + "SPECIAL_GRENADE_SCEPTER_NAME",
                skillDescriptionToken = prefix + "SPECIAL_GRENADE_SCEPTER_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texStunGrenadeScepterIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Scepter.ThrowMolotov)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
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
                stockToConsume = 1
            });

            Skills.knifeSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_KNIFE_NAME",
                skillNameToken = prefix + "SPECIAL_KNIFE_NAME",
                skillDescriptionToken = prefix + "SPECIAL_KNIFE_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SwingKnife)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 7f,
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
                stockToConsume = 1
            });

            Skills.scepterKnifeSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_KNIFE_SCEPTER_NAME",
                skillNameToken = prefix + "SPECIAL_KNIFE_SCEPTER_NAME",
                skillDescriptionToken = prefix + "SPECIAL_KNIFE_SCEPTER_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texKnifeScepterIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Scepter.SwingKnifeScepter)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 2,
                baseRechargeInterval = 4f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Skills.supplyDropSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_SUPPLY_DROP_NAME",
                skillNameToken = prefix + "SPECIAL_SUPPLY_DROP_NAME",
                skillDescriptionToken = prefix + "SPECIAL_SUPPLY_DROP_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSupplyDropIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SupplyDrop.Nerfed.AimCrapDrop)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 24f,
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
                stockToConsume = 0
            });

            Skills.scepterSupplyDropSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_SUPPLY_DROP_SCEPTER_NAME",
                skillNameToken = prefix + "SPECIAL_SUPPLY_DROP_SCEPTER_NAME",
                skillDescriptionToken = prefix + "SPECIAL_SUPPLY_DROP_SCEPTER_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSupplyDropScepterIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Scepter.SupplyDrop.AimVoidDrop)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 24f,
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
                stockToConsume = 0
            });

            Skills.supplyDropLegacySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_SUPPLY_DROP_LEGACY_NAME",
                skillNameToken = prefix + "SPECIAL_SUPPLY_DROP_LEGACY_NAME",
                skillDescriptionToken = prefix + "SPECIAL_SUPPLY_DROP_LEGACY_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSupplyDropLegacyIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.SupplyDrop.AimSupplyDrop)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 0
            });

            Skills.scepterSupplyDropLegacySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_SUPPLY_DROP_LEGACY_SCEPTER_NAME",
                skillNameToken = prefix + "SPECIAL_SUPPLY_DROP_LEGACY_SCEPTER_NAME",
                skillDescriptionToken = prefix + "SPECIAL_SUPPLY_DROP_LEGACY_SCEPTER_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSupplyDropLegacyScepterIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Scepter.SupplyDrop.AimVoidDrop)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 0f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = true,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = true,
                mustKeyPress = false,
                cancelSprintingOnActivation = true,
                rechargeStock = 0,
                requiredStock = 1,
                stockToConsume = 0
            });

            Skills.healSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_HEAL_NAME",
                skillNameToken = prefix + "SPECIAL_HEAL_NAME",
                skillDescriptionToken = prefix + "SPECIAL_HEAL_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texStunGrenadeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Heal)),
                activationStateMachineName = "Body",
                baseMaxStock = 1,
                baseRechargeInterval = 24f,
                beginSkillCooldownOnSkillEnd = true,
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
                stockToConsume = 1
            });

            Skills.syringeSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_SYRINGE_NAME",
                skillNameToken = prefix + "SPECIAL_SYRINGE_NAME",
                skillDescriptionToken = prefix + "SPECIAL_SYRINGE_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSyringeIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.UseSyringe)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
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
                stockToConsume = 1
            });

            Skills.scepterSyringeSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_SYRINGE_SCEPTER_NAME",
                skillNameToken = prefix + "SPECIAL_SYRINGE_SCEPTER_NAME",
                skillDescriptionToken = prefix + "SPECIAL_SYRINGE_SCEPTER_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSyringeScepterIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Scepter.UseSyringeScepter)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
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
                stockToConsume = 1
            });

            Skills.syringeLegacySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_SYRINGELEGACY_NAME",
                skillNameToken = prefix + "SPECIAL_SYRINGELEGACY_NAME",
                skillDescriptionToken = prefix + "SPECIAL_SYRINGELEGACY_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSyringeLegacyIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.UseSyringeLegacy)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
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
                stockToConsume = 1
            });

            Skills.scepterSyringeLegacySkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_SYRINGELEGACY_SCEPTER_NAME",
                skillNameToken = prefix + "SPECIAL_SYRINGELEGACY_SCEPTER_NAME",
                skillDescriptionToken = prefix + "SPECIAL_SYRINGELEGACY_SCEPTER_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texSyringeLegacyScepterIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Scepter.UseSyringeScepter)),
                activationStateMachineName = "Weapon",
                baseMaxStock = 1,
                baseRechargeInterval = 12f,
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
                stockToConsume = 1
            });

            Skills.coinSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_DRIVERCOIN_NAME",
                skillNameToken = prefix + "SPECIAL_DRIVERCOIN_NAME",
                skillDescriptionToken = prefix + "SPECIAL_DRIVERCOIN_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Coin)),
                activationStateMachineName = "AltWeapon",
                baseMaxStock = 2,
                baseRechargeInterval = 5f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });

            Skills.scepterCoinSkillDef = Skills.CreateAndAddSkillDef(new SkillDefInfo
            {
                skillName = prefix + "SPECIAL_DRIVERCOIN_SCEPTER_NAME",
                skillNameToken = prefix + "SPECIAL_DRIVERCOIN_SCEPTER_NAME",
                skillDescriptionToken = prefix + "SPECIAL_DRIVERCOIN_SCEPTER_DESCRIPTION",
                skillIcon = Assets.mainAssetBundle.LoadAsset<Sprite>("texShotgunSecondaryIcon"),
                activationState = new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Scepter.CoinScepter)),
                activationStateMachineName = "AltWeapon",
                baseMaxStock = 2,
                baseRechargeInterval = 5f,
                beginSkillCooldownOnSkillEnd = false,
                canceledFromSprinting = false,
                forceSprintDuringState = false,
                fullRestockOnAssign = false,
                interruptPriority = EntityStates.InterruptPriority.PrioritySkill,
                resetCooldownTimerOnUse = false,
                isCombatSkill = false,
                mustKeyPress = true,
                cancelSprintingOnActivation = false,
                rechargeStock = 1,
                requiredStock = 1,
                stockToConsume = 1
            });
            
            Skills.AddSpecialSkills(skillLoc, 
                new Skills.SkillDefPair(Skills.stunGrenadeSkillDef),
                new Skills.SkillDefPair(Skills.supplyDropSkillDef, Unlockables.supplyDropUnlockableDef));

            if (Config.cursed.Value)
                Skills.AddSpecialSkills(skillLoc, new Skills.SkillDefPair(Skills.supplyDropLegacySkillDef, Unlockables.supplyDropUnlockableDef));

            Skills.AddSpecialSkills(skillLoc,
                new Skills.SkillDefPair(Skills.knifeSkillDef),
                new Skills.SkillDefPair(Skills.syringeSkillDef));

            if (Config.cursed.Value)
                Skills.AddSpecialSkills(skillLoc, new Skills.SkillDefPair(Skills.syringeLegacySkillDef));

            Skills.AddSpecialSkills(skillLoc, new Skills.SkillDefPair(Skills.coinSkillDef));
            #endregion

            if (DriverPlugin.ScepterInstalled)
                InitializeScepterSkills();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void InitializeScepterSkills()
        {
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.scepterGrenadeSkillDef, bodyName, SkillSlot.Special, 0);
            AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.scepterSupplyDropSkillDef, bodyName, SkillSlot.Special, 1);

            if (Config.cursed.Value)
            {
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.scepterSupplyDropLegacySkillDef, bodyName, SkillSlot.Special, 2);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.scepterKnifeSkillDef, bodyName, SkillSlot.Special, 3);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.scepterSyringeSkillDef, bodyName, SkillSlot.Special, 4);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.scepterSyringeLegacySkillDef, bodyName, SkillSlot.Special, 5);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.scepterCoinSkillDef, bodyName, SkillSlot.Special, 6);
            }
            else
            {
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.scepterKnifeSkillDef, bodyName, SkillSlot.Special, 2);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.scepterSyringeSkillDef, bodyName, SkillSlot.Special, 3);
                AncientScepter.AncientScepterItem.instance.RegisterScepterSkill(Skills.scepterCoinSkillDef, bodyName, SkillSlot.Special, 4);
            }
        }

        private static void CreateSkins(GameObject prefab)
        {
            GameObject model = prefab.GetComponent<ModelLocator>().modelTransform.gameObject;
            CharacterModel characterModel = model.GetComponent<CharacterModel>();
            
            SkinnedMeshRenderer mainRenderer = characterModel.mainSkinnedMeshRenderer;
            CharacterModel.RendererInfo[] defaultRenderers = characterModel.baseRendererInfos;

            ModelSkinController skinController = model.AddComponent<ModelSkinController>();
            ChildLocator childLocator = model.GetComponent<ChildLocator>();

            GameObject sluggerCloth = childLocator.FindChildGameObject("SluggerCloth");
            GameObject tie = childLocator.FindChildGameObject("Tie");

            SkinDef defaultSkin = Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_DEFAULT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMainSkin"),
                model, 
                null,
                defaultRenderers,
                [
                    new SkinDef.MeshReplacement
                    {
                        renderer = mainRenderer,
                        mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshDriver")
                    }
                ],
                [
                    new SkinDef.GameObjectActivation
                    {
                        gameObject = sluggerCloth,
                        shouldActivate = false
                    },
                    new SkinDef.GameObjectActivation
                    {
                        gameObject = tie,
                        shouldActivate = false
                    }
                ]);

            SkinDef masterySkin = Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_MONSOON_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texMonsoonSkin"),
                model,
                Unlockables.masteryUnlockableDef,
                Skins.SkinRendererInfos(defaultRenderers,
                [
                    Assets.LoadMaterial("matJacket")
                ]),
                [
                    new SkinDef.MeshReplacement
                    {
                        renderer = mainRenderer,
                        mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshJacket")
                    }
                ], 
                [
                    new SkinDef.GameObjectActivation
                    {
                        gameObject = sluggerCloth,
                        shouldActivate = false
                    },
                    new SkinDef.GameObjectActivation
                    {
                        gameObject = tie,
                        shouldActivate = false
                    }
                ]);

            SkinDef grandMasterySkin = Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_TYPHOON_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texTyphoonSkin"),
                model,
                Unlockables.grandMasteryUnlockableDef,
                Skins.SkinRendererInfos(defaultRenderers,
                [
                    Assets.LoadMaterial("matSlugger")
                ]),
                [
                    new SkinDef.MeshReplacement
                    {
                        renderer = mainRenderer,
                        mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshSlugger")
                    }
                ], 
                [
                    new SkinDef.GameObjectActivation
                    {
                        gameObject = sluggerCloth,
                        shouldActivate = true
                    },
                    new SkinDef.GameObjectActivation
                    {
                        gameObject = tie,
                        shouldActivate = false
                    }
                ]);

            SkinDef specialForcesSkin = Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_SPECIALFORCES_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texSpecialForcesSkin"),
                model,
                null,
                Skins.SkinRendererInfos(defaultRenderers,
                [
                    Assets.LoadMaterial("matSpecialForces")
                ]), 
                [
                    new SkinDef.MeshReplacement
                    {
                        renderer = mainRenderer,
                        mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshSpecialForces")
                    }
                ],
                [
                    new SkinDef.GameObjectActivation
                    {
                        gameObject = sluggerCloth,
                        shouldActivate = false
                    },
                    new SkinDef.GameObjectActivation
                    {
                        gameObject = tie,
                        shouldActivate = false
                    }
                ]);

            SkinDef guerrillaSkin = Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_GUERRILLA_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texGuerrillaSkin"),
                model,
                null,
                Skins.SkinRendererInfos(defaultRenderers,
                [
                    Assets.LoadMaterial("matGuerrilla")
                ]), 
                [
                    new SkinDef.MeshReplacement
                    {
                        renderer = mainRenderer,
                        mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshGuerrilla")
                    }
                ],
                [
                    new SkinDef.GameObjectActivation
                    {
                        gameObject = sluggerCloth,
                        shouldActivate = false
                    },
                    new SkinDef.GameObjectActivation
                    {
                        gameObject = tie,
                        shouldActivate = false
                    }
                ]);

            SkinDef suitSkin = Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_SUIT_SKIN_NAME",
                Assets.mainAssetBundle.LoadAsset<Sprite>("texSuitSkin"),
                model,
                Unlockables.suitUnlockableDef,
                Skins.SkinRendererInfos(defaultRenderers,
                [
                    Assets.LoadMaterial("matSuit")
                ]), 
                [
                    new SkinDef.MeshReplacement
                    {
                        renderer = mainRenderer,
                        mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshSuit")
                    }
                ],
                [
                    new SkinDef.GameObjectActivation
                    {
                        gameObject = sluggerCloth,
                        shouldActivate = false
                    },
                    new SkinDef.GameObjectActivation
                    {
                        gameObject = tie,
                        shouldActivate = true
                    }
                ]);

            List<SkinDef> skins =
            [
                defaultSkin,
                masterySkin,
                grandMasterySkin,
                specialForcesSkin,
                guerrillaSkin,
                suitSkin
            ];

            if (Config.cursed.Value)
            {
                SkinDef suit2Skin = Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_SUIT2_SKIN_NAME",
                    Assets.mainAssetBundle.LoadAsset<Sprite>("texSuit2Skin"),
                    model,
                    Unlockables.suitUnlockableDef,
                    Skins.SkinRendererInfos(defaultRenderers,
                    [
                        Assets.LoadMaterial("matSuit")
                    ]), 
                    [
                        new SkinDef.MeshReplacement
                        {
                            renderer = mainRenderer,
                            mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshSuit2")
                        }
                    ],
                    [
                        new SkinDef.GameObjectActivation
                        {
                            gameObject = sluggerCloth,
                            shouldActivate = false
                        },
                        new SkinDef.GameObjectActivation
                        {
                            gameObject = tie,
                            shouldActivate = true
                        }
                    ]);

                SkinDef greenSkin = Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_GREEN_SKIN_NAME",
                    Assets.mainAssetBundle.LoadAsset<Sprite>("texGreenSkin"),
                    model,
                    null,
                    Skins.SkinRendererInfos(defaultRenderers,
                    [
                        Assets.LoadMaterial("matDriverGreen")
                    ]), 
                    [
                        new SkinDef.MeshReplacement
                        {
                            renderer = mainRenderer,
                            mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshDriver")
                        }
                    ], 
                    [
                        new SkinDef.GameObjectActivation
                        {
                            gameObject = sluggerCloth,
                            shouldActivate = false
                        },
                        new SkinDef.GameObjectActivation
                        {
                            gameObject = tie,
                            shouldActivate = false
                        }
                    ]);

                SkinDef minecraftSkin = Skins.CreateSkinDef(DriverPlugin.developerPrefix + "_DRIVER_BODY_MINECRAFT_SKIN_NAME",
                    Assets.mainAssetBundle.LoadAsset<Sprite>("texMinecraftSkin"),
                    model,
                    null,
                    Skins.SkinRendererInfos(defaultRenderers,
                    [
                        Assets.LoadMaterial("matMinecraftDriver")
                    ]),
                    [
                        new SkinDef.MeshReplacement
                        {
                            renderer = mainRenderer,
                            mesh = Assets.mainAssetBundle.LoadAsset<Mesh>("meshMinecraftDriver")
                        }
                    ], 
                    [
                        new SkinDef.GameObjectActivation
                        {
                            gameObject = sluggerCloth,
                            shouldActivate = false
                        },
                        new SkinDef.GameObjectActivation
                        {
                            gameObject = tie,
                            shouldActivate = false
                        }
                    ]);

                skins.AddRange([suit2Skin, greenSkin, minecraftSkin]);
            }

            skinController.skins = [.. skins];
        }

        #region Item Displays
        private static void InitializeItemDisplays(GameObject prefab)
        {
            CharacterModel characterModel = prefab.GetComponentInChildren<CharacterModel>();

            if (itemDisplayRuleSet == null)
            {
                itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
                itemDisplayRuleSet.name = "idrs" + bodyName;
            }

            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;
            characterModel.itemDisplayRuleSet.keyAssetRuleGroups = Resources.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody")
                .GetComponentInChildren<CharacterModel>().itemDisplayRuleSet.keyAssetRuleGroups;// itemDisplayRuleSet;
            itemDisplayRules = itemDisplayRuleSet.keyAssetRuleGroups.ToList();
        }

        internal static void SetItemDisplays(ReadOnlyArray<ReadOnlyContentPack> _)
        {
            // uhh
            ItemDisplays.PopulateDisplays();

            ReplaceItemDisplay(RoR2Content.Items.SecondarySkillMagazine,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayDoubleMag"),
                    limbMask = LimbFlags.None,
                    childName = "GunR",
                    localPos = new Vector3(0.00888F, -0.03648F, -0.20898F),
                    localAngles = new Vector3(39.35415F, 348.9445F, 164.0792F),
                    localScale = new Vector3(0.06F, 0.06F, 0.06F)
                }
            ]);

            ReplaceItemDisplay(RoR2Content.Items.CritGlasses,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayGlasses"),
                    limbMask = LimbFlags.None,
                    childName = "Head",
                    localPos = new Vector3(0.0006F, 0.25054F, 0.04672F),
                    localAngles = new Vector3(314.7648F, 358.1459F, 0.48047F),
                    localScale = new Vector3(0.30902F, 0.09537F, 0.30934F)
                }
            ]);

            if (Config.predatoryOnHead.Value)
            {
                ReplaceItemDisplay(RoR2Content.Items.AttackSpeedOnCrit,
                [
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayWolfPelt"),
                        limbMask = LimbFlags.None,
                        childName = "Head",
                        localPos = new Vector3(0F, 0.18766F, -0.11041F),
                        localAngles = new Vector3(302.566F, 0F, 0F),
                        localScale = new Vector3(0.47332F, 0.47332F, 0.47332F)
                    }
                ]);
            }
            else
            {
                ReplaceItemDisplay(RoR2Content.Items.AttackSpeedOnCrit,
                [
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefab = ItemDisplays.LoadDisplay("DisplayWolfPelt"),
                        limbMask = LimbFlags.None,
                        childName = "UpperArmR",
                        localPos = new Vector3(-0.01092F, 0.02048F, -0.00403F),
                        localAngles = new Vector3(309.4066F, 250.1116F, 175.7708F),
                        localScale = new Vector3(0.363F, 0.363F, 0.363F)
                    }
                ]);
            }

            ReplaceItemDisplay(DLC1Content.Items.CritGlassesVoid,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayGlassesVoid"),
                    limbMask = LimbFlags.None,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.1555F, 0.11598F),
                    localAngles = new Vector3(340.0668F, 0F, 0F),
                    localScale = new Vector3(0.30387F, 0.39468F, 0.46147F)
                }
            ]);

            ReplaceItemDisplay(DLC1Content.Items.LunarSun,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplaySunHeadNeck"),
                    limbMask = LimbFlags.None,
                    childName = "Chest",
                    localPos = new Vector3(-0.02605F, 0.38179F, -0.0112F),
                    localAngles = new Vector3(-0.00001F, 262.1551F, 0.00001F),
                    localScale = new Vector3(1.76594F, 1.84475F, 1.84475F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.LimbMask,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplaySunHead"),
                    limbMask = LimbFlags.Head,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.10143F, -0.01147F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.90836F, 0.90836F, 0.90836F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplaySunHead"),
                    limbMask = LimbFlags.Head,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.10143F, -0.01147F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.90836F, 0.90836F, 0.90836F)
                }
            ]);

            ReplaceItemDisplay(RoR2Content.Items.GhostOnKill,
[
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayMask"),
                    limbMask = LimbFlags.None,
                    childName = "Head",
                    localPos = new Vector3(0.0029F, 0.15924F, 0.07032F),
                    localAngles = new Vector3(355.7367F, 0.15F, 0F),
                    localScale = new Vector3(0.6F, 0.6F, 0.6F)
                }
            ]);

            ReplaceItemDisplay(RoR2Content.Items.GoldOnHit,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayBoneCrown"),
                    limbMask = LimbFlags.None,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.15159F, -0.0146F),
                    localAngles = new Vector3(8.52676F, 0F, 0F),
                    localScale = new Vector3(0.90509F, 0.90509F, 0.90509F)
                }
            ]);

            ReplaceItemDisplay(RoR2Content.Items.JumpBoost,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayWaxBird"),
                    limbMask = LimbFlags.None,
                    childName = "Head",
                    localPos = new Vector3(0F, -0.228F, -0.108F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.79857F, 0.79857F, 0.79857F)
                }
            ]);

            ReplaceItemDisplay(RoR2Content.Items.KillEliteFrenzy,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayBrainstalk"),
                    limbMask = LimbFlags.None,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.12823F, 0.035F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.17982F, 0.17982F, 0.17982F)
                }
            ]);

            ReplaceItemDisplay(RoR2Content.Items.LunarPrimaryReplacement,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayBirdEye"),
                    limbMask = LimbFlags.None,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.18736F, 0.08896F),
                    localAngles = new Vector3(306.9798F, 180F, 180F),
                    localScale = new Vector3(0.31302F, 0.31302F, 0.31302F)
                }
            ]);

            ReplaceItemDisplay(DLC1Content.Items.FragileDamageBonus,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayDelicateWatch"),
                    limbMask = LimbFlags.None,
                    childName = "HandL",
                    localPos = new Vector3(0.001145094f, -0.01941454f, 0.001435831f),
                    localAngles = new Vector3(84.24088f, 213.1651f, 131.5774f),
                    localScale = new Vector3(0.5f, 0.5f, 0.5f)
                }
            ]);

            ReplaceItemDisplay(RoR2Content.Items.BarrierOnOverHeal,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayAegis"),
                    limbMask = LimbFlags.None,
                    childName = "LowerArmL",
                    localPos = new Vector3(0.01781F, 0.11702F, 0.01516F),
                    localAngles = new Vector3(90F, 270F, 0F),
                    localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
            ]);

            ReplaceItemDisplay(RoR2Content.Items.SprintArmor,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayBuckler"),
                    limbMask = LimbFlags.None,
                    childName = "LowerArmL",
                    localPos = new Vector3(-0.012F, 0.171F, -0.027F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
            ]);

            ReplaceItemDisplay(RoR2Content.Items.ArmorPlate,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayRepulsionArmorPlate"),
                    limbMask = LimbFlags.None,
                    childName = "CalfR",
                    localPos = new Vector3(-0.02573F, 0.22602F, 0.0361F),
                    localAngles = new Vector3(90F, 180F, 0F),
                    localScale = new Vector3(-0.2958F, 0.2958F, 0.29581F)
                }
            ]);

            ReplaceItemDisplay(DLC1Content.Items.CritDamage,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefab = ItemDisplays.LoadDisplay("DisplayLaserSight"),
                    limbMask = LimbFlags.None,
                    childName = "Pistol",
                    localPos = new Vector3(-0.01876F, 0.26245F, 0.11694F),
                    localAngles = new Vector3(0F, 0F, 270F),
                    localScale = new Vector3(0.05261F, 0.05261F, 0.05261F)
                }
            ]);

            //if (DriverPlugin.LitInstalled) SetLITDisplays();

            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
            //itemDisplayRuleSet.GenerateRuntimeValues();
        }
        
        internal static void SetLITDisplays()
        {
            /*
            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = LostInTransit.LITContent.Items.Lopper,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayLopper"),
                            limbMask = LimbFlags.None,
                            childName = "Chest",
                            localPos = new Vector3(0F, 0.20282F, -0.19089F),
                            localAngles = new Vector3(0F, 0F, 0F),
                            localScale = new Vector3(0.19059F, 0.19059F, 0.19059F)
                        }
                    }
                }
            });

            itemDisplayRules.Add(new ItemDisplayRuleSet.KeyAssetRuleGroup
            {
                keyAsset = LostInTransit.LITContent.Items.Chestplate,
                displayRuleGroup = new DisplayRuleGroup
                {
                    rules = new ItemDisplayRule[]
                    {
                        new ItemDisplayRule
                        {
                            ruleType = ItemDisplayRuleType.ParentedPrefab,
                            followerPrefab = ItemDisplays.LoadDisplay("DisplayBackPlate"),
                            limbMask = LimbFlags.None,
                            childName = "Chest",
                            localPos = new Vector3(0F, 0.23366F, 0.01011F),
                            localAngles = new Vector3(349.1311F, 0F, 0F),
                            localScale = new Vector3(0.13457F, 0.19557F, 0.19557F)
                        }
                    }
                }
            });
            */
        }

        internal static void ReplaceItemDisplay(UnityEngine.Object keyAsset, ItemDisplayRule[] newDisplayRules)
        {
            ItemDisplayRuleSet.KeyAssetRuleGroup[] cock = itemDisplayRules.ToArray();
            for (int i = 0; i < cock.Length; i++)
            {
                if (cock[i].keyAsset == keyAsset)
                {
                    // replace the item display rule
                    cock[i].displayRuleGroup.rules = newDisplayRules;
                }
            }
            itemDisplayRules = cock.ToList();
        }
        #endregion
    }
}