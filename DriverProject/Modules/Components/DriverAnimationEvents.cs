using UnityEngine;
using RoR2;

namespace RobDriver.Modules.Components
{
    public class DriverAnimationEvents : MonoBehaviour
    {
        private ChildLocator childLocator;
        public DriverController iDrive;

        private void Start()
        {
            this.childLocator = this.GetComponent<ChildLocator>();
        }

        public void PlaySound(string soundString)
        {
            Util.PlaySound(soundString, this.gameObject);
        }

        public void FinishReload()
        {
            if (!this.iDrive)
                return;

            if (!this.iDrive.IsHoldingWeapon && !this.iDrive.HasSpecialBullets)
            {
                this.iDrive.SetBulletAmmo();
            }
        }

        public void EndHolster()
        {
            if (this.iDrive.weaponDef.animationSet != DriverWeaponDef.AnimationSet.Default)
            {
                this.childLocator.FindChildGameObject("PistolModel").SetActive(true);
                this.iDrive.DestroyHolsteredWeaponInstance();
            }

            this.childLocator.FindChildGameObject("AltWeaponModel").SetActive(true);
            this.childLocator.FindChildGameObject("KnifeModel").SetActive(false);
        }
    }
}