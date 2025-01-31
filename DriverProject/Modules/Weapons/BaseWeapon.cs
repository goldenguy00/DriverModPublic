using R2API;
using RobDriver.Modules.Survivors;
using RoR2.Skills;
using System;
using UnityEngine;

namespace RobDriver.Modules.Weapons
{
    /// <summary>
    /// used within the mod to easily add name, description, and icon
    /// </summary>
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
}