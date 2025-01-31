using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver.SupplyDrop.Scepter
{
    public class FireVoidDrop : FireSupplyDrop
    {
        protected override DriverWeaponDef weaponDef => DriverWeaponCatalog.PlasmaCannon;
        protected override DriverBulletDef bulletDef => DriverBulletCatalog.GetRandomBulletFromTier(DriverWeaponTier.Legendary);

        public override void OnEnter()
        {
            base.OnEnter();

            EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidSurvivor/VoidSurvivorMegaBlasterExplosion.prefab").WaitForCompletion(),
                new EffectData
                {
                    origin = this.FindModelChild("HandL").position,
                    rotation = Quaternion.identity,
                    scale = AimSupplyDrop.radius * 0.5f
                }, false);
        }

        protected override void PlayAnim()
        {
            base.PlayAnimation("Gesture, Override", "PressVoidButton", "Action.playbackRate", this.duration);
        }

        protected override void FireBlast()
        {
            if (base.isAuthority)
            {
                BlastAttack blastAttack = new BlastAttack();
                blastAttack.radius = AimSupplyDrop.radius;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = this.dropPosition;
                blastAttack.attacker = this.gameObject;
                blastAttack.crit = this.RollCrit();
                blastAttack.baseDamage = this.damageStat * FireSupplyDrop.damageCoefficient;
                blastAttack.falloffModel = BlastAttack.FalloffModel.Linear;
                blastAttack.baseForce = 4000f;
                blastAttack.teamIndex = this.teamComponent.teamIndex;
                blastAttack.damageType = DamageType.Stun1s;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;

                blastAttack.Fire();

                EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/VoidMegaCrab/VoidMegacrabAntimatterExplosion.prefab").WaitForCompletion(),
                    new EffectData
                    {
                        origin = this.dropPosition,
                        rotation = this.dropRotation,
                        scale = AimSupplyDrop.radius
                    }, true);
            }

            Util.PlaySound("sfx_driver_explosion", this.gameObject);
        }
    }
}