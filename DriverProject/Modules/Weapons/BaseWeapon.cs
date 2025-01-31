using R2API;
using RobDriver.Modules.Survivors;
using RoR2.Skills;
using System;
using UnityEngine;

namespace RobDriver.Modules.Weapons
{
    public abstract class BaseWeapon<T> : BaseWeapon where T : BaseWeapon<T>
    {
        public static T instance { get; private set; }

        public BaseWeapon()
        {
            if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting BaseWeapon was instantiated twice");
            instance = this as T;
        }
    }

    public abstract class BaseWeapon
    {
        public DriverWeaponDef weaponDef { get; set; }
        public abstract string weaponNameToken { get; }
        public abstract string weaponName { get; }
        public abstract string weaponDesc { get; }
        public abstract string iconName { get; }
        public virtual Texture icon { get; set; }
        public abstract GameObject crosshairPrefab { get; }
        public abstract DriverWeaponTier tier { get; }
        public abstract int shotCount { get; }
        public abstract DriverWeaponDef.BuffType buffType { get; }
        public abstract SkillDef primarySkillDef { get; }
        public abstract SkillDef secondarySkillDef { get; }
        public abstract Mesh mesh { get; }
        public abstract Material material { get; }
        public abstract DriverWeaponDef.AnimationSet animationSet { get; }
        public abstract string calloutSoundString { get; }
        public abstract string configIdentifier { get; }
        public abstract float dropChance { get; }
        public abstract bool addToPool { get; }
        public abstract string uniqueDropBodyName { get; }

        public abstract void Init();

        protected void CreateLang()
        {
            LanguageAPI.Add("ROB_DRIVER_WEAPON_" + weaponNameToken + "_NAME", weaponName);
            LanguageAPI.Add("ROB_DRIVER_WEAPON_" + weaponNameToken + "_DESC", weaponDesc);
        }

        protected void CreateWeapon()
        {
            if (icon == null && !string.IsNullOrEmpty(iconName))
                icon = Assets.mainAssetBundle.LoadAsset<Texture>(iconName);

            weaponDef = DriverWeaponDef.CreateWeaponDefFromInfo(new DriverWeaponDefInfo
            {
                nameToken = "ROB_DRIVER_WEAPON_" + weaponNameToken + "_NAME",
                descriptionToken = "ROB_DRIVER_WEAPON_" + weaponNameToken + "_DESC",
                icon = icon,
                crosshairPrefab = crosshairPrefab,
                tier = tier,
                shotCount = shotCount,
                primarySkillDef = primarySkillDef,
                secondarySkillDef = secondarySkillDef,
                mesh = mesh,
                material = material,
                animationSet = animationSet,
                calloutSoundString = calloutSoundString,
                configIdentifier = configIdentifier,
                dropChance = dropChance,
                buffType = buffType
            });
            DriverWeaponCatalog.AddWeapon(weaponDef);
            DriverWeaponCatalog.AddWeaponDrop(uniqueDropBodyName, weaponDef);
            if (Modules.Config.enableArsenal.Value) Skills.AddWeaponSkill(Driver.characterPrefab, weaponDef, locked: true);
        }
    }
}