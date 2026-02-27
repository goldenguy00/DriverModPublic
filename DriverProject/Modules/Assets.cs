using System.Collections.Generic;
using HunkMod.Modules.Survivors;
using R2API;
using RobDriver.Modules.Components;
using RobDriver.Modules.Components.UI;
using RoR2;
using RoR2.UI;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace RobDriver.Modules
{
    public static class Assets
    {
        public static AssetBundle mainAssetBundle;

        internal static List<EffectDef> effectDefs = new List<EffectDef>();
        internal static List<NetworkSoundEventDef> networkSoundEventDefs = new List<NetworkSoundEventDef>();

        #region Fields
        public static NetworkSoundEventDef hammerImpactSoundDef;
        public static NetworkSoundEventDef knifeImpactSoundDef;

        public static GameObject badassExplosionEffect;
        public static GameObject badassSmallExplosionEffect;
        public static GameObject explosionEffect;

        public static GameObject jammedEffectPrefab;
        public static GameObject upgradeEffectPrefab;
        public static GameObject damageBuffEffectPrefab;
        public static GameObject attackSpeedBuffEffectPrefab;
        public static GameObject critBuffEffectPrefab;
        public static GameObject scepterSyringeBuffEffectPrefab;

        public static GameObject damageBuffEffectPrefab2;
        public static GameObject attackSpeedBuffEffectPrefab2;
        public static GameObject critBuffEffectPrefab2;
        public static GameObject scepterSyringeBuffEffectPrefab2;

        public static GameObject defaultCrosshairPrefab;
        public static GameObject pistolAimCrosshairPrefab;
        public static GameObject revolverCrosshairPrefab;
        public static GameObject smgCrosshairPrefab;
        public static GameObject bazookaCrosshairPrefab;
        public static GameObject rocketLauncherCrosshairPrefab;
        public static GameObject grenadeLauncherCrosshairPrefab;
        public static GameObject needlerCrosshairPrefab;
        public static GameObject shotgunCrosshairPrefab;
        public static GameObject circleCrosshairPrefab;

        public static GameObject weaponNotificationPrefab;
        public static GameObject headshotOverlay;
        public static GameObject headshotVisualizer;

        public static GameObject bloodExplosionEffect;
        public static GameObject bloodSpurtEffect;
        public static GameObject coinTracer;
        public static GameObject coinImpact;
        public static GameObject coinOrbEffect;

        public static GameObject shotgunShell;
        public static GameObject shotgunSlug;

        public static Material pistolMat;
        public static Material nemKatanaMat;
        public static Material mainMat;
        public static Material clothMat;
        public static Material tieMat;
        public static Material buttonMat;
        public static Material skateboardMat;
        public static Material knifeMat;
        public static Material timbsMat;

        public static GameObject weaponPickup;
        public static GameObject commonPickupModel;
        public static GameObject uncommonPickupModel;
        public static GameObject legendaryPickupModel;
        public static GameObject uniquePickupModel;
        public static GameObject voidPickupModel;
        public static GameObject lunarPickupModel;
        public static GameObject ammoPickupModel;

        public static GameObject weaponPickupEffect;
        public static GameObject discardedWeaponEffect;
        public static GameObject backWeaponEffect;

        public static GameObject knifeImpactEffect;
        public static GameObject knifeSwingEffect;

        public static GameObject defaultMuzzleTrail;

        public static Sprite commonWeaponIcon;
        public static Sprite uncommonWeaponIcon;
        public static Sprite legendaryWeaponIcon;
        public static Sprite uniqueWeaponIcon;
        public static Sprite voidWeaponIcon;
        public static Sprite lunarWeaponIcon;

        public static Sprite bulletSprite;
        
        public static GameObject shotgunTracer;
        public static GameObject shotgunTracerCrit;
        public static GameObject sniperTracer;
        public static GameObject lunarTracer;
        public static GameObject chargedLunarTracer;
        public static GameObject lunarRifleTracer;
        public static GameObject nemmandoTracer;
        public static GameObject nemmercTracer;

        public static GameObject lunarShardMuzzleFlash;
        public static GameObject redSlashImpactEffect;
        public static GameObject redKnifeSlashEffect;
        public static GameObject redKatanaSwing;
        public static GameObject lunarShardMuzzleFlashRed;
        public static GameObject ravagerSlashEffect;
        public static GameObject ravagerBigSlashEffect;
        public static GameObject consumeOrb;

        public static Material syringeDamageOverlayMat;
        public static Material syringeAttackSpeedOverlayMat;
        public static Material syringeCritOverlayMat;
        public static Material syringeScepterOverlayMat;
        public static Material woundOverlayMat;
        #endregion

        internal static void PopulateAssets()
        {
            var path = System.IO.Path.GetDirectoryName(DriverPlugin.instance.Info.Location);
            if (mainAssetBundle == null)
            {
                mainAssetBundle = AssetBundle.LoadFromFile(System.IO.Path.Combine(path, "robdriver"));
            }

            Hunk.heartCostTypeIndex = System.Array.IndexOf(CostTypeCatalog.costTypeDefs, Hunk.heartCostDef);
            Hunk.spadeCostTypeIndex = System.Array.IndexOf(CostTypeCatalog.costTypeDefs, Hunk.spadeCostDef);
            Hunk.clubCostTypeIndex = System.Array.IndexOf(CostTypeCatalog.costTypeDefs, Hunk.clubCostDef);
            Hunk.diamondCostTypeIndex = System.Array.IndexOf(CostTypeCatalog.costTypeDefs, Hunk.diamondCostDef);
            Hunk.starCostTypeIndex = System.Array.IndexOf(CostTypeCatalog.costTypeDefs, Hunk.starCostDef);
            Hunk.wristbandCostTypeIndex = System.Array.IndexOf(CostTypeCatalog.costTypeDefs, Hunk.wristbandCostDef);
            Hunk.starsBadgeCostTypeIndex = System.Array.IndexOf(CostTypeCatalog.costTypeDefs, Hunk.starsBadgeCostDef);
            Hunk.sampleCostTypeIndex = System.Array.IndexOf(CostTypeCatalog.costTypeDefs, Hunk.sampleCostDef);

            Modules.Config.InitROO(Assets.mainAssetBundle.LoadAsset<Sprite>("texDriverIcon"), "Literally me");

            SwapAllShaders();

            jammedEffectPrefab = CreateTextPopupEffect("DriverGunJammedEffect", "ROB_DRIVER_JAMMED_POPUP");
            damageBuffEffectPrefab = CreateTextPopupEffect("DriverDamageBuffEffect", "DAMAGE!", new Color(1f, 70f / 255f, 75f / 255f));
            attackSpeedBuffEffectPrefab = CreateTextPopupEffect("DriverAttackSpeedBuffEffect", "ATTACK SPEED!", new Color(1f, 170f / 255f, 45f / 255f));
            critBuffEffectPrefab = CreateTextPopupEffect("DriverCritBuffEffect", "CRITICAL CHANCE!", new Color(1f, 80f / 255f, 17f / 255f));
            scepterSyringeBuffEffectPrefab = CreateTextPopupEffect("DriverScepterSyringeBuffEffect", "POWER!!!!", Modules.Survivors.Driver.characterColor);

            upgradeEffectPrefab = CreateTextPopupEffect("DriverGunUpgradeEffect", "ROB_DRIVER_UPGRADE_POPUP");

            syringeDamageOverlayMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidMegaCrab/matVoidCrabMatterOverlay.mat").WaitForCompletion());
            syringeDamageOverlayMat.SetColor("_TintColor", new Color(1f, 70f / 255f, 75f / 255f));

            syringeAttackSpeedOverlayMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidMegaCrab/matVoidCrabMatterOverlay.mat").WaitForCompletion());
            syringeAttackSpeedOverlayMat.SetColor("_TintColor", new Color(1f, 170f / 255f, 45f / 255f));

            syringeCritOverlayMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidMegaCrab/matVoidCrabMatterOverlay.mat").WaitForCompletion());
            syringeCritOverlayMat.SetColor("_TintColor", new Color(1f, 80f / 255f, 17f / 255f));

            syringeScepterOverlayMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidMegaCrab/matVoidCrabMatterOverlay.mat").WaitForCompletion());
            syringeScepterOverlayMat.SetColor("_TintColor", Modules.Survivors.Driver.characterColor);

            woundOverlayMat = Material.Instantiate(Addressables.LoadAssetAsync<Material>("RoR2/Base/ArmorReductionOnHit/matPulverizedOverlay.mat").WaitForCompletion());
            woundOverlayMat.SetColor("_TintColor", Color.red);

            hammerImpactSoundDef = CreateNetworkSoundEventDef("sfx_driver_impact_hammer");
            knifeImpactSoundDef = CreateNetworkSoundEventDef("sfx_driver_knife_hit");

            headshotOverlay = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/RailgunnerScopeLightOverlay.prefab").WaitForCompletion().InstantiateClone("DriverHeadshotOverlay", false);
            SniperTargetViewer viewer = headshotOverlay.GetComponentInChildren<SniperTargetViewer>();
            headshotOverlay.transform.Find("ScopeOverlay").gameObject.SetActive(false);

            headshotVisualizer = viewer.visualizerPrefab.InstantiateClone("DriverHeadshotVisualizer", false);
            viewer.visualizerPrefab = headshotVisualizer;
            Image headshotImage = headshotVisualizer.transform.Find("Scaler/Rectangle").GetComponent<Image>();
            headshotVisualizer.transform.Find("Scaler/Outer").gameObject.SetActive(false);
            headshotImage.color = Color.red;

            mainMat = LoadMaterial("matDriver");
            clothMat = LoadMaterial("matSlugger");
            tieMat = LoadMaterial("matSuit");
            buttonMat = LoadMaterial("matButton");
            pistolMat = LoadMaterial("matPistol");
            knifeMat = LoadMaterial("matKnife");
            skateboardMat = LoadMaterial("matSkateboard");
            nemKatanaMat = LoadMaterial("matNemKatana");
            timbsMat = LoadMaterial("matTimbs");

            shotgunShell = mainAssetBundle.LoadAsset<GameObject>("ShotgunShell");
            shotgunShell.GetComponentInChildren<MeshRenderer>().material = LoadMaterial("matShotgunShell");
            shotgunShell.AddComponent<ShellController>();

            shotgunSlug = mainAssetBundle.LoadAsset<GameObject>("ShotgunSlug");
            shotgunSlug.GetComponentInChildren<MeshRenderer>().material = LoadMaterial("matShotgunSlug");
            shotgunSlug.AddComponent<ShellController>();

            commonWeaponIcon = mainAssetBundle.LoadAsset<Sprite>("texGenericWeaponGrey");
            uncommonWeaponIcon = mainAssetBundle.LoadAsset<Sprite>("texGenericWeaponGreen");
            legendaryWeaponIcon = mainAssetBundle.LoadAsset<Sprite>("texGenericWeaponRed");
            uniqueWeaponIcon = mainAssetBundle.LoadAsset<Sprite>("texGenericWeaponYellow");
            voidWeaponIcon = mainAssetBundle.LoadAsset<Sprite>("texGenericWeaponPurple");
            lunarWeaponIcon = mainAssetBundle.LoadAsset<Sprite>("texGenericWeaponBlue");

            weaponNotificationPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/NotificationPanel2.prefab").WaitForCompletion().InstantiateClone("WeaponNotification", false);

            CreateVfx();
            CreateCrosshair();
            CreateWeaponPickups();
            CreateCoin();
            CreateOrb();
        }

        #region Assets
        private static void CreateVfx()
        {
            #region Vfx
            #region Explosion
            badassExplosionEffect = LoadEffect("BigExplosion", "sfx_driver_explosion_badass", false);
            badassExplosionEffect.transform.Find("Shockwave").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matDistortion.mat").WaitForCompletion();
            ShakeEmitter shake = badassExplosionEffect.AddComponent<ShakeEmitter>();
            ShakeEmitter shake2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/BFG/BeamSphereExplosion.prefab").WaitForCompletion().GetComponent<ShakeEmitter>();
            shake.shakeOnStart = true;
            shake.shakeOnEnable = false;
            shake.wave = shake2.wave;
            shake.duration = 0.5f;
            shake.radius = 200f;
            shake.scaleShakeRadiusWithLocalScale = false;
            shake.amplitudeTimeDecay = true;

            badassSmallExplosionEffect = LoadEffect("SmallExplosion", "sfx_driver_grenade_explosion_badass", false);
            badassSmallExplosionEffect.transform.Find("Shockwave").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matDistortion.mat").WaitForCompletion();
            shake = badassSmallExplosionEffect.AddComponent<ShakeEmitter>();
            shake.shakeOnStart = true;
            shake.shakeOnEnable = false;
            shake.wave = shake2.wave;
            shake.duration = 0.5f;
            shake.radius = 60f;
            shake.scaleShakeRadiusWithLocalScale = false;
            shake.amplitudeTimeDecay = true;

            explosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/OmniExplosionVFX.prefab").WaitForCompletion().InstantiateClone("DriverSmallStupidFuckExplosion", true);
            explosionEffect.AddComponent<NetworkIdentity>();

            GameObject nadeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/OmniExplosionVFXCommandoGrenade.prefab").WaitForCompletion();
            GameObject radiusIndicator = GameObject.Instantiate(nadeEffect.transform.Find("Nova Sphere").gameObject);
            radiusIndicator.transform.parent = explosionEffect.transform;
            radiusIndicator.transform.localPosition = Vector3.zero;
            radiusIndicator.transform.localScale = Vector3.one;
            radiusIndicator.transform.localRotation = Quaternion.identity;

            Assets.AddNewEffectDef(explosionEffect, "sfx_driver_explosion");
            #endregion

            #region Tracers
            shotgunTracer = CreateTracer("TracerCommandoShotgun", "TracerDriverShotgun", new Color(0.68f, 0.58f, 0.05f), new Color(0.68f, 0.58f, 0.05f));
            shotgunTracerCrit = CreateTracer("TracerCommandoShotgun", "TracerDriverShotgunCritical", Color.yellow, new Color(0.8f, 0.24f, 0f));
            lunarTracer = CreateTracer("TracerCommandoShotgun", "TracerDriverLunarPistol", new Color(0f, 102f / 255f, 1f), new Color(0f, 102f / 255f, 1f));
            nemmandoTracer = CreateTracer("TracerCommandoShotgun", "TracerDriverNemmandoPistol", Color.red, Color.red);
            nemmercTracer = CreateTracer("TracerCommandoShotgun", "TracerDriverNemmercShotgun", new Color(0f, 102f / 255f, 1f), new Color(0f, 102f / 255f, 1f));
            var ty = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/FalseSon/FalseSonLaserTracer.prefab").WaitForCompletion();

            lunarRifleTracer = CreateTracer("TracerGolem", "TracerDriverLunarRifle");
            lunarRifleTracer.transform.Find("SmokeBeam").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarGolem/matLunarGolemChargeGlow.mat").WaitForCompletion();
            lunarRifleTracer.transform.Find("SmokeBeam").transform.localScale = new Vector3(1f, 0.25f, 0.25f);

            sniperTracer = CreateTracer("TracerHuntressSnipe", "TracerDriverSniperRifle");
            sniperTracer.GetComponent<Tracer>().speed = 250f;
            sniperTracer.GetComponent<Tracer>().length = 50f;

            LineRenderer line = sniperTracer.transform.Find("TracerHead").GetComponent<LineRenderer>();
            line.startWidth *= 0.25f;
            line.endWidth *= 0.25f;
            // this did not work.
            line.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/MagmaWorm/matMagmaWormFireballTrail.mat").WaitForCompletion();

            chargedLunarTracer = CreateTracer("TracerHuntressSnipe", "TracerDriverLunarPistolCharged");
            chargedLunarTracer.GetComponent<Tracer>().speed = 250f;
            chargedLunarTracer.GetComponent<Tracer>().length = 50f;

            line = chargedLunarTracer.transform.Find("TracerHead").GetComponent<LineRenderer>();
            line.startWidth *= 0.25f;
            line.endWidth *= 0.25f;
            // this did not work.
            line.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/EliteLunar/matEliteLunarDonut.mat").WaitForCompletion();
            #endregion
            
            #region Muzzle Effects
            GameObject obj = new GameObject();
            defaultMuzzleTrail = obj.InstantiateClone("DriverPassiveMuzzleTrail", false);
            TrailRenderer trail = defaultMuzzleTrail.AddComponent<TrailRenderer>();
            trail.startWidth = 0.045f;
            trail.endWidth = 0f;
            trail.time = 0.5f;
            trail.emitting = true;
            trail.numCornerVertices = 0;
            trail.numCapVertices = 0;
            trail.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matSmokeTrail.mat").WaitForCompletion();
            trail.startColor = Color.white;
            trail.endColor = Color.gray;
            bulletSprite = Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texSniperBulletIndicator");

            lunarShardMuzzleFlash = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Brother/MuzzleflashLunarShard.prefab").WaitForCompletion().InstantiateClone("DriverMuzzleflashLunarShard", false);
            lunarShardMuzzleFlash.transform.GetChild(0).transform.localScale = Vector3.one * 0.35f;
            lunarShardMuzzleFlash.transform.GetChild(1).transform.localScale = Vector3.one * 0.35f;
            lunarShardMuzzleFlash.transform.GetChild(2).transform.localScale = Vector3.one * 0.35f;

            AddNewEffectDef(lunarShardMuzzleFlash);

            lunarShardMuzzleFlashRed = lunarShardMuzzleFlash.InstantiateClone("DriverMuzzleFlashLunarShardRed", false);
            var main = lunarShardMuzzleFlashRed.transform.GetChild(0).GetComponent<ParticleSystem>().main;
            main.startColor = Color.red;
            var shit = lunarShardMuzzleFlashRed.transform.GetChild(1).GetComponent<ParticleSystem>().colorOverLifetime;
            shit.enabled = false;
            lunarShardMuzzleFlashRed.transform.GetChild(1).GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.black);
            lunarShardMuzzleFlashRed.transform.GetChild(2).GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);

            AddNewEffectDef(lunarShardMuzzleFlashRed);
            #endregion

            #region Slash Effects
            redSlashImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion().InstantiateClone("DriverRedSwordImpact", false);
            redSlashImpactEffect.GetComponent<OmniEffect>().enabled = false;
            redSlashImpactEffect.transform.localScale = Vector3.one * 1.5f;
            var t = redSlashImpactEffect.transform;

            t.GetChild(1).gameObject.SetActive(true);
            t.GetChild(1).localScale = Vector3.one * 1.5f;
            t.GetChild(1).GetComponent<ParticleSystemRenderer>().material = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniHitspark3Merc.mat").WaitForCompletion());
            t.GetChild(1).GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);

            t.GetChild(2).gameObject.SetActive(true);
            t.GetChild(2).localScale = Vector3.one * 1.5f;
            t.GetChild(2).GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/VoidSurvivor/matVoidSurvivorBlasterFireCorrupted.mat").WaitForCompletion();

            t.GetChild(3).gameObject.SetActive(true);

            t.GetChild(4).gameObject.SetActive(true);
            t.GetChild(4).localScale = Vector3.one * 3f;
            t.GetChild(4).GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDust.mat").WaitForCompletion();

            t.GetChild(5).gameObject.SetActive(true);
            t.GetChild(5).GetComponent<ParticleSystemRenderer>().material = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniRadialSlash1Merc.mat").WaitForCompletion());
            t.GetChild(5).GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.red);

            t.GetChild(6).gameObject.SetActive(true);
            t.GetChild(6).transform.localScale = new Vector3(1f, 1f, 3f);
            t.GetChild(6).GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark2Void.mat").WaitForCompletion();

            t.GetChild(6).GetChild(0).gameObject.SetActive(true);
            t.GetChild(6).GetChild(0).GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/DLC1/Common/Void/matOmniHitspark1Void.mat").WaitForCompletion();

            AddNewEffectDef(redSlashImpactEffect);

            //***
            redKatanaSwing = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("DriverRedSwordSwing", false);
            redKatanaSwing.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            var sex = redKatanaSwing.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.6f;
            redKatanaSwing.transform.GetChild(0).localScale = Vector3.one * 2f;
            Object.Destroy(redKatanaSwing.GetComponent<EffectComponent>());

            //***
            redKnifeSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("DriverRedKnifeSwing", false);
            redKnifeSlashEffect.transform.GetChild(0).gameObject.SetActive(false);
            redKnifeSlashEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();

            //***
            ravagerSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordFinisherSlash.prefab").WaitForCompletion().InstantiateClone("DriverRavSwordSwing");
            ravagerSlashEffect.transform.GetChild(0).gameObject.SetActive(false);
            ravagerSlashEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();

            //***
            ravagerBigSlashEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlashWhirlwind.prefab").WaitForCompletion().InstantiateClone("DriverRavBigSwordSwing");
            ravagerBigSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpSwipe.mat").WaitForCompletion();
            sex = ravagerBigSlashEffect.transform.GetChild(0).gameObject.GetComponent<ParticleSystem>().main;
            sex.startLifetimeMultiplier = 0.6f;
            ravagerBigSlashEffect.transform.GetChild(0).localScale = Vector3.one * 2f;
            Object.Destroy(ravagerBigSlashEffect.GetComponent<EffectComponent>());

            //***
            knifeSwingEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/MercSwordSlash.prefab").WaitForCompletion().InstantiateClone("DriverKnifeSwing", false);
            knifeSwingEffect.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matHuntressSwingTrail.mat").WaitForCompletion();
            
            //***
            knifeImpactEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Merc/OmniImpactVFXSlashMerc.prefab").WaitForCompletion().InstantiateClone("DriverKnifeImpact", false);
            knifeImpactEffect.GetComponent<OmniEffect>().enabled = false;

            knifeImpactEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material = new Material(Addressables.LoadAssetAsync<Material>("RoR2/Base/Merc/matOmniHitspark3Merc.mat").WaitForCompletion());
            knifeImpactEffect.transform.GetChild(1).gameObject.GetComponent<ParticleSystemRenderer>().material.SetColor("_TintColor", Color.white);

            knifeImpactEffect.transform.GetChild(2).localScale = Vector3.one * 1.5f;
            knifeImpactEffect.transform.GetChild(2).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Huntress/matOmniRing2Huntress.mat").WaitForCompletion();

            //slashMat.SetColor("_TintColor", Color.white);

            knifeImpactEffect.transform.GetChild(5).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniRadialSlash1Generic.mat").WaitForCompletion();

            //knifeImpactEffect.transform.GetChild(4).localScale = Vector3.one * 3f;
            //knifeImpactEffect.transform.GetChild(4).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Imp/matImpDust.mat").WaitForCompletion();

            knifeImpactEffect.transform.GetChild(6).GetChild(0).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarWisp/matOmniHitspark1LunarWisp.mat").WaitForCompletion();
            knifeImpactEffect.transform.GetChild(6).gameObject.GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniHitspark2Generic.mat").WaitForCompletion();

            knifeImpactEffect.transform.GetChild(1).localScale = Vector3.one * 1.5f;

            knifeImpactEffect.transform.GetChild(1).gameObject.SetActive(true);
            knifeImpactEffect.transform.GetChild(2).gameObject.SetActive(true);
            knifeImpactEffect.transform.GetChild(3).gameObject.SetActive(true);
            knifeImpactEffect.transform.GetChild(4).gameObject.SetActive(true);
            knifeImpactEffect.transform.GetChild(5).gameObject.SetActive(true);
            knifeImpactEffect.transform.GetChild(6).gameObject.SetActive(true);
            knifeImpactEffect.transform.GetChild(6).GetChild(0).gameObject.SetActive(true);

            knifeImpactEffect.transform.GetChild(6).transform.localScale = new Vector3(1f, 1f, 3f);

            knifeImpactEffect.transform.localScale = Vector3.one * 1.5f;

            AddNewEffectDef(knifeImpactEffect);
            #endregion

            #region Buff Effects
            damageBuffEffectPrefab2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/LevelUpEffectEnemy.prefab").WaitForCompletion().InstantiateClone("DriverDamageBuffEffect2", false);
            damageBuffEffectPrefab2.transform.Find("Ring").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniRing2Generic.mat").WaitForCompletion();
            damageBuffEffectPrefab2.transform.Find("Spinner").gameObject.SetActive(false);
            damageBuffEffectPrefab2.transform.Find("TextCamScaler").gameObject.SetActive(false);
            foreach (ParticleSystem i in damageBuffEffectPrefab2.GetComponentsInChildren<ParticleSystem>())
            {
                var j = i.main;
                j.startColor = new Color(1f, 70f / 255f, 75f / 255f);
            }
            AddNewEffectDef(damageBuffEffectPrefab2);

            attackSpeedBuffEffectPrefab2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/LevelUpEffectEnemy.prefab").WaitForCompletion().InstantiateClone("DriverAttackSpeedBuffEffect2", false);
            attackSpeedBuffEffectPrefab2.transform.Find("Ring").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniRing2Generic.mat").WaitForCompletion();
            attackSpeedBuffEffectPrefab2.transform.Find("Spinner").gameObject.SetActive(false);
            attackSpeedBuffEffectPrefab2.transform.Find("TextCamScaler").gameObject.SetActive(false);
            foreach (ParticleSystem i in attackSpeedBuffEffectPrefab2.GetComponentsInChildren<ParticleSystem>())
            {
                var j = i.main;
                j.startColor = new Color(1f, 170f / 255f, 45f / 255f);
            }
            AddNewEffectDef(attackSpeedBuffEffectPrefab2);

            critBuffEffectPrefab2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/LevelUpEffectEnemy.prefab").WaitForCompletion().InstantiateClone("DriverCritBuffEffect2", false);
            critBuffEffectPrefab2.transform.Find("Ring").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniRing2Generic.mat").WaitForCompletion();
            critBuffEffectPrefab2.transform.Find("Spinner").gameObject.SetActive(false);
            critBuffEffectPrefab2.transform.Find("TextCamScaler").gameObject.SetActive(false);
            foreach (ParticleSystem i in critBuffEffectPrefab2.GetComponentsInChildren<ParticleSystem>())
            {
                var j = i.main;
                j.startColor = new Color(1f, 80f / 255f, 17f / 255f);
            }
            AddNewEffectDef(critBuffEffectPrefab2);

            scepterSyringeBuffEffectPrefab2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/LevelUpEffectEnemy.prefab").WaitForCompletion().InstantiateClone("DriverScepterSyringeBuffEffect2", false);
            scepterSyringeBuffEffectPrefab2.transform.Find("Ring").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniRing2Generic.mat").WaitForCompletion();
            scepterSyringeBuffEffectPrefab2.transform.Find("Spinner").gameObject.SetActive(false);
            scepterSyringeBuffEffectPrefab2.transform.Find("TextCamScaler").gameObject.SetActive(false);
            foreach (ParticleSystem i in scepterSyringeBuffEffectPrefab2.GetComponentsInChildren<ParticleSystem>())
            {
                var j = i.main;
                j.startColor = Survivors.Driver.characterColor;
            }
            AddNewEffectDef(scepterSyringeBuffEffectPrefab2);

            Material bloodMat = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion();
            Material bloodMat2 = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();

            bloodExplosionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ImpBoss/ImpBossBlink.prefab").WaitForCompletion().InstantiateClone("DriverBloodExplosion", false);
            bloodExplosionEffect.transform.Find("Particles/LongLifeNoiseTrails").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/LongLifeNoiseTrails, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/Dash").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/Dash, Bright").GetComponent<ParticleSystemRenderer>().material = bloodMat;
            bloodExplosionEffect.transform.Find("Particles/DashRings").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();
            bloodExplosionEffect.GetComponentInChildren<Light>().gameObject.SetActive(false);
            AddNewEffectDef(bloodExplosionEffect);

            bloodSpurtEffect = mainAssetBundle.LoadAsset<GameObject>("BloodSpurtEffect");
            bloodSpurtEffect.transform.Find("Blood").GetComponent<ParticleSystemRenderer>().material = bloodMat2;
            bloodSpurtEffect.transform.Find("Trails").GetComponent<ParticleSystemRenderer>().trailMaterial = bloodMat2;
            #endregion
            
            #region Weapon Prefabs
            discardedWeaponEffect = mainAssetBundle.LoadAsset<GameObject>("DiscardedWeapon");
            discardedWeaponEffect.AddComponent<DiscardedWeaponComponent>();
            discardedWeaponEffect.gameObject.layer = LayerIndex.ragdoll.intVal;

            backWeaponEffect = mainAssetBundle.LoadAsset<GameObject>("BackWeapon");
            backWeaponEffect.AddComponent<BackWeaponComponent>();
            backWeaponEffect.gameObject.layer = LayerIndex.ragdoll.intVal;
            backWeaponEffect.transform.localRotation = Quaternion.identity;
            backWeaponEffect.transform.localPosition = Vector3.zero;
            backWeaponEffect.transform.localScale = Vector3.one;
            #endregion
            #endregion
        }

        private static void CreateCrosshair()
        {
            #region Pistol Crosshair
            defaultCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion().InstantiateClone("DriverPistolCrosshair", false);
            if (!Modules.Config.enableCrosshairDot.Value) defaultCrosshairPrefab.GetComponent<RawImage>().enabled = false;
            if (Config.dynamicCrosshair.Value) defaultCrosshairPrefab.AddComponent<DynamicCrosshair>();
            #endregion

            #region Pistol Aim Mode Crosshair
            pistolAimCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion().InstantiateClone("DriverPistolAimCrosshair", false);
            if (!Modules.Config.enableCrosshairDot.Value) pistolAimCrosshairPrefab.GetComponent<RawImage>().enabled = false;
            if (Config.dynamicCrosshair.Value) pistolAimCrosshairPrefab.AddComponent<DynamicCrosshair>();

            GameObject stockHolder = GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mage/MageCrosshair.prefab").WaitForCompletion().transform.Find("Stock").gameObject);
            stockHolder.transform.SetParent(pistolAimCrosshairPrefab.transform);

            CrosshairController pistolCrosshair = pistolAimCrosshairPrefab.GetComponent<CrosshairController>();

            Sprite boolet = mainAssetBundle.LoadAsset<Sprite>("texBulletIndicator");
            stockHolder.transform.GetChild(0).GetComponent<Image>().sprite = boolet;
            stockHolder.transform.GetChild(0).GetComponent<RectTransform>().localScale *= 2.5f;
            stockHolder.transform.GetChild(1).GetComponent<Image>().sprite = boolet;
            stockHolder.transform.GetChild(1).GetComponent<RectTransform>().localScale *= 2.5f;
            stockHolder.transform.GetChild(2).GetComponent<Image>().sprite = boolet;
            stockHolder.transform.GetChild(2).GetComponent<RectTransform>().localScale *= 2.5f;
            stockHolder.transform.GetChild(3).GetComponent<Image>().sprite = boolet;
            stockHolder.transform.GetChild(3).GetComponent<RectTransform>().localScale *= 2.5f;

            pistolCrosshair.skillStockSpriteDisplays = new CrosshairController.SkillStockSpriteDisplay[]
            {
                new CrosshairController.SkillStockSpriteDisplay
                {
                    target = stockHolder.transform.GetChild(0).gameObject,
                    skillSlot = SkillSlot.Secondary,
                    minimumStockCountToBeValid = 1,
                    maximumStockCountToBeValid = 999
                },
                new CrosshairController.SkillStockSpriteDisplay
                {
                    target = stockHolder.transform.GetChild(1).gameObject,
                    skillSlot = SkillSlot.Secondary,
                    minimumStockCountToBeValid = 2,
                    maximumStockCountToBeValid = 999
                },
                new CrosshairController.SkillStockSpriteDisplay
                {
                    target = stockHolder.transform.GetChild(2).gameObject,
                    skillSlot = SkillSlot.Secondary,
                    minimumStockCountToBeValid = 3,
                    maximumStockCountToBeValid = 999
                },
                new CrosshairController.SkillStockSpriteDisplay
                {
                    target = stockHolder.transform.GetChild(3).gameObject,
                    skillSlot = SkillSlot.Secondary,
                    minimumStockCountToBeValid = 4,
                    maximumStockCountToBeValid = 999
                }
            };

            GameObject chargeBar = GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("ChargeBar"));
            chargeBar.transform.SetParent(pistolAimCrosshairPrefab.transform);

            RectTransform rect = chargeBar.GetComponent<RectTransform>();

            rect.localScale = new Vector3(0.75f, 0.075f, 1f);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(50f, 0f);
            rect.localPosition = new Vector3(0f, -60f, 0f);

            chargeBar.transform.GetChild(0).gameObject.AddComponent<CrosshairChargeBar>().crosshairController = pistolAimCrosshairPrefab.GetComponent<RoR2.UI.CrosshairController>();

            GameObject chargeRing = GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("ChargeRing"));
            chargeRing.transform.SetParent(pistolAimCrosshairPrefab.transform);

            rect = chargeRing.GetComponent<RectTransform>();

            rect.localScale = new Vector3(0.25f, 0.25f, 1f);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(50f, 0f);
            rect.localPosition = new Vector3(65f, -75f, 0f);

            chargeRing.transform.GetChild(0).gameObject.AddComponent<CrosshairChargeRing>().crosshairController = pistolAimCrosshairPrefab.GetComponent<RoR2.UI.CrosshairController>();
            #endregion

            #region Revolver Crosshair
            revolverCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion().InstantiateClone("DriverRevolverCrosshair", false);
            revolverCrosshairPrefab.GetComponent<RawImage>().enabled = false;
            if (Config.dynamicCrosshair.Value) revolverCrosshairPrefab.AddComponent<DynamicCrosshair>();
            revolverCrosshairPrefab.AddComponent<CrosshairStartRotate>();
            #endregion

            #region SMG Crosshair
            smgCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/UI/StandardCrosshair.prefab").WaitForCompletion().InstantiateClone("DriverSMGCrosshair", false);
            if (!Modules.Config.enableCrosshairDot.Value) smgCrosshairPrefab.GetComponent<RawImage>().enabled = false;
            if (Config.dynamicCrosshair.Value) smgCrosshairPrefab.AddComponent<DynamicCrosshair>();
            smgCrosshairPrefab.transform.GetChild(2).gameObject.SetActive(false);
            #endregion

            #region Bazooka Crosshair
            bazookaCrosshairPrefab = PrefabAPI.InstantiateClone(LoadCrosshair("ToolbotGrenadeLauncher"), "DriverBazookaCrosshair", false);
            CrosshairController crosshair = bazookaCrosshairPrefab.GetComponent<CrosshairController>();
            crosshair.skillStockSpriteDisplays = new CrosshairController.SkillStockSpriteDisplay[0];

            bazookaCrosshairPrefab.transform.GetChild(0).GetComponentInChildren<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunSniperNib.png").WaitForCompletion();
            rect = bazookaCrosshairPrefab.transform.GetChild(0).GetComponent<RectTransform>();
            rect.localEulerAngles = Vector3.zero;
            rect.anchoredPosition = new Vector2(-50f, -10f);

            bazookaCrosshairPrefab.transform.GetChild(1).GetComponentInChildren<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunSniperNib.png").WaitForCompletion();
            rect = bazookaCrosshairPrefab.transform.GetChild(1).GetComponent<RectTransform>();
            rect.localEulerAngles = new Vector3(0f, 0f, 90f);

            bazookaCrosshairPrefab.transform.GetChild(2).GetComponentInChildren<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunSniperNib.png").WaitForCompletion();
            rect = bazookaCrosshairPrefab.transform.GetChild(2).GetComponent<RectTransform>();
            rect.localEulerAngles = Vector3.zero;
            rect.anchoredPosition = new Vector2(50f, -10f);

            bazookaCrosshairPrefab.transform.Find("StockCountHolder").gameObject.SetActive(false);
            bazookaCrosshairPrefab.transform.Find("Image, Arrow (1)").gameObject.SetActive(true);

            crosshair.spriteSpreadPositions[0].zeroPosition = new Vector3(0f, 25f, 0f);
            crosshair.spriteSpreadPositions[0].onePosition = new Vector3(-50f, 25f, 0f);

            crosshair.spriteSpreadPositions[1].zeroPosition = new Vector3(100f, 0f, 0f);
            crosshair.spriteSpreadPositions[1].onePosition = new Vector3(150f, 0f, 0f);

            crosshair.spriteSpreadPositions[2].zeroPosition = new Vector3(0f, 25f, 0f);
            crosshair.spriteSpreadPositions[2].onePosition = new Vector3(50f, 25f, 0f);

            chargeBar = GameObject.Instantiate(mainAssetBundle.LoadAsset<GameObject>("ChargeBar"));
            chargeBar.transform.SetParent(bazookaCrosshairPrefab.transform);

            rect = chargeBar.GetComponent<RectTransform>();

            rect.localScale = new Vector3(0.5f, 0.1f, 1f);
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(0f, 0f);
            rect.pivot = new Vector2(0.5f, 0f);
            rect.anchoredPosition = new Vector2(50f, 0f);
            rect.localPosition = new Vector3(40f, -40f, 0f);
            rect.localEulerAngles = new Vector3(0f, 0f, 90f);

            chargeBar.transform.GetChild(0).gameObject.AddComponent<CrosshairChargeBar>().crosshairController = bazookaCrosshairPrefab.GetComponent<CrosshairController>();
            #endregion

            #region Grenade Launcher Crosshair
            grenadeLauncherCrosshairPrefab = PrefabAPI.InstantiateClone(LoadCrosshair("ToolbotGrenadeLauncher"), "DriverGrenadeLauncherCrosshair", false);
            if (Config.dynamicCrosshair.Value) grenadeLauncherCrosshairPrefab.AddComponent<DynamicCrosshair>();
            crosshair = grenadeLauncherCrosshairPrefab.GetComponent<CrosshairController>();
            crosshair.skillStockSpriteDisplays = new CrosshairController.SkillStockSpriteDisplay[0];

            grenadeLauncherCrosshairPrefab.transform.GetChild(0).GetComponentInChildren<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunSniperNib.png").WaitForCompletion();
            rect = grenadeLauncherCrosshairPrefab.transform.GetChild(0).GetComponent<RectTransform>();
            rect.localEulerAngles = Vector3.zero;
            rect.anchoredPosition = new Vector2(-50f, -10f);

            grenadeLauncherCrosshairPrefab.transform.GetChild(1).GetComponentInChildren<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunSniperNib.png").WaitForCompletion();
            rect = grenadeLauncherCrosshairPrefab.transform.GetChild(1).GetComponent<RectTransform>();
            rect.localEulerAngles = new Vector3(0f, 0f, 90f);

            grenadeLauncherCrosshairPrefab.transform.GetChild(2).GetComponentInChildren<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunSniperNib.png").WaitForCompletion();
            rect = grenadeLauncherCrosshairPrefab.transform.GetChild(2).GetComponent<RectTransform>();
            rect.localEulerAngles = Vector3.zero;
            rect.anchoredPosition = new Vector2(50f, -10f);

            grenadeLauncherCrosshairPrefab.transform.Find("StockCountHolder").gameObject.SetActive(false);
            grenadeLauncherCrosshairPrefab.transform.Find("Image, Arrow (1)").gameObject.SetActive(true);

            crosshair.spriteSpreadPositions[0].zeroPosition = new Vector3(25f, 25f, 0f);
            crosshair.spriteSpreadPositions[0].onePosition = new Vector3(-25f, 25f, 0f);

            crosshair.spriteSpreadPositions[1].zeroPosition = new Vector3(75f, 0f, 0f);
            crosshair.spriteSpreadPositions[1].onePosition = new Vector3(125f, 0f, 0f);

            crosshair.spriteSpreadPositions[2].zeroPosition = new Vector3(-25f, 25f, 0f);
            crosshair.spriteSpreadPositions[2].onePosition = new Vector3(25f, 25f, 0f);
            #endregion

            #region Rocket Launcher Crosshair
            rocketLauncherCrosshairPrefab = PrefabAPI.InstantiateClone(LoadCrosshair("ToolbotGrenadeLauncher"), "DriveRocketLauncherCrosshair", false);
            if (Config.dynamicCrosshair.Value) rocketLauncherCrosshairPrefab.AddComponent<DynamicCrosshair>();
            crosshair = rocketLauncherCrosshairPrefab.GetComponent<CrosshairController>();
            crosshair.skillStockSpriteDisplays = new CrosshairController.SkillStockSpriteDisplay[0];
            rocketLauncherCrosshairPrefab.transform.Find("StockCountHolder").gameObject.SetActive(false);
            #endregion

            #region Needler Crosshair
            needlerCrosshairPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/LoaderCrosshair"), "DriverNeedlerCrosshair", false);
            DriverPlugin.Destroy(needlerCrosshairPrefab.GetComponent<LoaderHookCrosshairController>());
            if (Config.dynamicCrosshair.Value) needlerCrosshairPrefab.AddComponent<DynamicCrosshair>();

            needlerCrosshairPrefab.GetComponent<RawImage>().enabled = false;

            var control = needlerCrosshairPrefab.GetComponent<CrosshairController>();

            control.maxSpreadAlpha = 0;
            control.maxSpreadAngle = 3;
            control.minSpreadAlpha = 0;
            control.spriteSpreadPositions = new CrosshairController.SpritePosition[]
            {
                new CrosshairController.SpritePosition
                {
                    target = needlerCrosshairPrefab.transform.GetChild(2).GetComponent<RectTransform>(),
                    zeroPosition = new Vector3(-20f, 0, 0),
                    onePosition = new Vector3(-48f, 0, 0)
                },
                new CrosshairController.SpritePosition
                {
                    target = needlerCrosshairPrefab.transform.GetChild(3).GetComponent<RectTransform>(),
                    zeroPosition = new Vector3(20f, 0, 0),
                    onePosition = new Vector3(48f, 0, 0)
                }
            };

            DriverPlugin.Destroy(needlerCrosshairPrefab.transform.GetChild(0).gameObject);
            DriverPlugin.Destroy(needlerCrosshairPrefab.transform.GetChild(1).gameObject);
            #endregion

            #region Shotgun Crosshair
            shotgunCrosshairPrefab = PrefabAPI.InstantiateClone(RoR2.LegacyResourcesAPI.Load<GameObject>("Prefabs/Crosshair/LoaderCrosshair"), "DriverShotgunCrosshair", false);
            DriverPlugin.Destroy(shotgunCrosshairPrefab.GetComponent<LoaderHookCrosshairController>());
            if (Config.dynamicCrosshair.Value) shotgunCrosshairPrefab.AddComponent<DynamicCrosshair>();

            shotgunCrosshairPrefab.GetComponent<RawImage>().enabled = false;

            control = shotgunCrosshairPrefab.GetComponent<CrosshairController>();

            control.maxSpreadAlpha = 0;
            control.maxSpreadAngle = 3;
            control.minSpreadAlpha = 0;
            control.spriteSpreadPositions = new CrosshairController.SpritePosition[]
            {
                new CrosshairController.SpritePosition
                {
                    target = shotgunCrosshairPrefab.transform.GetChild(2).GetComponent<RectTransform>(),
                    zeroPosition = new Vector3(-32f, 0, 0),
                    onePosition = new Vector3(-75f, 0, 0)
                },
                new CrosshairController.SpritePosition
                {
                    target = shotgunCrosshairPrefab.transform.GetChild(3).GetComponent<RectTransform>(),
                    zeroPosition = new Vector3(32f, 0, 0),
                    onePosition = new Vector3(75f, 0, 0)
                }
            };

            control.transform.Find("Bracket (2)").GetComponent<RectTransform>().localScale = new Vector3(1.25f, 1.75f, 1f);
            control.transform.Find("Bracket (3)").GetComponent<RectTransform>().localScale = new Vector3(1.25f, 1.75f, 1f);

            DriverPlugin.Destroy(shotgunCrosshairPrefab.transform.GetChild(0).gameObject);
            DriverPlugin.Destroy(shotgunCrosshairPrefab.transform.GetChild(1).gameObject);
            #endregion

            #region CircleCrosshair
            circleCrosshairPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandit2/Bandit2CrosshairPrepRevolver.prefab").WaitForCompletion().InstantiateClone("DriverCircleCrosshair", false);
            crosshair = circleCrosshairPrefab.GetComponent<CrosshairController>();
            crosshair.skillStockSpriteDisplays = new CrosshairController.SkillStockSpriteDisplay[0];

            DriverPlugin.DestroyImmediate(circleCrosshairPrefab.transform.Find("Outer").GetComponent<ObjectScaleCurve>());
            circleCrosshairPrefab.transform.Find("Outer").GetComponent<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/UI/texCrosshairTridant.png").WaitForCompletion();
            RectTransform rectR = circleCrosshairPrefab.transform.Find("Outer").GetComponent<RectTransform>();
            rectR.localScale = Vector3.one * 0.75f;

            GameObject nibL = GameObject.Instantiate(crosshair.transform.Find("Outer").gameObject);
            nibL.transform.SetParent(circleCrosshairPrefab.transform);
            //nibL.GetComponent<Image>().sprite = Addressables.LoadAssetAsync<Sprite>("RoR2/DLC1/Railgunner/texCrosshairRailgunSniperCenter.png").WaitForCompletion();
            RectTransform rectL = nibL.GetComponent<RectTransform>();
            rectL.localEulerAngles = new Vector3(0f, 0f, 180f);

            crosshair.spriteSpreadPositions = new CrosshairController.SpritePosition[]
            {
                new CrosshairController.SpritePosition
                {
                    target = rectR,
                    zeroPosition = new Vector3(0f, 0f, 0f),
                    onePosition = new Vector3(10f, 10f, 0f)
                },
                new CrosshairController.SpritePosition
                {
                    target = rectL,
                    zeroPosition = new Vector3(0f, 0f, 0f),
                    onePosition = new Vector3(-10f, -10f, 0f)
                }
            };

            circleCrosshairPrefab.AddComponent<CrosshairRotator>();
            #endregion
        }

        private static void CreateOrb()
        {
            consumeOrb = PrefabAPI.InstantiateClone(Resources.Load<GameObject>("Prefabs/Effects/OrbEffects/InfusionOrbEffect"), "DrivagerConsumeOrbEffect", true);
            if (!consumeOrb.GetComponent<NetworkIdentity>()) consumeOrb.AddComponent<NetworkIdentity>();

            TrailRenderer trail = consumeOrb.transform.Find("TrailParent").Find("Trail").GetComponent<TrailRenderer>();
            trail.widthMultiplier = 0.35f;
            trail.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/moon2/matBloodSiphon.mat").WaitForCompletion();

            consumeOrb.transform.Find("VFX").Find("Core").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matBloodHumanLarge.mat").WaitForCompletion();
            consumeOrb.transform.Find("VFX").localScale = Vector3.one * 0.5f;

            consumeOrb.transform.Find("VFX").Find("Core").localScale = Vector3.one * 4.5f;

            consumeOrb.transform.Find("VFX").Find("PulseGlow").GetComponent<ParticleSystemRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Common/VFX/matOmniRing2Generic.mat").WaitForCompletion();

            //consumeOrb.GetComponent<OrbEffect>().endEffect = Modules.Assets.slowStartPickupEffect;

            Modules.Assets.AddNewEffectDef(consumeOrb);
        }
        
        private static void CreateCoin()
        {
            #region Tracer
            coinTracer = mainAssetBundle.LoadAsset<GameObject>("CoinTracer");
            coinTracer.AddComponent<NetworkIdentity>();

            var attr = coinTracer.AddComponent<VFXAttributes>();
            attr.vfxPriority = VFXAttributes.VFXPriority.Always;
            attr.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            attr.DoNotPool = true;

            var effect1 = coinTracer.AddComponent<EffectComponent>();
            effect1.parentToReferencedTransform = false;
            effect1.positionAtReferencedTransform = false;
            effect1.applyScale = false;
            effect1.disregardZScale = false;

            coinTracer.AddComponent<EventFunctions>();
            var tracer = coinTracer.AddComponent<Tracer>();
            tracer.startTransform = coinTracer.transform.GetChild(2).GetChild(0);
            tracer.beamObject = coinTracer.transform.GetChild(2).GetChild(0).gameObject;
            tracer.beamDensity = 0.2f;
            tracer.speed = 1000f;
            tracer.headTransform = coinTracer.transform.GetChild(1);
            tracer.tailTransform = coinTracer.transform.GetChild(2).GetChild(0);
            tracer.length = 20f;

            coinTracer.AddComponent<DestroyOnTimer>().duration = 2;
            var trailChildObject = coinTracer.transform.GetChild(2).gameObject;

            var beamPoints = trailChildObject.AddComponent<BeamPointsFromTransforms>();
            beamPoints.target = trailChildObject.GetComponent<LineRenderer>();
            beamPoints.pointTransforms = [coinTracer.transform.GetChild(1), trailChildObject.transform.GetChild(0)];

            trailChildObject.GetComponent<LineRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Captain/matCaptainTracerTrail.mat").WaitForCompletion();
            trailChildObject.GetComponent<LineRenderer>().material.SetColor("_TintColor", Color.yellow);

            var animateShader = trailChildObject.AddComponent<AnimateShaderAlpha>();
            animateShader.timeMax = 0.5f;
            animateShader.pauseTime = false;
            animateShader.destroyOnEnd = true;
            animateShader.disableOnEnd = false;
            animateShader.alphaCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(0.675f, 0.8f), new Keyframe(1, 0.3f))
            {
                preWrapMode = WrapMode.Clamp,
                postWrapMode = WrapMode.Clamp
            };

            AddNewEffectDef(coinTracer);
            #endregion

            #region impact
            coinImpact = mainAssetBundle.LoadAsset<GameObject>("CoinImpactHit");
            attr = coinImpact.AddComponent<VFXAttributes>();
            attr.vfxPriority = VFXAttributes.VFXPriority.Always;
            attr.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            attr.DoNotPool = true;

            coinImpact.AddComponent<EffectComponent>();
            coinImpact.AddComponent<DestroyOnParticleEnd>();

            var eff = coinImpact.transform.Find("Streaks_Ps").GetComponent<ParticleSystemRenderer>();
            eff.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Firework/matFireworkSparkle.mat").WaitForCompletion();
            eff.material.SetColor("_TintColor", Color.yellow);

            eff = coinImpact.transform.Find("Flash_Ps").GetComponent<ParticleSystemRenderer>();
            eff.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/LunarSkillReplacements/matBirdHeartRuin.mat").WaitForCompletion();
            eff.material.SetColor("_TintColor", Color.yellow);

            AddNewEffectDef(coinImpact);
            #endregion

            #region orb
            coinOrbEffect = mainAssetBundle.LoadAsset<GameObject>("CoinOrbEffect");

            var effectComp = coinOrbEffect.AddComponent<EffectComponent>();
            effectComp.applyScale = true;

            var orbEffect = coinOrbEffect.AddComponent<CoinOrbEffect>();
            orbEffect.faceMovement = true;
            orbEffect.callArrivalIfTargetIsGone = true;
            orbEffect.endEffect = coinImpact;
            orbEffect.endEffectCopiesRotation = false;
            orbEffect.movementCurve = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1))
            {
                preWrapMode = WrapMode.Clamp,
                postWrapMode = WrapMode.Clamp
            };

            attr = coinOrbEffect.AddComponent<VFXAttributes>();
            attr.vfxPriority = VFXAttributes.VFXPriority.Always;
            attr.vfxIntensity = VFXAttributes.VFXIntensity.Low;
            attr.DoNotPool = true;

            coinOrbEffect.transform.GetChild(0).gameObject.GetComponent<TrailRenderer>().material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Captain/matCaptainTracerTrail.mat").WaitForCompletion();
            coinOrbEffect.transform.GetChild(0).gameObject.GetComponent<TrailRenderer>().material.SetColor("_TintColor", Color.yellow);

            var pscfed = coinOrbEffect.AddComponent<ParticleSystemColorFromEffectData>();
            pscfed.particleSystems = [coinOrbEffect.transform.Find("Head").GetComponent<ParticleSystem>()];
            pscfed.effectComponent = effectComp;

            var trcfed = coinOrbEffect.AddComponent<TrailRendererColorFromEffectData>();
            trcfed.renderers = [coinOrbEffect.transform.Find("Trail").GetComponent<TrailRenderer>()];
            trcfed.effectComponent = effectComp;

            var shaderAlpha = coinOrbEffect.transform.Find("Trail").gameObject.AddComponent<AnimateShaderAlpha>();
            shaderAlpha.timeMax = 0.75f;
            shaderAlpha.pauseTime = false;
            shaderAlpha.destroyOnEnd = true;
            shaderAlpha.disableOnEnd = false;
            shaderAlpha.alphaCurve = new AnimationCurve(new Keyframe(0, 1), new Keyframe(1, 0))
            {
                preWrapMode = WrapMode.Clamp,
                postWrapMode = WrapMode.Clamp
            };

            var effect = coinOrbEffect.transform.Find("Head").GetComponent<ParticleSystemRenderer>();
            effect.material = Addressables.LoadAssetAsync<Material>("RoR2/Base/Firework/matFireworkSparkle.mat").WaitForCompletion();
            effect.material.SetColor("_TintColor", Color.yellow);

            AddNewEffectDef(coinOrbEffect);
            #endregion
        }

        internal static void CreateWeaponPickups()
        {
            var ammoPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandolier/AmmoPack.prefab").WaitForCompletion();
            weaponPickupEffect = ammoPrefab.GetComponentInChildren<AmmoPickup>().pickupEffect.InstantiateClone("RobDriverWeaponPickupEffect", true);
            weaponPickupEffect.AddComponent<NetworkIdentity>();
            AddNewEffectDef(weaponPickupEffect, "sfx_driver_pickup");

            CreateDefaultPickupObject();

            ammoPickupModel = CreatePickupVisuals("mdlAmmoPickup");
            commonPickupModel = CreatePickupVisuals("mdlWeaponPickup");
            uncommonPickupModel = CreatePickupVisuals("mdlWeaponPickupUncommon");
            legendaryPickupModel = CreatePickupVisuals("mdlWeaponPickupLegendary");
            uniquePickupModel = CreatePickupVisuals("mdlWeaponPickupUnique");
            voidPickupModel = CreatePickupVisuals("mdlWeaponPickupVoid");
            lunarPickupModel = CreatePickupVisuals("mdlWeaponPickupLunar");
        }

        internal static void CreateDefaultPickupObject()
        {
            weaponPickup = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Bandolier/AmmoPack.prefab").WaitForCompletion().InstantiateClone("DriverWeaponPickup", true);
            var pickupTrigger = weaponPickup.transform.Find("PickupTrigger");
            var gravitationController = weaponPickup.transform.Find("GravitationController");
            var visuals = weaponPickup.transform.Find("Visuals");
            var pointLight = weaponPickup.transform.Find("Point light");

            weaponPickup.GetComponent<BoxCollider>().size = new Vector3(1.8f, 1f, 0.8f);

            visuals.localPosition = new Vector3(0f, -0.45f, 0f);

            pointLight.parent = visuals;
            pointLight.localPosition = Vector3.zero;
            visuals.Find("Particle System").localPosition = Vector3.zero;

            var light = pointLight.GetComponent<Light>();
            light.shadows = LightShadows.Hard;
            light.shadowStrength = 0.5f;
            light.range = 10f;

            WeaponPickup weaponPickupComponent = pickupTrigger.gameObject.AddComponent<WeaponPickup>();
            weaponPickupComponent.baseObject = weaponPickup;
            weaponPickupComponent.modelParent = visuals;
            weaponPickupComponent.light = light;
            weaponPickupComponent.systems = visuals.GetComponentsInChildren<ParticleSystem>();
            weaponPickupComponent.blinker = weaponPickup.GetComponent<BeginRapidlyActivatingAndDeactivating>();
            weaponPickupComponent.blinker.delayBeforeBeginningBlinking = 55f;
            weaponPickupComponent.destroyOnTimer = weaponPickup.AddComponent<DestroyWeaponOnTimer>();
            weaponPickup.AddComponent<SyncPickup>().weaponPickup = weaponPickupComponent;

            var grav = gravitationController.gameObject.AddComponent<MagneticPickup>();
            grav.weaponPickup = weaponPickupComponent;
            grav.rigidbody = weaponPickup.GetComponent<Rigidbody>();
            grav.rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;

            GameObject.Destroy(visuals.Find("mdlBandolierShell").gameObject);
            MonoBehaviour.Destroy(weaponPickup.GetComponent<DestroyOnTimer>());
            MonoBehaviour.Destroy(pickupTrigger.GetComponent<AmmoPickup>());
            MonoBehaviour.Destroy(gravitationController.GetComponent<GravitatePickup>());
        }

        internal static GameObject CreatePickupVisuals(string baseAssetName) => CreatePickupVisuals(mainAssetBundle.LoadAsset<GameObject>(baseAssetName));
        internal static GameObject CreatePickupVisuals(GameObject pickupModel)
        {
            if (pickupModel.GetComponentInChildren<LanguageTextMeshController>())
                return pickupModel;

            pickupModel.transform.localPosition = Vector3.zero;
            pickupModel.transform.localRotation = Quaternion.identity;

            GameObject textShit = GameObject.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BearProc"));
            textShit.transform.parent = pickupModel.transform;
            textShit.transform.localPosition = Vector3.zero;
            textShit.transform.localRotation = Quaternion.identity;
            textShit.transform.Find("TextCamScaler/TextRiser/TextMeshPro").localPosition = Vector3.zero;

            MonoBehaviour.Destroy(textShit.GetComponent<EffectComponent>());
            MonoBehaviour.Destroy(textShit.GetComponent<DestroyOnTimer>());

            ObjectScaleCurve whatTheFuckIsThis = textShit.GetComponentInChildren<ObjectScaleCurve>();
            Transform helpMe = whatTheFuckIsThis.transform;
            MonoBehaviour.DestroyImmediate(whatTheFuckIsThis);
            helpMe.transform.localScale = Vector3.one * 1.25f;

            return pickupModel;
        }
        #endregion

        #region Helpers
        private static GameObject CreateTracer(string originalTracerName, string newTracerName)
        {
            GameObject newTracer = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/" + originalTracerName).InstantiateClone(newTracerName, true);

            if (!newTracer.GetComponent<EffectComponent>()) newTracer.AddComponent<EffectComponent>();
            if (!newTracer.GetComponent<VFXAttributes>()) newTracer.AddComponent<VFXAttributes>();
            if (!newTracer.GetComponent<NetworkIdentity>()) newTracer.AddComponent<NetworkIdentity>();

            AddNewEffectDef(newTracer);

            return newTracer;
        }

        private static GameObject CreateTracer(string originalTracerName, string newTracerName, Color tintColor, Color color)
        {
            var newTracer = CreateTracer(originalTracerName, newTracerName);

            foreach (var i in newTracer.GetComponentsInChildren<LineRenderer>())
            {
                if (i.material)
                {
                    i.material = new Material(i.material);
                    i.material.SetColor("_TintColor", tintColor);
                    i.startColor = color;
                    i.endColor = color;
                }
            }

            return newTracer;
        }

        internal static GameObject CreateTextPopupEffect(string prefabName, string token, Color color)
        {
            GameObject i = CreateTextPopupEffect(prefabName, token);

            i.GetComponentInChildren<TMP_Text>().color = color;

            return i;
        }

        internal static GameObject CreateTextPopupEffect(string prefabName, string token, string soundName = "")
        {
            GameObject i = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/BearProc").InstantiateClone(prefabName, true);

            i.GetComponent<EffectComponent>().soundName = soundName;
            if (!i.GetComponent<NetworkIdentity>()) i.AddComponent<NetworkIdentity>();

            i.GetComponentInChildren<RoR2.UI.LanguageTextMeshController>().token = token;

            Assets.AddNewEffectDef(i);

            return i;
        }

        internal static NetworkSoundEventDef CreateNetworkSoundEventDef(string eventName)
        {
            NetworkSoundEventDef networkSoundEventDef = ScriptableObject.CreateInstance<NetworkSoundEventDef>();
            networkSoundEventDef.akId = AkSoundEngine.GetIDFromString(eventName);
            networkSoundEventDef.eventName = eventName;

            networkSoundEventDefs.Add(networkSoundEventDef);

            return networkSoundEventDef;
        }

        internal static void ConvertAllRenderersToHopooShader(GameObject objectToConvert)
        {
            var renderers = objectToConvert.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                var materials = renderers[i].materials;
                for (int j = 0; j < materials.Length; j++)
                {
                    TrySwapShader(materials[j]);
                }
                renderers[i].materials = materials;
            }
        }

        internal static CharacterModel.RendererInfo[] SetupRendererInfos(GameObject obj)
        {
            MeshRenderer[] meshes = obj.GetComponentsInChildren<MeshRenderer>();
            CharacterModel.RendererInfo[] rendererInfos = new CharacterModel.RendererInfo[meshes.Length];

            for (int i = 0; i < meshes.Length; i++)
            {
                rendererInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = meshes[i].material,
                    renderer = meshes[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false
                };
            }

            return rendererInfos;
        }

        internal static Texture LoadCharacterIcon(string characterName) => mainAssetBundle.LoadAsset<Texture>("tex" + characterName + "Icon");

        internal static Mesh LoadMesh(string meshName) => mainAssetBundle.LoadAsset<Mesh>(meshName);

        internal static GameObject LoadCrosshair(string crosshairName) => Resources.Load<GameObject>("Prefabs/Crosshair/" + crosshairName + "Crosshair");

        private static GameObject LoadEffect(string resourceName, string soundName = "", bool parentToTransform = false)
        {
            GameObject newEffect = mainAssetBundle.LoadAsset<GameObject>(resourceName).InstantiateClone("Driver" + resourceName, true);

            newEffect.AddComponent<DestroyOnTimer>().duration = 12;
            newEffect.AddComponent<NetworkIdentity>();
            newEffect.AddComponent<VFXAttributes>().vfxPriority = VFXAttributes.VFXPriority.Always;

            var effect = newEffect.AddComponent<EffectComponent>();
            effect.applyScale = false;
            effect.parentToReferencedTransform = parentToTransform;
            effect.positionAtReferencedTransform = true;
            effect.soundName = soundName;

            AddNewEffectDef(newEffect, soundName);

            return newEffect;
        }

        internal static void AddNewEffectDef(GameObject effectPrefab, string soundName = "")
        {
            effectPrefab.GetComponent<EffectComponent>().soundName = soundName;
            
            effectDefs.Add(new EffectDef(effectPrefab));
        }

        public static Material LoadMaterial(string materialName, float emission, Color emissionColor)
        {
            var material = LoadMaterial(materialName);
            material.SetColor("_EmColor", emissionColor);
            material.SetFloat("_EmPower", emission);

            return material;
        }

        public static Material LoadMaterial(string materialName)
        {
            var material = Assets.mainAssetBundle.LoadAsset<Material>(materialName);
            
            TrySwapShader(material);
            return material;
        }

        private static void SwapAllShaders()
        {
            foreach (var material in mainAssetBundle.LoadAllAssets<Material>())
            {
                TrySwapShader(material);
            }
        }

        internal static void TrySwapShader(Material material)
        {
            var shaderName = material.shader.name;
            if (shaderName.Contains("Stubbed"))
            {
                shaderName = shaderName.Replace("Stubbed", string.Empty) + ".shader";
                var replacementShader = Addressables.LoadAssetAsync<Shader>(shaderName).WaitForCompletion();

                if (replacementShader != null)
                {
                    material.shader = replacementShader;
                }
                else
                {
                    Log.Error("Failed to load shader " + shaderName);
                }
            }
            else if (shaderName == "Standard")
            {
                var normalMap = material.GetTexture("_BumpMap");
                var normalStrength = material.GetFloat("_BumpScale");
                var emissionMap = material.GetTexture("_EmissionMap");

                material.shader = Resources.Load<Shader>("Shaders/Deferred/HGStandard");

                material.SetTexture("_NormalMap", normalMap);
                material.SetFloat("_NormalStrength", normalStrength);
                material.SetTexture("_EmTex", emissionMap);

                material.SetColor("_EmColor", new Color(0.2f, 0.2f, 0.2f));
                material.SetFloat("_EmPower", 0.15f);
            }
        }
        #endregion
    }
}