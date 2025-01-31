using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver.GoldenGun
{
    public class LightsOut : Revolver.LightsOut
    {
        protected override string shootSoundString => "sfx_driver_pistol_shoot_charged";
        protected override void FireBullet()
        {
            Ray aimRay = base.GetAimRay();

            BulletAttack bulletAttack = new BulletAttack
            {
                bulletCount = 1,
                aimVector = aimRay.direction,
                origin = aimRay.origin,
                damage = LightsOut.damageCoefficient * this.damageStat,
                damageColorIndex = DamageColorIndex.Default,
                damageType = DamageType.BypassArmor | iDrive.DamageType,
                falloffModel = BulletAttack.FalloffModel.None,
                maxDistance = 9999f,
                force = 9999f,
                hitMask = LayerIndex.CommonMasks.bullet,
                minSpread = 0f,
                maxSpread = 0f,
                isCrit = true,
                owner = this.gameObject,
                muzzleName = "PistolMuzzle",
                smartCollision = true,
                procChainMask = default(ProcChainMask),
                procCoefficient = LightsOut.procCoefficient,
                radius = 1f,
                sniper = false,
                stopperMask = LayerIndex.CommonMasks.bullet,
                weapon = null,
                tracerEffectPrefab = Shoot.critTracerEffectPrefab,
                spreadPitchScale = 1f,
                spreadYawScale = 1f,
                queryTriggerInteraction = QueryTriggerInteraction.UseGlobal,
                hitEffectPrefab = EntityStates.Commando.CommandoWeapon.FirePistol2.hitEffectPrefab,
            };
            bulletAttack.damageType.damageSource = DamageSource.Secondary;

            bulletAttack.modifyOutgoingDamageCallback = delegate (BulletAttack _bulletAttack, ref BulletAttack.BulletHit hitInfo, DamageInfo damageInfo)
            {
                if (BulletAttack.IsSniperTargetHit(hitInfo))
                {
                    damageInfo.damage *= 2f;
                    damageInfo.damageColorIndex = DamageColorIndex.Sniper;

                    EffectData effectData = new EffectData
                    {
                        origin = hitInfo.point,
                        rotation = Quaternion.LookRotation(-hitInfo.direction)
                    };

                    effectData.SetHurtBoxReference(hitInfo.hitHurtBox);
                    EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Common/VFX/WeakPointProcEffect.prefab").WaitForCompletion(), effectData, true);
                    //RoR2.Util.PlaySound("Play_SniperClassic_headshot", base.gameObject);
                }
            };

            bulletAttack.Fire();
        }
    }
}