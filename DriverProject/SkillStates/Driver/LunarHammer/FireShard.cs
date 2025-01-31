using EntityStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RobDriver.SkillStates.Driver.LunarHammer
{
    public class FireShard : BaseDriverSkillState
    {
        public static float damageCoefficient = 1.8f;
        public static float baseDuration = 0.1f;
        public static float recoilAmplitude = 0.8f;
        public static float spreadBloomValue = 1f;
        public static string muzzleString = "HandL";

        private float duration;
        protected virtual GameObject projectilePrefab => Modules.Projectiles.lunarShard;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = FireShard.baseDuration / this.attackSpeedStat;

            if (base.isAuthority)
            {
                Ray aimRay = base.GetAimRay();

                FireProjectileInfo fireProjectileInfo = new FireProjectileInfo
                {
                    position = aimRay.origin,
                    rotation = Quaternion.LookRotation(aimRay.direction),
                    crit = base.characterBody.RollCrit(),
                    damage = base.characterBody.damage * FireShard.damageCoefficient,
                    damageColorIndex = DamageColorIndex.Default,
                    owner = base.gameObject,
                    procChainMask = default(ProcChainMask),
                    force = 0f,
                    useFuseOverride = false,
                    useSpeedOverride = false,
                    target = null,
                    projectilePrefab = this.projectilePrefab,
                    damageTypeOverride = iDrive.DamageType
                };

                ProjectileManager.instance.FireProjectile(fireProjectileInfo);
            }

            base.PlayAnimation("LeftArm, Override", "FireShard", "Shard.playbackRate", this.duration * 5f);

            float recoil = FireShard.recoilAmplitude / this.attackSpeedStat;
            base.AddRecoil2(-0.4f * recoil, -0.8f * recoil, -0.3f * recoil, 0.3f * recoil);
            base.characterBody.AddSpreadBloom(FireShard.spreadBloomValue);

            EffectManager.SimpleMuzzleFlash(Modules.Assets.lunarShardMuzzleFlash, base.gameObject, "HandL", false);
            Util.PlaySound(EntityStates.BrotherMonster.Weapon.FireLunarShards.fireSound, base.gameObject);

            //base.skillLocator.secondary.rechargeStopwatch = 0f;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.isAuthority && base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}