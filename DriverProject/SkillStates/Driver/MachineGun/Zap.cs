using UnityEngine;
using RoR2;
using UnityEngine.AddressableAssets;
using RobDriver.SkillStates.BaseStates;
using EntityStates;

namespace RobDriver.SkillStates.Driver.MachineGun
{
    public class Zap : BaseDriverProjectileAttack
    {
        public static float _damageCoefficient = 3.8f;

        private static GameObject _projectilePrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Captain/CaptainTazer.prefab").WaitForCompletion();

        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override string animationString => "Zap";
        protected override string shootSound => "sfx_driver_zap";
        protected override DamageTypeCombo? damageType => new DamageTypeCombo { damageSource = DamageSource.Secondary, damageType = DamageType.Shock5s };
        protected override GameObject projectilePrefab => _projectilePrefab;


        private uint playID;

        public override void OnEnter()
        {
            base.damageCoefficient = _damageCoefficient;
            base.ammoConsumption = 0f;
            base.useAttackSpeed = false;
            base.useICBM = false;

            base.interruptPriority = InterruptPriority.PrioritySkill;
            base.damageColorIndex = DamageColorIndex.Default;
            base.target = null;
            base.maxDistance = -1f;
            base.fuseOverride = -1f;
            base.speedOverride = 75f;
            base.force = 120f;
            base.selfForce = 10f;

            base.baseDuration = 0.8f;
            base.earlyExitFraction = 0.7f;
            base.fireDelayFraction = 0.5f;

            base.visualRecoilAmplitude = 6f;
            base.visualRecoilVertical = 0.3f;
            base.visualRecoilHorizontal = 0.1f;
            base.arcPitch = 0f;
            base.spreadBloom = 4f;
            base.aimTimer = 2f;

            base.playbackRateString = "Action.playbackRate";

            base.OnEnter();

            this.playID = Util.PlaySound("sfx_driver_zap_prep", this.gameObject);
        }

        public override void FireProjectile()
        {
            base.FireProjectile();

            if (this.playID != 0u)
            {
                AkSoundEngine.StopPlayingID(this.playID);
                this.playID = 0u;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.playID != 0u)
            {
                AkSoundEngine.StopPlayingID(this.playID);
                this.playID = 0u;
            }
        }
    }
}