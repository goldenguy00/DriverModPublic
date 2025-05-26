using System.Collections.Generic;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class DriverArsenal : MonoBehaviour
    {
        public static List<SkillDef> passiveSkills = [];

        public GenericSkill weaponSkillSlot;

        public DriverWeaponDef LoadoutWeapon
        {
            get
            {
                if (!this.weaponSkillSlot?.skillDef)
                    return DriverWeaponCatalog.Pistol;

                return DriverWeaponCatalog.GetWeaponFromIndex(passiveSkills.IndexOf(this.weaponSkillSlot.skillDef));
            }
        }
    }
}