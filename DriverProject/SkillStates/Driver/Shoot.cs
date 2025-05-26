using EntityStates;
using RobDriver.SkillStates.BaseStates;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver
{
    public class Shoot : BaseDriverShootState
    {
        public static float _damageCoefficient = 2.2f;

        internal static GameObject tracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerGoldGat");
        internal static GameObject critTracerEffectPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/Tracers/TracerCaptainShotgun");
        internal static GameObject spinEffectPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoReloadFX.prefab").WaitForCompletion();

        protected override float damageCoefficient => _damageCoefficient;
        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => (this.isCrit ? 1f : 1.5f) * this.duration;
        protected override float maxBulletSpread => this.characterBody.spreadBloomAngle * 2f;
        protected override string shootSoundString => this.isCrit ? "sfx_driver_pistol_shoot_critical" : "sfx_driver_pistol_shoot";
        protected override string animationString => this._animationString;
        protected override DamageTypeCombo damageType => this.iDrive.DamageType;
        protected override GameObject tracerPrefab => this.isCrit ? Shoot.critTracerEffectPrefab : Shoot.tracerEffectPrefab;
        protected override GameObject muzzleFlashPrefab => EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab;
        protected override GameObject hitEffectPrefab => EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab;

        protected virtual BulletAttack.FalloffModel falloff => BulletAttack.FalloffModel.DefaultBullet;

        private string _animationString = "Shoot";
        private float spinDelay;
        private float fireTime2;
        private bool hasFired2;

        private GameObject spinEffectInstance;
        private uint spinPlayID;
        private bool oldShoot;

        public override void OnEnter()
        {
            this.oldShoot = Modules.Config.oldCritShot.Value;

            base.procCoefficient = 1f;
            base.bulletCount = 1;
            base.dropShells = 0;
            base.ammoConsumption = 1f;
            base.useAttackSpeed = true;

            base.hitMask = LayerIndex.CommonMasks.bullet;
            base.stopperMask = LayerIndex.world.mask;
            base.damageColorIndex = DamageColorIndex.Default;

            base.bulletRange = 2000f;
            base.bulletThiccness = 0.75f;
            base.bulletForce = 200f;
            base.selfForce = 0f;

            base.baseDuration = 0.7f;
            base.earlyExitFraction = 0.5f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 2f;
            base.visualRecoilVertical = 2f;
            base.visualRecoilHorizontal = 0.5f;
            base.spreadBloom = 1.25f;
            base.aimTimer = 2f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "PistolMuzzle";

            base.OnEnter();

            this.characterBody.isSprinting = false;
        }

        protected override float GetDuration()
        {
            if (this.isCrit)
            {
                if (Modules.Config.oldCritShot.Value)
                {
                    CreateSpinEffect();

                    this.spinDelay = 0.4f;
                    base.baseDuration = 0.9f;
                    base.earlyExitFraction = 0.75f;
                    base.fireDelayFraction = 0.5f;
                    this._animationString = "ShootCritical";
                }
                else
                {
                    this.spinDelay = 0.55f;
                    base.baseDuration = 1.4f;
                    base.earlyExitFraction = 0.5f;
                    base.fireDelayFraction = 0f;
                    this._animationString = "ShootCriticalAlt";
                }
            }

            var duration = base.baseDuration / this.attackSpeedStat;
            this.fireTime2 = (0.05f + this.fireDelayFraction) * duration;
            this.spinDelay *= duration;

            return duration;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.cancelling || !this.isCrit)
                return;

            if (!this.hasFired2 && base.fixedAge >= this.fireTime2)
            {
                this.hasFired2 = true;
                this.FireBullet();
            }

            if (base.fixedAge >= this.spinDelay)
            {
                if (!this.oldShoot)
                {
                    if (!this.spinEffectInstance)
                        CreateSpinEffect();
                }
                else if (this.spinEffectInstance)
                {
                    DestroySpinEffect();

                    Util.PlaySound("sfx_driver_pistol_ready", this.gameObject);
                }
            }
        }

        private void CreateSpinEffect()
        {
            this.spinEffectInstance = GameObject.Instantiate(Shoot.spinEffectPrefab, this.FindModelChild("Pistol"));
            this.spinEffectInstance.transform.localRotation = Quaternion.Euler(new Vector3(0f, 80f, 0f));
            this.spinEffectInstance.transform.localPosition = Vector3.zero;

            this.spinPlayID = Util.PlaySound("sfx_driver_pistol_spin", this.gameObject);
        }

        private void DestroySpinEffect()
        {
            if (this.spinEffectInstance)
            {
                GameObject.Destroy(this.spinEffectInstance);
                this.spinEffectInstance = null;
            }

            if (this.spinPlayID != 0u)
            {
                AkSoundEngine.StopPlayingID(this.spinPlayID);
                this.spinPlayID = 0u;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            DestroySpinEffect();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (this.isCrit && !this.hasFired2)
                return InterruptPriority.PrioritySkill;

            return base.GetMinimumInterruptPriority();
        }
    }
}