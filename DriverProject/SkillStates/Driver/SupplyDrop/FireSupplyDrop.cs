using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using EntityStates;
using RobDriver.Modules.Components;
using RobDriver.SkillStates.BaseStates;

namespace RobDriver.SkillStates.Driver.SupplyDrop
{
    public class FireSupplyDrop : BaseDriverSkillState
    {
        public static float baseDuration = 0.8f;
        public static float damageCoefficient = 16f;

        public Vector3 dropPosition;
        public Quaternion dropRotation;
        protected float duration;
        private bool hasFired;

        protected override string showProp => "ButtonModel";
        protected override bool holsterGun => true;
        protected virtual bool cutAmmo => false;
        protected virtual DriverWeaponDef weaponDef => DriverWeaponCatalog.PrototypeRocketLauncher;
        protected virtual DriverBulletDef bulletDef => DriverBulletCatalog.GetRandomBulletFromTier(DriverWeaponTier.Legendary);

        public override void OnEnter()
        {
            base.OnEnter();

            this.duration = baseDuration / this.attackSpeedStat;

            this.PlayAnim();
        }

        protected virtual void PlayAnim()
        {
            Util.PlaySound("sfx_driver_button_foley", this.gameObject);
            base.PlayAnimation("Gesture, Override", "PressButton", "Action.playbackRate", this.duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (base.fixedAge >= (0.4f * this.duration))
            {
                if (!this.hasFired)
                {
                    this.hasFired = true;
                    this.skillLocator.special.DeductStock(1);

                    this.SpawnWeapon();
                    this.FireBlast();
                }
            }

            if (base.fixedAge >= this.duration)
            {
                this.outer.SetNextStateToMain();
            }
        }

        protected virtual void SpawnWeapon()
        {
            if (NetworkServer.active)
            {
                var weaponPickup = GameObject.Instantiate(Modules.Assets.weaponPickup, this.dropPosition, Random.rotation);
                weaponPickup.GetComponent<SyncPickup>().SpawnWeapon(this.weaponDef, this.bulletDef, this.cutAmmo, false);
            }
        }

        protected virtual void FireBlast()
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
                blastAttack.falloffModel = BlastAttack.FalloffModel.SweetSpot;
                blastAttack.baseForce = 4000f;
                blastAttack.teamIndex = this.teamComponent.teamIndex;
                blastAttack.damageType = DamageType.Stun1s;
                blastAttack.damageType.damageSource = DamageSource.Special;
                blastAttack.attackerFiltering = AttackerFiltering.NeverHitSelf;

                blastAttack.Fire();

                EffectManager.SpawnEffect(Addressables.LoadAssetAsync<GameObject>("RoR2/Base/SurvivorPod/PodGroundImpact.prefab").WaitForCompletion(),
                    new EffectData
                    {
                        origin = this.dropPosition,
                        rotation = this.dropRotation,
                        scale = AimSupplyDrop.radius
                    }, true);
            }

            Util.PlaySound("sfx_driver_explosion", this.gameObject);
        }

        public override InterruptPriority GetMinimumInterruptPriority()
        {
            return InterruptPriority.Pain;
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            writer.Write(this.dropPosition);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.dropPosition = reader.ReadVector3();
        }
    }
}