using R2API;
using RoR2;
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

        public abstract string weaponName { get; }
        public abstract string weaponNameToken { get; }
        public abstract string weaponDesc { get; }
        public abstract string weaponDescToken { get; }

        public abstract Sprite icon { get; }
        public abstract DriverWeaponTier dropTier { get; }
        public abstract DriverWeaponDef.AnimationSet animationSet { get; }
        public abstract DriverWeaponDef.BuffType buffType { get; }
        public abstract int shotCount { get; }

        public abstract Mesh mesh { get; }
        public abstract Material material { get; }
        public virtual GameObject crosshairPrefab { get; }
        public virtual GameObject pickupPrefabOverride { get; }
        public virtual Color? colorOverride { get; }

        public abstract SkillDef primarySkillDef { get; }
        public abstract SkillDef secondarySkillDef { get; }
        public virtual SkillDef arsenalSkillDef => Skills.CreateAndAddWeaponSkillDef(this.weaponNameToken, this.weaponDescToken);
        public virtual UnlockableDef unlockableDef => Unlockables.CreateAndAddWeaponUnlockableDef(this.weaponNameToken, this.weaponDescToken);

        public virtual string equipAnimationString => "BufferEmpty";
        public virtual string reloadAnimationString => "ReloadPistol";
        public virtual string calloutSoundString => "sfx_driver_callout_generic";
        public virtual string uniqueDropBodyName => "";
        public virtual float dropChance => 0f;
        public virtual bool disableHolster => false;

        public virtual void Init()
        {
            CreateLang();
            CreateWeaponDef();

            AddWeaponToCatalog();
            AddWeaponDrops();
        }

        protected virtual void CreateLang()
        {
            LanguageAPI.Add(this.weaponNameToken.ToUpperInvariant(), this.weaponName);
            LanguageAPI.Add(this.weaponDescToken.ToUpperInvariant(), this.weaponDesc);
        }

        protected virtual void CreateWeaponDef()
        {
            this.weaponDef ??= DriverWeaponDef.CreateWeaponDefFromInfo(new DriverWeaponDefInfo
            {
                name = this.weaponName,
                nameToken = this.weaponNameToken,
                description = this.weaponDesc,
                descriptionToken = this.weaponDescToken,

                icon = this.icon,
                tier = this.dropTier,
                animationSet = this.animationSet,
                buffType = this.buffType,
                shotCount = this.shotCount,

                primarySkillDef = this.primarySkillDef,
                secondarySkillDef = this.secondarySkillDef,
                arsenalSkillDef = this.arsenalSkillDef,
                unlockableDef = this.unlockableDef,

                mesh = this.mesh,
                material = this.material,
                crosshairPrefab = this.crosshairPrefab,
                pickupPrefabOverride = this.pickupPrefabOverride,
                colorOveride = this.colorOverride,

                equipAnimationString = this.equipAnimationString,
                calloutSoundString = this.calloutSoundString,
                dropChance = this.dropChance,
                disableHolster = this.disableHolster,
                reloadAnimationString = this.reloadAnimationString,
            });
        }

        protected virtual void AddWeaponToCatalog()
        {
            DriverWeaponCatalog.CreateAndAddWeapon(this.weaponDef);
        }

        protected virtual void AddWeaponDrops()
        {
            DriverWeaponCatalog.AddWeaponDrop(uniqueDropBodyName, weaponDef);
        }
    }
}