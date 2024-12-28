using RoR2;
using System.Linq;
using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class DriverArsenal : MonoBehaviour
    {
        public GenericSkill weaponSkillSlot;

        public DriverWeaponDef weaponDef;

        public DriverWeaponDef DefaultWeapon
        {
            get
            {
                if (!this.weaponSkillSlot || !this.weaponSkillSlot.skillDef)
                    return DriverWeaponCatalog.Pistol;

                if (!this.weaponDef)
                    this.weaponDef = DriverWeaponCatalog.weaponDefs.FirstOrDefault(def => def.nameToken == this.weaponSkillSlot.skillDef.skillName);

                return this.weaponDef ? this.weaponDef : DriverWeaponCatalog.Pistol;
            }
        }
    }
}