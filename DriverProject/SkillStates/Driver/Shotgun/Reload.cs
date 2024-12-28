using UnityEngine;
using EntityStates;

namespace RobDriver.SkillStates.Driver.Shotgun
{
    public class Reload : BaseDriverSkillState
    {
        public float duration = 1.75f;

        public override void OnEnter()
        {
            base.OnEnter();

            base.PlayAnimation("Gesture, Override", "ReloadShotgun", "Shoot.playbackRate", this.duration);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.iDrive && this.iDrive.weaponDef != this.cachedWeaponDef)
            {
                base.PlayAnimation("Gesture, Override", "BufferEmpty");
                this.outer.SetNextStateToMain();
                return;
            }

            if (base.fixedAge >= this.duration)
            {
                if (this.iDrive.weaponDef == this.iDrive.defaultWeaponDef && !this.iDrive.HasSpecialBullets) iDrive.FinishReload();
                this.outer.SetNextStateToMain();
            }
        }
    }
}