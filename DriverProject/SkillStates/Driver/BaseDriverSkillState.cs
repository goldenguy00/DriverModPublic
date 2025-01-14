using EntityStates;
using RobDriver.Modules.Components;

namespace RobDriver.SkillStates.Driver
{
    public class BaseDriverSkillState : BaseSkillState
    {
        public virtual void AddRecoil2(float x1, float x2, float y1, float y2)
        {
            if (!Modules.Config.enableRecoil.Value) return;
            this.AddRecoil(x1, x2, y1, y2);
        }

        protected virtual bool hideGun => false;
        protected virtual string prop => string.Empty;

        protected DriverController iDrive;
        protected DriverWeaponDef cachedWeaponDef;


        public override void OnEnter()
        {
            base.OnEnter();
            RefreshState();

            if (this.hideGun) this.GetModelChildLocator().FindChild("PistolModel").gameObject.SetActive(false);
            if (this.prop != string.Empty) this.GetModelChildLocator().FindChild(this.prop).gameObject.SetActive(true);
        }

        public void RefreshState()
        {
            if (!this.iDrive) this.iDrive = this.GetComponent<DriverController>();
            if (this.iDrive) this.cachedWeaponDef = this.iDrive.weaponDef;
        }

        public override void OnExit()
        {
            base.OnExit();

            if (this.hideGun) this.GetModelChildLocator().FindChild("PistolModel").gameObject.SetActive(true);
            if (this.prop != string.Empty) this.GetModelChildLocator().FindChild(this.prop).gameObject.SetActive(false);
        }
    }
}