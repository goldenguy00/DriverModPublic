using RoR2;
using UnityEngine;
using EntityStates;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver
{
    public class Bash : BaseDriverSkillState
    {
        public static string hitboxString = "BashHitbox"; //transform where the hitbox is fired
        public static float baseDuration = 0.8f;
        public static float hitboxRadius = 6f;
        public static float damageCoefficient = 2.5f;
        public static float procCoefficient = 1f;
        public static float knockbackForce = 0.11f;
        public static float recoilAmplitude = 1f;

        public float lungeForce = 18f;
        public float hopForce = 7f;

        private float fireTime;
        private bool hasLunged;
        private bool hasFired;
        private float duration;
        private float stopwatch;

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = baseDuration / this.attackSpeedStat;
            this.fireTime = 0.2f * this.duration;
            StartAimMode(2f);

            PlayCrossfade("Gesture, Override", "Bash", "Action.playbackRate", this.duration, 0.05f);

            Util.PlaySound("sfx_driver_bash_prep", this.gameObject);

            //if (this.iDrive) this.iDrive.StartTimer();
        }

        private void Fire()
        {
            if (!this.hasFired)
            {
                this.hasFired = true;

                Util.PlaySound("sfx_driver_swing", this.gameObject);

                var center = this.FindModelChild(hitboxString).position;

                if (isAuthority)
                {
                    //EffectManager.SimpleMuzzleFlash(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/LoaderSwingBasic.prefab").WaitForCompletion(), base.gameObject, hitboxString, true);
                    base.AddRecoil(1.5f * recoilAmplitude, 1.5f * recoilAmplitude);

                    var blastAttack = new BlastAttack();
                    blastAttack.radius = hitboxRadius;
                    blastAttack.procCoefficient = procCoefficient;
                    blastAttack.position = center;
                    blastAttack.attacker = this.gameObject;
                    blastAttack.crit = this.RollCrit();
                    blastAttack.baseDamage = this.damageStat * damageCoefficient;
                    blastAttack.falloffModel = BlastAttack.FalloffModel.None;
                    blastAttack.baseForce = 0f;
                    blastAttack.teamIndex = TeamComponent.GetObjectTeam(blastAttack.attacker);
                    blastAttack.damageType = DamageType.Stun1s;
                    blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;
                    blastAttack.damageType.damageSource = DamageSource.Secondary;

                    blastAttack.Fire();

                    // spawn hit effect on targets, play hit sound, do the funny melee hop thing
                    var hitPoints = blastAttack.CollectHits();
                    if (hitPoints.Length > 0)
                    {
                        var j = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Loader/OmniImpactVFXLoader.prefab").WaitForCompletion();
                        foreach (var i in hitPoints)
                        {
                            EffectManager.SpawnEffect(j, new EffectData
                            {
                                origin = i.hitPosition,
                                scale = 1f
                            }, true);
                        }

                        Util.PlaySound("sfx_driver_bash", this.gameObject);
                        this.characterMotor.velocity.y = this.hopForce;
                    }
                }

                if (NetworkServer.active)
                {
                    var aimRay = this.GetAimRay();
                    var pushForce = (aimRay.origin + 200 * aimRay.direction - center + 75 * Vector3.up) * knockbackForce;

                    var affectedTargets = new List<HealthComponent>();

                    var array = Physics.OverlapSphere(center, hitboxRadius, LayerIndex.entityPrecise.mask);
                    for (var i = 0; i < array.Length; i++)
                    {
                        var hb = array[i].GetComponent<HurtBox>();
                        if (hb && hb.healthComponent && healthComponent != hb.healthComponent && !affectedTargets.Contains(hb.healthComponent))
                        {
                            affectedTargets.Add(hb.healthComponent);
                            var healthComponent = hb.healthComponent;
                            var teamComponent = healthComponent.body.teamComponent;

                            var enemyTeam = teamComponent.teamIndex != base.teamComponent.teamIndex;

                            if (enemyTeam)
                            {
                                //Util.PlaySound(Sounds.BashHitEnemy, healthComponent.gameObject);

                                var hitCharacterBody = healthComponent.body;
                                if (hitCharacterBody)
                                {
                                    var hitCharacterMotor = hitCharacterBody.characterMotor;
                                    var hitRigidbody = hitCharacterBody.rigidbody;
                                    var force = pushForce;

                                    var bossMult = 0.1f;
                                    var mass = 0f;

                                    var isGrounded = false;

                                    if (hitCharacterMotor)
                                    {
                                        mass = hitCharacterMotor.mass;
                                        isGrounded = hitCharacterMotor.isGrounded;
                                    }
                                    else if (hitRigidbody)
                                    {
                                        mass = hitRigidbody.mass;
                                    }

                                    force *= 80f;   //100f is full forcce

                                    //Launch grounded enemies into the air.
                                    if (isGrounded)
                                    {
                                        force.y = Mathf.Max(force.y, 1200f);
                                        if (hitCharacterBody.isChampion)
                                            force.y /= bossMult;    //Negate boss forcce penalty
                                    }

                                    force *= Mathf.Max(mass / 100f, 1f);

                                    //Champions have a knockback penalty.
                                    if (hitCharacterBody.isChampion)
                                        force *= bossMult;

                                    healthComponent.TakeDamageForce(force, true, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            this.stopwatch += Time.fixedDeltaTime;

            if (this.stopwatch >= 0.75f * this.fireTime)
            {
                if (!this.hasLunged)
                {
                    this.hasLunged = true;

                    if (this.isGrounded) this.characterMotor.velocity += this.characterDirection.forward * this.lungeForce;
                    else this.characterMotor.velocity += this.characterDirection.forward * 0.5f * this.lungeForce;
                }
            }

            if (this.stopwatch >= this.fireTime)
                this.Fire();

            if (this.stopwatch >= this.duration && isAuthority)
            {
                this.outer.SetNextStateToMain();
                return;
            }
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.PrioritySkill;
        }
    }
}