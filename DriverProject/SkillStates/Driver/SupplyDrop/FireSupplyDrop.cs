﻿using UnityEngine;
using RoR2;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using EntityStates;
using RobDriver.Modules;
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

        protected virtual DriverWeaponDef weaponDef => DriverWeaponCatalog.PrototypeRocketLauncher;

        protected virtual void SpawnWeapon()
        {
            if (NetworkServer.active)
            {
                GameObject weaponPickup = UnityEngine.Object.Instantiate<GameObject>(this.weaponDef.pickupPrefab, this.dropPosition, UnityEngine.Random.rotation);

                TeamFilter teamFilter = weaponPickup.GetComponent<TeamFilter>();
                if (teamFilter) teamFilter.teamIndex = this.teamComponent.teamIndex;

                var weaponComponent = weaponPickup.GetComponent<SyncPickup>();
                weaponComponent.bulletDef = DriverBulletCatalog.GetRandomBulletFromTier(DriverWeaponTier.Legendary);

                NetworkServer.Spawn(weaponPickup);
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