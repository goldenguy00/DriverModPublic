using R2API;
using RoR2;
using UnityEngine;

[CreateAssetMenu(fileName = "blt", menuName = "ScriptableObjects/BulletDef", order = 2)]
public class DriverBulletDef : ScriptableObject
{
    [Header("General")]
    public string bulletName = "";
    public string bulletNameToken = "";
    public string description = "";
    public string descriptionToken = "";

    [Header("Visuals")]
    public DriverWeaponTier tier = DriverWeaponTier.Common;
    public Color trailColor = Color.black;
    public DamageTypeCombo damageType = DamageTypeCombo.GenericPrimary;

    [HideInInspector]
    public ushort index; // assigned at runtime
    [HideInInspector]
    public bool enabled;

    public static DriverBulletDef CreateBulletDefFromInfo(DriverBulletDefInfo bulletDefInfo)
    {
        var bulletDef = ScriptableObject.CreateInstance<DriverBulletDef>();
        bulletDef.name = bulletDefInfo.bulletName;
        bulletDef.bulletName = bulletDefInfo.bulletName;
        bulletDef.bulletNameToken = bulletDefInfo.bulletNameToken;
        bulletDef.description = bulletDefInfo.description;
        bulletDef.descriptionToken = bulletDefInfo.descriptionToken;
        bulletDef.tier = bulletDefInfo.tier;
        bulletDef.trailColor = bulletDefInfo.trailColor;
        bulletDef.damageType.damageType = bulletDefInfo.damageType ?? DamageType.Generic;
        bulletDef.damageType.damageTypeExtended = bulletDefInfo.damageTypeExtended ?? DamageTypeExtended.Generic;

        return bulletDef;
    }
}

[System.Serializable]
public struct DriverBulletDefInfo
{
    public string bulletName;
    public string bulletNameToken;
    public string description;
    public string descriptionToken;

    public DriverWeaponTier tier;
    public Color trailColor;

    public DamageType? damageType;
    public DamageTypeExtended? damageTypeExtended;
    public DamageAPI.ModdedDamageType? moddedDamageType;
}
