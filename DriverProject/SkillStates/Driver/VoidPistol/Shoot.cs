using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver.VoidPistol
{
    public class Shoot : Driver.Shoot
    {
        internal static GameObject _tracerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorBeamTracer.prefab").WaitForCompletion();
        internal static new float _damageCoefficient = 3.5f;
        protected override float damageCoefficient => 3.5f;
        protected override GameObject tracerPrefab => _tracerPrefab;
        protected override BulletAttack.FalloffModel falloff => BulletAttack.FalloffModel.None;
    }
}