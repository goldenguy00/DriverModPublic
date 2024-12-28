using RobDriver.Modules.Survivors;
using RoR2.Skills;
using UnityEngine;

namespace RobDriver.Modules.Weapons
{
    public class NemSword : BaseWeapon<NemSword>
    {
        public override string weaponNameToken => "NEM_SWORD";
        public override string weaponName => "HF Murasama";
        public override string weaponDesc => "The only thing I know.";
        public override string iconName => "texNemCommandoPrimary";
        public override GameObject crosshairPrefab => Modules.Assets.needlerCrosshairPrefab;
        public override DriverWeaponTier tier => DriverWeaponTier.Unique;
        public override int shotCount => 64;
        public override Mesh mesh => Modules.Assets.nemKatanaMesh;
        public override Material material => Assets.nemKatanaMat;
        public override DriverWeaponDef.AnimationSet animationSet => DriverWeaponDef.AnimationSet.BigMelee;
        public override DriverWeaponDef.BuffType buffType => DriverWeaponDef.BuffType.Damage;
        public override string calloutSoundString => "sfx_driver_callout_generic";
        public override string configIdentifier => "NemKatana";
        public override float dropChance => 100f;
        public override bool addToPool => false;
        public override string uniqueDropBodyName => "NemCommando";

        public override SkillDef primarySkillDef => Modules.Skills.CreatePrimarySkillDef(
            new EntityStates.SerializableEntityStateType(typeof(SkillStates.Driver.Compat.NemmandoSword.SwingSword)),
            "Weapon",
            DriverPlugin.developerPrefix + "_DRIVER_BODY_PRIMARY_NEMMANDOSWORD_NAME",
            DriverPlugin.developerPrefix + "_DRIVER_BODY_PRIMARY_NEMMANDOSWORD_DESCRIPTION",
            Modules.Assets.mainAssetBundle.LoadAsset<Sprite>("texNemCommandoPrimary"),
            false);

        public override SkillDef secondarySkillDef => Driver.nemmandoGunSecondarySkillDef;

        public override void Init()
        {
            CreateLang();
            CreateWeapon();
        }
    }
}