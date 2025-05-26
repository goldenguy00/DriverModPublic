using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver.FalsePistol
{
    public class Shoot : Driver.Shoot
    {
        internal static GameObject _tracerPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC2/FalseSon/FalseSonLaserTracer.prefab").WaitForCompletion();
        public static new float _damageCoefficient => 3.5f;

        protected override float damageCoefficient => _damageCoefficient;
        protected override GameObject tracerPrefab => _tracerPrefab;
        protected override BulletAttack.FalloffModel falloff => BulletAttack.FalloffModel.None;
    }
}