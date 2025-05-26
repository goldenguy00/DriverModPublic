using EntityStates;
using RobDriver.SkillStates.BaseStates;
using RoR2.Projectile;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver.CaptainGun
{
    public class CallAirstrike : BaseDriverAimThrowable
    {
        public static float _damageCoefficient = 10f;
        private static GameObject _projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainAirstrikeProjectile1.prefab").WaitForCompletion();
        private static GameObject _endpointPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Treebot/TreebotMortarAreaIndicator.prefab").WaitForCompletion();

        public override void OnEnter()
        {
            base.projectilePrefab = _projectilePrefab;
            base.endpointVisualizerPrefab = _endpointPrefab;
            base.damageCoefficient = _damageCoefficient;
            base.baseMinimumDuration = 0.1f;
            base.setFuse = false;
            base.endpointVisualizerRadiusScale = 0f;
            base.arcVisualizerPrefab = null;
            base.rayRadius = 0.2f;
            base.maxDistance = 1000f;
            base.detonationRadius = 20f;
            
            base.OnEnter();

            base.characterBody.SetSpreadBloom(0.4f);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            base.characterBody.SetAimTimer(4f);
        }

        protected override void ModifyProjectile(ref FireProjectileInfo fireProjectileInfo)
        {
            base.ModifyProjectile(ref fireProjectileInfo);

            fireProjectileInfo.position = base.currentTrajectoryInfo.hitPoint;
            fireProjectileInfo.rotation = Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f);
            fireProjectileInfo.speedOverride = 0f;

            var damageType = fireProjectileInfo.damageTypeOverride.Value;
            damageType.damageSource = DamageSource.Secondary;
            damageType.damageType |= DamageType.Stun1s;

            fireProjectileInfo.damageTypeOverride = damageType;
        }

        protected override bool KeyIsDown()
        {
            return base.inputBank.skill1.down;
        }

        public override void OnExit()
        {
            PlayAnimation("Gesture, Override", "PressVoidButton", "Action.playbackRate", 0.3f);
            Util.PlaySound("Play_captain_shift_confirm", base.gameObject);
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}
