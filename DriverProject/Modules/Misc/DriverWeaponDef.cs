using UnityEngine;
using RoR2.Skills;

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

    [Header("General")]
    public string nameToken = "";
    public string descriptionToken = "";
    public Texture icon = null;
    public GameObject crosshairPrefab = null;
    public DriverWeaponTier tier = DriverWeaponTier.Common;
    public int shotCount = 8;
    public BuffType buffType = BuffType.Damage;

    [Header("Skills")]
    public SkillDef primarySkillDef;
    public SkillDef secondarySkillDef;

    [Header("Visuals")]
    public Mesh mesh;
    public Material material;
    public AnimationSet animationSet = AnimationSet.Default;
    public string calloutSoundString = "";

    [Header("Other")]
    public string configIdentifier = "";
    public float dropChance = 0f;

    [HideInInspector]
    public ushort index; // assigned at runtime
    [HideInInspector]
    public GameObject pickupPrefab; // same thing

    public string equipAnimationString
    {
        get
        {
            if (this.animationSet == AnimationSet.Default) return "EquipPistol";
            return "BufferEmpty";
        }
    }

    public Color color
    {
        get => this.tier switch
        {
            DriverWeaponTier.Common => RobDriver.Modules.Helpers.whiteItemColor,
            DriverWeaponTier.Uncommon => RobDriver.Modules.Helpers.greenItemColor,
            DriverWeaponTier.Legendary => RobDriver.Modules.Helpers.redItemColor,
            DriverWeaponTier.Unique => RobDriver.Modules.Helpers.yellowItemColor,
            DriverWeaponTier.Lunar => RobDriver.Modules.Helpers.lunarItemColor,
            DriverWeaponTier.Void => RobDriver.Modules.Helpers.voidItemColor,
            _ => RobDriver.Modules.Helpers.whiteItemColor,
        };
    }

    public static DriverWeaponDef CreateWeaponDefFromInfo(DriverWeaponDefInfo weaponDefInfo)
    {
        DriverWeaponDef weaponDef = (DriverWeaponDef)ScriptableObject.CreateInstance(typeof(DriverWeaponDef));
        weaponDef.name = weaponDefInfo.nameToken;

        weaponDef.nameToken = weaponDefInfo.nameToken;
        weaponDef.descriptionToken = weaponDefInfo.descriptionToken;
        weaponDef.icon = weaponDefInfo.icon;
        weaponDef.crosshairPrefab = weaponDefInfo.crosshairPrefab;
        weaponDef.tier = weaponDefInfo.tier;
        weaponDef.shotCount = weaponDefInfo.shotCount;
        weaponDef.buffType = weaponDefInfo.buffType;

        weaponDef.primarySkillDef = weaponDefInfo.primarySkillDef;
        weaponDef.secondarySkillDef = weaponDefInfo.secondarySkillDef;

        weaponDef.mesh = weaponDefInfo.mesh;
        weaponDef.material = weaponDefInfo.material;
        weaponDef.animationSet = weaponDefInfo.animationSet;
        weaponDef.calloutSoundString = weaponDefInfo.calloutSoundString;

        weaponDef.configIdentifier = weaponDefInfo.configIdentifier;
        weaponDef.dropChance = weaponDefInfo.dropChance;

        return weaponDef;
    }
}

[System.Serializable]
public struct DriverWeaponDefInfo
{
    public string nameToken;
    public string descriptionToken;
    public Texture icon;
    public GameObject crosshairPrefab;
    public DriverWeaponTier tier;
    public int shotCount;
    public DriverWeaponDef.BuffType buffType;

    public SkillDef primarySkillDef;
    public SkillDef secondarySkillDef;

    public Mesh mesh;
    public Material material;
    public DriverWeaponDef.AnimationSet animationSet;
    public string calloutSoundString;

    public string configIdentifier;
    public float dropChance;
}

public enum DriverWeaponTier
{
    Common,
    Uncommon,
    Legendary,
    Unique,
    Void,
    Lunar,
}