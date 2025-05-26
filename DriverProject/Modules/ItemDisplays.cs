using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace RobDriver.Modules
{
    internal static class ItemDisplays
    {
        private static Dictionary<string, GameObject> itemDisplayPrefabs = new Dictionary<string, GameObject>();

        internal static void PopulateDisplays()
        {
            PopulateFromBody("Commando");
            PopulateFromBody("Croco");

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
            ItemDisplayRuleSet itemDisplayRuleSet = Resources.Load<GameObject>("Prefabs/CharacterBodies/" + bodyName + "Body").GetComponent<ModelLocator>().modelTransform.GetComponent<CharacterModel>().itemDisplayRuleSet;

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
                        string key = (name != null) ? name.ToLower() : null;
                        if (!itemDisplayPrefabs.ContainsKey(key))
                        {
                            itemDisplayPrefabs[key] = followerPrefab;
                        }
                    }
                }
            }
        }

        internal static GameObject LoadDisplay(string name)
        {
            if (itemDisplayPrefabs.ContainsKey(name.ToLower()))
            {
                if (itemDisplayPrefabs[name.ToLower()]) return itemDisplayPrefabs[name.ToLower()];
            }

            Debug.LogError("Could not find display prefab " + name);

            return null;
        }
    }
}