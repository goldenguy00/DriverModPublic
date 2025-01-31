using RoR2;
using UnityEngine;
using EntityStates;

namespace RobDriver.SkillStates.Driver.DualShotgun
{
    public class Shoot : BaseDriverSkillState
    {
        public const float RAD2 = 1.414f;//for area calculation
        //public const float RAD3 = 1.732f;//for area calculation

        public static float damageCoefficient = 1.9f;
        public static float procCoefficient = 1f;
        public float baseDuration = 2.2f; // the base skill duration. i.e. attack speed
        public static int bulletCount = 6;
        public static float bulletSpread = 4f;
        public static float bulletRecoil = 8f;
        public static float bulletRange = 100f;
        public static float bulletThiccness = 0.7f;

        private float earlyExitTime;
        protected float duration;
        protected float fireDuration;
        protected bool hasFired;
        private bool isCrit;
        protected string muzzleString;

        public override void OnEnter()
        {
            base.OnEnter();
            this.characterBody.SetAimTimer(5f);
            this.muzzleString = "ShotgunMuzzle";
            this.hasFired = false;
            this.duration = this.baseDuration / this.attackSpeedStat;
            this.isCrit = base.RollCrit();
            this.earlyExitTime = 0.75f * this.duration;
            this.fireDuration = this.duration * 0.1f;

            //this.PlayCrossfade("Gesture, Override", "FireShotgun", "Shoot.playbackRate", Mathf.Max(0.05f, 1.75f * duration), 0.06f);
            base.PlayAnimation("Gesture, Override", "FireShotgun", "Shoot.playbackRate", this.duration);

            if (this.iDrive) this.iDrive.ConsumeAmmo();

            this.Fire();

            this.muzzleString = "ShotgunMuzzle2";
        }

        public virtual void Fire()
        {
            if (this.isCrit) Util.PlaySound("sfx_driver_shotgun_shoot_critical", base.gameObject);
            else Util.PlaySound("sfx_driver_shotgun_shoot", base.gameObject);

            if (this.iDrive)
            {
                this.iDrive.DropShell(-this.GetModelBaseTransform().transform.right * -Random.Range(4, 12));
                this.iDrive.DropShell(-this.GetModelBaseTransform().transform.right * -Random.Range(4, 12));
            }

            float recoilAmplitude = Shoot.bulletRecoil / this.attackSpeedStat;

            base.AddRecoil2(-0.4f * recoilAmplitude, -0.8f * recoilAmplitude, -0.3f * recoilAmplitude, 0.3f * recoilAmplitude);
            this.characterBody.AddSpreadBloom(4f);
            EffectManager.SimpleMuzzleFlash(EntityStates.Commando.CommandoWeapon.FireBarrage.effectPrefab, gameObject, muzzleString, false);

            GameObject tracer = Modules.Assets.shotgunTracer;
            if (this.isCrit) tracer = Modules.Assets.shotgunTracerCrit;

            if (base.isAuthority)
            {
                float damage = Shoot.damageCoefficient * this.damageStat;

                Ray aimRay = GetAimRay();

                float spread = Shoot.bulletSpread;
                float thiccness = Shoot.bulletThiccness;
                float force = 50;

                BulletAttack bulletAttack = new BulletAttack
                {
                    aimVector = aimRay.direction,
                    origin = aimRay.origin,
                    damage = damage,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = iDrive.DamageType,
                    falloffModel = BulletAttack.FalloffModel.DefaultBullet,
                    maxDistance = bulletRange,
                    force = force,// RiotShotgun.bulletForce,
                    hitMask = LayerIndex.CommonMasks.bullet,
                    isCrit = this.isCrit,
                    owner = gameObject,
                    muzzleName = muzzleString,
                    smartCollision = true,
                    procChainMask = default,
                    procCoefficient = procCoefficient,
                    radius = thiccness,
                    sniper = false,
                    stopperMask = LayerIndex.world.collisionMask,
                    weapon = null,
                    tracerEffectPrefab = tracer,
                    spreadPitchScale = 1f,
                    spreadYawScale = 1f,
                    queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                    hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FireBarrage.hitEffectPrefab,
                    HitEffectNormal = false,
                };
                bulletAttack.minSpread = 0;
                bulletAttack.maxSpread = 0;
                bulletAttack.bulletCount = 1;
                //bulletAttack.modifyOutgoingDamageCallback += RicochetUtils.BulletAttackShootableDamageCallback;
                bulletAttack.Fire();

                uint secondShot = (uint)Mathf.CeilToInt(bulletCount / 2f) - 1;
                bulletAttack.minSpread = 0;
                bulletAttack.maxSpread = spread / 1.45f;
                bulletAttack.bulletCount = secondShot;
                //bulletAttack.modifyOutgoingDamageCallback += RicochetUtils.BulletAttackShootableDamageCallback;
                bulletAttack.Fire();

                bulletAttack.minSpread = spread / 1.45f;
                bulletAttack.maxSpread = spread;
                bulletAttack.bulletCount = (uint)Mathf.FloorToInt(bulletCount / 2f);
                //bulletAttack.modifyOutgoingDamageCallback += RicochetUtils.BulletAttackShootableDamageCallback;
                bulletAttack.Fire();
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.hasFired)
            {
                if (base.fixedAge >= this.fireDuration)
                {
                    this.hasFired = true;
                    this.Fire();
                }
            }

            if (this.iDrive && this.iDrive.weaponDef != this.cachedWeaponDef)
            {
                base.PlayAnimation("Gesture, Override", "BufferEmpty");
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.fixedAge >= this.duration && base.isAuthority)
            {
                this.outer.SetNextStateToMain();
                //this.outer.SetNextState(new Shotgun.Reload());
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            if (base.fixedAge >= this.earlyExitTime) return InterruptPriority.Any;
            return InterruptPriority.Skill;
        }
    }
}