using System.Collections.Generic;
using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class DriverArsenal : MonoBehaviour
    {
        public static Dictionary<SkillDef, ushort> passiveSkillsToWeaponIndex = [];

        public GenericSkill weaponSkillSlot;

        public DriverWeaponDef DefaultWeapon
        {
            get
            {
                var skillDef = this.weaponSkillSlot ? this.weaponSkillSlot.skillDef : null;
                if (!skillDef || !passiveSkillsToWeaponIndex.ContainsKey(skillDef))
                    return DriverWeaponCatalog.Pistol;

                return DriverWeaponCatalog.GetWeaponFromIndex(passiveSkillsToWeaponIndex[skillDef]);
            }
        }
    }
}