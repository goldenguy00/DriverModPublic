using RoR2;
using UnityEngine;
using EntityStates;
using UnityEngine.AddressableAssets;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.SMG
{
    public class SuppressiveFire : BaseDriverShootState
    {
        public static float _damageCoefficient = 2.5f;
        public static int _baseShotCount = 6;

        protected float _baseShotDuration = 0.05f;
        protected static GameObject _muzzleFlashPrefrab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/MuzzleflashSmokeRing.prefab").WaitForCompletion();

        private int remainingShots;
        private float shotTimer;
        private bool finishing;

        private uint spinPlayID;
        private GameObject spinEffectInstance;

        protected override float damageCoefficient => _damageCoefficient;
        protected override float fireTime => this.shotTimer;
        protected override float earlyExitTime => this.duration - 0.7f;
        protected override float animationDuration => this.baseDuration;
        protected override float maxBulletSpread => this.characterBody.spreadBloomAngle;
        protected override string animationString => "ShootSubmission";
        protected override string shootSoundString => DriverPlugin.StarstormInstalled ? "NemmandoSubmissionFire" : "sfx_driver_rocket_launcher_shoot";
        protected override DamageTypeCombo damageType => this.iDrive.DamageType;
        protected override GameObject tracerPrefab => Modules.Assets.nemmandoTracer;
        protected override GameObject muzzleFlashPrefab => _muzzleFlashPrefrab;
        protected override GameObject hitEffectPrefab => EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab;

        public override void OnEnter()
        {
            base.procCoefficient = 1f;
            base.bulletCount = 1;
            base.dropShells = 0;
            base.useAttackSpeed = false;

            base.interruptPriority = InterruptPriority.PrioritySkill;
            base.bulletFalloff = BulletAttack.FalloffModel.None;
            base.hitMask = LayerIndex.CommonMasks.bullet;
            base.stopperMask = LayerIndex.world.mask;
            base.damageColorIndex = DamageColorIndex.Default;

            base.bulletRange = 150f;
            base.bulletThiccness = 1f;
            base.bulletForce = 50f;
            base.selfForce = 0f;

            base.baseDuration = 1.4f;
            base.earlyExitFraction = 1f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 4f;
            base.spreadBloom = 2f;
            base.aimTimer = 5f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "PistolMuzzle";

            base.OnEnter();
        }

        protected override float GetDuration()
        {
            // okay hear me out on this one..

            // 0.3 = 0.05 * 6   |    no attack speed lasts 0.3s, 6 rounds
            // 0.016666 = 1 / 60    |    cant shoot more than this, kinda

            //18 = 0.3 / 0.01666     |      lets just cap it here, it'll be the lerp range

            // start out by reducing the time between shots ever so slightly
            // we max out hitting 18 total rounds at 3.0 atk speed
            // time for fuck tons of damage after that good bye frames
            // remainder above this goes into pellet count
            /*
            this.remainingShots = Mathf.RoundToInt(Util.Remap(base.attackSpeedStat, 0f, 3f, 0f, 18f));
            if (this.remainingShots > 18)
            {
                this.remainingShots +
            }*/
            this.remainingShots = System.Math.Clamp(Mathf.RoundToInt(_baseShotCount * this.attackSpeedStat), _baseShotCount, 40);
            base.ammoConsumption = _baseShotDuration;

            return base.baseDuration + ((this.remainingShots - 1) * _baseShotDuration);
        }

        protected override void FireBullet()
        {
            base.PlayEntryAnimation();
            base.FireBullet();

            if (this.remainingShots > 0)
            {
                this.remainingShots--;
                this.ammoConsumption = _baseShotDuration;

                this.shotTimer += _baseShotDuration;
                this.hasFired = false;
            }
        }

        protected override void AuthorityModifyBulletAttack(ref BulletAttack bulletAttack)
        {
            base.AuthorityModifyBulletAttack(ref bulletAttack);

            var damageType = bulletAttack.damageType;
            damageType.damageSource = DamageSource.Secondary;
            damageType.damageType |= DamageType.Stun1s;

            bulletAttack.damageType = damageType;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.finishing)
                return;

            if (base.fixedAge >= this.duration - 0.7f)
            {
                if (!this.spinEffectInstance)
                {
                    this.spinPlayID = Util.PlaySound("sfx_driver_pistol_spin", this.gameObject);
                    this.spinEffectInstance = GameObject.Instantiate(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Commando/CommandoReloadFX.prefab").WaitForCompletion());
                    this.spinEffectInstance.transform.parent = this.FindModelChild("Pistol");
                    this.spinEffectInstance.transform.localRotation = Quaternion.Euler(new Vector3(0f, 80f, 0f));
                    this.spinEffectInstance.transform.localPosition = Vector3.zero;
                }

                if (base.fixedAge >= this.duration - 0.175f)
                {
                    this.finishing = true;
                    this.StopEffects();
                }
            }
        }

        private void StopEffects()
        {
            if (this.spinEffectInstance)
            {
                EntityState.Destroy(this.spinEffectInstance);
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

            this.StopEffects();
        }
    }
}