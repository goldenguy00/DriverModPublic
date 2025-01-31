using R2API;
using RoR2;
using UnityEngine;

[CreateAssetMenu(fileName = "blt", menuName = "ScriptableObjects/BulletDef", order = 2)]
public class DriverBulletDef : ScriptableObject
{
    [Header("General")]
    public string nameToken = "";
    public DamageTypeCombo bulletType = DamageTypeCombo.GenericPrimary;
    public DriverWeaponTier tier = DriverWeaponTier.Common;

    [Header("Visuals")]
    public Color trailColor = Color.black;

    [HideInInspector]
    public ushort index; // assigned at runtime

    public static DriverBulletDef CreateBulletDefFromInfo(DriverBulletDefInfo bulletDefInfo)
    {
        DriverBulletDef bulletDef = ScriptableObject.CreateInstance<DriverBulletDef>();
        bulletDef.name = bulletDefInfo.nameToken;
        bulletDef.nameToken = bulletDefInfo.nameToken;
        bulletDef.tier = bulletDefInfo.tier;
        bulletDef.trailColor = bulletDefInfo.trailColor;
        bulletDef.bulletType = new DamageTypeCombo
        {
            damageType = bulletDefInfo.damageType ?? DamageType.Generic,
            damageTypeExtended = bulletDefInfo.damageTypeExtended ?? DamageTypeExtended.Generic,
            damageSource = DamageSource.Primary
        };

        if (bulletDefInfo.moddedDamageType.HasValue)
            bulletDef.bulletType.AddModdedDamageType(bulletDefInfo.moddedDamageType.Value);

        return bulletDef;
    }
}

[System.Serializable]
public struct DriverBulletDefInfo
{
    public string nameToken;

    public DamageType? damageType;
    public DamageTypeExtended? damageTypeExtended;
    public DamageAPI.ModdedDamageType? moddedDamageType;

    public DriverWeaponTier tier;
    public Color trailColor;
}
