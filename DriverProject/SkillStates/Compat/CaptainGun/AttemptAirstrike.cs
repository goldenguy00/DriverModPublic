using EntityStates.Captain.Weapon;
using RobDriver.Modules;
using RobDriver.SkillStates.BaseStates;
using RoR2;
using RoR2.Skills;
using RoR2.UI;
using UnityEngine;

namespace RobDriver.SkillStates.Driver.CaptainGun
{
    public class AttemptAirstrike : BaseDriverSkillState
    {
        public static SkillDef primarySkillDef;

        private float timerSinceComplete;

        private CrosshairUtils.OverrideRequest crosshairOverrideRequest;
        private GameObject effectMuzzleInstance;

        public override void OnEnter()
        {
            base.OnEnter();

            PlayAnimation("Gesture, Override", "ReadyVoidButton", "Action.playbackRate", 0.8f);

            this.skillLocator.primary?.SetWeaponSkill(AttemptAirstrike.primarySkillDef, GenericSkill.SkillOverridePriority.Contextual);

            this.effectMuzzleInstance = Object.Instantiate(SetupAirstrike.effectMuzzlePrefab, base.FindModelChild("PistolMuzzle"));
            this.crosshairOverrideRequest = CrosshairUtils.RequestOverrideForBody(base.characterBody, SetupAirstrike.crosshairOverridePrefab, CrosshairUtils.OverridePriority.Skill);

            Util.PlaySound("Play_captain_shift_start", base.gameObject);
            Util.PlaySound("Play_captain_shift_active_loop", base.gameObject);
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            if (this.cancelling)
                return;

            if (base.characterDirection)
                base.characterDirection.moveVector = base.GetAimRay().direction;

            if (this.skillLocator.primary?.stock == 0)
            {
                this.timerSinceComplete += base.GetDeltaTime();
                if (this.timerSinceComplete > SetupAirstrike.baseExitDuration / base.attackSpeedStat)
                {
                    base.outer.SetNextStateToMain();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            this.skillLocator.primary?.UnsetWeaponSkill(AttemptAirstrike.primarySkillDef, GenericSkill.SkillOverridePriority.Contextual);

            Util.PlaySound("Play_captain_shift_end", base.gameObject);
            Util.PlaySound("Stop_captain_shift_active_loop", base.gameObject);

            if (this.effectMuzzleInstance)
                Destroy(this.effectMuzzleInstance);

            this.crosshairOverrideRequest?.Dispose();
        }
    }
}
