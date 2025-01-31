using RoR2;
using RoR2.Skills;
using UnityEngine;

namespace RobDriver.Modules.Components
{
    public class DriverPassive : MonoBehaviour
    {
        public SkillDef defaultPassive;
        public SkillDef pistolOnlyPassive;
        public SkillDef bulletsPassive;
        public SkillDef godslingPassive;
        public GenericSkill passiveSkillSlot;

        public bool isDefault => this.passiveSkillSlot && this.defaultPassive && this.passiveSkillSlot.skillDef == this.defaultPassive;

        public bool isPistolOnly => this.passiveSkillSlot && this.pistolOnlyPassive && this.passiveSkillSlot.skillDef == this.pistolOnlyPassive;

        public bool isBullets => this.passiveSkillSlot && this.bulletsPassive && this.passiveSkillSlot.skillDef == this.bulletsPassive;

        public bool isRyan => this.passiveSkillSlot && this.godslingPassive && this.passiveSkillSlot.skillDef == this.godslingPassive;
    }
}