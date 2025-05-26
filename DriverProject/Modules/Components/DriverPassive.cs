using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class DriverPassive : MonoBehaviour
    {
        public static SkillDef defaultPassive;
        public static SkillDef pistolOnlyPassive;
        public static SkillDef bulletsPassive;
        public static SkillDef godslingPassive;

        public GenericSkill passiveSkillSlot;

        public bool isDefault => this.passiveSkillSlot?.skillDef == DriverPassive.defaultPassive;
        public bool isBullets => this.passiveSkillSlot?.skillDef == DriverPassive.bulletsPassive;
        public bool isRyan => this.passiveSkillSlot?.skillDef == DriverPassive.godslingPassive;
        public bool isPistolOnly => this.passiveSkillSlot?.skillDef == DriverPassive.pistolOnlyPassive;
    }
}