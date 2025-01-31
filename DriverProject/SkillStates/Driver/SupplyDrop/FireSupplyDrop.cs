﻿using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using EntityStates;
using RobDriver.Modules.Components;

namespace RobDriver.SkillStates.Driver.SupplyDrop
{
    public class FireSupplyDrop : BaseDriverSkillState
    {
        public float baseDuration = 0.8f;

        public static float damageCoefficient = 16f;

        public Vector3 dropPosition;
        public Quaternion dropRotation;

        protected float duration;
        private bool hasFired;

        protected virtual bool cutAmmo => false;
        protected virtual DriverWeaponDef weaponDef => DriverWeaponCatalog.PrototypeRocketLauncher;
        protected virtual DriverBulletDef bulletDef => DriverBulletCatalog.GetRandomBulletFromTier(DriverWeaponTier.Legendary);

        public override void OnEnter()
        {
            base.OnEnter();
            this.duration = this.baseDuration / this.attackSpeedStat;

            this.PlayAnim();
        }

        public override void OnExit()
        {
            base.OnExit();
            this.HideButton();
        }

        protected virtual void PlayAnim()
        {
            Util.PlaySound("sfx_driver_button_foley", this.gameObject);
            base.PlayAnimation("Gesture, Override", "PressButton", "Action.playbackRate", this.duration);
        }

        protected virtual void HideButton()
        {
            this.FindModelChild("ButtonModel").gameObject.SetActive(false);
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
                    this.Fire();
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
                weaponPickup.GetComponent<SyncPickup>().SpawnWeapon(this.teamComponent.teamIndex, this.weaponDef, this.bulletDef, this.cutAmmo);
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

        private void Fire()
        {
            this.SpawnWeapon();
            this.FireBlast();
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