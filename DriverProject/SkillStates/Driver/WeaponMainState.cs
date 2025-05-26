using RobDriver.Modules.Components;
using UnityEngine;

namespace RobDriver.SkillStates.Driver
{
    public class WeaponMainState : EntityStates.Idle
    {
        private DriverController iDrive;
        private DriverWeaponDef cachedWeaponDef;
        private bool cancelling;

        public override void OnEnter()
        {
            base.OnEnter(); 
            this.iDrive = this.GetComponent<DriverController>();
            this.iDrive.onWeaponChanged += OnWeaponChanged;
            this.cachedWeaponDef = this.iDrive.weaponDef;
        }

        protected virtual void OnWeaponChanged(DriverWeaponDef weaponDef)
        {
            if (this.cachedWeaponDef != weaponDef)
            {
                this.cancelling = true;
                base.PlayCrossfade("Gesture, Override", weaponDef.equipAnimationString, 0.05f);

                if (this.iDrive.defaultWeaponDef != this.cachedWeaponDef && this.iDrive.defaultWeaponDef == weaponDef)
                {
                    if (base.isAuthority)
                        this.outer.SetNextState(new DiscardWeapon());

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

            this.cachedWeaponDef = weaponDef;
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.cancelling)
                return;
            
            if (!base.isAuthority || !this.iDrive)
                return;

            if (base.fixedAge > 0.35f && this.iDrive.weaponTimer < this.iDrive.maxWeaponTimer &&
               !this.iDrive.IsHoldingWeapon && !this.iDrive.HasSpecialBullets)
            {
                this.outer.SetNextState(new Reload());
            }
        }
        public override void OnExit()
        {
            base.OnExit();

            if (this.iDrive)
                this.iDrive.onWeaponChanged -= OnWeaponChanged;
        }
    }
}
