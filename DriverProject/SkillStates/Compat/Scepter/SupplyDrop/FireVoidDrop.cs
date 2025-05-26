using RobDriver.SkillStates.Driver.SupplyDrop;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace RobDriver.SkillStates.Driver.Scepter.SupplyDrop
{
    public class FireVoidDrop : FireSupplyDrop
    {
        protected override string showProp => "";
        protected override DriverWeaponDef weaponDef => DriverWeaponCatalog.GetRandomWeaponFromTier(DriverWeaponTier.Void);
        protected override DriverBulletDef bulletDef => DriverBulletCatalog.GetRandomBulletFromTier(DriverWeaponTier.Unique);

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
            PlayAnimation("Gesture, Override", "PressVoidButton", "Action.playbackRate", this.duration);
        }

        protected override void FireBlast()
        {
            if (isAuthority)
            {
                var blastAttack = new BlastAttack();
                blastAttack.radius = AimSupplyDrop.radius;
                blastAttack.procCoefficient = 1f;
                blastAttack.position = this.dropPosition;
                blastAttack.attacker = this.gameObject;
                blastAttack.crit = this.RollCrit();
                blastAttack.baseDamage = this.damageStat * damageCoefficient;
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