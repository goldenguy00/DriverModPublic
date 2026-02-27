using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace RobDriver.Modules
{
    internal static class ItemDisplays
    {
        private static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();

        internal static void PopulateDisplays()
        {
            PopulateFromBody(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Commando.idrsCommando_asset);
            PopulateFromBody(RoR2BepInExPack.GameAssetPathsBetter.RoR2_Base_Croco.idrsCroco_asset);

            // i forgot this cursed code was lying here
            // waht the fuck dude?
            GameObject fuckYou = Assets.mainAssetBundle.LoadAsset<GameObject>("DriverStunGrenadeGhost").InstantiateClone("DriverStunGrenadeGhost", true);//ItemDisplays.LoadDisplay("DisplayStunGrenade").InstantiateClone("DriverStunGrenadeGhost", true);
            fuckYou.AddComponent<RoR2.Projectile.ProjectileGhostController>();
            fuckYou.AddComponent<NetworkIdentity>();

            GameObject model = GameObject.Instantiate(ItemDisplays.LoadDisplay("DisplayStunGrenade"));
            model.transform.parent = fuckYou.transform;
            model.transform.localPosition = Vector3.zero;
            model.transform.localRotation = Quaternion.identity;
            model.transform.localScale = Vector3.one * 3f;

            Projectiles.stunGrenadeProjectilePrefab.GetComponent<RoR2.Projectile.ProjectileController>().ghostPrefab = fuckYou;
        }

        private static void PopulateFromBody(string bodyName)
        {
            ItemDisplayRuleSet itemDisplayRuleSet = Addressables.LoadAssetAsync<ItemDisplayRuleSet>(bodyName).WaitForCompletion();
            if (!itemDisplayRuleSet)
            {
                Log.Error("No idrs for " + bodyName);
                return;
            }

            ItemDisplayRuleSet.KeyAssetRuleGroup[] item = itemDisplayRuleSet.keyAssetRuleGroups;

            for (int i = 0; i < item.Length; i++)
            {
                ItemDisplayRule[] rules = item[i].displayRuleGroup.rules;

                for (int j = 0; j < rules.Length; j++)
                {
                    GameObject followerPrefab = rules[j].followerPrefab;
                    if (followerPrefab)
                    {
                        string name = followerPrefab.name;
                        string key = name?.ToLower();
                        if (!string.IsNullOrEmpty(key) && !itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
        }

        internal static GameObject LoadDisplay(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (itemDisplayPrefabs.ContainsKey(name.ToLower()))
                {
                    if (itemDisplayPrefabs[name.ToLower()]) return itemDisplayPrefabs[name.ToLower()];
                }
            }

            Debug.LogError("Could not find display prefab " + name);

            return null;
        }
    }
}