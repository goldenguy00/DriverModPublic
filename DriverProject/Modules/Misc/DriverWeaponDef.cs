using System;
using RobDriver.Modules;
using RoR2;
using RoR2.Skills;
using UnityEngine;

[CreateAssetMenu(fileName = "wpn", menuName = "ScriptableObjects/WeaponDef", order = 1)]
public class DriverWeaponDef : ScriptableObject
{
    public enum AnimationSet // i hate enums but this is okay being one since animation sets won't be added often
    {
        Default,
        TwoHanded,
        BigMelee
    }

    public enum BuffType
    {
        Crit,
        Damage,
        AttackSpeed
    }

    [Serializable]
    public struct ModelSwapInfo
    {
        public Mesh mesh;
        public Material material;
        public string childName;
    }

    [Header("General")]
    public string weaponName = "";
    public string nameToken = "";
    public string description = "";
    public string descriptionToken = "";

    public Sprite icon = null;
    public DriverWeaponTier tier = DriverWeaponTier.Common;
    public AnimationSet animationSet = AnimationSet.Default;
    public BuffType buffType = BuffType.Damage;
    public int shotCount = 8;

    [Header("Skills")]
    public SkillDef primarySkillDef;
    public SkillDef secondarySkillDef;
    public SkillDef arsenalSkillDef;
    public UnlockableDef unlockableDef;

    [Header("Visuals")]
    public ModelSwapInfo[] modelSwapInfo = new ModelSwapInfo[] { new ModelSwapInfo { childName = "PistolModel" } };
    public GameObject crosshairPrefab = null;
    public GameObject pickupPrefabOverride = null;
    public Color? colorOverride = null;
    public bool disableHolster = false;

    [Header("Other")]
    public string equipAnimationString = "BufferEmpty";
    public string reloadAnimationString = "ReloadPistol";
    public string calloutSoundString = "sfx_driver_callout_generic";
    public string dropBodyName = "";
    public float dropChance = 0f;

    public Mesh mesh
    {
        get => modelSwapInfo[0].mesh;
        set => modelSwapInfo[0].mesh = value;
    }

    public Material material
    {
        get => modelSwapInfo[0].material;
        set => modelSwapInfo[0].material = value;
    }

    public Color color => this.colorOverride ?? Helpers.GetColorForTier(this.tier);
    public GameObject pickupPrefab => this.pickupPrefabOverride ?? Helpers.GetPickupPrefabForTier(this.tier);

    [HideInInspector]
    public ushort index; // assigned at runtime

    [HideInInspector]
    public bool enabled; // assigned at runtime

    public static DriverWeaponDef CreateWeaponDefFromInfo(DriverWeaponDefInfo weaponDefInfo)
    {
        DriverWeaponDef weaponDef = ScriptableObject.CreateInstance<DriverWeaponDef>();

        weaponDef.name = weaponDefInfo.name;
        weaponDef.weaponName = weaponDefInfo.name;
        weaponDef.nameToken = weaponDefInfo.nameToken;
        weaponDef.description = weaponDefInfo.description;
        weaponDef.descriptionToken = weaponDefInfo.descriptionToken;

        weaponDef.icon = weaponDefInfo.icon;
        weaponDef.tier = weaponDefInfo.tier;
        weaponDef.animationSet = weaponDefInfo.animationSet;
        weaponDef.buffType = weaponDefInfo.buffType;
        weaponDef.shotCount = weaponDefInfo.shotCount;

        weaponDef.primarySkillDef = weaponDefInfo.primarySkillDef;
        weaponDef.secondarySkillDef = weaponDefInfo.secondarySkillDef;
        weaponDef.arsenalSkillDef = weaponDefInfo.arsenalSkillDef;
        weaponDef.unlockableDef = weaponDefInfo.unlockableDef;

        weaponDef.mesh = weaponDefInfo.mesh;
        weaponDef.material = weaponDefInfo.material;
        weaponDef.crosshairPrefab = weaponDefInfo.crosshairPrefab;
        weaponDef.pickupPrefabOverride = weaponDefInfo.pickupPrefabOverride;
        weaponDef.colorOverride = weaponDefInfo.colorOveride;
        weaponDef.disableHolster = weaponDefInfo.disableHolster;

        weaponDef.equipAnimationString = weaponDefInfo.equipAnimationString;
        weaponDef.reloadAnimationString = weaponDefInfo.reloadAnimationString;
        weaponDef.calloutSoundString = weaponDefInfo.calloutSoundString;
        weaponDef.dropBodyName = weaponDefInfo.dropBodyName;
        weaponDef.dropChance = weaponDefInfo.dropChance;

        return weaponDef;
    }
}

[System.Serializable]
public struct DriverWeaponDefInfo
{
    public string name;
    public string nameToken;
    public string description;
    public string descriptionToken;

    public Sprite icon;
    public DriverWeaponTier tier;
    public DriverWeaponDef.AnimationSet animationSet;
    public DriverWeaponDef.BuffType buffType;
    public int shotCount;

    public SkillDef primarySkillDef;
    public SkillDef secondarySkillDef;
    public SkillDef arsenalSkillDef;
    public UnlockableDef unlockableDef;

    public Mesh mesh;
    public Material material;
    public GameObject crosshairPrefab;
    public GameObject pickupPrefabOverride;
    public Color? colorOveride;
    public bool disableHolster;

    public string equipAnimationString;
    public string reloadAnimationString;
    public string calloutSoundString;
    public string dropBodyName;
    public float dropChance;
}

public enum DriverWeaponTier
{
    NoTier,
    Common,
    Uncommon,
    Legendary,
    Unique,
    Void,
    Lunar
}