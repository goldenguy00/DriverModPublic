using UnityEngine;
using UnityEngine.AddressableAssets;
using RobDriver.SkillStates.Driver;

namespace RobDriver.SkillStates.Driver.Scepter
{
    public class ThrowMolotov : ThrowGrenade
    {
        protected override GameObject projectilePrefab => Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Molotov/MolotovClusterProjectile.prefab").WaitForCompletion();
    }
}