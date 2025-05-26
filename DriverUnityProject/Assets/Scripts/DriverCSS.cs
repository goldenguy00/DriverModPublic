using UnityEngine;
using RoR2;

namespace RobDriver.Modules.Components
{
    public class DriverCSS : MonoBehaviour
    {
        public void ThrowGun()
        {
            //RoR2.Util.PlaySound("sfx_driver_gun_throw", this.gameObject);
        }

        public void CatchGun()
        {
        //    Util.PlaySound("sfx_driver_gun_catch", this.gameObject);
        }

        public void FailCatchGun()
        {

        }

        public void GunDrop()
        {
          //  Util.PlaySound("sfx_driver_gun_drop", this.gameObject);
        }
    }
}