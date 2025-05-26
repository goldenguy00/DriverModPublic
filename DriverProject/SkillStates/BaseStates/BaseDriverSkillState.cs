using EntityStates;
using RobDriver.Modules.Components;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace RobDriver.SkillStates.BaseStates
{
    public abstract class BaseDriverSkillState : BaseDriverState, ISkillState
    {
        protected virtual bool cancelOnPickup => true;
        public GenericSkill activatorSkillSlot { get; set; }

        protected override void OnWeaponChanged(DriverWeaponDef weaponDef)
        {
            if (this.cancelOnPickup && this.cachedWeaponDef != weaponDef)
            {
                this.cancelling = true;
                base.PlayAnimation("Gesture, Override", weaponDef.equipAnimationString);

                // return to default
                if (this.iDrive.defaultWeaponDef != this.cachedWeaponDef && this.iDrive.defaultWeaponDef == weaponDef)
                {
                    if (base.isAuthority)
                        this.outer.SetNextState(new Driver.DiscardWeapon());

                    GameObject newEffect = GameObject.Instantiate(Modules.Assets.discardedWeaponEffect);
                    newEffect.GetComponent<DiscardedWeaponComponent>().Init(this.cachedWeaponDef, this.characterBody.characterDirection.forward, this.characterBody.characterMotor.velocity);
                    newEffect.transform.rotation = this.characterBody.modelLocator.modelTransform.rotation;
                    newEffect.transform.position = this.GetModelChildLocator().FindChild("Pistol").position + (Vector3.up * 0.5f);
                }
                else
                {
                    if (base.isAuthority)
                        this.outer.SetNextStateToMain();
                }
            }

            base.OnWeaponChanged(weaponDef);
        }

        public override void OnSerialize(NetworkWriter writer)
        {
            base.OnSerialize(writer);
            this.Serialize(base.skillLocator, writer);
        }

        public override void OnDeserialize(NetworkReader reader)
        {
            base.OnDeserialize(reader);
            this.Deserialize(base.skillLocator, reader);
        }

        public bool IsKeyDownAuthority()
        {
            return this.IsKeyDownAuthority(base.skillLocator, base.inputBank);
        }
    }
}