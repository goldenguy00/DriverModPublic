using System.Collections;
using RoR2;
using RoR2.Orbs;
using RoR2.Projectile;
using UnityEngine;
using UnityEngine.Networking;

namespace RobDriver.Modules.Components
{
    public class CoinController : NetworkBehaviour, IProjectileImpactBehavior, IOnIncomingDamageServerReceiver
    {
        public HurtBox mainHurtbox;
        public NetworkSoundEventDef ricochetSound;
        public ProjectileController projectileController;
        public TeamComponent teamComponent;
        public TeamFilter teamFilter;

        public int bounceCountStored = 1;
        public Color color = Color.yellow;
        public bool scepter = false;

        private float coolStopwatchScale = 0.01f;
        private bool startCoolStopwatch = false;
        private Vector3 rotationSpeed = new Vector3(Random.Range(500f, 2000f), 0f, 0f);

        private DamageInfo damageInfo;
        private float ownerBaseDamage;

        private void Start()
        {
            this.StartCoroutine(nameof(SwitchLayer));
        }

        private IEnumerator SwitchLayer()
        {
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();
            yield return new WaitForFixedUpdate();

            if (this.mainHurtbox)
            {
                this.gameObject.layer = LayerIndex.defaultLayer.intVal;
                this.mainHurtbox.gameObject.layer = LayerIndex.entityPrecise.intVal;
                this.projectileController.IgnoreCollisionsWithOwner(true);
            }

            if (this.projectileController.owner)
            {
                ownerBaseDamage = this.projectileController.owner.GetComponent<CharacterBody>().baseDamage;
            }
        }

        private void FixedUpdate()
        {
            base.transform.Rotate(this.rotationSpeed * Time.fixedDeltaTime);
            if (startCoolStopwatch)
            {
                this.coolStopwatchScale -= Time.fixedDeltaTime;
                if (this.damageInfo.attacker && this.coolStopwatchScale <= 0f)
                {
                    this.GetComponent<Rigidbody>().velocity = Vector3.zero;
                    this.teamComponent.teamIndex = this.teamFilter.teamIndex;

                    OrbManager.instance.AddOrb(new CoinRicochetOrb
                    {
                        scepter = this.scepter,
                        color = this.color,
                        origin = base.transform.position,
                        speed = 180f,
                        attacker = this.damageInfo.attacker,
                        damageValue = this.damageInfo.damage,
                        damageType = this.damageInfo.damageType,
                        teamIndex = this.teamFilter.teamIndex,
                        procCoefficient = 1f,
                        isCrit = this.damageInfo.crit,
                        bounceCount = this.bounceCountStored,
                        ownerBaseDamage = this.ownerBaseDamage
                    });

                    EffectManager.SpawnEffect(Assets.coinImpact, new EffectData
                    {
                        origin = base.transform.position,
                        scale = 1f
                    }, transmit: true);
                    EffectManager.SimpleSoundEffect(this.ricochetSound.index, base.transform.position, true);

                    Destroy(base.gameObject);
                }
            }
        }

        public void OnProjectileImpact(ProjectileImpactInfo impactInfo)
        {
            if (!impactInfo.collider.GetComponent<HurtBox>() && !impactInfo.collider.GetComponent<HurtBoxGroup>() && !impactInfo.collider.GetComponent<CharacterBody>())
            {
                EffectManager.SpawnEffect(Assets.coinImpact, new EffectData
                {
                    origin = base.transform.position,
                    scale = 1f
                }, transmit: true);

                Destroy(base.gameObject);
            }
        }

        public void OnIncomingDamageServer(DamageInfo damageInfo)
        {
            if (damageInfo.attacker && damageInfo.attacker == this.projectileController.owner &&
               (this.damageInfo != null || damageInfo.damageType.IsDamageSourceSkillBased))
            {
                RicochetBullet(damageInfo);

                damageInfo.procCoefficient = 0f;
                damageInfo.damageColorIndex = DamageColorIndex.Item;
            }
            else
            {
                damageInfo.rejected = true;
            }
        }

        public void RicochetBullet(DamageInfo damageInfo)
        {
            if (this.damageInfo == null)
            {
                this.damageInfo = new DamageInfo
                {
                    attacker = damageInfo.attacker,
                    canRejectForce = damageInfo.canRejectForce,
                    crit = damageInfo.crit,
                    damage = damageInfo.damage,
                    damageType = damageInfo.damageType,
                    position = damageInfo.position,
                    procChainMask = damageInfo.procChainMask,
                };

                startCoolStopwatch = true;
            }
            else
            {
                this.damageInfo.damage += damageInfo.damage * 0.75f;
                this.damageInfo.damageType |= damageInfo.damageType;
            }

            this.coolStopwatchScale = (coolStopwatchScale * bounceCountStored) + 0.01f;
        }
    }
}