using HG;
using RobDriver.Modules.Survivors;
using RoR2;
using RoR2BepInExPack.GameAssetPathsBetter;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.Modules
{
    internal static class ItemDisplays
    {

        // item display stuffs
        internal static List<ItemDisplayRuleSet.KeyAssetRuleGroup> itemDisplayRules;

        internal static void InitializeItemDisplays(GameObject prefab)
        {
            CharacterModel characterModel = prefab.GetComponentInChildren<CharacterModel>();

            var itemDisplayRuleSet = ScriptableObject.CreateInstance<ItemDisplayRuleSet>();
            itemDisplayRuleSet.name = "idrs" + Driver.bodyName;

            itemDisplayRules = Addressables.LoadAssetAsync<ItemDisplayRuleSet>(RoR2_Base_Commando.idrsCommando_asset).WaitForCompletion().keyAssetRuleGroups.ToList();
            
            SetItemDisplays();
            
            itemDisplayRuleSet.keyAssetRuleGroups = itemDisplayRules.ToArray();
            characterModel.itemDisplayRuleSet = itemDisplayRuleSet;

            // modded support
            // RoR2.ContentManagement.ContentManager.onContentPacksAssigned += ContentManager_onContentPacksAssigned;

        }

        internal static void ReplaceItemDisplay(string keyAsset, ItemDisplayRule[] newDisplayRules)
        {
            var assetRef = new RoR2.AddressableAssets.IDRSKeyAssetReference(keyAsset);
            var asset = assetRef.LoadAssetAsync<ScriptableObject>().WaitForCompletion();

            for (int i = 0; i < itemDisplayRules.Count; i++)
            {
                if (itemDisplayRules[i].keyAsset == asset)
                {
                    // replace the item display rule
                    var newRuleGroup = new ItemDisplayRuleSet.KeyAssetRuleGroup
                    {
                        keyAsset = asset,
                        keyAssetAddress = assetRef,
                        displayRuleGroup = itemDisplayRules[i].displayRuleGroup
                    };
                    newRuleGroup.displayRuleGroup.rules = newDisplayRules;

                    itemDisplayRules[i] = newRuleGroup;
                    return;
                }
            }
            Log.Error($"IDRS for GUID {keyAsset} not found");
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void ContentManager_onContentPacksAssigned(ReadOnlyArray<RoR2.ContentManagement.ReadOnlyContentPack> obj)
        {

            if (DriverPlugin.LitInstalled)
                SetLITDisplays();
        }

        private static void SetItemDisplays()
        {
            ReplaceItemDisplay(RoR2_Base_SecondarySkillMagazine.SecondarySkillMagazine_asset,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_Base_SecondarySkillMagazine.DisplayDoubleMag_prefab),
                    limbMask = LimbFlags.None,
                    childName = "GunR",
                    localPos = new Vector3(0.00888F, -0.03648F, -0.20898F),
                    localAngles = new Vector3(39.35415F, 348.9445F, 164.0792F),
                    localScale = new Vector3(0.06F, 0.06F, 0.06F)
                }
            ]);

            ReplaceItemDisplay(RoR2_Base_CritGlasses.CritGlasses_asset,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_Base_CritGlasses.DisplayGlasses_prefab),
                    limbMask = LimbFlags.None,
                    childName = "Head",
                    localPos = new Vector3(0.0006F, 0.25054F, 0.04672F),
                    localAngles = new Vector3(314.7648F, 358.1459F, 0.48047F),
                    localScale = new Vector3(0.30902F, 0.09537F, 0.30934F)
                }
            ]);

            if (Config.predatoryOnHead.Value)
            {
                ReplaceItemDisplay(RoR2_Base_AttackSpeedOnCrit.AttackSpeedOnCrit_asset,
                [
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefabAddress = new AssetReferenceGameObject(RoR2_Base_AttackSpeedOnCrit.DisplayWolfPelt_prefab),
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
                ReplaceItemDisplay(RoR2_Base_AttackSpeedOnCrit.AttackSpeedOnCrit_asset,
                [
                    new ItemDisplayRule
                    {
                        ruleType = ItemDisplayRuleType.ParentedPrefab,
                        followerPrefabAddress = new AssetReferenceGameObject(RoR2_Base_AttackSpeedOnCrit.DisplayWolfPelt_prefab),
                        limbMask = LimbFlags.None,
                        childName = "UpperArmR",
                        localPos = new Vector3(-0.01092F, 0.02048F, -0.00403F),
                        localAngles = new Vector3(309.4066F, 250.1116F, 175.7708F),
                        localScale = new Vector3(0.363F, 0.363F, 0.363F)
                    }
                ]);
            }

            ReplaceItemDisplay(RoR2_DLC1_CritGlassesVoid.CritGlassesVoid_asset,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_DLC1_CritGlassesVoid.DisplayGlassesVoid_prefab),
                    limbMask = LimbFlags.None,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.1555F, 0.11598F),
                    localAngles = new Vector3(340.0668F, 0F, 0F),
                    localScale = new Vector3(0.30387F, 0.39468F, 0.46147F)
                }
            ]);

            ReplaceItemDisplay(RoR2_DLC1_LunarSun.LunarSun_asset,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_DLC1_LunarSun.DisplaySunHeadNeck_prefab),
                    limbMask = LimbFlags.None,
                    childName = "Chest",
                    localPos = new Vector3(-0.02605F, 0.38179F, -0.0112F),
                    localAngles = new Vector3(-0.00001F, 262.1551F, 0.00001F),
                    localScale = new Vector3(1.76594F, 1.84475F, 1.84475F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.LimbMask,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_DLC1_LunarSun.DisplaySunHead_prefab),
                    limbMask = LimbFlags.Head,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.10143F, -0.01147F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.90836F, 0.90836F, 0.90836F)
                },
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_DLC1_LunarSun.DisplaySunHead_prefab),
                    limbMask = LimbFlags.Head,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.10143F, -0.01147F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.90836F, 0.90836F, 0.90836F)
                }
            ]);

            ReplaceItemDisplay(RoR2_Base_GhostOnKill.GhostOnKill_asset,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_Base_GhostOnKill.DisplayMask_prefab),
                    limbMask = LimbFlags.None,
                    childName = "Head",
                    localPos = new Vector3(0.0029F, 0.15924F, 0.07032F),
                    localAngles = new Vector3(355.7367F, 0.15F, 0F),
                    localScale = new Vector3(0.6F, 0.6F, 0.6F)
                }
            ]);

            ReplaceItemDisplay(RoR2_Base_GoldOnHit.GoldOnHit_asset,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_Base_GoldOnHit.DisplayBoneCrown_prefab),
                    limbMask = LimbFlags.None,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.15159F, -0.0146F),
                    localAngles = new Vector3(8.52676F, 0F, 0F),
                    localScale = new Vector3(0.90509F, 0.90509F, 0.90509F)
                }
            ]);

            ReplaceItemDisplay(RoR2_Base_JumpBoost.JumpBoost_asset,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_Base_JumpBoost.DisplayWaxBird_prefab),
                    limbMask = LimbFlags.None,
                    childName = "Head",
                    localPos = new Vector3(0F, -0.228F, -0.108F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.79857F, 0.79857F, 0.79857F)
                }
            ]);

            ReplaceItemDisplay(RoR2_Base_KillEliteFrenzy.KillEliteFrenzy_asset,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_Base_KillEliteFrenzy.DisplayBrainstalk_prefab),
                    limbMask = LimbFlags.None,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.12823F, 0.035F),
                    localAngles = new Vector3(0F, 0F, 0F),
                    localScale = new Vector3(0.17982F, 0.17982F, 0.17982F)
                }
            ]);

            ReplaceItemDisplay(RoR2_Base_LunarSkillReplacements.LunarPrimaryReplacement_asset_b89712f6,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_Base_LunarSkillReplacements.DisplayBirdEye_prefab),
                    limbMask = LimbFlags.None,
                    childName = "Head",
                    localPos = new Vector3(0F, 0.18736F, 0.08896F),
                    localAngles = new Vector3(306.9798F, 180F, 180F),
                    localScale = new Vector3(0.31302F, 0.31302F, 0.31302F)
                }
            ]);

            ReplaceItemDisplay(RoR2_DLC1_FragileDamageBonus.FragileDamageBonus_asset,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_DLC1_FragileDamageBonus.DisplayDelicateWatch_prefab),
                    limbMask = LimbFlags.None,
                    childName = "HandL",
                    localPos = new Vector3(0.001145094f, -0.01941454f, 0.001435831f),
                    localAngles = new Vector3(84.24088f, 213.1651f, 131.5774f),
                    localScale = new Vector3(0.5f, 0.5f, 0.5f)
                }
            ]);

            ReplaceItemDisplay(RoR2_Base_BarrierOnOverHeal.BarrierOnOverHeal_asset,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_Base_BarrierOnOverHeal.DisplayAegis_prefab),
                    limbMask = LimbFlags.None,
                    childName = "LowerArmL",
                    localPos = new Vector3(0.01781F, 0.11702F, 0.01516F),
                    localAngles = new Vector3(90F, 270F, 0F),
                    localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
            ]);

            ReplaceItemDisplay(RoR2_Base_SprintArmor.SprintArmor_asset,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_Base_SprintArmor.DisplayBuckler_prefab),
                    limbMask = LimbFlags.None,
                    childName = "LowerArmL",
                    localPos = new Vector3(-0.012F, 0.171F, -0.027F),
                    localAngles = new Vector3(0F, 90F, 0F),
                    localScale = new Vector3(0.3F, 0.3F, 0.3F)
                }
            ]);

            ReplaceItemDisplay(RoR2_Base_ArmorPlate.ArmorPlate_asset,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_Base_ArmorPlate.DisplayRepulsionArmorPlate_prefab),
                    limbMask = LimbFlags.None,
                    childName = "CalfR",
                    localPos = new Vector3(-0.02573F, 0.22602F, 0.0361F),
                    localAngles = new Vector3(90F, 180F, 0F),
                    localScale = new Vector3(-0.2958F, 0.2958F, 0.29581F)
                }
            ]);

            ReplaceItemDisplay(RoR2_DLC1_CritDamage.CritDamage_asset,
            [
                new ItemDisplayRule
                {
                    ruleType = ItemDisplayRuleType.ParentedPrefab,
                    followerPrefabAddress = new AssetReferenceGameObject(RoR2_DLC1_CritDamage.DisplayLaserSight_prefab),
                    limbMask = LimbFlags.None,
                    childName = "Pistol",
                    localPos = new Vector3(-0.01876F, 0.26245F, 0.11694F),
                    localAngles = new Vector3(0F, 0F, 270F),
                    localScale = new Vector3(0.05261F, 0.05261F, 0.05261F)
                }
            ]);

        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
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
    }
}