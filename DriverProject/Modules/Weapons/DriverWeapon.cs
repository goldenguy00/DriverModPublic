using EntityStates;
using R2API;
using RobDriver.Modules.Survivors;
using RoR2.Skills;
using System;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

namespace RobDriver.Modules.Weapons
{
    public abstract class BaseWeapon<T> : DriverWeapon<T> where T : BaseWeapon<T>
    {
        public abstract string weaponNameToken { get; }
        public abstract string weaponName { get; }
        public abstract string weaponDesc { get; }
        public abstract string iconName { get; }

        public override string nameToken => "ROB_DRIVER_WEAPON_" + weaponNameToken + "_NAME";
        public override string descriptionToken => "ROB_DRIVER_WEAPON_" + weaponNameToken + "_DESC";
        public override Texture icon
        {
            get
            {
                if (!string.IsNullOrEmpty(iconName))
                {
                    return Assets.mainAssetBundle.LoadAsset<Texture>(iconName);
                }
                return null;
            }
        }

        protected void CreateLang()
        {
            LanguageAPI.Add(nameToken, weaponName);
            LanguageAPI.Add(descriptionToken, weaponDesc);
        }

        public override void Init()
        {
            CreateLang();
            CreateWeapon();
        }
    }

    public abstract class DriverWeapon<T> where T: DriverWeapon<T>
    {
        public DriverWeapon()
        {
            if (instance != null) throw new InvalidOperationException("Singleton class \"" + typeof(T).Name + "\" inheriting BaseWeapon was instantiated twice");
            instance = this as T;
        }

        public static T instance { get; private set; }

        public DriverWeaponDef weaponDef { get; set; }

        public abstract string nameToken { get; }
        public abstract string descriptionToken { get; }
        public abstract Texture icon { get; }
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

        protected void CreateWeapon()
        {
            CreateWeaponDef();

            DriverWeaponCatalog.AddWeapon(weaponDef);
            DriverWeaponCatalog.AddWeaponDrop(uniqueDropBodyName, weaponDef);
            if (Modules.Config.enableArsenal.Value) Skills.AddWeaponSkill(Driver.characterPrefab, weaponDef, locked: true);
        }

        protected virtual void CreateWeaponDef()
        {
            weaponDef = DriverWeaponDef.CreateWeaponDefFromInfo(new DriverWeaponDefInfo
            {
                nameToken = nameToken,
                descriptionToken = descriptionToken,
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
        }
    }
}