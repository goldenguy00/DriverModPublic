using RoR2;
using UnityEngine;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.GolemGun
{
    public class FireLaser : BaseDriverShootState
    {
        public static float _damageCoefficient = 8f;

        protected override float damageCoefficient => _damageCoefficient;
        protected override float earlyExitTime => this.earlyExitFraction * this.duration;
        protected override float animationDuration => this.duration;
        protected override float maxBulletSpread => this.characterBody.spreadBloomAngle;
        protected override string shootSoundString => EntityStates.GolemMonster.FireLaser.attackSoundString;
        protected override string animationString => "FireTwohand";
        protected override DamageTypeCombo damageType => this.iDrive.DamageType;
        protected override GameObject tracerPrefab => null;
        protected override GameObject muzzleFlashPrefab => EntityStates.GolemMonster.FireLaser.effectPrefab;
        protected override GameObject hitEffectPrefab => null;

        public override void OnEnter()
        {
            base.procCoefficient = 1f;
            base.bulletCount = 1;
            base.dropShells = 0;
            base.ammoConsumption = 2f;
            base.useAttackSpeed = true;

            base.bulletFalloff = BulletAttack.FalloffModel.None;
            base.hitMask = LayerIndex.CommonMasks.laser;
            base.stopperMask = LayerIndex.CommonMasks.laser;
            base.damageColorIndex = DamageColorIndex.Default;

            base.bulletRange = 2000f;
            base.bulletThiccness = 2f;
            base.bulletForce = 200f;
            base.selfForce = 20f;

            base.baseDuration = 0.7f;
            base.earlyExitFraction = 0.5f;
            base.fireDelayFraction = 0f;

            base.visualRecoilAmplitude = 16f;
            base.spreadBloom = 4f;
            base.aimTimer = 2f;

            base.playbackRateString = "Shoot.playbackRate";
            base.muzzleString = "ShotgunMuzzle";

            base.OnEnter();
        }

        protected override void AuthorityModifyBulletAttack(ref BulletAttack bulletAttack)
        {
            base.AuthorityModifyBulletAttack(ref bulletAttack);

            bulletAttack.modifyOutgoingDamageCallback += delegate (BulletAttack _bulletAttack, ref BulletAttack.BulletHit hitInfo, DamageInfo damageInfo)
            {
                new BlastAttack
                {
                    attacker = base.gameObject,
                    inflictor = base.gameObject,
                    teamIndex = base.teamComponent.teamIndex,
                    baseDamage = base.damageStat * _damageCoefficient * 0.5f,
                    damageType = base.iDrive.DamageType,
                    baseForce = this.bulletForce * 0.2f,
                    position = hitInfo.point,
                    radius = 10f,
                    falloffModel = BlastAttack.FalloffModel.SweetSpot,
                    bonusForce = this.bulletForce * hitInfo.direction
                }.Fire();

                EffectData effectData = new EffectData
                {
                    origin = hitInfo.point,
                    start = _bulletAttack.origin
                };
                effectData.SetChildLocatorTransformReference(base.gameObject, base.childLocator.FindChildIndex("ShotgunMuzzle"));

                EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.tracerEffectPrefab, effectData, true);
                EffectManager.SpawnEffect(EntityStates.GolemMonster.FireLaser.hitEffectPrefab, effectData, true);
            };
        }
    }
}