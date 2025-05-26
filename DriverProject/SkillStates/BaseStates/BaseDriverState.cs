using EntityStates;
using RobDriver.Modules.Components;

namespace RobDriver.SkillStates.BaseStates
{
    public abstract class BaseDriverState : BaseState
    {
        protected virtual bool holsterGun => false;
        protected virtual string showProp => string.Empty;

        protected DriverController iDrive;
        protected ChildLocator childLocator;
        protected DriverWeaponDef cachedWeaponDef;

        protected bool cancelling;

        public override void OnEnter()
        {
            base.OnEnter();
            this.RefreshState();

            this.iDrive.onWeaponChanged += this.OnWeaponChanged;

            if (this.holsterGun)
                this.iDrive.SetHolsteredWeaponInstance(this.cachedWeaponDef);

            if (!string.IsNullOrEmpty(this.showProp))
                this.childLocator.FindChildGameObject(this.showProp).SetActive(true);
        }

        protected void RefreshState()
        {
            if (!this.childLocator)
                this.childLocator = this.GetModelChildLocator();

            if (!this.iDrive)
                this.iDrive = this.GetComponent<DriverController>();

            this.cachedWeaponDef = this.iDrive.weaponDef;
        }

        protected virtual void OnWeaponChanged(DriverWeaponDef weaponDef)
        {
            this.cachedWeaponDef = weaponDef;

            if (this.iDrive.HolsteredWeapon)
                this.iDrive.SetHolsteredWeaponInstance(weaponDef);
        }

        public virtual void AddRecoil(float x, float y)
        {
            if (Modules.Config.enableRecoil.Value)
                this.AddRecoil(-0.5f * x, -1f * x, -1f * y, y);
        }

        public override void ModifyNextState(EntityState nextState)
        {
            base.ModifyNextState(nextState);

            if (nextState is BaseDriverState driverState)
            {
                driverState.iDrive = this.iDrive;
                driverState.childLocator = this.childLocator;
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.iDrive)
            {
                this.iDrive.onWeaponChanged -= OnWeaponChanged;

                if (this.holsterGun)
                    this.iDrive.DestroyHolsteredWeaponInstance();
            }

            if (this.childLocator)
            {
                this.childLocator.FindChildGameObject("PistolModel")?.SetActive(true);

                if (!string.IsNullOrEmpty(this.showProp))
                    this.childLocator.FindChildGameObject(this.showProp)?.SetActive(false);
            }
        }
    }
}