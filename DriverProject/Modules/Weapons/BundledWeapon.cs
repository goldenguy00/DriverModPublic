using System;
using RoR2.Skills;
using UnityEngine;

namespace RobDriver.Modules.Weapons
{
    public abstract class BundledWeapon<T> : BaseWeapon where T : BaseWeapon
    {
        public static T instance { get; private set; }

        public BundledWeapon()
        {
            if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting BaseWeapon was instantiated twice");
            instance = this as T;

            LoadWeaponFromBundle();
            instance.Init();
        }

        public override string weaponName => this.weaponDef.weaponName;
        public override string weaponNameToken => this.weaponDef.nameToken;
        public override string weaponDesc => this.weaponDef.description;
        public override string weaponDescToken => this.weaponDef.descriptionToken;
        public override Sprite icon => this.weaponDef.icon;
        public override DriverWeaponTier dropTier => this.weaponDef.tier;
        public override DriverWeaponDef.AnimationSet animationSet => this.weaponDef.animationSet;
        public override DriverWeaponDef.BuffType buffType => this.weaponDef.buffType;
        public override int shotCount => this.weaponDef.shotCount;
        public override Mesh mesh => this.weaponDef.mesh;
        public override Material material => this.weaponDef.material;
        public override GameObject crosshairPrefab => this.weaponDef.crosshairPrefab;
        public override SkillDef primarySkillDef => this.weaponDef.primarySkillDef;
        public override SkillDef secondarySkillDef => this.weaponDef.secondarySkillDef;

        public abstract void LoadWeaponFromBundle();
    }
}
