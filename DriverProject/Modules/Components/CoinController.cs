using System.Collections;
using System.Linq;
using HG;
using R2API;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RobDriver.Modules.Components
{
    public class CoinController : NetworkBehaviour, IProjectileImpactBehavior, IOnIncomingDamageServerReceiver
    {
        public DriverController iDrive;

        public NetworkSoundEventDef ricochetSound;

        public bool canRicochet = true;
        private float coolStopwatchScale = 0.01f;
        private bool startCoolStopwatch = false;
        public float ricochetMultiplier = 2f;
        private Vector3 rotationSpeed;
        public int bounceCountStored = 0;
        private DamageInfo damageInfo;

        public void OnIncomingDamageServer(DamageInfo damageInfo)
        {
            if (damageInfo.attacker && iDrive &&
               (damageInfo.attacker == iDrive.gameObject ||
                damageInfo.attacker.GetComponent<CoinController>()))
            {
                RicochetBullet(damageInfo);
            }
            else damageInfo.rejected = true;
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (!impactInfo.collider.GetComponent<HurtBox>() && !impactInfo.collider.GetComponent<CoinController>())
            {
                EffectData effectData = new EffectData
                {
                    origin = base.transform.position,
                    scale = 1f
                };
                EffectManager.SpawnEffect(Assets.coinImpact, effectData, transmit: true);
                Destroy(base.gameObject);
            }
        }

        private void Start()
        {
            this.gameObject.layer = LayerIndex.fakeActor.intVal;
            this.rotationSpeed = new Vector3(Random.Range(500f, 2000f), 0f, 0f);

            this.StartCoroutine(nameof(SwitchLayer));
        }

        private IEnumerator SwitchLayer()
        {
            yield return null;

            if (this.TryGetComponent<ProjectileController>(out var pc))
                iDrive = pc.owner.GetComponent<DriverController>();

            if (this.TryGetComponent<HealthComponent>(out var hc))
            {
                IOnIncomingDamageServerReceiver value = this;

                if (!hc.onIncomingDamageReceivers.Contains(value))
                    ArrayUtils.ArrayAppend(ref hc.onIncomingDamageReceivers, in value);
            }

            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            this.gameObject.layer = LayerIndex.defaultLayer.intVal;
            var hurtBox = this.GetComponentInChildren<HurtBox>();
            if (hurtBox)
                hurtBox.gameObject.layer = LayerIndex.entityPrecise.intVal;
        }

        private void FixedUpdate()
        {
            base.transform.Rotate(this.rotationSpeed * Time.fixedDeltaTime);
            if(startCoolStopwatch)
            {
                this.coolStopwatchScale -= Time.fixedDeltaTime;
                if (damageInfo.attacker && this.coolStopwatchScale <= 0f)
                {
                    this.canRicochet = false;
                    var attackerBody = this.damageInfo.attacker.GetComponent<CharacterBody>();
                    var orb = new CoinRicochetOrb
                    {
                        origin = base.transform.position,
                        speed = 180f + (10f * bounceCountStored),
                        attacker = this.damageInfo.attacker,
                        damageCoefficient = this.damageInfo.damage / attackerBody.damage,
                        damageValue = this.damageInfo.damage * this.ricochetMultiplier,
                        damageType = this.iDrive.DamageType,
                        teamIndex = attackerBody.teamComponent.teamIndex,
                        procCoefficient = 1f,
                        isCrit = this.damageInfo.crit,
                        bounceCount = bounceCountStored,
                    };

                    this.GetComponent<Rigidbody>().velocity = Vector3.zero;

                    OrbManager.instance.AddOrb(orb);

                    EffectData effectData = new EffectData
                    {
                        origin = base.transform.position,
                        scale = 1f
                    };
                    EffectManager.SpawnEffect(Assets.coinImpact, effectData, transmit: true);
                    EffectManager.SimpleSoundEffect(this.ricochetSound.index, base.transform.position, true);

                    Destroy(base.gameObject);
                }
            }
        }

        [Command]
        public void CmdRicochetMelee(GameObject attacker, GameObject inflictor, bool isCrit, float damage, uint procChainMask, Vector3 force,
            bool canRejectForce, byte colorIndex, uint damageType, uint damageTypeExtended, byte damageSource, int[] moddedDamageTypes)
        {
            this.damageInfo = new DamageInfo
            {
                attacker = attacker,
                inflictor = inflictor,
                crit = isCrit,
                damage = damage,
                procCoefficient = 0f,
                force = force,
                canRejectForce = canRejectForce,
                procChainMask = new ProcChainMask { mask = procChainMask },
                damageColorIndex = (DamageColorIndex)colorIndex,
                damageType = new DamageTypeCombo
                {
                    damageType = (DamageType)damageType,
                    damageTypeExtended = (DamageTypeExtended)damageTypeExtended,
                    damageSource = (DamageSource)damageSource,
                },
            };
            for (int i = 0; i < moddedDamageTypes.Length; i++)
            {
                this.damageInfo.AddModdedDamageType((DamageAPI.ModdedDamageType)moddedDamageTypes[i]);
            }

            bounceCountStored++;
            coolStopwatchScale = (coolStopwatchScale * bounceCountStored) + 0.01f;
            startCoolStopwatch = true;
            canRicochet = false;
        }

        public void RicochetBullet(DamageInfo damageInfo)
        {
            if (this.damageInfo != null)
            {
                this.damageInfo.damage += damageInfo.damage * 0.5f;
                return;
            }
            this.damageInfo = damageInfo;
            this.damageInfo.procCoefficient = 0f;
            this.damageInfo.damageColorIndex = DamageColorIndex.Item;

            bounceCountStored++;
            coolStopwatchScale = (coolStopwatchScale * bounceCountStored) + 0.01f;
            startCoolStopwatch = true;
        }
    }
}