using R2API.Networking;
using RobDriver.Modules.Components;
using RoR2;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine;
using MaterialHud;
using System.Linq;
using TMPro;
using UnityEngine.AddressableAssets;
using R2API;
using R2API.Networking.Interfaces;
using RoR2.UI;
using RoR2.CharacterAI;

namespace RobDriver.Modules.Survivors
{
    public static class DriverHooks
    {
        internal static void Hook()
        {
            RoR2.GlobalEventManager.onCharacterDeathGlobal += GlobalEventManager_onCharacterDeathGlobal;

            RoR2.UI.HUD.onHudTargetChangedGlobal += HUDSetup;

            On.RoR2.UI.HGButton.Start += HGButton_Start;

            On.RoR2.SkillLocator.ApplyAmmoPack += SkillLocator_ApplyAmmoPack;
            On.RoR2.SkillLocator.ResetSkills += SkillLocator_ResetSkills;

            // heresy anims
            On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.OnEnter += PlayVisionsAnimation;
            On.EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary.PlayChargeAnimation += PlayChargeLunarAnimation;
            On.EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary.PlayThrowAnimation += PlayThrowLunarAnimation;
            On.EntityStates.GlobalSkills.LunarDetonator.Detonate.OnEnter += PlayRuinAnimation;

            // dazed debuff
            On.EntityStates.AI.BaseAIState.AimAt += BaseAIState_AimAt;
            On.EntityStates.AI.BaseAIState.AimInDirection += BaseAIState_AimInDirection;

            On.RoR2.UI.LoadoutPanelController.Rebuild += LoadoutPanelController_Rebuild;// the most useless hook ever.
            On.RoR2.UI.GameEndReportPanelController.AssignUnlockToStrip += GameEndReportPanelController_AssignUnlockToStrip;
        }

        private static void GameEndReportPanelController_AssignUnlockToStrip(On.RoR2.UI.GameEndReportPanelController.orig_AssignUnlockToStrip orig,
            GameEndReportPanelController self, UnlockableDef unlockableDef, GameObject destUnlockableStrip)
        {
            orig(self, unlockableDef, destUnlockableStrip);

            if (DriverWeaponCatalog.weaponDefs.Any(def => def.nameToken == unlockableDef.nameToken))
            {
                if (unlockableDef.achievementIcon?.texture is Texture icon)
                {
                    destUnlockableStrip.transform.Find("IconImage").GetComponent<RawImage>().texture = icon;
                }
                destUnlockableStrip.GetComponent<TooltipProvider>().overrideTitleText = Language.GetString("ROB_DRIVER_BODY_WEAPON_UNLOCKABLE_NAME");
                destUnlockableStrip.GetComponent<TooltipProvider>().overrideBodyText = Language.GetString("ROB_DRIVER_BODY_WEAPON_UNLOCKABLE_DESC"); ;
            }
        }

        private static void LoadoutPanelController_Rebuild(On.RoR2.UI.LoadoutPanelController.orig_Rebuild orig, LoadoutPanelController self)
        {
            orig(self);

            // this is beyond stupid lmfao who let this monkey code
            if (self.currentDisplayData.bodyIndex == BodyCatalog.FindBodyIndex("RobDriverBody"))
            {
                // i made it worse, youre welcome
                string newToken = "Passive";
                foreach (var label in self.gameObject.GetComponentsInChildren<LanguageTextMeshController>().Where(label => label && label.token == "LOADOUT_SKILL_MISC"))
                {
                    if (newToken != null)
                    {
                        label.token = newToken;
                        newToken = newToken == "Passive" ? "Arsenal" : null;
                    }
                }
            }
        }

        private static void HGButton_Start(On.RoR2.UI.HGButton.orig_Start orig, HGButton self)
        {
            orig(self);

            if (!Config.enableGodslingInMultiplayer.Value)
            {
                // this is literally the worst thing ever
                if (self && !string.IsNullOrEmpty(self.hoverToken) &&
                    self.hoverToken.Contains("Godsling") && !RoR2Application.isInSinglePlayer)
                {
                    self.gameObject.SetActive(false);
                }
            }
        }

        private static void BaseAIState_AimInDirection(On.EntityStates.AI.BaseAIState.orig_AimInDirection orig, EntityStates.AI.BaseAIState self, ref BaseAI.BodyInputs dest, Vector3 aimDirection)
        {
            if (self.body && self.body.HasBuff(Buffs.dazedDebuff))
            {
                orig(self, ref dest, UnityEngine.Random.onUnitSphere);
                dest.desiredAimDirection = UnityEngine.Random.onUnitSphere;
            }
            else orig(self, ref dest, aimDirection);
        }

        private static void BaseAIState_AimAt(On.EntityStates.AI.BaseAIState.orig_AimAt orig, EntityStates.AI.BaseAIState self, ref BaseAI.BodyInputs dest, BaseAI.Target aimTarget)
        {
            orig(self, ref dest, aimTarget);

            if (self.body && self.body.HasBuff(Modules.Buffs.dazedDebuff))
            {
                dest.desiredAimDirection = UnityEngine.Random.onUnitSphere;
            }
        }

        private static void SkillLocator_ApplyAmmoPack(On.RoR2.SkillLocator.orig_ApplyAmmoPack orig, SkillLocator self)
        {
            orig(self);

            if (self.gameObject.name == Driver.bodyName && self.TryGetComponent<DriverController>(out var iDrive))
            {
                iDrive.ServerResetTimer();
            }
        }

        private static void SkillLocator_ResetSkills(On.RoR2.SkillLocator.orig_ResetSkills orig, SkillLocator self)
        {
            orig(self);

            if (self.gameObject.name == Driver.bodyName && self.TryGetComponent<DriverController>(out var iDrive))
            {
                iDrive.ServerResetTimer();
            }
        }

        private static void GlobalEventManager_onCharacterDeathGlobal(DamageReport damageReport)
        {
            if (!NetworkServer.active || !(damageReport.attackerBody && damageReport.attackerMaster && damageReport.victim))
                return;

            bool isDriverOnPlayerTeam = false;
            foreach (var tc in TeamComponent.GetTeamMembers(TeamIndex.Player))
            {
                if (tc.body && tc.body.baseNameToken == Driver.bodyNameToken)
                {
                    isDriverOnPlayerTeam = true;
                    break;
                }
            }

            if (!isDriverOnPlayerTeam)
                return;

            // headshot first
            if (damageReport.attackerBody.baseNameToken == Driver.bodyNameToken &&
               (damageReport.damageInfo.HasModdedDamageType(DamageTypes.BloodExplosionIdentifier) || damageReport.victim.GetComponent<DriverHeadshotTracker>()))
            {
                if (damageReport.victim.TryGetComponent<NetworkIdentity>(out var identity))
                {
                    new SyncDecapitation(identity.netId, damageReport.victim.gameObject).Send(NetworkDestination.Clients);
                }

                // rav orb yep
                if (damageReport.attackerBody.skillLocator.primary.skillDef == DriverWeaponCatalog.RavSword?.primarySkillDef)
                {
                    RoR2.Orbs.OrbManager.instance.AddOrb(new ConsumeOrb
                    {
                        origin = damageReport.victim.transform.position,
                        target = Util.FindBodyMainHurtBox(damageReport.attackerBody)
                    });
                }
            }

            // weapon drops
            float chance = Modules.Config.baseDropRate.Value;
            if (chance <= 0) return; // drop nothing

            bool fuckMyAss = chance >= 100f;

            // higher chance if it's a big guy
            if (damageReport.victimBody.hullClassification == HullClassification.Golem)
                chance = Mathf.Clamp(1.1f * chance, 0f, 100f);

            // minimum 25% chance if the slain enemy is an elite
            if (damageReport.victimBody.isElite)
                chance = Mathf.Clamp(chance, 25f, 100f);

            // halved on swarms, fuck You
            if (Run.instance && RoR2.RunArtifactManager.instance.IsArtifactEnabled(RoR2Content.Artifacts.Swarms))
                chance *= 0.5f;

            chance *= Driver.instance.pityMultiplier;

            bool droppedWeapon = Util.CheckRoll(chance, damageReport.attackerMaster);

            // guaranteed if the slain enemy is a boss
            bool isBoss = damageReport.victimBody.isChampion || damageReport.victimIsChampion;

            // simulacrum boss wave fix
            if ((damageReport.victimBody.isBoss || damageReport.victimIsBoss) && !InfiniteTowerRun.instance)
                isBoss = true;

            // terminal enemies from starstorm's relic of termination
            if (DriverPlugin.CheckIfBodyIsTerminal(damageReport.victimBody))
                isBoss = true;

            if (isBoss || fuckMyAss)
                droppedWeapon = true;

            // all the above checks were originally checking the ATTACKER body
            // not the fucking victim
            // how

            // stop dropping weapons when void monsters kill each other plz this is an annoying bug
            if (damageReport.attackerTeamIndex != TeamIndex.Player) 
                droppedWeapon = false;

            if (DriverWeaponCatalog.weaponDrops.TryGetValue(damageReport.victimBody.baseNameToken, out var uniqueDrop) && uniqueDrop.dropChance >= 100f)
            {
                droppedWeapon = true;
            }

            if (droppedWeapon)
            {
                Driver.instance.pityMultiplier = 0.8f;

                Vector3 position = damageReport.victim.transform ? damageReport.victim.transform.position : Vector3.zero;

                //if (Modules.Config.oldPickupModel.Value) pickupPrefab = Modules.Assets.weaponPickupOld;

                DriverWeaponTier weaponTier = damageReport.victimBody.isChampion ? DriverWeaponTier.Legendary : DriverWeaponTier.Uncommon;

                // use unique drop, otherwise roll random
                DriverWeaponDef weaponDef;
                if (uniqueDrop && Util.CheckRoll(uniqueDrop.dropChance)) weaponDef = uniqueDrop;
                else weaponDef = DriverWeaponCatalog.GetRandomWeaponFromTier(weaponTier);

                GameObject weaponPickup = UnityEngine.Object.Instantiate<GameObject>(weaponDef.pickupPrefab, position, UnityEngine.Random.rotation);
                var weaponComponent = weaponPickup.GetComponent<SyncPickup>();

                // add passive specific stuff
                // give the poor godsling players the ultra rare weapons, nobody likes getting bullets from michael
                if (!uniqueDrop || uniqueDrop.dropChance < 100)
                    weaponComponent.isNewAmmoType = Util.CheckRoll(Config.godslingDropRateSplit.Value);

                // non-legendary gets rerolled
                weaponComponent.bulletDef = isBoss ? DriverBulletCatalog.GetRandomBulletFromTier(DriverWeaponTier.Legendary) :
                    DriverBulletCatalog.GetWeightedRandomBullet(DriverWeaponTier.Uncommon);

                if (weaponPickup.TryGetComponent<TeamFilter>(out var teamFilter))
                    teamFilter.teamIndex = damageReport.attackerTeamIndex;

                NetworkServer.Spawn(weaponPickup);
            }
            else
            {
                // add pity
                Driver.instance.pityMultiplier += 0.025f;
            }
            // combo extension would be huge but i need to network it and that's annoying
            /*if (damageReport.attackerBody.baseNameToken == Driver.bodyNameToken)
            {
                // combo extension
                Components.DriverController iDrive = damageReport.attackerBody.gameObject.GetComponent<Components.DriverController>();
                if (iDrive) iDrive.ExtendTimer();
            }*/
        }

        private static void HUDSetup(RoR2.UI.HUD hud)
        {
            if (hud.targetBodyObject && hud.targetMaster && hud.targetMaster.bodyPrefab == Driver.characterPrefab)
            {
                if (!hud.targetMaster.hasAuthority) return;

                if (DriverPlugin.riskUIInstalled)
                {
                    RiskUIHudSetup(hud);
                    return;
                }

                Transform skillsContainer = hud.equipmentIcons[0].gameObject.transform.parent;

                // remove existing
                if (skillsContainer.Find("WeaponSlot")) GameObject.Destroy(skillsContainer.Find("WeaponSlot").gameObject);

                var oldUI = hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras").Find("AmmoTracker");
                if (oldUI) GameObject.Destroy(oldUI.gameObject);

                // no one will notice these missing
                skillsContainer.Find("SprintCluster").gameObject.SetActive(false);
                skillsContainer.Find("InventoryCluster").gameObject.SetActive(false);

                GameObject weaponSlot = GameObject.Instantiate(skillsContainer.Find("EquipmentSlot").gameObject, skillsContainer);
                weaponSlot.name = "WeaponSlot";

                EquipmentIcon equipmentIconComponent = weaponSlot.GetComponent<EquipmentIcon>();
                Components.WeaponIcon weaponIconComponent = weaponSlot.AddComponent<Components.WeaponIcon>();

                weaponIconComponent.iconImage = equipmentIconComponent.iconImage;
                weaponIconComponent.displayRoot = equipmentIconComponent.displayRoot;
                weaponIconComponent.flashPanelObject = equipmentIconComponent.stockFlashPanelObject;
                weaponIconComponent.reminderFlashPanelObject = equipmentIconComponent.reminderFlashPanelObject;
                weaponIconComponent.isReadyPanelObject = equipmentIconComponent.isReadyPanelObject;
                weaponIconComponent.tooltipProvider = equipmentIconComponent.tooltipProvider;
                weaponIconComponent.targetHUD = hud;
                weaponSlot.GetComponent<RectTransform>().anchoredPosition = new Vector2(-480f, -17.1797f);

                HGTextMeshProUGUI keyText = weaponSlot.transform.Find("DisplayRoot").Find("EquipmentTextBackgroundPanel").Find("EquipmentKeyText").gameObject.GetComponent<HGTextMeshProUGUI>();
                keyText.gameObject.GetComponent<InputBindingDisplayController>().enabled = false;
                keyText.text = "Weapon";

                weaponSlot.transform.Find("DisplayRoot").Find("EquipmentStack").gameObject.SetActive(false);
                weaponSlot.transform.Find("DisplayRoot").Find("CooldownText").gameObject.SetActive(false);

                // duration bar
                GameObject chargeBar = GameObject.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("WeaponChargeBar"));
                chargeBar.transform.SetParent(weaponSlot.transform.Find("DisplayRoot"));

                RectTransform rect = chargeBar.GetComponent<RectTransform>();

                rect.localScale = new Vector3(0.75f, 0.1f, 1f);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 0f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(-10f, 13f);
                rect.localPosition = new Vector3(-33f, -10f, 0f);
                rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

                weaponIconComponent.durationDisplay = chargeBar;
                weaponIconComponent.durationBar = chargeBar.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>();
                weaponIconComponent.durationBarRed = chargeBar.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();

                MonoBehaviour.Destroy(equipmentIconComponent);

                // weapon pickup notification

                GameObject notificationPanel = GameObject.Instantiate(hud.transform.Find("MainContainer").Find("NotificationArea").gameObject);
                notificationPanel.transform.SetParent(hud.transform.Find("MainContainer"), true);
                notificationPanel.GetComponent<RectTransform>().localPosition = new Vector3(0f, -265f, -150f);
                notificationPanel.transform.localScale = Vector3.one;

                NotificationUIController _old = notificationPanel.GetComponent<NotificationUIController>();
                WeaponNotificationUIController _new = notificationPanel.AddComponent<WeaponNotificationUIController>();

                _new.hud = _old.hud;
                _new.genericNotificationPrefab = Modules.Assets.weaponNotificationPrefab;
                _new.notificationQueue = hud.targetMaster.gameObject.AddComponent<WeaponNotificationQueue>();

                _old.enabled = false;

                // ammo display for alt passive
                Transform healthbarContainer = hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster").Find("BarRoots").Find("LevelDisplayCluster");

                GameObject ammoTracker = GameObject.Instantiate(healthbarContainer.gameObject, hud.transform.Find("MainContainer").Find("MainUIArea").Find("SpringCanvas").Find("BottomLeftCluster"));
                ammoTracker.name = "AmmoTracker";
                ammoTracker.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

                GameObject.DestroyImmediate(ammoTracker.transform.GetChild(0).gameObject);
                MonoBehaviour.Destroy(ammoTracker.GetComponentInChildren<LevelText>());
                MonoBehaviour.Destroy(ammoTracker.GetComponentInChildren<ExpBar>());

                ammoTracker.transform.Find("LevelDisplayRoot").Find("ValueText").gameObject.SetActive(false);
                GameObject.DestroyImmediate(ammoTracker.transform.Find("ExpBarRoot").gameObject);

                ammoTracker.transform.Find("LevelDisplayRoot").GetComponent<RectTransform>().anchoredPosition = new Vector2(-12f, 0f);

                rect = ammoTracker.GetComponent<RectTransform>();
                rect.localScale = new Vector3(0.8f, 0.8f, 1f);
                rect.anchorMin = new Vector2(0f, 0f);
                rect.anchorMax = new Vector2(0f, 0f);
                rect.offsetMin = new Vector2(120f, -40f);
                rect.offsetMax = new Vector2(120f, -40f);
                rect.pivot = new Vector2(0.5f, 0f);
                //positional data doesnt get sent to clients? Manually making offsets works..
                rect.anchoredPosition = new Vector2(50f, 0f);
                rect.localPosition = new Vector3(120f, -40f, 0f);

                GameObject chargeBarAmmo = GameObject.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("WeaponChargeBar"));
                chargeBarAmmo.name = "AmmoBar";
                chargeBarAmmo.transform.SetParent(hud.transform.Find("MainContainer").Find("MainUIArea").Find("CrosshairCanvas").Find("CrosshairExtras"));

                rect = chargeBarAmmo.GetComponent<RectTransform>();

                rect.localScale = new Vector3(0.75f, 0.1f, 1f);
                rect.anchorMin = new Vector2(100f, 2f);
                rect.anchorMax = new Vector2(100f, 2f);
                rect.pivot = new Vector2(0.5f, 0f);
                rect.anchoredPosition = new Vector2(100f, 2f);
                rect.localPosition = new Vector3(100f, 2f, 0f);
                rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

                AmmoDisplay ammoTrackerComponent = ammoTracker.AddComponent<AmmoDisplay>();

                ammoTrackerComponent.targetHUD = hud;
                ammoTrackerComponent.targetText = ammoTracker.transform.Find("LevelDisplayRoot").Find("PrefixText").gameObject.GetComponent<LanguageTextMeshController>();
                ammoTrackerComponent.durationDisplay = chargeBarAmmo;
                ammoTrackerComponent.durationBar = chargeBarAmmo.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>();
                ammoTrackerComponent.durationBarRed = chargeBarAmmo.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();

            }
        }

        private static void RiskUIHudSetup(RoR2.UI.HUD hud)
        {
            // Get rid of old hud, im tired of fighting this
            var eIcon = hud.equipmentIcons.First();
            var weaponSlot = eIcon.transform.parent.Find("WeaponSlot")?.gameObject;
            if (weaponSlot) GameObject.Destroy(weaponSlot);

            weaponSlot = GameObject.Instantiate(eIcon.gameObject, eIcon.transform.parent);
            weaponSlot.name = "WeaponSlot";
            MonoBehaviour.Destroy(weaponSlot.GetComponent<BepinConfigParentManager>());

            EquipmentIcon equipmentIconComponent = weaponSlot.GetComponent<EquipmentIcon>();
            Components.WeaponIcon weaponIconComponent = weaponSlot.AddComponent<Components.WeaponIcon>();

            // whoever deleted the stock flash animations is a bad guy
            weaponIconComponent.iconImage = equipmentIconComponent.iconImage;
            weaponIconComponent.displayRoot = equipmentIconComponent.displayRoot;
            weaponIconComponent.flashPanelObject = equipmentIconComponent.stockFlashPanelObject;
            weaponIconComponent.reminderFlashPanelObject = equipmentIconComponent.reminderFlashPanelObject;
            weaponIconComponent.isReadyPanelObject = equipmentIconComponent.isReadyPanelObject;
            weaponIconComponent.tooltipProvider = equipmentIconComponent.tooltipProvider;
            weaponIconComponent.targetHUD = hud;

            var weaponIcon = weaponSlot.AddComponent<Components.MaterialWeaponIcon>();

            weaponIcon.targetHUD = hud;
            weaponIcon.icon = weaponIconComponent;
            weaponIcon.mask = weaponSlot.transform.Find("DisplayRoot").Find("Mask").gameObject.GetComponent<UnityEngine.UI.Image>();
            weaponIcon.cooldownRing = weaponSlot.transform.Find("DisplayRoot").Find("Mask").Find("CooldownRing").gameObject.GetComponent<UnityEngine.UI.Image>();
            weaponIcon.cooldownRing.fillCenter = false;

            RectTransform iconRect = weaponSlot.GetComponent<RectTransform>();
            iconRect.localScale = new Vector3(2f, 2f, 2f);
            iconRect.anchoredPosition = new Vector2(-128f, 60f);

            if (DriverPlugin.extendedLoadoutInstalled)
            {
                iconRect.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                iconRect.anchoredPosition = new Vector2(-110f, 60f);
            }
            // text for ammo type
            weaponIcon.ammoBackground = weaponSlot.transform.Find("DisplayRoot").Find("BottomContainer").Find("StockTextContainer").gameObject;
            weaponIcon.ammoBackground.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 2.5f);
            weaponIcon.ammoBackground.GetComponent<RectTransform>().localScale = new Vector3(0.8f, -0.8f, 0.8f);
            weaponIcon.ammoBackground.transform.SetAsFirstSibling();

            weaponIcon.ammoText = weaponIcon.ammoBackground.transform.Find("StockText").gameObject.GetComponent<TextMeshProUGUI>();

            GameObject.Destroy(weaponSlot.transform.Find("DisplayRoot").Find("BottomContainer").Find("SkillBackgroundPanel").gameObject);
            GameObject.Destroy(weaponSlot.transform.Find("DisplayRoot").Find("CooldownText").gameObject);
            weaponSlot.transform.Find("DisplayRoot").Find("BgImage").Find("IconPanel").Find("OnCooldown").gameObject.SetActive(false);
            MonoBehaviour.Destroy(weaponIcon.cooldownRing.GetComponent<RedToColorRemapperIndividual>());
            MonoBehaviour.Destroy(weaponSlot.transform.Find("DisplayRoot").Find("BottomContainer").gameObject.GetComponent<HideFromBepinConfig>());
            MonoBehaviour.Destroy(weaponSlot.GetComponent<MaterialHud.MaterialEquipmentIcon>());
            MonoBehaviour.Destroy(equipmentIconComponent);

            // duration bar
            /**
            GameObject chargeBar = GameObject.Instantiate(Assets.mainAssetBundle.LoadAsset<GameObject>("WeaponChargeBar"));
            chargeBar.transform.SetParent(weaponSlot.transform.Find("DisplayRoot"));

            RectTransform rect = chargeBar.GetComponent<RectTransform>();

            rect.localScale = new Vector3(0.75f, 0.1f, 1f);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.localPosition = new Vector3(0f, 0f, 0f);
            rect.anchoredPosition = new Vector2(-8f, 36f);
            rect.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));

            weaponIconComponent.durationDisplay = chargeBar;
            weaponIconComponent.durationBar = chargeBar.transform.GetChild(1).gameObject.GetComponent<UnityEngine.UI.Image>();
            weaponIconComponent.durationBarRed = chargeBar.transform.GetChild(0).gameObject.GetComponent<UnityEngine.UI.Image>();
            **/
            // weapon pickup notification

            GameObject notificationPanel = GameObject.Instantiate(hud.transform.Find("MainContainer").Find("NotificationArea").gameObject);
            notificationPanel.transform.SetParent(hud.transform.Find("MainContainer"), true);
            notificationPanel.GetComponent<RectTransform>().localPosition = new Vector3(0f, -210f, -50f);
            notificationPanel.transform.localScale = Vector3.one;

            NotificationUIController _old = notificationPanel.GetComponent<NotificationUIController>();
            WeaponNotificationUIController _new = notificationPanel.AddComponent<WeaponNotificationUIController>();

            _new.hud = _old.hud;
            _new.genericNotificationPrefab = Modules.Assets.weaponNotificationPrefab;
            _new.notificationQueue = hud.targetMaster.gameObject.AddComponent<WeaponNotificationQueue>();

            _old.enabled = false;
        }

        private static void PlayVisionsAnimation(On.EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.orig_OnEnter orig, EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle self)
        {
            orig(self);

            if (self.characterBody.baseNameToken == Driver.bodyNameToken)
            {
                self.PlayAnimation("Gesture, Override", "Shoot", "Shoot.playbackRate", self.duration * 12f);
                EffectManager.SimpleMuzzleFlash(EntityStates.GlobalSkills.LunarNeedle.FireLunarNeedle.muzzleFlashEffectPrefab, self.gameObject, "PistolMuzzle", false);
            }
        }

        private static void PlayChargeLunarAnimation(On.EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary.orig_PlayChargeAnimation orig, EntityStates.GlobalSkills.LunarNeedle.ChargeLunarSecondary self)
        {
            orig(self);

            if (self.characterBody.baseNameToken == Driver.bodyNameToken)
            {
                self.PlayAnimation("Gesture, Override", "ChargeHooks", "Hooks.playbackRate", self.duration * 0.5f);
            }
        }

        private static void PlayThrowLunarAnimation(On.EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary.orig_PlayThrowAnimation orig, EntityStates.GlobalSkills.LunarNeedle.ThrowLunarSecondary self)
        {
            orig(self);

            if (self.characterBody.baseNameToken == Driver.bodyNameToken)
            {
                self.PlayAnimation("Gesture, Override", "ThrowHooks", "Hooks.playbackRate", self.duration);
            }
        }

        private static void PlayRuinAnimation(On.EntityStates.GlobalSkills.LunarDetonator.Detonate.orig_OnEnter orig, EntityStates.GlobalSkills.LunarDetonator.Detonate self)
        {
            orig(self);

            if (self.characterBody.baseNameToken == Driver.bodyNameToken)
            {
                //self.PlayAnimation("Gesture, Override", "CastRuin", "Ruin.playbackRate", self.duration * 0.5f);
                //Util.PlaySound("PaladinFingerSnap", self.gameObject);
                self.PlayAnimation("Gesture, Override", "PressVoidButton", "Action.playbackRate", 0.5f * self.duration);
                self.StartAimMode(self.duration + 0.5f);

                EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterExplosion.prefab").WaitForCompletion(),
                    new EffectData
                    {
                        origin = self.FindModelChild("HandL").position,
                        rotation = Quaternion.identity,
                        scale = 0.5f
                    }, false);
            }
        }
    }
}
