using System.Linq;
using EntityStates;
using R2API;
using RobDriver.Modules;
using RobDriver.SkillStates.BaseStates;
using RoR2;
using RoR2.Projectile;
using UnityEngine;

namespace RobDriver.SkillStates.Driver.RavSword
{
    public class DashPunch : BaseDriverSkillState
    {
        private static readonly AnimationCurve dashSpeedCurve = new(
        [
            new Keyframe(0f, 14f),
            new Keyframe(0.8f, 0f),
            new Keyframe(1f, 0f)
        ]);

        private const float baseDuration = 0.7f;
        private const float punchDuration = 0.5f;
        private const float windupDuration = 0.2f;
        public const float punchDamageCoefficient = 12.5f;

        private BullseyeSearch bullseyeSearch;
        private Vector3 aimDirection;
        private float stopwatch;
        private bool hasDashed;

        public override void OnEnter()
        {
            base.OnEnter();
            aimDirection = GetAimRay().direction;
            aimDirection.y = Mathf.Clamp(aimDirection.y, -0.75f, 0.75f);
            characterMotor.velocity *= 0.1f;

            PlayAnimation("FullBody, Override", "DashPunchStart", "Grab.playbackRate", windupDuration);

            if (DriverPlugin.RavagerInstalled)
                Util.PlaySound("sfx_ravager_shine", gameObject);

            GetModelAnimator().SetFloat("leapDir", inputBank.aimDirection.y);

            bullseyeSearch = new BullseyeSearch
            {
                teamMaskFilter = TeamMask.GetEnemyTeams(GetTeam()),
                filterByLoS = false,
                sortMode = BullseyeSearch.SortMode.Distance,
                maxDistanceFilter = 8f,
                maxAngleFilter = 360f
            };
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (!this.hasDashed)
            {
                this.characterBody.isSprinting = false;

                if (fixedAge >= windupDuration)
                {
                    this.hasDashed = true;

                    PlayAnimation("FullBody, Override", "DashPunch", "Grab.playbackRate", punchDuration * 1.25f);
                    Util.PlaySound(DriverPlugin.RavagerInstalled ? "sfx_ravager_lunge" : "sfx_driver_dodge", gameObject);
                }

                this.characterMotor.velocity.y = 0f;
            }
            else
            {
                stopwatch += Time.fixedDeltaTime;
                this.characterBody.isSprinting = true;

                var num = dashSpeedCurve.Evaluate(stopwatch / punchDuration);
                this.characterMotor.rootMotion += aimDirection * (num * moveSpeedStat * Time.fixedDeltaTime);
                this.characterMotor.velocity.y = 0f;

                AttemptPunch();
            }

            if (fixedAge >= baseDuration)
                this.outer.SetNextStateToMain();
        }

        public void AttemptPunch()
        {
            var aimRay = GetAimRay();

            this.bullseyeSearch.searchOrigin = transform.position;
            this.bullseyeSearch.searchDirection = Random.onUnitSphere;
            this.bullseyeSearch.RefreshCandidates();
            this.bullseyeSearch.FilterOutGameObject(gameObject);

            var hurtBox = bullseyeSearch.GetResults().FirstOrDefault();

            if (!hurtBox)
                return;

            if (this.characterBody.characterMotor.jumpCount > 0)
                this.characterBody.characterMotor.jumpCount--;

            EffectManager.SpawnEffect(Modules.Assets.bloodExplosionEffect, new EffectData
            {
                origin = hurtBox.transform.position,
                scale = 2f
            }, false);

            if (DriverPlugin.RavagerInstalled)
            {
                Util.PlaySound("sfx_ravager_punch", gameObject);
                Util.PlaySound("sfx_ravager_punch_generic", hurtBox.gameObject);
            }
            else
            {
                Util.PlaySound("Play_loader_shift_release", gameObject);
                Util.PlaySound("sfx_driver_impact_hammer", hurtBox.gameObject);
            }

            if (isAuthority)
            {
                var force = 4000f;
                if (hurtBox.healthComponent.body && hurtBox.healthComponent.body.isChampion)
                    force = 24000f;

                // damage
                var damageType = iDrive.DamageType;
                damageType.damageType |= DamageType.Stun1s | DamageType.NonLethal;
                damageType.damageSource = DamageSource.Secondary;

                new BlastAttack
                {
                    attacker = gameObject,
                    procChainMask = default,
                    impactEffect = EffectIndex.Invalid,
                    losType = BlastAttack.LoSType.None,
                    damageColorIndex = DamageColorIndex.Default,
                    damageType = damageType,
                    procCoefficient = 1f,
                    bonusForce = this.GetAimRay().direction.normalized * force,
                    baseForce = 0f,
                    baseDamage = punchDamageCoefficient * this.damageStat,
                    falloffModel = BlastAttack.FalloffModel.None,
                    radius = 0.4f,
                    position = hurtBox.transform.position,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    teamIndex = GetTeam(),
                    inflictor = gameObject,
                    crit = RollCrit()
                }.Fire();

                // shockwave
                damageType.AddModdedDamageType(DriverDamageTypes.BloodExplosionIdentifier);
                ProjectileManager.instance.FireProjectile(new FireProjectileInfo
                {
                    position = hurtBox.transform.position + aimRay.direction * -4f,
                    rotation = Quaternion.LookRotation(aimRay.direction),
                    crit = this.RollCrit(),
                    damage = 10f * this.damageStat,
                    owner = this.gameObject,
                    projectilePrefab = Projectiles.punchShockwave,
                    damageTypeOverride = damageType,
                });

                this.outer.SetNextState(new PunchRecoil());
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority() => InterruptPriority.Frozen;
    }
}