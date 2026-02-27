using RoR2;
using UnityEngine;
using RobDriver.SkillStates.BaseStates;
using UnityEngine.Networking;
using RobDriver.Modules.Components.UI;

namespace RobDriver.SkillStates.Driver.SniperRifle
{
    public class Shoot : BaseDriverShootState
    {
        public static float _damageCoefficient = 6f;

        protected override float damageCoefficient => _damageCoefficient;
        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override float maxBulletSpread => 0f;
        protected override string shootSoundString => "sfx_driver_sniper_shoot";
        protected override string animationString => "FireSniper";
        protected override DamageTypeCombo damageType => this.iDrive.DamageType;
        protected override GameObject tracerPrefab => Modules.Assets.sniperTracer;
        protected override GameObject muzzleFlashPrefab => EntityStates.Commando.CommandoWeapon.FirePistol2.muzzleEffectPrefab;
        protected override GameObject hitEffectPrefab => EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab;

        public bool aiming;

        public override void OnEnter()
        {
            base.procCoefficient = 1f;
            base.bulletCount = 1;
            base.dropShells = 0;
            base.ammoConsumption = 1f;
            base.useAttackSpeed = true;

            base.bulletFalloff = BulletAttack.FalloffModel.None;
            base.hitMask = LayerIndex.CommonMasks.bullet;
            base.stopperMask = LayerIndex.world.collisionMask;
            base.damageColorIndex = DamageColorIndex.Default;

            base.bulletRange = 2000f;
            base.bulletThiccness = 0.35f;
            base.bulletForce = 2500f;
            base.selfForce = 100f;

            base.baseDuration = 1.2f;
            base.earlyExitFraction = 0.75f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 16f;
            base.spreadBloom = 4f;
            base.aimTimer = 5f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "ShotgunMuzzle";

            base.OnEnter();

            if (this.aiming && this.iDrive.shurikenComponent)
                this.iDrive.shurikenComponent.OnSkillActivated(base.skillLocator.primary);

            this.iDrive.machineGunVFX.Play();
        }

        protected override void AuthorityModifyBulletAttack(ref BulletAttack bulletAttack)
        {
            base.AuthorityModifyBulletAttack(ref bulletAttack);

            bulletAttack.damageType.damageType |= DamageType.BypassArmor | DamageType.Stun1s;
            bulletAttack.sniper = true;

            if (this.aiming)
            {
                bulletAttack.damageType.damageType |= DamageType.BypassBlock;
                bulletAttack.damageType.damageSource = DamageSource.Secondary;
                bulletAttack.modifyOutgoingDamageCallback = delegate (BulletAttack _bulletAttack, ref BulletAttack.BulletHit hitInfo, DamageInfo damageInfo)
                {
                    if (hitInfo.isSniperHit)
                    {
                        damageInfo.damage *= 2f;
                        damageInfo.damageColorIndex = DamageColorIndex.Sniper;

                        EffectData effectData = new EffectData
                        {
                            origin = hitInfo.point,
                            rotation = Quaternion.LookRotation(-hitInfo.direction)
                        };

                        effectData.SetHurtBoxReference(hitInfo.hitHurtBox);
                        EffectManager.SpawnEffect(SteadyAim.weakPoint, effectData, true);
                        Util.PlaySound("sfx_driver_headshot", base.gameObject);
                        hitInfo.hitHurtBox.healthComponent.gameObject.AddComponent<DriverHeadshotTracker>();
                    }
                };
            }
            else
            {
                bulletAttack.minSpread = 3f;
                bulletAttack.maxSpread = 6f;
            }
        }

        protected override void FireBullet()
        {
            if (this.aiming && NetworkServer.active)
            {
                var itemCount = this.characterBody.inventory ? this.characterBody.inventory.GetItemCountEffective(DLC2Content.Items.IncreasePrimaryDamage) : 0;
                if (itemCount > 0)
                    this.characterBody.AddIncreasePrimaryDamageStack();
            }

            base.FireBullet();
        }
    }
}